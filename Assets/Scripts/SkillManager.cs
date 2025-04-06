using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class SkillManager : MonoBehaviour
{
    //Action 타입은 입력과 출력이 없는 메서드를 가리킬 수 있는 델리게이트
    private Dictionary<KeyCode, Action> skillSlots = new Dictionary<KeyCode, Action>();
    private List<Action> skillList = new List<Action>();
    [SerializeField] private PlayerInput playerInput;


    // 등록 가능한 슬롯 리스트 
    private readonly KeyCode[] slotKeys = { KeyCode.Q, KeyCode.E };

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

        foreach (var key in slotKeys)
        {
            skillSlots[key] = null;
        }

    }

    void Update()
    {
        // Q, E에 대해 입력 확인
        foreach (var key in slotKeys)
        {
            if (Input.GetKeyDown(key) && skillSlots[key] != null)
            {
                Debug.Log($"{key} 슬롯에서 실행된 스킬: {skillSlots[key].Method.Name}");
                skillSlots[key]?.Invoke();
            }
        }
    }

    // 배운 스킬을 순서대로 슬롯에 등록
    public void LearnNewSkill(string skillName)
    {
        Action skillAction = GetSkillAction(skillName);
        Debug.Log("스킬 아이콘 업데이트");
        if (skillAction == null)
        {
            Debug.LogWarning($"알 수 없는 스킬 : {skillName}");
            return;
        }
        skillList.Add(skillAction);
        foreach (var key in slotKeys)
        {
            if (skillSlots[key] == null)
            {
                skillSlots[key] = skillAction;
                Debug.Log($"{key}에 {skillName}스킬이 등록됨");
                break;
            }
        }
        //스킬 아이콘 업데이트
        UIManager.Instance.UpdateSkillIcons();
    }

    // 스킬 이름에 따라 PlayerInput의 메서드 반환
    public Action GetSkillAction(string skillName)
    {
        switch (skillName)
        {
            case "FireSlashs":
                return playerInput.UseFireSlash;
            case "IcePillar":
                return playerInput.UseIcePillar;
            case "Thunder":
                return playerInput.UseThunder;
            case "BlackHole":
                return playerInput.UseBlackHole;
            case "Infierno":
                return playerInput.UseInfierno;
            default:
                Debug.LogWarning($"알 수 없는 스킬 : {skillName}");
                return null;
        }
    }

    // 키에 맞는 스킬 가져오기
    public Action GetSkill(KeyCode key)
    {
        return skillSlots.ContainsKey(key) ? skillSlots[key] : null;
    }



    // 스킬 데미지 할당
    public float GetSkillDamage(string skillName)
    {
        switch (skillName)
        {
            case "FireSlashs": return player.skill.fireSlashsDamage;
            case "IcePillar": return player.skill.icePillarDamage;
            case "Thunder": return player.skill.thunderDamage;
            case "Infierno": return player.skill.infiernoDamage;
            default:
                Debug.LogWarning($"알 수 없는 스킬 데미지 요청: {skillName}");
                return 0f;
        }
    }

}