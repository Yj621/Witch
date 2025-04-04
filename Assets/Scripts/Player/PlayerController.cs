using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool isHurt;
    Player player;
    public StateMachine stateMachine { get; private set; }
    private PlayerSkill playerSkill;

    private void Awake()
    {
        stateMachine = new StateMachine(this);
        player = GameManager.Instance.player;
    }

    private void Start()
    {
        playerSkill = GetComponentInChildren<PlayerSkill>();
    }

    private void Update()
    {
      
    }
    

    // 적에게 부딪혔을때
    private void Hurt(int damage)
    {
        if (!isHurt)
        {
            player.Hurt(damage);
            stateMachine.TransitionTo(stateMachine.hurtState);
            playerSkill.SyncSkillAnimation();
            Debug.Log($"player.hp : {player.hp}");
        }
    }

  

    private void OnTriggerEnter2D(Collider2D other)
    {
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


    // 애니메이션이 끝나면 자동으로 Idle 상태로 전환
    public void GoIdle()
    {
        stateMachine.TransitionTo(stateMachine.idleState);
        playerSkill.SyncSkillAnimation();
    }
}
