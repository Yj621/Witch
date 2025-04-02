using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //Action 타입은 입력과 출력이 없는 메서드를 가리킬 수 있는 델리게이트
    private Dictionary<KeyCode, Action> skillSlots = new Dictionary<KeyCode, Action>();
    private List<Action> skillList = new List<Action>();
    private PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();

        //기본 설정
        skillSlots[KeyCode.Q] = null;
        skillSlots[KeyCode.E] = null;
    }

    void Update()
    {
        // Q 키를 눌렀을 때 실행되는 스킬 로그 출력
        if (Input.GetKeyDown(KeyCode.Q) && skillSlots[KeyCode.Q] != null)
        {
            Debug.Log("Q 슬롯에서 실행된 스킬: " + skillSlots[KeyCode.Q].Method.Name);
            skillSlots[KeyCode.Q]?.Invoke();
        }

        // E 키를 눌렀을 때 실행되는 스킬 로그 출력
        if (Input.GetKeyDown(KeyCode.E) && skillSlots[KeyCode.E] != null)
        {
            Debug.Log("E 슬롯에서 실행된 스킬: " + skillSlots[KeyCode.E].Method.Name);
            skillSlots[KeyCode.E]?.Invoke();
        }
    }

    // 배운 스킬을 순서대로 Q,E에 넣음
    public void LearnNewSkill(string skillName)
    {
        Action skillAction = null;

        switch(skillName)
        {
            case "FireBall":
                skillAction = playerInput.UseFireBall;
                break;
            case "IcePillar":
                skillAction = playerInput.UseIcePillar;
                break;
            case "Thunder":
                skillAction = playerInput.UseThunder;
                break;
            case "BlackHole":
                skillAction = playerInput.UseBlackHole;
                break;
            default:
                Debug.LogWarning($"알 수 없는 스킬 : {skillName}");
                return;
        }
        skillList.Add(skillAction);

        if (skillSlots[KeyCode.Q] == null)
        {
            skillSlots[KeyCode.Q] = skillAction;
            Debug.Log($"Q 슬롯에 {skillName} 스킬이 등록됨");
        }
        else if (skillSlots[KeyCode.E] == null)
        {
            skillSlots[KeyCode.E] = skillAction;
            Debug.Log($"E 슬롯에 {skillName} 스킬이 등록됨");
        }
        else
        {
            Debug.Log("슬롯이 가득 찼습니다.");
        }
    }

    public Action GetSkill(KeyCode key)
    {
        return skillSlots.ContainsKey(key) ? skillSlots[key] : null;
    }

}
