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
            Hurt(5);
            Debug.Log("적에게 닿음!");
        }
        if (other.CompareTag("Exp"))
        {
            player.GetExperience(10);
            Destroy(other.gameObject);
            SoundManager.Instance.PlaySFX("ItemSound");
            Debug.Log("경험치 획득!");
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
