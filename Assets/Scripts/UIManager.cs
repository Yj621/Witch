using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider HpSlider;
    public Slider ExpSlider;
    public Slider CleanSlider;
    public GameObject LevelUpPanel;
    public GameObject EscPanel;
    public GameObject GameOverPanel;
    private bool isEscOpen = false;
    Player player;
    [SerializeField] private Image Qskill;
    [SerializeField] private Image Eskill;
    [SerializeField] private Image P1Skill;
    [SerializeField] private Image P2Skill;
    [SerializeField] private Image P3Skill;
    [SerializeField] private UpgradeDataBase upgradeDB;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject HowToPanel;
    public Sprite[] skillIcons;
    SkillManager skillManager;
    Animator ani;
    [SerializeField] public TextMeshProUGUI DCText;
    [SerializeField] public TextMeshProUGUI DSText;
    [SerializeField] public TextMeshProUGUI PSText;
    [SerializeField] public TextMeshProUGUI EPText;
    [SerializeField] public TextMeshProUGUI HPText;
    [SerializeField] public TextMeshProUGUI LvText;

    [SerializeField] public TextMeshProUGUI CoinText;

    private float escBlockTime = 2f;
    private float startTime;
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
        startTime = Time.time;


        player = GameManager.Instance.player;
        skillManager = SkillManager.Instance;
        HpSlider.maxValue = player.maxHp;
        ExpSlider.maxValue = player.maxExp;
        CleanSlider.maxValue = 100;
        ani = GetComponent<Animator>();
        LevelUpPanel.SetActive(false);
        UpdateStatNum();
        upgradeDB.ResetAllLearned();
        StartCoroutine(HowToPlay());
        SoundManager.Instance.PlayBGM("GameSceneBGM");
    }

    void Update()
    {
        HpSlider.value = player.Hp;
        ExpSlider.maxValue = player.maxExp;
        ExpSlider.value = player.Exp;
        CleanSlider.value = GameManager.Instance.currentClean;
        LvText.text = "Lv " + player.level;
        if (Time.time - startTime >= escBlockTime)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ToggleEscapePanel();
            }
        }

        CoinText.text = Coin.coin.ToString();
    }

    //스킬 아이콘 업데이트
    public void UpdateSkillIcons()
    {
        UpdateSkillIconKey(KeyCode.Q, Qskill, skillManager);
        UpdateSkillIconKey(KeyCode.E, Eskill, skillManager);
        UpdateSkillIconNonKey(0, P1Skill, skillManager);
        UpdateSkillIconNonKey(1, P2Skill, skillManager);
        UpdateSkillIconNonKey(2, P3Skill, skillManager);
    }

    
    

    private void UpdateSkillIconKey(KeyCode key, Image image, SkillManager skillManager)
    {
        string skillName = skillManager.GetSkillName(key);

        //스킬이 없으면
        if(skillName == null)
        {
            image.sprite = null;
            image.enabled = false;
            return;
        }


        int index = -1;


        switch(skillName)
        {
            case "FireSlashs":
                index = 0;
                break;
            case "Thunder":
                index = 1;
                break;
            default:
                break;
        }

        if (index >= 0 && index < skillIcons.Length)
        {
            image.sprite   = skillIcons[index];
            image.enabled  = true;
        }
        else
        {
            image.enabled = false;
        }
    }

    private void UpdateSkillIconNonKey(int indexInList, Image image, SkillManager skillManager)
    {
        var autoSkills = skillManager.GetAutoSkills(); // List<(string, Action)>

        if (indexInList >= autoSkills.Count)
        {
            image.sprite = null;
            image.enabled = false;
            return;
        }

        string skillName = autoSkills[indexInList].skillName;

        int iconIndex = 1;
        switch (skillName)
        {
            case "IcePillar":
                iconIndex = 2;
                Debug.Log("A");
                break;
            case "Infierno":
                iconIndex = 3;
                Debug.Log("B");
                break;
            case "Blackhole":
                iconIndex = 4;
                Debug.Log("C");
                break;
            case "ThunderStrike":
                iconIndex = 5;
                Debug.Log("D");
                break;
            default:
                break;
        }

        if (iconIndex >= 0 && iconIndex < skillIcons.Length)
        {
            image.sprite = skillIcons[iconIndex];
            image.enabled = true;
        }
        else
        {
            image.sprite = null;
            image.enabled = false;
        }
    }

    /// <summary>
    /// 스킬 쿨타임을 아이콘에 표시하고 감소
    /// </summary>
    /// <param name="skillName">쿨타임을 표시할 스킬 이름</param>
    /// <param name="duration">쿨타임 시간(초)</param>
    public void StartCooldownUI(string skillName, float duration)
    {
        // 해당 스킬에 대응하는 Image 컴포넌트를 찾기
        Image icon = GetSkillIconImage(skillName);
        if (icon == null)
            return;

        Transform maskT = icon.transform.GetChild(0);

        Image maskImage = maskT.GetComponent<Image>();

        maskImage.enabled = true;
        StartCoroutine(CooldownCoroutine(maskImage, duration));
    }

    /// <summary>
    /// 아이콘의 fillAmount를 duration초에 걸쳐 1→0으로 감소시킵니다.
    /// </summary>
    private IEnumerator CooldownCoroutine(Image maskImage, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            maskImage.fillAmount = Mathf.Clamp01(1f - (elapsed / duration));
            yield return null;
        }
        maskImage.fillAmount = 0f;
    }


    /// <summary>
    /// skillName에 맞는 Image 컴포넌트를 반환
    /// Q/E 스킬은 Qskill/Eskill, 자동 스킬은 P1~P3Skill 중 하나를 리턴
    /// </summary>
    private Image GetSkillIconImage(string skillName)
    {
        // 수동(Q/E) 스킬 매핑
        switch (skillName)
        {
            case "FireSlashs":
                return Qskill;
            case "Thunder":
                return Eskill;
        }

        // 자동 스킬 슬롯 매핑
        var autos = skillManager.GetAutoSkills(); // List<(string, Action)>
        for (int i = 0; i < autos.Count; i++)
        {
            if (autos[i].skillName == skillName)
            {
                switch (i)
                {
                    case 0: return P1Skill;
                    case 1: return P2Skill;
                    case 2: return P3Skill;
                }
            }
        }

        return null;
    }




    //스킬 레벨, 데미지 갱신
    public void LevelUpPanelPop()
    {
        UpgradePanelController.Instance.ClearCards();
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

/*        DSText.text = UpgradeManager.Instance.IsMaxLevel(UpgradeType.DefaultSkillRange)
            ? "Max"
            : "Lv " + UpgradeManager.Instance.GetLevel(UpgradeType.DefaultSkillRange);
*/
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

    private void ToggleEscapePanel()
    {
        SoundManager.Instance.PlaySFX("ButtonClickSfx");
        isEscOpen = !isEscOpen;
        EscPanel.SetActive(isEscOpen);

        if (isEscOpen)
        {
            // 일시정지 하고 싶으면
            Time.timeScale = 0f;
        }
        else
        {
            // 다시 게임 재개
            Time.timeScale = 1f;
        }
    }

    public void GameOverPanelOpen()
    {
        GameOverPanel.SetActive(true);
    }

    public IEnumerator HowToPlay()
    {
        HowToPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        HowToPanel.SetActive(false);
    }
}
