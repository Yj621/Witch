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
    private PlayerInput playerInput;
    [SerializeField] private float defaultSkillCooldown;

    // 등록 가능한 슬롯 리스트 
    private readonly KeyCode[] slotKeys = { KeyCode.Q, KeyCode.E };

    Player player;
    void Awake()
    {
        player = GameManager.Instance.player;
    }
    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        foreach (var key in slotKeys)
        {
            skillSlots[key] = null;
        }

        // 일정 시간마다 OnDefaultSkill 자동 실행
        StartCoroutine(AutoFireDefaultSkill());
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
                return;
            }
        }
    }
    // 스킬 이름에 따라 PlayerInput의 메서드 반환
    public Action GetSkillAction(string skillName)
    {
        switch (skillName)
        {
            case "FireSlahs":
                return playerInput.UseFireSlahs;
            case "IcePillar":
                return playerInput.UseIcePillar;
            case "Thunder":
                return playerInput.UseThunder;
            case "BlackHole":
                return playerInput.UseBlackHole;

            default:
                Debug.LogWarning($"알 수 없는 스킬 : {skillName}");
                return null;
        }
    }

    public Action GetSkill(KeyCode key)
    {
        return skillSlots.ContainsKey(key) ? skillSlots[key] : null;
    }

    private IEnumerator AutoFireDefaultSkill()
    {
        while (true)
        {
            yield return new WaitForSeconds(defaultSkillCooldown);
            DefaultSkill();
        }
    }

    //기본 스킬 
    public void DefaultSkill()
    {
        Vector2 bulletV = new Vector2(3, 0);
        float direction = Mathf.Sign(transform.parent.localScale.x); // 캐릭터 방향 (왼쪽:-1, 오른쪽:1)

        // 총알 생성 및 위치 설정
        GameObject bullet = GameManager.Instance.skillObjectPool.GetObject();
        Vector2 spawnPosition = transform.position + new Vector3(0.5f * direction, 0, 0);
        bullet.transform.position = spawnPosition;

        // 총알 속도 및 방향 설정
        bullet.GetComponent<DefaultSkill>().velocity = new Vector2(10f * direction, 0);
        bullet.transform.localScale = new Vector3(direction * Mathf.Abs(bullet.transform.localScale.x),
                                          bullet.transform.localScale.y,
                                          bullet.transform.localScale.z);
    }

    public void ApplySkillDamage()
    {
        //현재 실행중인 애니메이션 이름
        string currentSkill = playerInput.GetCurrentTriggerName();

        float damage = GetSkillDamage(currentSkill);
        if (damage > 0)
        {
            //other.GetComponent<EnemyMove>().Hit(damage);
        }
    }
    public float GetSkillDamage(string skillName)
    {
        switch (skillName)
        {
            case "FireSlahs": return player.skill.fireSlashsDamage;
            case "IcePillar": return player.skill.icePillarDamage;
            case "Thunder": return player.skill.thunderDamage;
            case "BlackHole": return player.skill.infiernoDamage;
            default:
                Debug.LogWarning($"알 수 없는 스킬 데미지 요청: {skillName}");
                return 0f;
        }
    }

}