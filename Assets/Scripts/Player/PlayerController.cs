using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool isHurt;
    Player player;
    public StateMachine stateMachine { get; private set; }
    private PlayerSkill playerSkill;

    private bool isDie = false;

    // 자석 관련 필드
    [SerializeField] private GameObject magnetFieldPrefab; // 인스펙터에 프리팹 할당
    private GameObject magnetFieldObj;              // 자석 범위 오브젝트
    [SerializeField] private float magnetRadius = 3f;     // 자석 반경
    [SerializeField] private float magnetDuration = 5f;   // 자석 효과 지속 시간


    private void Awake()
    {
        stateMachine = new StateMachine(this);

    }

    private void Start()
    {
        player = GameManager.Instance.player;
        playerSkill = PlayerSkill.Instance;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Skill"), true);
    }

    private void Update()
    {
      
    }
    

    // 적에게 부딪혔을때
    private void Hurt(int damage)
    {
        if (isDie || isHurt) return;

        player.Hurt(damage);
        Debug.Log($"player.hp : {player.hp}");

        // 사망 체크
        if (player.hp <= 0)
        {
            Die();
            return;
        }

        isHurt = true;
        StartCoroutine(HurtRoutine(1f));
    }

    private void Die()
    {
        isDie = true;

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        enabled = false;
        player.Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDie) return;
        if (other.CompareTag("Enemy"))
        {
            Hurt(10);
            Debug.Log("적에게 닿음!");
        }
        if (other.CompareTag("Exp"))
        {
            player.GetExperience(10);
            Destroy(other.gameObject);
            SoundManager.Instance.PlaySFX("ItemSound");
            Debug.Log("경험치 획득!");
        }
        else if (other.CompareTag("MagnetItem"))  // 아이템 태그 확인
        {
            Destroy(other.gameObject);
            EnableMagnet(magnetRadius, magnetDuration);
            Debug.Log("자석 아이템 획득: Exp 끌어오기 시작");
        }
    }
    public void EnableMagnet(float radius, float duration)
    {
        if (magnetFieldObj != null) return;

        // Instantiate
        magnetFieldObj = Instantiate(magnetFieldPrefab, transform.position, Quaternion.identity, transform);
        magnetFieldObj.tag = "Magnet";

        // Collider 반경 설정
        var circle = magnetFieldObj.GetComponent<CircleCollider2D>();
        if (circle != null) circle.radius = radius;

        // 스크립트 초기화
        var mf = magnetFieldObj.GetComponent<Magnet>();
        if (mf != null) mf.Initialize(transform);

        StartCoroutine(DisableMagnetAfter(duration, radius));
    }

    private IEnumerator DisableMagnetAfter(float duration, float radius)
    {
        yield return new WaitForSeconds(duration);
        if (magnetFieldObj != null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (var col in hits)
            {
                if (col.CompareTag("Exp"))
                {
                    var exp = col.GetComponent<Exp>();
                    if (exp != null)
                        exp.ClearMagnet();
                }
            }
            Destroy(magnetFieldObj);
            magnetFieldObj = null;
            Debug.Log("자석 효과 종료");
        }
    }

    public IEnumerator HurtRoutine(float hurtDuration)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Monster");

        //플레이어와 적 충돌 무시
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        //stateMachine.TransitionTo(stateMachine.hurtState);
        yield return new WaitForSeconds(hurtDuration);

        // 다시 충돌 켜기
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        isHurt = false;
    }

    public void StartHurtRoutine()
    {
        Debug.Log("A");
        StartCoroutine(HurtRoutine(1f));
    }

    // 애니메이션이 끝나면 자동으로 Idle 상태로 전환
    public void GoIdle()
    {
        stateMachine.TransitionTo(stateMachine.idleState);
    }
}
