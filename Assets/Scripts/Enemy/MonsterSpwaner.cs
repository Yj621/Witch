using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MonsterPool;

public class MonsterSpwaner : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject monsterPrefab;
    public float baseSpawnInterval = 0.5f;
    public float spawnRadius = 10f;
    public MonsterType type;

    private int currentLevel = 1;
    private float currentInterval;
    private List<MonsterType> availableTypes = new();
    Player player;

    void Start()
    {
        player = GameManager.Instance.player;
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        UpdateLevelFromPlayer();
    }

    void ApplyLevelScaling(int level)
    {
        currentInterval = Mathf.Max(0.5f, baseSpawnInterval - (level * 0.1f));
    }

    void UpdateSpawnTypes(int level)
    {
        availableTypes.Clear();
        if (level >= 1) availableTypes.Add(MonsterType.Pumpkin);
        if (level >= 5) availableTypes.Add(MonsterType.Ghost);
        if (level >= 10) availableTypes.Add(MonsterType.Spider);
        if (level >= 15) availableTypes.Add(MonsterType.Skull);
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (availableTypes.Count > 0)
            {
                SpawnMonster(GetRandomMonsterType());
            }

            yield return new WaitForSeconds(currentInterval);
        }
    }

    void UpdateLevelFromPlayer()
    {
        if (playerTransform == null) return;

        int level = player.Level;

        if (level != currentLevel)
        {
            currentLevel = level;
            ApplyLevelScaling(currentLevel);
            UpdateSpawnTypes(currentLevel);
        }
    }

    MonsterType GetRandomMonsterType()
    {
        return availableTypes[Random.Range(0, availableTypes.Count)];
    }



    void SpawnMonster(MonsterType type)
    {
        if (playerTransform == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnPos = (Vector2)playerTransform.position + randomDirection * spawnRadius;

        GameObject monster = MonsterPool.Instance.Get(type);
        if (monster == null) return;

        monster.transform.position = spawnPos;
        monster.GetComponent<EnemyMove>().Init(playerTransform, type);
    }
}
