using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UpgradeStat
{
    Damage,
    Cooltime,
    Range
}

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
        skillSlotNames = new Dictionary<KeyCode, string>()
        {
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

        //자동 스킬은 다른 슬롯에 배치
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

        return null;
    }

    /// <summary>
    /// 스킬 실행: 수동 스킬인지 먼저 확인 후, 그렇지 않으면 자동 풀 이용
    /// </summary>
    private void InvokeSkill(string skillName)
    {
        float now = Time.time;
        // 스킬 데이터 가져오기
        if (!runtimeSkillData.TryGetValue(skillName, out var data))
        {
            return;
        }

        // 쿨타임 체크
        if (now < nextUseTime[skillName])
        {
            // 아직 쿨타임이 남아있으면 그냥 무시
            return;
        }

        // 쿨타임 시작 (다음 사용 가능 시간 설정)
        nextUseTime[skillName] = now + data.cooltime;

        // 쿨다운 UI 시작
        UIManager.Instance.StartCooldownUI(skillName, data.cooltime);

        // 실제 스킬 실행 (내부 액션 또는 오토 스킬 분기)
        if (internalSkillActions.TryGetValue(skillName, out var manualAction))
        {
            manualAction();  // UseFireSlash() 또는 UseThunder() 호출
        }
        else
        {
            SpawnAutoSkill(skillName);
        }
    }


    /// <summary>
    /// 자동 스킬 오브젝트를 풀에서 꺼내어 설정 후 활성화
    /// </summary>
    private void SpawnAutoSkill(string skillName)
    {
        // 스킬 데이터 가져오기
        var data = runtimeSkillData[skillName];

        // 풀에서 오브젝트 꺼내기
        var obj = GameManager.Instance.autoSkillPool.GetSkillObject(skillName);
        if (obj == null)
        {
            Debug.LogError($"AutoSkillPool에 '{skillName}' 케이스가 없습니다.");
            return;
        }

        // 스폰 위치 계산 (플레이어 위치 혹은 가장 가까운 적 위치)
        Vector3 spawnPos = playerGo.transform.position;
        var hits = Physics2D.OverlapCircleAll(playerGo.transform.position, 10f, LayerMask.GetMask("Enemy"));
        if (hits.Length > 0)
        {
            var nearest = hits
                .OrderBy(h => Vector2.Distance(playerGo.transform.position, h.transform.position))
                .First();
            spawnPos = nearest.transform.position;
        }
        obj.transform.position = spawnPos;

        // 플레이어와 충돌 무시
        var playerCol = playerGo.GetComponent<Collider2D>();
        var skillCol = obj.GetComponent<Collider2D>();
        if (playerCol != null && skillCol != null)
            Physics2D.IgnoreCollision(playerCol, skillCol);

        // 범위(radio) 설정
        if (obj.TryGetComponent<ISkill>(out var skillComp))
            skillComp.InitializeRange(data.radius);
        else
            obj.transform.localScale = Vector3.one * data.radius;

        // 오브젝트 활성화
        obj.SetActive(true);
    }

    public void SetNextUseTime(string skillName, float time)
    {
        if (nextUseTime.ContainsKey(skillName))
            nextUseTime[skillName] = time;
        else
            Debug.LogWarning($"쿨타임 설정 대상 스킬이 없습니다: {skillName}");
    }

    // 스킬의 특정 스탯(데미지, 쿨타임, 범위) 강화 함수
    public void UpgradeSkillStat(string skillName, UpgradeStat stat, float amount)
    {
        if(!runtimeSkillData.TryGetValue(skillName, out var data))
        {
            Debug.LogWarning($"업그레이드 대상 스킬이 없음 : {skillName}");
            return;
        }
        switch (stat)
        {
            case UpgradeStat.Damage:
                data.damage += amount;
                break;

            case UpgradeStat.Cooltime:
                data.cooltime += amount;
                break;

            case UpgradeStat.Range:
                data.radius += amount;
                break;

        }
        //레벨 저장/카운트업
        if (skillLevels.ContainsKey(skillName))
            skillLevels[skillName]++;

    }

    /*
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
        /// Q/E 스킬(수동 스킬)의 쿨타임 감소 업그레이드
        /// </summary>
        public void ReduceQESkillCooltime(string skillName, float amount)
        {
            if (!runtimeSkillData.TryGetValue(skillName, out var data))
            {
                Debug.LogWarning($"쿨타임 감소 대상 스킬이 없습니다: {skillName}");
                return;
            }
            // 최소 0초가 되지 않도록 clamp
            data.cooltime = Mathf.Max(0f, data.cooltime - amount);
            // 필요하다면 레벨 카운트 업
            skillLevels[skillName]++;
            Debug.Log($"[{skillName}] 쿨타임이 {amount}초 감소하여, 이제 {data.cooltime}초 입니다.");
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
                Debug.Log($"data.cooltime : {data.cooltime}");
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
        }*/


}
