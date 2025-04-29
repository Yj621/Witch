using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 스킬 관리 및 호출을 담당하는 싱글톤 매니저 클래스
/// </summary>
public class SkillManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SkillManager Instance { get; private set; }

    // 스킬 데이터베이스 에셋
    [SerializeField] private SkillDatabase skillDatabase;

    // Q/E 키 슬롯에 바인딩된 스킬 액션
    private Dictionary<KeyCode, Action> skillSlots = new Dictionary<KeyCode, Action>(2);

    // 런타임 복제된 스킬 데이터
    private Dictionary<string, SkillData> runtimeSkillData;

    // 스킬 레벨 저장
    private Dictionary<string, int> skillLevels;

    // 수동 스킬 실행 액션 맵
    private Dictionary<string, Action> internalSkillActions;

    // 자동 스킬 슬롯(이름, 액션)
    private List<(string skillName, Action action)> autoskillSlots = new List<(string, Action)>();

    // 플레이어 및 입력 참조
    private PlayerInput playerInput;
    private Player player;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 자료구조 초기화
        runtimeSkillData = new Dictionary<string, SkillData>(skillDatabase.allSkills.Length);
        skillLevels = new Dictionary<string, int>(skillDatabase.allSkills.Length);
        internalSkillActions = new Dictionary<string, Action>();

        // Q/E 슬롯 초기화
        skillSlots[KeyCode.Q] = null;
        skillSlots[KeyCode.E] = null;

        // 데이터베이스에 등록된 스킬 복제
        foreach (var data in skillDatabase.allSkills)
        {
            var clone = Instantiate(data);
            clone.name = data.skillName;
            runtimeSkillData[clone.name] = clone;
            skillLevels[clone.name] = 1;
        }
    }

    private void Start()
    {
        // 게임 매니저에서 플레이어 정보 가져오기
        player = GameManager.Instance.player;
        playerInput = PlayerInput.Instance;

        // 수동 스킬 등록
        RegisterInternalSkill("FireSlashs", () => playerInput.UseFireSlash());
        RegisterInternalSkill("Thunder", () => playerInput.UseThunder());

        // 초기 스킬 학습 및 UI 업데이트
        LearnNewSkill("FireSlashs");
        UpgradeManager.Instance.data.SetDisabled(UpgradeType.FSSkillLearn, true);
        LearnNewSkill("Thunder");
        UpgradeManager.Instance.data.SetDisabled(UpgradeType.TDSkillLearn, true);
    }

    private void Update()
    {
        // Q/E 키 입력 처리
        foreach (var kv in skillSlots)
        {
            if (kv.Value != null && Input.GetKeyDown(kv.Key))
            {
                kv.Value.Invoke();
            }
        }
    }

    /// <summary>
    /// 수동 스킬 실행 액션 등록
    /// </summary>
    private void RegisterInternalSkill(string skillName, Action useAction)
    {
        internalSkillActions[skillName] = useAction;
    }

    /// <summary>
    /// 새로운 스킬을 슬롯에 배치하고, 자동 여부 옵션 처리
    /// </summary>
    public void LearnNewSkill(string skillName, bool isAuto = false)
    {
        if (!runtimeSkillData.ContainsKey(skillName))
        {
            Debug.LogWarning($"스킬 '{skillName}'이(가) 존재하지 않습니다.");
            return;
        }

        Action invokeAction = () => InvokeSkill(skillName);

        if (isAuto)
        {
            autoskillSlots.Add((skillName, invokeAction));
        }
        else
        {
            // 빈 슬롯(Q -> E) 찾기
            foreach (var key in new[] { KeyCode.Q, KeyCode.E })
            {
                if (skillSlots[key] == null)
                {
                    skillSlots[key] = invokeAction;
                    UIManager.Instance.UpdateSkillIcons();
                    return;
                }
            }
            Debug.LogWarning("빈 스킬 슬롯이 없습니다.");
        }
    }

    /// <summary>
    /// 슬롯 키에 매핑된 수동 스킬 반환
    /// </summary>
    public Action GetSkill(KeyCode key)
    {
        return skillSlots.TryGetValue(key, out var action) ? action : null;
    }

    /// <summary>
    /// 자동 스킬 리스트 반환
    /// </summary>
    public List<(string skillName, Action action)> GetAutoSkills()
    {
        return autoskillSlots;
    }

    /// <summary>
    /// 런타임 복제된 SkillData 반환
    /// </summary>
    public SkillData GetRuntimeSkillData(string skillName)
    {
        if (runtimeSkillData.TryGetValue(skillName, out var data))
            return data;
        Debug.LogWarning($"런타임 스킬 데이터가 없습니다: {skillName}");
        return null;
    }

    /// <summary>
    /// 스킬 실행: 수동/자동 분기 처리
    /// </summary>
    private void InvokeSkill(string skillName)
    {
        // 수동 스킬 실행
        if (internalSkillActions.TryGetValue(skillName, out var manualAction))
        {
            manualAction.Invoke();
            return;
        }

        // 자동 스킬 실행
        var autoNames = new[] { "IcePillar", "Blackhole", "Infierno", "ThunderStrike" };
        if (autoNames.Contains(skillName))
        {
            var obj = GameManager.Instance.autoSkillPool.GetSkillObject(skillName);
            obj.GetComponent<MonoBehaviour>().Invoke("StartSkill", 0f);
        }
    }

    /// <summary>
    /// 스킬 데미지 반환
    /// </summary>
    public float GetSkillDamage(string skillName)
    {
        return runtimeSkillData.TryGetValue(skillName, out var data) ? data.damage : 0f;
    }

    /// <summary>
    /// Q/E 스킬 데미지 업그레이드
    /// </summary>
    public void UpgradeQESkillDamage(string skillName, float amountPerLevel)
    {
        if (runtimeSkillData.TryGetValue(skillName, out var data))
        {
            data.damage += amountPerLevel;
            skillLevels[skillName]++;
        }
        else
        {
            Debug.LogWarning($"업그레이드 대상 스킬 없음: {skillName}");
        }
    }

    /// <summary>
    /// 패시브 스킬 데미지 업그레이드
    /// </summary>
    public void UpgradePassiveSkill(string skillName, float amount)
    {
        if (runtimeSkillData.TryGetValue(skillName, out var data))
        {
            data.damage += amount;
            skillLevels[skillName]++;
        }
        else
        {
            Debug.LogWarning($"패시브 업그레이드 대상 스킬 없음: {skillName}");
        }
    }

    /// <summary>
    /// 패시브 스킬 쿨타임 감소 업그레이드
    /// </summary>
    public void UpgradePassiveCooltime(string skillName, float amount)
    {
        if (runtimeSkillData.TryGetValue(skillName, out var data))
        {
            data.duration = Mathf.Max(0f, data.duration - amount);
            skillLevels[skillName]++;
        }
        else
        {
            Debug.LogWarning($"쿨타임 업그레이드 대상 스킬 없음: {skillName}");
        }
    }
}
