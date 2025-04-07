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
        var options = upgradeOptions;

        if (excludeList != null)
            options = options.FindAll(opt => !excludeList.Contains(opt.type));

        return options[Random.Range(0, options.Count)];
    }
}
