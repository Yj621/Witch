using UnityEngine;

public class QESkill : MonoBehaviour
{
    private Animator animator; // Player의 Animator
    private string currentSkillAnimation;

    public static QESkill Instance { get; private set; }

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
    }

    void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    void Update()
    {

    }

    // 기본 공격 데미지
    public void Attack(Collider2D enemyCollider)
    {
        //현재 재생중인 애니메이션 이름 확인
        currentSkillAnimation = GetCurrentSkillAnimation();

        SkillAttack(currentSkillAnimation, enemyCollider);
    }
    public void SkillAttack(string skillName, Collider2D enemyCollider)
    {
        if (string.IsNullOrEmpty(skillName))
            return;  // None, null 은 무시

        var data = SkillManager.Instance.GetRuntimeSkillData(skillName);
        if (data == null) return;

        float damage = data.damage;
        if (damage > 0)
        {
            enemyCollider.GetComponent<EnemyMove>().EnemyHurt(damage);
            Debug.Log($"{enemyCollider.gameObject.name}에게 {damage}만큼 데미지 입힘");
        }
    }


    private string GetCurrentSkillAnimation()
    {
        // 애니메이터에서 현재 재생 중인 애니메이션 상태 이름 반환
        string triggerName = PlayerInput.Instance.GetCurrentTriggerName();

        // 트리거 이름에 따라 현재 스킬 애니메이션 결정
        if (triggerName == "FireSlashs")
        {
            return "FireSlashs";
        }
        else if (triggerName == "Thunder")
        {
            return "Thunder";
        }
        else
        {
            return "None";
        }
    }
}
