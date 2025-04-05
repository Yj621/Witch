using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider HpSlider;
    public Slider ExpSlider;
    Player player;
    [SerializeField] private Image Qskill;
    [SerializeField] private Image Eskill;
    public Sprite[] skillIcons;
    SkillManager skillManager;
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
    }

    void Update()
    {
        HpSlider.value = player.Hp;
        ExpSlider.maxValue = player.maxExp;
        ExpSlider.value = player.Exp;
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
}
