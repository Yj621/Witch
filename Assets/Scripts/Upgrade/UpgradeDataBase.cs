using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDataBase", menuName = "Upgrades/Database")]
public class UpgradeDataBase : ScriptableObject
{
    public List<UpgradeOption> upgradeOptions;

    public UpgradeOption GetOption(UpgradeType type)
    {
        return upgradeOptions.Find(opt => opt.type == type);
    }

    public UpgradeOption GetRandomOption(List<UpgradeType> excludeList = null)
    {
        var options = upgradeOptions.FindAll(opt =>
            (excludeList?.Contains(opt.type) != true) &&            // 제외 타입이 아니면서
            !opt.hasLearned &&                                      // 아직 배우지 않았고
            (!opt.requireUnlockFS || GetOption(UpgradeType.FSSkillLearn)?.hasLearned == true) &&
            (!opt.requireUnlockIP || GetOption(UpgradeType.IPSkillLearn)?.hasLearned == true) &&
            (!opt.requireUnlockTD || GetOption(UpgradeType.TDSkillLearn)?.hasLearned == true) &&
            (!opt.requireUnlockIF || GetOption(UpgradeType.IFSkillLearn)?.hasLearned == true) &&
            (!opt.requireUnlockBH || GetOption(UpgradeType.BHSkillLearn)?.hasLearned == true)
        );

        if (options.Count == 0)
        {
            Debug.LogWarning("[UpgradeDataBase] 조건에 맞는 업그레이드가 없습니다.");
            return null;
        }

        return options[Random.Range(0, options.Count)];
    }




    public void SetDisabled(UpgradeType type, bool hasLearned)
    {
        var option = upgradeOptions.Find(opt => opt.type == type);
        if (option != null)
        {
            option.hasLearned = hasLearned;
        }
    }

    public void ResetAllLearned()
    {
        foreach (var opt in upgradeOptions)
        {
            opt.hasLearned = false;
        }
    }
}
