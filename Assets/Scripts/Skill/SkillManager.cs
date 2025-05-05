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

    // 스킬 쿨타임
    private Dictionary<string, float> nextUseTime = new Dictionary<string, float>();

    private Dictionary<KeyCode, string> skillSlotNames = new Dictionary<KeyCode, string>(2);

    // 플레이어 및 입력 참조
    private PlayerInput playerInput;
    private Player player;

    public GameObject playerGo;

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

        // 딕셔너리/리스트 초기화
        // Q/E 슬롯
        skillSlots = new Dictionary<KeyCode, Action>(2)
        {
            [KeyCode.Q] = null,
            [KeyCode.E] = null
        };

        // 수동 스킬 맵
        internalSkillActions = new Dictionary<string, Action>();

        // 스킬 레벨 저장
        skillLevels = new Dictionary<string, int>(skillDatabase.allSkills.Length);

        // 런타임 스킬 데이터
        runtimeSkillData = new Dictionary<string, SkillData>(skillDatabase.allSkills.Length);

        // 쿨타임 맵
        nextUseTime = new Dictionary<string, float>(skillDatabase.allSkills.Length);

        // 자동 스킬 슬롯 리스트
        autoskillSlots = new List<(string, Action)>();

        // 데이터베이스에서 클론 떠서 각종 맵에 채우기
        foreach (var d in skillDatabase.allSkills)
        {
            var clone = Instantiate(d);
            clone.name = d.skillName;

            runtimeSkillData[d.skillName] = clone;
            skillLevels[d.skillName] = 1;
            nextUseTime[d.skillName] = 0f;
        }
            skillSlotNames = new Dictionary<KeyCode,string>() {
        [KeyCode.Q] = null,
        [KeyCode.E] = null
    };
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
        LearnNewSkill("FireSlashs", false);
        UpgradeManager.Instance.data.SetDisabled(UpgradeType.FSSkillLearn, true);
        LearnNewSkill("Thunder", false);
        UpgradeManager.Instance.data.SetDisabled(UpgradeType.TDSkillLearn, true);


        UIManager.Instance.UpdateSkillIcons();
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
                    skillSlotNames[key] = skillName;

                    return;
                }
            }
            Debug.LogWarning("빈 스킬 슬롯이 없습니다.");
        }
        UIManager.Instance.UpdateSkillIcons();
    }

    // 스킬 이름 리턴
    public string GetSkillName(KeyCode key)
        => skillSlotNames.TryGetValue(key, out var name) ? name : null;


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
    /// 스킬 실행: 수동 스킬인지 먼저 확인 후, 그렇지 않으면 자동 풀 이용
    /// </summary>
    private void InvokeSkill(string skillName)
    {
        // 1) 수동 스킬 처리
        if (internalSkillActions.TryGetValue(skillName, out var manualAction))
        {
            manualAction();
            return;
        }

        // 2) 자동 스킬 처리
        var data = runtimeSkillData[skillName];
        float now = Time.time;
        if (now < nextUseTime[skillName]) return;

        var obj = GameManager.Instance.autoSkillPool.GetSkillObject(skillName);
        if (obj == null)
        {
            Debug.LogError($"AutoSkillPool에 '{skillName}' 케이스가 없습니다.");
            return;
        }

        // 스폰 위치 계산
        Vector3 spawnPos = playerGo.transform.position;
        var hits = Physics2D.OverlapCircleAll(playerGo.transform.position, 10f, LayerMask.GetMask("Enemy"));
        if (hits.Length > 0)
        {
            var nearest = hits.OrderBy(h => Vector2.Distance(playerGo.transform.position, h.transform.position)).First();
            spawnPos = nearest.transform.position;
        }
        obj.transform.position = spawnPos;

        // 충돌 무시
        var playerCol = playerGo.GetComponent<Collider2D>();
        var skillCol  = obj.GetComponent<CircleCollider2D>();
        if (playerCol != null && skillCol != null)
            Physics2D.IgnoreCollision(playerCol, skillCol);

        // 범위 설정
        if (obj.TryGetComponent<ISkill>(out var skillComp))
            skillComp.InitializeRange(data.radius);
        else
            obj.transform.localScale = Vector3.one * data.radius;

        obj.SetActive(true);
        nextUseTime[skillName] = now + data.cooltime;
        UIManager.Instance.StartCooldownUI(skillName, data.cooltime);
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
            data.cooltime = Mathf.Max(0f, data.cooltime - amount);
            skillLevels[skillName]++;
        }
        else
        {
            Debug.LogWarning($"쿨타임 업그레이드 대상 스킬 없음: {skillName}");
        }
    }

    /// <summary>
    /// 스킬 범위(radius) 업그레이드
    /// </summary>
    public void UpgradeSkillRange(string skillName, float amount)
    {
        if (runtimeSkillData.TryGetValue(skillName, out var data))
        {
            data.radius += amount;
            skillLevels[skillName]++;
        }
        else
        {
            Debug.LogWarning($"업그레이드 대상 스킬 없음: {skillName}");
        }
    }

    
}
