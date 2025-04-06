using System.Collections;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Animator skillAnimator;
    private PlayerInput playerInput;
    private SkillManager skillManager;

    [SerializeField] private float defaultSkillCooldown;
    [SerializeField] private Transform skillSpawnPoint;
    private float currentDamage;

    private void Awake()
    {
        skillAnimator = GetComponent<Animator>(); 
        playerInput = GetComponentInParent<PlayerInput>();
        skillManager = SkillManager.Instance;
    }
    private void Start()
    {
        // 일정 시간마다 OnDefaultSkill 자동 실행
        StartCoroutine(AutoFireDefaultSkill());
    }
    private void Update()
    {
    }

    // 플레이어와 스킬 싱크 맞추기
    public void SyncSkillAnimation()
    {
        if (playerInput == null || skillAnimator == null) return;

        string currentState = playerInput.GetCurrentTriggerName();

        // 애니메이션 상태 중 스킬과 관련 없는 것들은 필터링
        if (currentState == "Idle" || currentState == "Walk" || currentState == "Dash" || currentState == "Die")
        {
            return; // 스킬 데미지 요청 안 함
        }

        float damage = skillManager.GetSkillDamage(currentState);
        Debug.Log($"currentState는 {currentState}, 데미지는 {damage}");

        // 스킬 애니메이터의 상태도 변경
        skillAnimator.SetTrigger(currentState);
    }

    // 스킬 데미지
    private void OnTriggerEnter2D(Collider2D other)
    {
        float damage = skillManager.GetSkillDamage(playerInput.GetCurrentTriggerName());
        if (other.gameObject.tag == "Ground")
        {
            Destroy(other.gameObject);     
        }
        else if (other.gameObject.tag == "Enemy")
        {
            other.GetComponent<EnemyMove>().EnemyHurt(currentDamage);
        }
    }
    private IEnumerator AutoFireDefaultSkill()
    {
        while (true)
        {
            yield return new WaitForSeconds(defaultSkillCooldown);
            DefaultSkill();
        }
    }

    //기본 스킬 
    public void DefaultSkill()
    {
        float direction = Mathf.Sign(transform.parent.localScale.x); // 캐릭터 방향 (왼쪽:-1, 오른쪽:1)

        Vector2 spawnPosition = skillSpawnPoint.position;

        // 총알 생성 및 위치 설정
        GameObject skill = GameManager.Instance.skillObjectPool.GetObject();
        skill.transform.position = spawnPosition;
        skill.transform.rotation = Quaternion.identity;

        // 총알 속도 및 방향 설정
        skill.GetComponent<DefaultSkill>().velocity = new Vector2(4f * direction, 0);
        skill.transform.localScale = new Vector3(direction * Mathf.Abs(skill.transform.localScale.x),
                                          skill.transform.localScale.y,
                                          skill.transform.localScale.z);
    }
}
