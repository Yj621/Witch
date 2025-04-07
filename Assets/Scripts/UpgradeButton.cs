using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    //스킬 업그레이드
    QESkillRange,
    QESkillCooldown,
    QESkillDamage  // FireSlash, IcePillar 등
}
public class UpgradeButton : MonoBehaviour, IUpgrade
{
    private Player player;
    [SerializeField] UpgradeType upgradeType;

    [SerializeField] private TextMeshProUGUI upgradeTitleText; //업그레이드할 능력치 타이틀(스킬, 캐릭터 능력치)
    [SerializeField] private TextMeshProUGUI upgradeDetailText; //업그레이드할 능력치 설명
    [SerializeField] private Image upgradeImage; //업그레이드할 능력치 아이콘
    public UpgradeLevel upgradeLevel;
    private string skillName;

    void Start()
    {

    }

    void Update()
    {

    }

//업그레이드 버튼(upgradeType에 따라 다르게 해줌)
    public void OnUpgrade()
    {
        if (!upgradeLevel.IsMaxLevel())
        {
            upgradeLevel.LevelUp();

            switch (upgradeType)
            {
                //캐릭터 이동속도 업그레이드
                case UpgradeType.CharacterMoveSpeed:
                    player.playerSpeed += 0.5f;
                    break;
                //캐릭터 대쉬속도 업그레이드
                case UpgradeType.DashSpeed:
                    player.dashSpeed += 0.5f;
                    break;
                //캐릭터 대쉬 쿨타임 감소
                case UpgradeType.DashCooldown:
                    PlayerInput.Instance.dashCoolTime -= 0.5f;
                    break;
                //최대체력 증가
                case UpgradeType.MaxHPIncrease:
                    player.maxHp += 10;
                    break;
                //기본 스킬 데미지 증가
                case UpgradeType.DefaultSkillDamage:
                    DefaultSkill.Instance.defaultDamage += 2;
                    break;
                //기본 스킬 쿨타임 감소
                case UpgradeType.DefaultSkillCooldown:
                    PlayerInput.Instance.GetComponentInChildren<PlayerSkill>().defaultSkillCooldown -= 0.5f;
                    break;
                //스킬 데미지 증가
                case UpgradeType.QESkillDamage:
                    if (skillName == "FireSlash")
                    {
                        UpgradeSkillDamage("FireSlash", 10f);
                    }
                    else if (skillName == "IcePillar")
                    {
                        UpgradeSkillDamage("IcePillar", 10f);
                    }
                    else if (skillName == "Thunder")
                    {
                        UpgradeSkillDamage("Thunder", 10f);
                    }
                    else if (skillName == "Infierno")
                    {
                        UpgradeSkillDamage("Infierno", 10f);
                    }
                    break;
            }
        }
    }

    public void UpgradeSkillDamage(string skillName, float damage)
    {
        SkillManager.Instance.UpgradeSkillDamage(skillName, damage);
    }
}
