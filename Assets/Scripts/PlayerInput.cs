using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCoolTime;
    [SerializeField] private float dashDuration = 0.5f; //대쉬 지속시간
    private float dashCooldownTimer; //대쉬 쿨타임을 계산하기 위한 변수
    private bool isDash;
    private bool isHorizonMove; // 수평 이동 여부

    Rigidbody2D rigid;
    Vector2 moveInput;
    private StateMachine stateMachine;
    private Player player;
    private PlayerController playerController;
    private PlayerSkill playerSkill;  

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameManager.Instance.player;
    }


    void Start()
    {
        playerController = GetComponent<PlayerController>();
        stateMachine = playerController.stateMachine;
        stateMachine.Initialize(stateMachine.idleState); 
        // PlayerSkill 가져오기
        playerSkill = GetComponentInChildren<PlayerSkill>();

        if (playerSkill == null)
        {
            Debug.LogError("PlayerSkill 스크립트를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        stateMachine.Execute();


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


    // 이동
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

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

        // 움직임 여부를 체크하고 상태 전환
        if (moveInput.x != 0)
        {
            // 이동 입력이 있는 경우 Walk 상태로 전환
            stateMachine.TransitionTo(stateMachine.walkState);
            playerSkill.PlayAnimation("Idle");
        }
        else
        {
            stateMachine.TransitionTo(stateMachine.idleState);
            playerSkill.PlayAnimation("Idle");
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

    public void OnFirstSkill(InputValue value)
    {
        if (playerSkill != null)
        {
            playerSkill.PlayAnimation("FireBall");
            stateMachine.TransitionTo(stateMachine.skillFireBallState);
        }
        else
        {
            Debug.LogError("PlayerSkill이 할당되지 않았습니다!");
        }
    }
    public void OnSecondSkill(InputValue value)
    {

    }

}
