using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using static UnityEngine.EventSystems.EventTrigger;
public class PlayerInput : MonoBehaviour
{
    public float dashCoolTime = 5f;
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

    public static PlayerInput Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        rigid = GetComponent<Rigidbody2D>();
    }


    void Start()
    {
        player = GameManager.Instance.player;
        playerController = GetComponent<PlayerController>();
        skillManager = SkillManager.Instance;
        stateMachine = playerController.stateMachine;
        stateMachine.Initialize(stateMachine.idleState);
        // PlayerSkill 가져오기
        playerSkill = PlayerSkill.Instance;

    }

    private void Update()
    {
        stateMachine.Execute();

        var curr = stateMachine.CurrentState;
        if (curr == stateMachine.idleState || curr == stateMachine.walkState)
        {
            if (moveInput.sqrMagnitude > 0.01f)
            {
                stateMachine.TransitionTo(stateMachine.walkState);
            }
            else
            {
                stateMachine.TransitionTo(stateMachine.idleState);
            }
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
            rigid.linearVelocity = moveVec * player.playerSpeed;
        }
    }


    // 이동
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
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
        Vector2 dashForce = new Vector2(moveInput.x * player.dashSpeed, 0);
        rigid.AddForce(dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dashDuration);
        isDash = false;
        dashCooldownTimer = dashCoolTime;
        stateMachine.TransitionTo(stateMachine.idleState);
    }

    // Q 스킬
    public void OnFirstSkill(InputValue value)
    {
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
    }

    public void UseThunder()
    {
        stateMachine.TransitionTo(stateMachine.skillThunderState);
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
