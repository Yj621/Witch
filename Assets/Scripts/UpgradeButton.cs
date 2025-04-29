using UnityEngine;

public enum UpgradeType
{
    // 캐릭터 능력치
    CharacterMoveSpeed,
    DashSpeed,
    DashCooldown,
    ExpIncrease,
    MaxHPIncrease,

    // 기본 스킬 업그레이드
    DefaultSkillRange,
    DefaultSkillCooldown,
    DefaultSkillDamage,

    // QE 스킬 업그레이드
    FSSkillRange,
    FSSkillCooldown,
    FSSkillDamage,

    TDSkillRange,
    TDSkillCooldown,
    TDSkillDamage,

    // 패시브 스킬 업그레이드
    IPSkillRange,
    IPSkillCooldown,
    IPSkillDamage,

    IFSkillRange,
    IFSkillCooldown,
    IFSkillDamage,

    BHSkillRange,
    BHSkillCooldown,
    BHSkillDamage,

    TSSkillRange,
    TSSkillCooldown,
    TSSkillDamage,

    //스킬 습득
    FSSkillLearn,
    IPSkillLearn,
    TDSkillLearn,
    IFSkillLearn,
    BHSkillLearn,
    TSSkillLearn
}

public class UpgradeButton : MonoBehaviour
{
    private Player player;
    [SerializeField] private UpgradeType upgradeType;
    
    void Start()
    {
        player = GameManager.Instance.player;
    }

    public void SetUpgrade(UpgradeType type)
    {
        this.upgradeType = type;
    }

    public void OnUpgrade()
    {
        switch (upgradeType)
        {
            // QE 스킬
            case UpgradeType.FSSkillRange:
                UpgradeSkillRange("FireSlashs", 1f);
                break;
            case UpgradeType.FSSkillCooldown:
                UpgradePassiveCooltime("FireSlashs", 0.5f);
                break;
            case UpgradeType.FSSkillDamage:
                UpgradeQESkillDamage("FireSlashs", 10f);
                break;

            case UpgradeType.TDSkillRange:
               UpgradeSkillRange("Thunder", 1f);
                break;
            case UpgradeType.TDSkillCooldown:
                UpgradePassiveCooltime("Thunder", 0.5f);
                break;
            case UpgradeType.TDSkillDamage:
                UpgradeQESkillDamage("Thunder", 10f);
                break;

            // 패시브 스킬
            case UpgradeType.IPSkillRange:
                UpgradeSkillRange("IcePillar", 1f);
                break;
            case UpgradeType.IPSkillCooldown:
                UpgradePassiveCooltime("IcePillar", 0.5f);
                break;
            case UpgradeType.IPSkillDamage:
                UpgradePassiveSkill("IcePillar", 10f);
                break;

            case UpgradeType.IFSkillRange:
               UpgradeSkillRange("Infierno", 1f);
                break;
            case UpgradeType.IFSkillCooldown:
                UpgradePassiveCooltime("Infierno", 0.5f);
                break;
            case UpgradeType.IFSkillDamage:
                UpgradePassiveSkill("Infierno", 10f);
                break;

            case UpgradeType.BHSkillRange:
               UpgradeSkillRange("Blackhole", 1f);
                break;
            case UpgradeType.BHSkillCooldown:
                UpgradePassiveCooltime("Blackhole", 0.5f);
                break;
            case UpgradeType.BHSkillDamage:
                UpgradePassiveSkill("Blackhole", 10f);
                break;


            case UpgradeType.TSSkillRange:
                UpgradeSkillRange("ThunderStrike", 1f);
                break;
            case UpgradeType.TSSkillCooldown:
                UpgradePassiveCooltime("ThunderStrike", 0.5f);
                break;
            case UpgradeType.TSSkillDamage:
                UpgradeQESkillDamage("ThunderStrike", 10f);
                break;
        }

        UpgradeManager.Instance.LevelUp(upgradeType);
        Time.timeScale = 1f;
        UIManager.Instance.UpdateStatNum();
        UIManager.Instance.UpdateSkillIcons();
    }
    public void UpgradeQESkillDamage(string skillName, float damage)
    {
        SkillManager.Instance.UpgradeQESkillDamage(skillName, damage);
    }

    public void UpgradePassiveSkill(string skillName, float damage)
    {
        SkillManager.Instance.UpgradePassiveSkill(skillName, damage);
    }

    public void UpgradePassiveCooltime(string skillName, float damage)
    {
        SkillManager.Instance.UpgradePassiveCooltime(skillName, damage);
    }
    public void UpgradeSkillRange(string skillName, float damage)
    {
        SkillManager.Instance.UpgradeSkillRange(skillName, damage);
    }
}
