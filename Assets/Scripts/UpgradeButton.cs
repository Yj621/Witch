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

    //스킬 습득
    FSSkillLearn,
    IPSkillLearn,
    TDSkillLearn,
    IFSkillLearn,
    BHSkillLearn
}

public class UpgradeButton : MonoBehaviour
{
    private Player player;
    [SerializeField] private UpgradeType upgradeType;
    public UpgradeLevel upgradeLevel;
    
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
            case UpgradeType.CharacterMoveSpeed:
                player.playerSpeed += 0.5f;
                Debug.Log("강화! 현재: " +  player.playerSpeed);
                break;
            case UpgradeType.DashSpeed:
                player.dashSpeed += 0.5f;
                Debug.Log("강화! 현재: " + player.dashSpeed);
                break;
            case UpgradeType.DashCooldown:
                PlayerInput.Instance.dashCoolTime -= 0.5f;
                Debug.Log("강화! 현재: " + PlayerInput.Instance.dashCoolTime);
                break;
            case UpgradeType.MaxHPIncrease:
                player.maxHp += 10;
                Debug.Log("강화! 현재: " + player.maxHp);
                break;
            case UpgradeType.DefaultSkillDamage:
                DefaultSkill.Instance.defaultDamage += 2;
                Debug.Log("강화! 현재: " + DefaultSkill.Instance.defaultDamage);
                break;
            case UpgradeType.DefaultSkillCooldown:
               PlayerSkill.Instance.defaultSkillCooldown -= 0.5f;
                Debug.Log("강화! 현재: " + PlayerSkill.Instance.defaultSkillCooldown);
                break;

            case UpgradeType.FSSkillDamage:
                UpgradeQESkillDamage("FireSlashs", 10f);
                break;
            case UpgradeType.FSSkillCooldown:
                UpgradeQESkillDamage("FireSlashs", 10f);
                break;
            case UpgradeType.FSSkillRange:
                UpgradeQESkillDamage("FireSlashs", 10f);
                break;

            case UpgradeType.TDSkillDamage:
                UpgradeQESkillDamage("Thunder", 10f);
                break;
            case UpgradeType.TDSkillCooldown:
                UpgradeQESkillDamage("Thunder", 10f);
                break;
            case UpgradeType.TDSkillRange:
                UpgradeQESkillDamage("Thunder", 10f);
                break;


            case UpgradeType.IPSkillLearn:
                SkillManager.Instance.LearnNewSkill("IcePillar", true);
                UpgradeManager.Instance.data.SetDisabled(UpgradeType.IPSkillLearn, true);
                break;
            case UpgradeType.IPSkillDamage:
                UpgradePassiveSkill("IcePillar", 10f);
                break;
            case UpgradeType.IPSkillCooldown:
                UpgradePassiveCooltime("IcePillar", 10f);
                break;
            case UpgradeType.IPSkillRange:
                UpgradePassiveSkill("IcePillar", 10f);
                break;


            case UpgradeType.IFSkillLearn:
                SkillManager.Instance.LearnNewSkill("Infierno", true);
                UpgradeManager.Instance.data.SetDisabled(UpgradeType.IFSkillLearn, true);
                break;
            case UpgradeType.IFSkillDamage:
                UpgradePassiveSkill("Infierno", 10f);
                break;
            case UpgradeType.IFSkillCooldown:
                UpgradePassiveCooltime("Infierno", 10f);
                break;
            case UpgradeType.IFSkillRange:
                UpgradePassiveSkill("Infierno", 10f);
                break;


            case UpgradeType.BHSkillLearn:
                SkillManager.Instance.LearnNewSkill("Blackhole", true);
                UpgradeManager.Instance.data.SetDisabled(UpgradeType.BHSkillLearn, true);
                break;
            case UpgradeType.BHSkillDamage:
                UpgradePassiveSkill("Blackhole", 10f);
                break;
            case UpgradeType.BHSkillCooldown:
                UpgradePassiveCooltime("Blackhole", 10f);
                break;
            case UpgradeType.BHSkillRange:
                UpgradePassiveSkill("Blackhole", 10f);
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
        Debug.Log($"SkillManager.Instance.UpgradePassiveSkill : {SkillManager.Instance}");
    }

    public void UpgradePassiveCooltime(string skillName, float damage)
    {
        SkillManager.Instance.UpgradePassiveCooltime(skillName, damage);
        Debug.Log($"SkillManager.Instance.UpgradePassiveSkill : {SkillManager.Instance}");
    }
}
