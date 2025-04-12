using UnityEngine;

[System.Serializable]
public class UpgradeOption
{
    public UpgradeType type;
    public string title;
    public string description;
    public Sprite icon;

    public bool hasLearned;
    public bool requireUnlockFS ;
    public bool requireUnlockIP;
    public bool requireUnlockTD;
    public bool requireUnlockIF;
}