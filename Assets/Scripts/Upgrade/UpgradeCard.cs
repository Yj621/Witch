using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image[] UpgradeComStars;

    private UpgradeOption option;
    private UpgradeType type;
    private UpgradeManager manager;
    UpgradeButton upgradeButton;

    private void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            UpgradeComStars[i].enabled = false;
        }
    }

    public void Init(UpgradeOption optionData, UpgradeButton upgradeLogic)
    {
        option = optionData;
        type = option.type;
        upgradeButton = upgradeLogic;

        titleText.text = option.title;
        descriptionText.text = option.description;
        iconImage.sprite = option.icon;
        //UpdateStar();
    }

    public void OnSelect()
    {
        upgradeButton.SetUpgrade(type);
        upgradeButton.OnUpgrade();  // 실질적 업그레이드 실행
        Debug.Log("강화 완료");
        UIManager.Instance.OnPressed();
    }

    public void UpdateStar()
    {
        Debug.Log(manager.levels[type]);
        for(int i = 0; i < manager.levels[type]; i++)
        {
            UpgradeComStars[i].enabled = true;
        }
    }
}
