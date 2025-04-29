using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerSkill : MonoBehaviour
{
    private Animator skillAnimator;
    private PlayerInput playerInput;
    private SkillManager skillManager;

    public float defaultSkillCooldown;

    // 스킬 쿨다운 타이머 관리
    private Dictionary<string, float> skillCooldownTimers = new Dictionary<string, float>();

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

        // 기본 스킬 자동 발사
        StartCoroutine(AutoFireDefaultSkill());

        // 자동 발사 타이머 초기화: 지금은 사전 없이, LearnNewSkill 시점에 세팅해도 됩니다.
        foreach (var auto in skillManager.GetAutoSkills())
        {
            skillCooldownTimers[auto.skillName] = 0f;
        }

        // 추가 스킬 자동 발사 시작
        StartCoroutine(AutoAddSkills());
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

    //근처 적 주변에 추가 스킬 생성
    public void SpawnSkillNearEnemy(GameObject skillPrefab)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(playerTransform.position, 10f, LayerMask.GetMask("Monster"));

        if (hits.Length == 0)
        {
            skillPrefab.transform.position = playerTransform.position;
            return;
        }

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(playerTransform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        skillPrefab.transform.position = nearest.position;
    }

    // 기본 스킬 발사
    public void DefaultSkill()
    {
        FireSkill(GameManager.Instance.skillObjectPool.GetFireObject(), 4f); // SkillObjectPool에서 가져오기
    }

    // 추가 스킬 자동 발사
    private IEnumerator AutoAddSkills()
    {
        while (true)
        {
            // 배운 오토 스킬 목록
            var learned = skillManager.GetAutoSkills();

            foreach (var (skillName, action) in learned)
            {
                // 스킬 데이터에서 쿨타임을 가져옴
                var data = skillManager.GetRuntimeSkillData(skillName);
                if (data == null) continue;

                // 타이머가 만료되면 발사
                if (Time.time >= skillCooldownTimers[skillName])
                {
                    skillCooldownTimers[skillName] = Time.time + data.cooltime;
                    AddSkill(skillName);
                    Debug.Log($"[Auto Skill Fired] {skillName}, next in {data.cooltime}s");
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    // 추가 스킬 발사
    public void AddSkill(string skillName)
    {
        GameObject skillPrefab = GameManager.Instance.autoSkillPool.GetSkillObject(skillName);

        if (skillPrefab == null) return;

        if (skillName == "IcePillar" || skillName == "Infierno" || skillName == "Blackhole" || skillName == "ThunderStrike")
        {
            SpawnSkillNearEnemy(skillPrefab); //생성 위치 설정
        }
        else
        {
            FireSkill(skillPrefab, 4f); // 기본 발사 스킬만 여기로
        }
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
