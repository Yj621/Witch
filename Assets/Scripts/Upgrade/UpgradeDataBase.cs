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
            (excludeList == null || !excludeList.Contains(opt.type)) &&
            !opt.hasLearned &&
            (!opt.requireUnlockFS || GetOption(UpgradeType.FSSkillLearn).hasLearned) &&
            (!opt.requireUnlockIP || GetOption(UpgradeType.IPSkillLearn).hasLearned) &&
            (!opt.requireUnlockTD || GetOption(UpgradeType.TDSkillLearn).hasLearned) &&
            (!opt.requireUnlockIF || GetOption(UpgradeType.IFSkillLearn).hasLearned)
        );

        if (options.Count == 0)
            return null;

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
