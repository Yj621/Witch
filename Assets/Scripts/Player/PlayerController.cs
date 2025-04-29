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
        player = GameManager.Instance.player; 
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Skill"), true);

    }

    private void Start()
    {
        playerSkill = PlayerSkill.Instance;
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
        StartCoroutine(HurtRoutine());
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
            Debug.Log("경험치 획득!");
        }
    }

    private IEnumerator HurtRoutine()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Monster");

        //플레이어와 적 충돌 무시
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        float hurtDuration = 1f; // 1초 동안 무적
        stateMachine.TransitionTo(stateMachine.hurtState);
        yield return new WaitForSeconds(hurtDuration);

        // 다시 충돌 켜기
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        isHurt = false;
        GoIdle();
    }

    // 애니메이션이 끝나면 자동으로 Idle 상태로 전환
    public void GoIdle()
    {
        stateMachine.TransitionTo(stateMachine.idleState);
    }
}
