using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using static UnityEngine.EventSystems.EventTrigger;
public class PlayerInput : MonoBehaviour
{
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
    private SkillManager skillManager;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }


    void Start()
    {
        player = GameManager.Instance.player;
        playerController = GetComponent<PlayerController>();
        skillManager = GetComponentInChildren<SkillManager>();
        stateMachine = playerController.stateMachine;
        stateMachine.Initialize(stateMachine.idleState);
        // PlayerSkill 가져오기
        playerSkill = GetComponentInChildren<PlayerSkill>();

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
            rigid.linearVelocity = moveVec * player.playerSpeed;
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
            gameObject.transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
        }
        else if (moveInput.x > 0)
        {
            gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

        // 움직임 여부를 체크하고 상태 전환
        if (moveInput.x != 0)
        {
            // 이동 입력이 있는 경우 Walk 상태로 전환
            stateMachine.TransitionTo(stateMachine.walkState);
            playerSkill.SyncSkillAnimation();
        }
        else
        {
            stateMachine.TransitionTo(stateMachine.idleState);
            playerSkill.SyncSkillAnimation();
        }

    }

    // 대쉬
    public void OnDash(InputValue value)
    {
        if (value.isPressed && !isDash && dashCooldownTimer <= 0)
        {
            StartCoroutine(Dash());
            stateMachine.TransitionTo(stateMachine.dashState);
            playerSkill.SyncSkillAnimation();
        }
    }

    private IEnumerator Dash()
    {
        isDash = true;
        //대쉬 방향 설정
        Vector2 dashForce = new Vector2(moveInput.x * player.dashSpeed, 0);
        rigid.AddForce(dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dashDuration);
        isDash = false;
        dashCooldownTimer = dashCoolTime;
        stateMachine.TransitionTo(stateMachine.idleState);
        playerSkill.SyncSkillAnimation();
    }

    // Q 스킬
    public void OnFirstSkill(InputValue value)
    {
        Debug.Log("Q");
        Action skill = skillManager.GetSkill(KeyCode.Q);
        skill?.Invoke();
    }

    //E 스킬
    public void OnSecondSkill(InputValue value)
    {
        Action skill = skillManager.GetSkill(KeyCode.E);
        skill?.Invoke();
    }

       // 스킬 실행 함수
    public void UseFireSlash()
    {
        stateMachine.TransitionTo(stateMachine.skillFireSlashState);
        playerSkill.SyncSkillAnimation();
    }

    public void UseIcePillar()
    {
        stateMachine.TransitionTo(stateMachine.skillIcePillarState);
        playerSkill.SyncSkillAnimation();
    }

    public void UseThunder()
    {
        stateMachine.TransitionTo(stateMachine.skillThunderState);
        playerSkill.SyncSkillAnimation();
    }

    public void UseBlackHole()
    {
        stateMachine.TransitionTo(stateMachine.skillBlackHoleState);
        playerSkill.SyncSkillAnimation();
    }

    public void UseInfierno()
    {
        stateMachine.TransitionTo(stateMachine.skillInfiernoState);
        playerSkill.SyncSkillAnimation();
    }

    // Trigger 파라미터 리턴 함수
    public string GetCurrentTriggerName()
    {
        if (stateMachine.CurrentState is IState stateWithTrigger)
        {
            string triggerName = stateWithTrigger.GetTrigger();
            return triggerName;
        }

        return string.Empty;
    }
}
