using UnityEngine;

public class UpgradeLevel
{
    public int level;
    public int maxLevel;

    public void LevelUp()
    {
        level+=1;
    }

    public bool IsMaxLevel()
    {
        return level == maxLevel;
    }
}
