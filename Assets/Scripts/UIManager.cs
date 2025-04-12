using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider HpSlider;
    public Slider ExpSlider;
    public Slider CleanSlider;
    public GameObject LevelUpPanel;
    Player player;
    [SerializeField] private Image Qskill;
    [SerializeField] private Image Eskill;
    [SerializeField] private UpgradeDataBase upgradeDB;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;
    public Sprite[] skillIcons;
    SkillManager skillManager;
    Animator ani;
    [SerializeField] public TextMeshProUGUI DCText;
    [SerializeField] public TextMeshProUGUI DSText;
    [SerializeField] public TextMeshProUGUI PSText;
    [SerializeField] public TextMeshProUGUI EPText;
    [SerializeField] public TextMeshProUGUI HPText;
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        player = GameManager.Instance.player;
        skillManager = SkillManager.Instance;
        HpSlider.maxValue = player.maxHp;
        ExpSlider.maxValue = player.maxExp;
        CleanSlider.maxValue = 100;
        ani = GetComponent<Animator>();
        LevelUpPanel.SetActive(false);
        UpdateStatNum();
        upgradeDB.ResetAllLearned();
    }

    void Update()
    {
        HpSlider.value = player.Hp;
        ExpSlider.maxValue = player.maxExp;
        ExpSlider.value = player.Exp;
        CleanSlider.value = GameManager.Instance.currentClean;
    }

    //스킬 아이콘 업데이트
    public void UpdateSkillIcons()
    {
        UpdateSkillIconKey(KeyCode.Q, Qskill, skillManager);
        UpdateSkillIconKey(KeyCode.E, Eskill, skillManager);
    }
    

    private void UpdateSkillIconKey(KeyCode key, Image image, SkillManager skillManager)
    {
        Action skillAction = skillManager.GetSkill(key);

        //스킬이 없으면
        if(skillAction == null)
        {
            image.sprite = null;
            image.enabled = false;
            return;
        }

        string methodName = skillAction.Method.Name;
        int index = -1;

        switch(methodName)
        {
            case "UseFireSlash":
                index = 0;
                break;
            case "UseIcePillar":
                index = 1;
                break;
            case "UseThunder":
                index = 2;
                break;
            case "UseBlackHole":
                index = 3;
                break;
            case "UseInfierno":
                index = 4;
                break;
            default:
                break;
        }
        if (index >= 0 && index < skillIcons.Length)
        {
            image.sprite = skillIcons[index];
            image.enabled = true;
        }
        else
        {
            image.sprite = null;
            image.enabled = false;
        }

    }

    //스킬 레벨, 데미지 갱신

    public void LevelUpPanelPop()
    {
        LevelUpPanel.SetActive(true);

        foreach (Transform child in cardParent)
            Destroy(child.gameObject);

        var selectedOptions = new List<UpgradeOption>();

        while (selectedOptions.Count < 3)
        {
            var option = upgradeDB.GetRandomOption(
                excludeList: selectedOptions.Select(opt => opt.type).ToList()
            );

            if (option == null)
                break;

            selectedOptions.Add(option);
        }

        foreach (var option in selectedOptions)
        {
            var cardGO = Instantiate(cardPrefab, cardParent);
            var card = cardGO.GetComponent<UpgradeCard>();
            card.Init(option, FindFirstObjectByType<UpgradeButton>());
        }
    }


    public void UpdateStatNum()
    {
        DCText.text = UpgradeManager.Instance.IsMaxLevel(UpgradeType.DefaultSkillCooldown)
            ? "Max"
            : "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.DefaultSkillCooldown);

        DSText.text = UpgradeManager.Instance.IsMaxLevel(UpgradeType.DefaultSkillRange)
            ? "Max"
            : "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.DefaultSkillRange);

        PSText.text = UpgradeManager.Instance.IsMaxLevel(UpgradeType.CharacterMoveSpeed)
            ? "Max"
            : "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.CharacterMoveSpeed);

        EPText.text = UpgradeManager.Instance.IsMaxLevel(UpgradeType.ExpIncrease)
            ? "Max"
            : "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.ExpIncrease);

        HPText.text = UpgradeManager.Instance.IsMaxLevel(UpgradeType.MaxHPIncrease)
            ? "Max"
            : "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.MaxHPIncrease);
    }


    public void OnSelect(BaseEventData eventData)
    {
        ani.SetTrigger("Select");
    }

    public void OnPressed()
    {
        LevelUpPanel.SetActive(false);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ani.SetTrigger("Idle");
    }

    public void OnLevelUp()
    {
        player.LevelUp();
    }

}
