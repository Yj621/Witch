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
        LearnNewSkill("Thunder", false);
        UpgradeManager.Instance.data.SetDisabled(UpgradeType.FSSkillLearn, true);
        UpgradeManager.Instance.data.SetDisabled(UpgradeType.TDSkillLearn, true);

        if (PlayerPrefs.GetInt("IceLrn") == 1)
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.IPSkillLearn, true);
        else
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.IPSkillLearn, false);

        if (PlayerPrefs.GetInt("InfLrn") == 1)
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.IFSkillLearn, true);
        else 
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.IFSkillLearn, false);

        if (PlayerPrefs.GetInt("BlackLrn") == 1)
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.BHSkillLearn, true);
        else
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.BHSkillLearn, false);

        UIManager.Instance.UpdateSkillIcons();
    }


    private void Update()
    {
    }
    
    public void ReStartInit()
    {
        // 게임 매니저에서 플레이어 정보 가져오기
        player = GameManager.Instance.player;
        playerInput = PlayerInput.Instance;

        // 수동 스킬 등록
        RegisterInternalSkill("FireSlashs", () => playerInput.UseFireSlash());
        RegisterInternalSkill("Thunder", () => playerInput.UseThunder());

        // 초기 스킬 학습 및 UI 업데이트
        LearnNewSkill("FireSlashs", false);
        LearnNewSkill("Thunder", false);
        UpgradeManager.Instance.data.SetDisabled(UpgradeType.FSSkillLearn, true);
        UpgradeManager.Instance.data.SetDisabled(UpgradeType.TDSkillLearn, true);


        if (PlayerPrefs.GetInt("IceLrn") == 1)
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.IPSkillLearn, true);
        else
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.IPSkillLearn, false);

        if (PlayerPrefs.GetInt("InfLrn") == 1)
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.IFSkillLearn, true);
        else
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.IFSkillLearn, false);

        if (PlayerPrefs.GetInt("BlackLrn") == 1)
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.BHSkillLearn, true);
        else
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.BHSkillLearn, false);

        UIManager.Instance.UpdateSkillIcons();
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
            // 중복 방지
            if (autoskillSlots.Any(s => s.skillName == skillName))
            {
                Debug.Log($"이미 등록된 자동 스킬: {skillName}");
            }
            else
            {
                if (autoskillSlots.Count >= 3)
                {
                    Debug.LogWarning("자동 스킬 슬롯이 가득 찼습니다 (최대 3개).");
                }
                else
                {
                    autoskillSlots.Add((skillName, invokeAction));
                }
            }

            // 아이콘 즉시 갱신
            UIManager.Instance.UpdateSkillIcons();

            // 자동 발사 코루틴 시작
            PlayerSkill.Instance.StartAutoSkill(skillName);
        }
        else
        {
            foreach (var key in new[] { KeyCode.Q, KeyCode.E })
            {
                if (skillSlots[key] == null)
                {
                    skillSlots[key] = invokeAction;
                    skillSlotNames[key] = skillName;
                    UIManager.Instance.UpdateSkillIcons();
                    return;
                }
            }
            Debug.LogWarning("빈 스킬 슬롯이 없습니다.");
        }

        // 업그레이드 비활성 플래그들 유지
        if (skillName == "FireSlashs")
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.FSSkillLearn, true);
        else if (skillName == "Thunder")
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.TDSkillLearn, true);
        else if (skillName == "IcePillar")
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.IPSkillLearn, true);
        else if (skillName == "Infierno")
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.IFSkillLearn, true);
        else if (skillName == "Blackhole")
            UpgradeManager.Instance.data.SetDisabled(UpgradeType.BHSkillLearn, true);

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

        if(skillName == "IcePillar")
        {
            SoundManager.Instance.PlaySFX("IcePillar");
        }
        else if (skillName == "Infierno")
        {
            SoundManager.Instance.PlaySFX("Inferno");
        }
        else if (skillName == "Blackhole")
        {
            SoundManager.Instance.PlaySFX("BlackHole");
        }
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

    // 패시브/QE 스킬의 특정 스탯(데미지, 쿨타임, 범위) 강화 함수
    public void UpgradeSkillStat(string skillName, UpgradeStat stat, float amount)
    {
        if(!runtimeSkillData.TryGetValue(skillName, out var data))
        {
            Debug.LogWarning($"업그레이드 대상 스킬이 없음 : {skillName}");
            return;
        }

        // 업그레이드 전 값 저장
        float oldValue = stat switch
        {
            UpgradeStat.Damage => data.damage,
            UpgradeStat.Cooltime => data.cooltime,
            UpgradeStat.Range => data.radius,
            _ => 0f
        };

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

        
        // 업그레이드 후 값
        float newValue = stat switch
        {
            UpgradeStat.Damage => data.damage,
            UpgradeStat.Cooltime => data.cooltime,
            UpgradeStat.Range => data.radius,
            _ => 0f
        };

        // 로그 출력
        Debug.Log($"[{skillName}] {stat} 업그레이드: {oldValue} -> {newValue} (증감 {newValue - oldValue})");


        //레벨 저장/카운트업
        if (skillLevels.ContainsKey(skillName))
            skillLevels[skillName]++;

    }

    /// <summary>
    /// 기본 스킬(DefaultSkill)의 스탯을 업그레이드
    /// </summary>
    public void UpgradeDefaultSkillStat(UpgradeStat stat, float amount)
    {
        // 업그레이드 전 값 저장
        float oldValue = stat switch
        {
            UpgradeStat.Damage => DefaultSkill.Instance.defaultDamage,
            UpgradeStat.Cooltime => PlayerSkill.Instance.defaultSkillCooldown,
            UpgradeStat.Range => 0f,  // DefaultSkill에 Range가 없으면 0으로
            _ => 0f
        };

        // 실제 적용
        switch (stat)
        {
            case UpgradeStat.Damage:
                DefaultSkill.Instance.defaultDamage += amount;
                break;
            case UpgradeStat.Cooltime:
                PlayerSkill.Instance.defaultSkillCooldown += amount;
                break;
            case UpgradeStat.Range:
                // Range가 필요하면 DefaultSkill 쪽에 radius 개념을 추가하세요.
                break;
        }

        // 업그레이드 후 값
        float newValue = stat switch
        {
            UpgradeStat.Damage => DefaultSkill.Instance.defaultDamage,
            UpgradeStat.Cooltime => PlayerSkill.Instance.defaultSkillCooldown,
            UpgradeStat.Range => 0f,
            _ => 0f
        };

        Debug.Log($"[DefaultSkill] {stat} 업그레이드: {oldValue} -> {newValue} (증감 {newValue - oldValue})");
        // 레벨 카운트업(원한다면 skillLevels에 DefaultSkill 키도 추가해두세요)
    }
}
