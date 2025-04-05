using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Animator skillAnimator;
    private PlayerInput playerInput;
    private SkillManager skillManager;

    private float currentDamage;

    private void Awake()
    {
        skillAnimator = GetComponent<Animator>(); 
        playerInput = GetComponentInParent<PlayerInput>();
        skillManager = GetComponentInParent<SkillManager>();
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
}
