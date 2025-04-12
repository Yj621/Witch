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

    // 스킬 업그레이드
    FSSkillRange,
    FSSkillCooldown,
    FSSkillDamage,

    IPSkillRange,
    IPSkillCooldown,
    IPSkillDamage,

    TDSkillRange,
    TDSkillCooldown,
    TDSkillDamage,

    IFSkillRange,
    IFSkillCooldown,
    IFSkillDamage,

    //스킬 습득
    FSSkillLearn,
    IPSkillLearn,
    TDSkillLearn,
    IFSkillLearn
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


            case UpgradeType.FSSkillLearn:
                SkillManager.Instance.LearnNewSkill("FireSlashs");
                UpgradeManager.Instance.data.SetDisabled(UpgradeType.FSSkillLearn, true);
                break;
            case UpgradeType.FSSkillDamage:
                UpgradeSkillDamage("FireSlashs", 10f);
                break;
            case UpgradeType.FSSkillCooldown:
                UpgradeSkillDamage("FireSlashs", 10f);
                break;
            case UpgradeType.FSSkillRange:
                UpgradeSkillDamage("FireSlashs", 10f);
                break;



            case UpgradeType.IPSkillLearn:
                SkillManager.Instance.LearnNewSkill("IcePillar");
                UpgradeManager.Instance.data.SetDisabled(UpgradeType.IPSkillLearn, true);
                break;
            case UpgradeType.IPSkillDamage:
                UpgradeSkillDamage("IcePillar", 10f);
                break;
            case UpgradeType.IPSkillCooldown:
                UpgradeSkillDamage("IcePillar", 10f);
                break;
            case UpgradeType.IPSkillRange:
                UpgradeSkillDamage("IcePillar", 10f);
                break;


            case UpgradeType.TDSkillLearn:
                SkillManager.Instance.LearnNewSkill("Thunder");
                UpgradeManager.Instance.data.SetDisabled(UpgradeType.TDSkillLearn, true);
                break;
            case UpgradeType.TDSkillDamage:
                UpgradeSkillDamage("Thunder", 10f);
                break;
            case UpgradeType.TDSkillCooldown:
                UpgradeSkillDamage("Thunder", 10f);
                break;
            case UpgradeType.TDSkillRange:
                UpgradeSkillDamage("Thunder", 10f);
                break;


            case UpgradeType.IFSkillLearn:
                SkillManager.Instance.LearnNewSkill("Infierno");
                UpgradeManager.Instance.data.SetDisabled(UpgradeType.IFSkillLearn, true);
                break;
            case UpgradeType.IFSkillDamage:
                UpgradeSkillDamage("Infierno", 10f);
                break;
            case UpgradeType.IFSkillCooldown:
                UpgradeSkillDamage("Infierno", 10f);
                break;
            case UpgradeType.IFSkillRange:
                UpgradeSkillDamage("Infierno", 10f);
                break;
        }

        UpgradeManager.Instance.LevelUp(upgradeType);
        Time.timeScale = 1f;
        UIManager.Instance.UpdateStatNum();
    }
    public void UpgradeSkillDamage(string skillName, float damage)
    {
        SkillManager.Instance.UpgradeSkillDamage(skillName, damage);
    }
}
