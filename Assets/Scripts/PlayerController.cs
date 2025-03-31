using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool isHurt;
    Player player;
    public StateMachine stateMachine { get; private set; }
    private void Awake()
    {
        stateMachine = new StateMachine(this);
        player = GameManager.Instance.player;
    }

    private void Start()
    {
        Debug.Log($"maxExp : {player.maxExp}");
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
            Debug.Log("경험치 획득!");
        }
    }
}
