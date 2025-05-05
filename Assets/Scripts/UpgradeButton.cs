using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private UpgradeType upgradeType;

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
        { UpgradeType.FSSkillRange,    new ActionInfo("FireSlashs",  false, UpgradeStat.Range,    1f) },
        { UpgradeType.FSSkillCooldown, new ActionInfo("FireSlashs",  false, UpgradeStat.Cooltime, 0.5f) },
        { UpgradeType.FSSkillDamage,   new ActionInfo("FireSlashs",  false, UpgradeStat.Damage,   5f) },

        // Thunder 스킬 업그레이드
        { UpgradeType.TDSkillRange,    new ActionInfo("Thunder",      false, UpgradeStat.Range,    1f) },
        { UpgradeType.TDSkillCooldown, new ActionInfo("Thunder",      false, UpgradeStat.Cooltime, 0.5f) },
        { UpgradeType.TDSkillDamage,   new ActionInfo("Thunder",      false, UpgradeStat.Damage,   5f) },

        // IcePillar 패시브 업그레이드
        { UpgradeType.IPSkillRange,    new ActionInfo("IcePillar",    false, UpgradeStat.Range,    1f) },
        { UpgradeType.IPSkillCooldown, new ActionInfo("IcePillar",    false, UpgradeStat.Cooltime, 0.5f) },
        { UpgradeType.IPSkillDamage,   new ActionInfo("IcePillar",    false, UpgradeStat.Damage,   5f) },

        // Infierno 패시브 업그레이드
        { UpgradeType.IFSkillRange,    new ActionInfo("Infierno",     false, UpgradeStat.Range,    1f) },
        { UpgradeType.IFSkillCooldown, new ActionInfo("Infierno",     false, UpgradeStat.Cooltime, 0.5f) },
        { UpgradeType.IFSkillDamage,   new ActionInfo("Infierno",     false, UpgradeStat.Damage,   5f) },

        // Blackhole 패시브 업그레이드
        { UpgradeType.BHSkillRange,    new ActionInfo("Blackhole",    false, UpgradeStat.Range,    1f) },
        { UpgradeType.BHSkillCooldown, new ActionInfo("Blackhole",    false, UpgradeStat.Cooltime, 0.5f) },
        { UpgradeType.BHSkillDamage,   new ActionInfo("Blackhole",    false, UpgradeStat.Damage,   5f) },

        // ThunderStrike 패시브 업그레이드
        { UpgradeType.TSSkillRange,    new ActionInfo("ThunderStrike",false, UpgradeStat.Range,   5f) },
        { UpgradeType.TSSkillCooldown, new ActionInfo("ThunderStrike",false, UpgradeStat.Cooltime, 0.5f) },
        { UpgradeType.TSSkillDamage,   new ActionInfo("ThunderStrike",false, UpgradeStat.Damage,   5f) },
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
        if (actionMap.TryGetValue(upgradeType, out var info))
        {
            if (info.IsLearn)
                SkillManager.Instance.LearnNewSkill(info.SkillName, true);
            else
                SkillManager.Instance.UpgradeSkillStat(info.SkillName, info.Stat, info.Amount);
        }
        else
        {
            Debug.LogWarning($"업그레이드 액션이 없습니다: {upgradeType}");
        }

        UpgradeManager.Instance.LevelUp(upgradeType);
        Time.timeScale = 1f;
        UIManager.Instance.UpdateStatNum();
        UIManager.Instance.UpdateSkillIcons();
        UIManager.Instance.OnPressed();
    }
}
