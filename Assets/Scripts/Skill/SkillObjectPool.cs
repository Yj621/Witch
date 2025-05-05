using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillObjectPool : MonoBehaviour
{
    [Header("Basic Skill Pool")]
    [SerializeField] private GameObject firePrefab;      // 기본 스킬 프리팹
    [SerializeField] private int fireInitialCount = 20;  // 기본 스킬 초기 개수
    private List<GameObject> firePool;                   // 기본 스킬 풀

    void Awake()
    {

        InitializeFirePool();    // 기본 스킬 풀 초기화
    }
    
    void Start()
    {

    }

    // 기본 스킬 풀 초기화
    private void InitializeFirePool()
    {
        firePool = new List<GameObject>();
        for (int i = 0; i < fireInitialCount; i++)
        {
            // 부모를 명확히 지정하여 오브젝트를 생성
            GameObject fire = Instantiate(firePrefab, transform);
            fire.SetActive(false);  // 기본적으로 비활성화
            firePool.Add(fire);
        }
    }


    // 기본 스킬 가져오기
    public GameObject GetFireObject()
    {
        for (int i = firePool.Count - 1; i >= 0; i--)
        {
            GameObject fire = firePool[i];

            // 오브젝트가 파괴된 경우 리스트에서 제거
            if (fire == null)
            {
                firePool.RemoveAt(i);
                continue;
            }

            if (!fire.activeSelf)
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

}
