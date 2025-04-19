using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class SkillManager : MonoBehaviour
{
    //Action 타입은 입력과 출력이 없는 메서드를 가리킬 수 있는 델리게이트
    //각 키에 할당할 스킬
    private Dictionary<KeyCode, Action> skillSlots = new Dictionary<KeyCode, Action>();
    private List<Action> autoskillSlots = new List<Action>();
    //스킬 업그레이드 레벨 저장
    private Dictionary<string, int> skillUpgradeLevels = new Dictionary<string, int>();
    //배운 스킬 리스트
    private List<Action> skillList = new List<Action>();
    PlayerInput playerInput;

    // 등록 가능한 슬롯 리스트 
    private readonly KeyCode[] slotKeys = { KeyCode.Q, KeyCode.E };

    //스킬 매핑 및 관련 함수 딕셔너리
    private Dictionary<string, Action> skillActionMap;
    private Dictionary<string, Func<float>> skillDamageMap;
    private Dictionary<string, Action<float>> skillUpgradeMap;

    Player player;

    public static SkillManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        player = GameManager.Instance.player;
        playerInput = PlayerInput.Instance;

        foreach (var key in slotKeys)
        {
            skillSlots[key] = null;
        }

        // 스킬 액션 미리 등록
        skillActionMap = new Dictionary<string, Action>()
        {
            { "FireSlashs", playerInput.UseFireSlash },
            { "IcePillar", playerInput.UseIcePillar },
            { "Thunder", playerInput.UseThunder },
            { "Infierno", playerInput.UseInfierno }
        };


        // 스킬 데미지 조회 딕셔너리
        skillDamageMap = new Dictionary<string, Func<float>>()
        {
            { "FireSlashs", ()=> player.skill.fireSlashsDamage },
            { "IcePillar", ()=> player.skill.icePillarDamage },
            { "Thunder", ()=> player.skill.thunderDamage},
            { "Infierno", ()=> player.skill.infiernoDamage }
        };

        skillUpgradeMap = new Dictionary<string, Action<float>>()
        {
            { "FireSlashs", amount => player.skill.fireSlashsDamage += amount },
            { "IcePillar", amount=> player.skill.icePillarDamage+= amount  },
            { "Thunder", amount=> player.skill.thunderDamage+= amount },
            { "Infierno", amount=> player.skill.infiernoDamage+= amount  }
        };

        LearnNewSkill("FireSlashs");
        UpgradeManager.Instance.data.SetDisabled(UpgradeType.FSSkillLearn, true);
        LearnNewSkill("Infierno");
        UpgradeManager.Instance.data.SetDisabled(UpgradeType.IFSkillLearn, true);
    }

    void Update()
    {
        // Q, E에 대해 입력 확인
        foreach (var key in slotKeys)
        {
            if (Input.GetKeyDown(key) && skillSlots[key] != null)
            {
                skillSlots[key]?.Invoke();
            }
        }
    }

    // 배운 스킬을 순서대로 슬롯에 등록
    public void LearnNewSkill(string skillName, bool isAuto = false)
    {
        Action skillAction = GetSkillAction(skillName);
        // 스킬 기본 레벨 설정
        if (!skillUpgradeLevels.ContainsKey(skillName))
        {
            skillUpgradeLevels[skillName] = 1;
        }
        if (skillAction == null)
        {
            Debug.LogWarning($"알 수 없는 스킬 : {skillName}");
            return;
        }
        skillList.Add(skillAction);

        if (isAuto)
        {
            autoskillSlots.Add(skillAction);
        }
        else
        {
            foreach (var key in slotKeys)
            {
                if (skillSlots[key] == null)
                {
                    skillSlots[key] = skillAction;
                    break;
                }
            }
            UIManager.Instance.UpdateSkillIcons(); // 수동 스킬만 UI 반영
        }
    }

    // 스킬 이름에 따라 PlayerInput의 메서드 반환
    public Action GetSkillAction(string skillName)
    {
        if(skillActionMap.TryGetValue(skillName, out Action action))
        {
            return action;
        }
        return null;
    }

    // 키에 맞는 스킬 가져오기
    public Action GetSkill(KeyCode key)
    {
        return skillSlots.ContainsKey(key) ? skillSlots[key] : null;
    }

    public List<Action> GetAutoSkills()
    {
        return autoskillSlots;
    }

    // 스킬 데미지 할당
    public float GetSkillDamage(string skillName)
    {
        if(skillDamageMap.TryGetValue(skillName, out Func<float> getDamage))
        {
            return getDamage();
        }
        return 0f;
    }
     
    // 스킬 데미지 업그레이드 함수
    public void UpgradeSkillDamage(string skillName, float amountPerLevel)
    {
        // skillName이 맞지 않으면
        if (!skillUpgradeLevels.ContainsKey(skillName))
        {
            Debug.LogWarning($"업그레이드 레벨 정보 없음: {skillName}");
            return;
        }
        skillUpgradeLevels[skillName]++;
        int level = skillUpgradeLevels[skillName];
        
        if(skillUpgradeMap.TryGetValue(skillName, out Action<float> upgrade))
        {
            upgrade(amountPerLevel);
        }
    }
}