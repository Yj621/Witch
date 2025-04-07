using System;
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
    public Sprite[] skillIcons;
    SkillManager skillManager;
    Animator ani;
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
        HpSlider.maxValue = player.Hp;
        ExpSlider.maxValue = player.maxExp;
        CleanSlider.maxValue = 100;
        ani = GetComponent<Animator>();
        LevelUpPanel.SetActive(false);
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

    public void OnUpgradeFireSlash()
    {
        SkillManager.Instance.UpgradeSkillDamage("FireSlashs", 10f);
    }
    public void OnUpgradeIcePillar()
    {
        SkillManager.Instance.UpgradeSkillDamage("IcePillar", 10f);
    }
    public void OnUpgradeThunder()
    {
        SkillManager.Instance.UpgradeSkillDamage("Thunder", 10f);
    }
    public void OnUpgradeInfierno()
    {
        SkillManager.Instance.UpgradeSkillDamage("Infierno", 10f);
    }

    //스킬 레벨, 데미지 갱신
    public void UpdateSkillUpgradeUI(string skillName, int level, float damage)
    {
        Debug.Log($"UI 갱신 : {skillName} 레벨 {level}, 데미지 {damage}");
    }

    public void LevelUpPanelPop()
    {
        LevelUpPanel.SetActive(true);
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
