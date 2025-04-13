using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSkill : MonoBehaviour
{
    private Animator skillAnimator;
    private PlayerInput playerInput;
    private SkillManager skillManager;

    public float defaultSkillCooldown;

    // 스킬 쿨다운 타이머 관리
    private Dictionary<string, float> skillCooldownTimers = new Dictionary<string, float>();
    public Dictionary<string, float> addSkillCooldown = new Dictionary<string, float>()
    {
        { "IcePillar", 3f },
        { "Blackhole", 4f },
        { "Infierno", 2f }
    };

    [SerializeField] private Transform skillSpawnPoint;
    [SerializeField] private Transform playerTransform;

    public static PlayerSkill Instance { get; private set; }

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
        skillAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        skillManager = SkillManager.Instance;
        playerInput = PlayerInput.Instance;

        // 기본 스킬 자동 발사 시작
        StartCoroutine(AutoFireDefaultSkill());

        // 추가 스킬 쿨다운 타이머 초기화
        foreach (var skill in addSkillCooldown)
        {
            skillCooldownTimers[skill.Key] = 0f;
        }

    }

    // 스킬 애니메이션과 동기화
    public void SyncSkillAnimation()
    {
        if (playerInput == null || skillAnimator == null) return;

        string currentState = playerInput.GetCurrentTriggerName();
        if (IsNonSkillState(currentState)) return;

        float damage = skillManager.GetSkillDamage(currentState);
        Debug.Log($"currentState는 {currentState}, 데미지는 {damage}");

        skillAnimator.SetTrigger(currentState);
    }

    private bool IsNonSkillState(string state)
    {
        return state == "Idle" || state == "Walk" || state == "Dash" || state == "Die";
    }

    // 기본 스킬 자동 발사
    private IEnumerator AutoFireDefaultSkill()
    {
        while (true)
        {
            yield return new WaitForSeconds(defaultSkillCooldown);
            DefaultSkill();
        }
    }

    // 기본 스킬 발사
    public void DefaultSkill()
    {
        FireSkill(GameManager.Instance.skillObjectPool.GetFireObject(), 4f);
    }

// 추가 스킬 자동 발사
public IEnumerator AutoAddSkills()
{
    while (true)
    {
        // SkillManager에서 현재 배운 스킬 목록 가져오기
        var learnedSkills = SkillManager.Instance.GetAutoSkills();

        foreach (var skill in addSkillCooldown.Keys)
        {
            // 배운 스킬인지 확인
            if (!learnedSkills.Exists(action => SkillManager.Instance.GetSkillAction(skill) == action))
                continue;

            // 스킬 발사 처리
            if (Time.time >= skillCooldownTimers[skill])
            {
                skillCooldownTimers[skill] = Time.time + addSkillCooldown[skill];
                AddSkill(skill);
                Debug.Log($"Learned skill fired: {skill}");
            }
        }
        yield return null; // 매 프레임 체크
    }
}

    // 추가 스킬 발사
    private void AddSkill(string skillName)
    {
        GameObject skill = GameManager.Instance.skillObjectPool.GetSkillObject(skillName);
        FireSkill(skill, 0f); // 속도는 스킬별로 설정 가능
    }

    // 스킬 발사 로직
    private void FireSkill(GameObject skill, float speed)
    {
        if (skill == null) return;

        float direction = Mathf.Sign(playerTransform.localScale.x); // 캐릭터 방향
        Vector2 spawnPosition = skillSpawnPoint.position;

        skill.transform.position = spawnPosition;
        skill.transform.rotation = Quaternion.identity;

        // 좌우 반전 설정
        skill.transform.localScale = new Vector3(direction * Mathf.Abs(skill.transform.localScale.x),
                                                  skill.transform.localScale.y,
                                                  skill.transform.localScale.z);

        // 속도 설정
        var skillComponent = skill.GetComponent<DefaultSkill>();
        if (skillComponent != null)
        {
            skillComponent.velocity = new Vector2(speed * direction, 0);
        }
    }
}
