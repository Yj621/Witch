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
    QESkillRange,
    QESkillCooldown,
    QESkillDamage
}

public class UpgradeButton : MonoBehaviour
{
    private Player player;
    [SerializeField] private UpgradeType upgradeType;
    public UpgradeLevel upgradeLevel;
    [SerializeField] private string skillName;
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
                break;
            case UpgradeType.DashSpeed:
                player.dashSpeed += 0.5f;
                break;
            case UpgradeType.DashCooldown:
                PlayerInput.Instance.dashCoolTime -= 0.5f;
                break;
            case UpgradeType.MaxHPIncrease:
                player.maxHp += 10;
                break;
            case UpgradeType.DefaultSkillDamage:
                DefaultSkill.Instance.defaultDamage += 2;
                break;
            case UpgradeType.DefaultSkillCooldown:
               PlayerSkill.Instance.defaultSkillCooldown -= 0.5f;
                break;
            case UpgradeType.QESkillDamage:
                if (skillName == "FireSlashs")
                    UpgradeSkillDamage("FireSlashs", 10f);
                else if (skillName == "IcePillar")
                    UpgradeSkillDamage("IcePillar", 10f);
                else if (skillName == "Thunder")
                    UpgradeSkillDamage("Thunder", 10f);
                else if (skillName == "Infierno")
                    UpgradeSkillDamage("Infierno", 10f);
                break;
        }
        Time.timeScale = 1f;
    }
    public void UpgradeSkillDamage(string skillName, float damage)
    {
        SkillManager.Instance.UpgradeSkillDamage(skillName, damage);
    }
}
