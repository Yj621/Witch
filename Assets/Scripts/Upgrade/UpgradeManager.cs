using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public Dictionary<UpgradeType, int> levels = new();
    private Dictionary<UpgradeType, int> maxLevels = new();
    public UpgradeDataBase data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
            {
                levels[type] = 1;
                maxLevels[type] = 5;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetLevel(UpgradeType type) => levels[type];
    public int GetMaxLevel(UpgradeType type) => maxLevels[type];
    public bool IsMaxLevel(UpgradeType type) => levels[type] >= maxLevels[type];

    public void LevelUp(UpgradeType type)
    {
        if (!IsMaxLevel(type))
        {
            levels[type]++;
        }
    }

    public void LevelUpByShop(UpgradeType type, int Lv)
    {
        if (!IsMaxLevel(type))
        {
            for(int i = 0;  i < Lv; i++)
            {
                levels[type]++;
            }
        }
    }
}

