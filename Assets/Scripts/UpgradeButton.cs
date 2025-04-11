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
    [SerializeField] private bool hasLearnFS = false;
    [SerializeField] private bool hasLearnIP = false;
    [SerializeField] private bool hasLearnTD = false;
    [SerializeField] private bool hasLearnIF = false;
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
            case UpgradeType.QESkillDamage:
                if (skillName == "FireSlashs")
                {
                    if (!hasLearnFS) { SkillManager.Instance.LearnNewSkill("FireSlashs"); hasLearnFS = true; }
                    else UpgradeSkillDamage("FireSlashs", 10f);
                }
                else if (skillName == "IcePillar")
                {
                    if (!hasLearnIP) {SkillManager.Instance.LearnNewSkill("IcePillar"); hasLearnIP = true; }
                    else UpgradeSkillDamage("IcePillar", 10f);
                }
                else if (skillName == "Thunder")
                {
                    if (!hasLearnTD) {SkillManager.Instance.LearnNewSkill("Thunder"); hasLearnTD = true; }
                    else UpgradeSkillDamage("Thunder", 10f);
                }
                else if (skillName == "Infierno")
                {
                    if (!hasLearnIF) {SkillManager.Instance.LearnNewSkill("Infierno"); hasLearnIF = true; }
                    else UpgradeSkillDamage("Infierno", 10f);
                }
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
