using System;
using System.Collections.Generic;
using UnityEngine;
public class SkillObjectPool : MonoBehaviour
{
    [Serializable]
    public class SkillPrefabInfo
    {
        public string skillName;
        public GameObject prefab;
        public int initialObjectNumber = 20;
        public List<GameObject> pool;
    }

    [Header("Basic Skill Pool")]
    [SerializeField] private GameObject firePrefab;             // 기본 스킬 프리팹
    [SerializeField] private int fireInitialCount = 20;         // 기본 스킬 초기 개수
    private List<GameObject> firePool;                          // 기본 스킬 풀

    [Header("Additional Skill Pools")]
    [SerializeField] private List<SkillPrefabInfo> skillPrefabs; // 추가 스킬 리스트
    private Dictionary<string, SkillPrefabInfo> skillPools;     // 추가 스킬 풀 딕셔너리

    void Awake()
    {
        InitializeFirePool();    // 기본 스킬 풀 초기화
        InitializeSkillPools(); // 추가 스킬 풀 초기화
    }

    // 기본 스킬 풀 초기화
    private void InitializeFirePool()
    {
        firePool = new List<GameObject>();
        for (int i = 0; i < fireInitialCount; i++)
        {
            GameObject fire = Instantiate(firePrefab, transform);
            fire.SetActive(false);
            firePool.Add(fire);
        }
    }

    // 추가 스킬 풀 초기화
    private void InitializeSkillPools()
    {
        skillPools = new Dictionary<string, SkillPrefabInfo>();
        foreach (var skillInfo in skillPrefabs)
        {
            skillInfo.pool = new List<GameObject>();
            for (int i = 0; i < skillInfo.initialObjectNumber; i++)
            {
                GameObject skill = Instantiate(skillInfo.prefab, transform);
                skill.SetActive(false);
                skillInfo.pool.Add(skill);
            }
            skillPools[skillInfo.skillName] = skillInfo;
        }
    }

    // 기본 스킬 가져오기
    public GameObject GetFireObject()
    {
        foreach (GameObject fire in firePool)
        {
            if (fire != null && !fire.activeSelf)
            {
                fire.SetActive(true);
                return fire;
            }
        }

        // 풀이 부족하면 새로 생성
        GameObject newFire = Instantiate(firePrefab, transform);
        newFire.SetActive(true);
        firePool.Add(newFire);
        return newFire;
    }

    // 특정 스킬 가져오기
    public GameObject GetSkillObject(string skillName)
    {
        if (skillPools.TryGetValue(skillName, out SkillPrefabInfo skillInfo))
        {
            foreach (GameObject skill in skillInfo.pool)
            {
                if (skill != null && !skill.activeSelf)
                {
                    skill.SetActive(true);
                    Debug.Log("POOOOLL");
                    return skill;
                }
            }

            // 풀이 부족하면 새로 생성
            GameObject newSkill = Instantiate(skillInfo.prefab, transform);
            newSkill.SetActive(true);
            skillInfo.pool.Add(newSkill);
            return newSkill;
        }

        Debug.LogError($"Skill '{skillName}' not found!");
        return null;
    }
}
