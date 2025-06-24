using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject[] UpgradeComStars;    
    [SerializeField] private Animator animator; // Animator 추가
        
    private UpgradeOption option;
    private UpgradeType type;
    UpgradeButton upgradeButton;

    public void SetHighlight(bool isOn)
    {
        if (animator != null)
            animator.SetBool("Select", isOn); // Animator의 Select 파라미터 on/off
    }

    private void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            UpgradeComStars[i].SetActive(false);
        }
    }

    public void Update()
    {
        for (int i = 0; i < UpgradeManager.Instance.GetLevel(type) - 1; i++)
        {
            UpgradeComStars[i].SetActive(true);
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
    }

    public void OnSelect()
    {
        SetHighlight(true); // 선택 시 하이라이트 표시
        upgradeButton.SetUpgrade(type);
        upgradeButton.OnUpgrade();  // 실질적 업그레이드 실행
        Debug.Log("강화 완료");
        UIManager.Instance.OnPressed();
    }
}
