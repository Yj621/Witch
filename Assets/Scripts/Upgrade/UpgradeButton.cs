using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private Exp exp; 
    private Player player;

    private void Start()
    {
        player = GameManager.Instance.player;
    }

    /// <summary>
    /// 스킬 업그레이드/습득 정보를 담는 구조체
    /// </summary>
    private struct ActionInfo
    {
        public string SkillName;
        public bool IsLearn;
        public UpgradeStat Stat;
        public float Amount;

        public ActionInfo(string skillName, bool isLearn, UpgradeStat stat = default, float amount = 0f)
        {
            SkillName = skillName;
            IsLearn = isLearn;
            Stat = stat;
            Amount = amount;
        }
    }

    // 각 UpgradeType에 대응하는 동작 매핑
    private static readonly Dictionary<UpgradeType, ActionInfo> actionMap = new Dictionary<UpgradeType, ActionInfo>
    {
        // 스킬 습득
        { UpgradeType.IPSkillLearn,   new ActionInfo("IcePillar",    true) },
        { UpgradeType.IFSkillLearn,   new ActionInfo("Infierno",     true) },
        { UpgradeType.BHSkillLearn,   new ActionInfo("Blackhole",    true) },
        { UpgradeType.TSSkillLearn,   new ActionInfo("ThunderStrike",true) },

        // QE 스킬 업그레이드
        { UpgradeType.FSSkillRange,    new ActionInfo("FireSlashs",  false, UpgradeStat.Range,    0.2f) },
        { UpgradeType.FSSkillCooldown, new ActionInfo("FireSlashs",  false, UpgradeStat.Cooltime, -0.5f) },
        { UpgradeType.FSSkillDamage,   new ActionInfo("FireSlashs",  false, UpgradeStat.Damage,   2.5f) },

        // Thunder 스킬 업그레이드
        { UpgradeType.TDSkillRange,    new ActionInfo("Thunder",      false, UpgradeStat.Range,    0.2f) },
        { UpgradeType.TDSkillCooldown, new ActionInfo("Thunder",      false, UpgradeStat.Cooltime, -0.5f) },
        { UpgradeType.TDSkillDamage,   new ActionInfo("Thunder",      false, UpgradeStat.Damage,   2.5f) },

        // IcePillar 패시브 업그레이드
        { UpgradeType.IPSkillRange,    new ActionInfo("IcePillar",    false, UpgradeStat.Range,    0.2f) },
        { UpgradeType.IPSkillCooldown, new ActionInfo("IcePillar",    false, UpgradeStat.Cooltime, -0.5f) },
        { UpgradeType.IPSkillDamage,   new ActionInfo("IcePillar",    false, UpgradeStat.Damage,   2.5f) },

        // Infierno 패시브 업그레이드
        { UpgradeType.IFSkillRange,    new ActionInfo("Infierno",     false, UpgradeStat.Range,    0.2f) },
        { UpgradeType.IFSkillCooldown, new ActionInfo("Infierno",     false, UpgradeStat.Cooltime, -0.5f) },
        { UpgradeType.IFSkillDamage,   new ActionInfo("Infierno",     false, UpgradeStat.Damage,   2.5f) },

        // Blackhole 패시브 업그레이드
        { UpgradeType.BHSkillRange,    new ActionInfo("Blackhole",    false, UpgradeStat.Range,    0.2f) },
        { UpgradeType.BHSkillCooldown, new ActionInfo("Blackhole",    false, UpgradeStat.Cooltime, -0.5f) },
        { UpgradeType.BHSkillDamage,   new ActionInfo("Blackhole",    false, UpgradeStat.Damage,   2.5f) },

        // ThunderStrike 패시브 업그레이드
        { UpgradeType.TSSkillRange,    new ActionInfo("ThunderStrike",false, UpgradeStat.Range,   0.2f) },
        { UpgradeType.TSSkillCooldown, new ActionInfo("ThunderStrike",false, UpgradeStat.Cooltime, -0.5f) },
        { UpgradeType.TSSkillDamage,   new ActionInfo("ThunderStrike",false, UpgradeStat.Damage,   2.5f) },

        // 기본 스킬 업그레이드
        { UpgradeType.DefaultSkillDamage,   new ActionInfo("DefaultSkill", false, UpgradeStat.Damage,   2.5f) },
        { UpgradeType.DefaultSkillCooldown, new ActionInfo("DefaultSkill", false, UpgradeStat.Cooltime, -0.2f) },

    };

    /// <summary>
    /// 업그레이드 타입 설정
    /// </summary>
    public void SetUpgrade(UpgradeType type)
    {
        upgradeType = type;
    }

    /// <summary>
    /// 업그레이드 실행
    /// </summary>
    public void OnUpgrade()
    {
        // 캐릭터 스탯 업그레이드 처리
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
            case UpgradeType.ExpIncrease:
                player.expAmount += 5;
                Debug.Log($"player.expAmount  : {player.expAmount}");
                break;
            default:

                //  스킬 업그레이드/습득 처리
                if (actionMap.TryGetValue(upgradeType, out var info))
                {
                    if (info.SkillName == "DefaultSkill")
                    {
                        // 기본 스킬 전용 처리
                        SkillManager.Instance.UpgradeDefaultSkillStat(info.Stat, info.Amount);
                    }
                    else if (info.IsLearn)
                    {
                        // 스킬 습득
                        SkillManager.Instance.LearnNewSkill(info.SkillName, true);
                    }
                    else
                    {
                        // 패시브/QE 스킬 업그레이드
                        SkillManager.Instance.UpgradeSkillStat(info.SkillName, info.Stat, info.Amount);
                    }
                }
                else
                {
                    Debug.LogWarning($"업그레이드 액션이 없습니다: {upgradeType}");
                }
                break;
        }

        UpgradeManager.Instance.LevelUp(upgradeType);
        Time.timeScale = 1f;
        UIManager.Instance.UpdateStatNum();
        UIManager.Instance.UpdateSkillIcons();
        UIManager.Instance.OnPressed();
    }
}
