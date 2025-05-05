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
            //스킬 습득
            case UpgradeType.IPSkillLearn:
                SkillManager.Instance.LearnNewSkill("IcePillar", true);
                break;
            case UpgradeType.IFSkillLearn:
                SkillManager.Instance.LearnNewSkill("Infierno", true);
                break;
            case UpgradeType.BHSkillLearn:
                SkillManager.Instance.LearnNewSkill("Blackhole", true);
                break;
            case UpgradeType.TSSkillLearn:
                SkillManager.Instance.LearnNewSkill("ThunderStrike", true);
                break;

            // QE 스킬
            case UpgradeType.FSSkillRange:
                SkillManager.Instance.UpgradeSkillStat("FireSlashs", UpgradeStat.Range, 1f);
                break;
            case UpgradeType.FSSkillCooldown:
                SkillManager.Instance.UpgradeSkillStat("FireSlashs", UpgradeStat.Cooltime, 0.5f);
                break;
            case UpgradeType.FSSkillDamage:
                SkillManager.Instance.UpgradeSkillStat("FireSlashs", UpgradeStat.Damage, 5f);
                break;

            case UpgradeType.TDSkillRange:
                SkillManager.Instance.UpgradeSkillStat("Thunder", UpgradeStat.Range, 1f);
                break;
            case UpgradeType.TDSkillCooldown:
                SkillManager.Instance.UpgradeSkillStat("Thunder", UpgradeStat.Cooltime, 0.5f);
                break;
            case UpgradeType.TDSkillDamage:
                SkillManager.Instance.UpgradeSkillStat("Thunder", UpgradeStat.Damage, 5f);
                break;

            // 패시브 스킬
            case UpgradeType.IPSkillRange:
                SkillManager.Instance.UpgradeSkillStat("IcePillar", UpgradeStat.Range, 1f);
                break;
            case UpgradeType.IPSkillCooldown:
                SkillManager.Instance.UpgradeSkillStat("IcePillar", UpgradeStat.Cooltime, 0.5f);
                break;
            case UpgradeType.IPSkillDamage:
                SkillManager.Instance.UpgradeSkillStat("IcePillar", UpgradeStat.Damage, 5f);
                break;

            case UpgradeType.IFSkillRange:
                SkillManager.Instance.UpgradeSkillStat("Infierno", UpgradeStat.Range, 1f);
                break;
            case UpgradeType.IFSkillCooldown:
                SkillManager.Instance.UpgradeSkillStat("Infierno", UpgradeStat.Cooltime, 0.5f);
                break;
            case UpgradeType.IFSkillDamage:
                SkillManager.Instance.UpgradeSkillStat("Infierno", UpgradeStat.Damage, 5f);
                break;

            case UpgradeType.BHSkillRange:
               SkillManager.Instance.UpgradeSkillStat("Blackhole", UpgradeStat.Range, 1f);
                break;
            case UpgradeType.BHSkillCooldown:
                SkillManager.Instance.UpgradeSkillStat("Blackhole", UpgradeStat.Cooltime, 0.5f);
                break;
            case UpgradeType.BHSkillDamage:
                SkillManager.Instance.UpgradeSkillStat("Blackhole", UpgradeStat.Damage, 5f);
                break;


            case UpgradeType.TSSkillRange:
                SkillManager.Instance.UpgradeSkillStat("ThunderStrike", UpgradeStat.Damage, 5f);
                break;
            case UpgradeType.TSSkillCooldown:
                SkillManager.Instance.UpgradeSkillStat("ThunderStrike", UpgradeStat.Cooltime, 0.5f);
                break;
            case UpgradeType.TSSkillDamage:
                SkillManager.Instance.UpgradeSkillStat("ThunderStrike", UpgradeStat.Damage, 5f);
                break;
        }

        UpgradeManager.Instance.LevelUp(upgradeType);
        Time.timeScale = 1f;
        UIManager.Instance.UpdateStatNum();
        UIManager.Instance.UpdateSkillIcons();
    }
}
