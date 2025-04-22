using UnityEngine;

public class QESKill : MonoBehaviour
{
    private Animator animator; // Player의 Animator
    private string currentSkillAnimation;

    public static QESKill Instance { get; private set; }

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

    public void SkillAttack(string skillAnimation, Collider2D enemyCollider)
    {
        float damage = 0;
        switch (skillAnimation)
        {
            case "FireSlashs":
                damage = SkillManager.Instance.GetSkillDamage("FireSlashs");
                break;
            case "Thunder":
                damage = SkillManager.Instance.GetSkillDamage("Thunder");
                break;
            
        }
        if (damage > 0)
        {
            enemyCollider.GetComponent<EnemyMove>().EnemyHurt(damage);
            Debug.Log($"{enemyCollider.gameObject}에게 {damage}만큼 데미지 입히기");
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
