using System.Collections;
using UnityEngine;
using static MonsterPool;

public class MonsterSpwaner : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject monsterPrefab;
    public float spawnInterval = 0.5f;
    public float spawnRadius = 10f;
    public MonsterType type;


    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnMonster(type);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnMonster(MonsterType type)
    {
        if (playerTransform == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnPos = (Vector2)playerTransform.position + randomDirection * spawnRadius;

        GameObject monster = MonsterPool.Instance.Get(type);
        monster.transform.position = spawnPos;
        monster.GetComponent<EnemyMove>().Init(playerTransform, type);
    }
}
