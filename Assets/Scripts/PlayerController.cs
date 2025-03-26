using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCoolTime;
    [SerializeField] private float dashDuration = 0.5f; //대쉬 지속시간
    private float dashCooldownTimer; //대쉬 쿨타임을 계산하기 위한 변수
    private bool isDash;
    [SerializeField] private bool isHurt;
    private bool isHorizonMove; // 수평 이동 여부

    Rigidbody2D rigid;
    Vector2 moveInput;

    private Player player;
    private StateMachine stateMachine;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine(this);
        player = new Player(100);
    }

    //최초 State 설정
    private void Start()
    {
        stateMachine.Initialize(stateMachine.idleState);
    }

    private void Update()
    {
        stateMachine.Execute();

        // 움직임 여부를 체크하고 상태 전환
        if (moveInput != Vector2.zero)
        {
            stateMachine.TransitionTo(stateMachine.walkState);
        }
        else
        {
            stateMachine.TransitionTo(stateMachine.idleState);
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if (!isDash)
        {
            // 입력 벡터를 정규화(normalized)하여 방향만 유지하고, 대각선 입력 시에도 속도가 일정하게 유지되도록 함
            Vector2 moveVec = moveInput.normalized;
            rigid.linearVelocity = moveVec * playerSpeed;
        }
    }

    private void Hurt(int damage)
    {
        if (!isHurt)
        {
            player.Hurt(damage);
            stateMachine.TransitionTo(stateMachine.hurtState);
        }
    }

    // 이동
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        stateMachine.TransitionTo(stateMachine.walkState);

        //입력 벡터의 x, y 절대값을 비교해 수평 또는 수직 이동
        isHorizonMove = Mathf.Abs(moveInput.x) >= Mathf.Abs(moveInput.y);

        // 스프라이트 방향 설정
        if (moveInput.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (moveInput.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    // 대쉬
    public void OnDash(InputValue value)
    {
        if (value.isPressed && !isDash && dashCooldownTimer <= 0)
        {
            StartCoroutine(Dash());
            stateMachine.TransitionTo(stateMachine.dashState);
        }
    }

    private IEnumerator Dash()
    {
        isDash = true;
        //대쉬 방향 설정
        Vector2 dashForce = new Vector2(moveInput.x * dashSpeed, 0);
        rigid.AddForce(dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dashDuration);
        isDash = false;
        dashCooldownTimer = dashCoolTime;
        stateMachine.TransitionTo(stateMachine.idleState);
    }

}
