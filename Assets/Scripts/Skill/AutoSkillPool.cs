using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoSKillPool : MonoBehaviour
{
    [Header("Skill Prefabs")]
    [SerializeField] private GameObject icePrefab;       // IcePillar 스킬 프리팹
    [SerializeField] private GameObject blackholePrefab; // Blackhole 스킬 프리팹
    [SerializeField] private GameObject infiernoPrefab;  // Infierno 스킬 프리팹

    [SerializeField] private int initialCount = 20;      // 초기 개수
    private List<GameObject> icePool;                    // IcePillar 스킬 풀
    private List<GameObject> blackholePool;              // Blackhole 스킬 풀
    private List<GameObject> infiernoPool;               // Infierno 스킬 풀

    public static AutoSKillPool Instance { get; private set; }

    void Awake()
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

    private void Start()
    {
        InitializePools();    // 스킬 풀 초기화
    }
    // 모든 스킬 풀 초기화
    private void InitializePools()
    {
        icePool = new List<GameObject>();
        blackholePool = new List<GameObject>();
        infiernoPool = new List<GameObject>();

        // 각 스킬 풀에 대해 프리팹을 초기화
        InitializePool(icePrefab, icePool);
        InitializePool(blackholePrefab, blackholePool);
        InitializePool(infiernoPrefab, infiernoPool);
    }

    // 특정 스킬의 풀 초기화
    private void InitializePool(GameObject prefab, List<GameObject> pool)
    {
        for (int i = 0; i < initialCount; i++)
        {
            GameObject skill = Instantiate(prefab, transform);
            skill.SetActive(false);  // 기본적으로 비활성화
            pool.Add(skill);
        }
    }

    // 이름으로 해당 풀에서 오브젝트 가져오기
    public GameObject GetSkillObject(string skillName)
    {
        return skillName switch
        {
            "IcePillar" => GetFromPool(icePool, icePrefab),
            "Blackhole" => GetFromPool(blackholePool, blackholePrefab),
            "Infierno" => GetFromPool(infiernoPool, infiernoPrefab),
            _ => null
        };
    }

    // 풀에서 스킬 오브젝트 가져오기
    private GameObject GetFromPool(List<GameObject> pool, GameObject prefab)
    {
        for (int i = pool.Count - 1; i >= 0; i--)
        {
            GameObject skill = pool[i];

            if (skill == null)
            {
                pool.RemoveAt(i);
                continue;
            }

            if (!skill.activeSelf)
            {
                skill.SetActive(true);
                return skill;
            }
        }

        GameObject newSkill = Instantiate(prefab, transform);
        newSkill.SetActive(true);
        pool.Add(newSkill);
        return newSkill;
    }
    public void ReturnSkillObject(GameObject skill)
    {
        if (skill == null) return;

        // 활성화 끄기
        skill.SetActive(false);
    }
}
