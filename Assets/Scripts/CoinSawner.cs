using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public int initialCoinCount = 10;
    public float mapRadius = 20f; // 맵 크기 반지름

    public Transform playerTransform;
    public float spawnRadius = 10f;
    public float spawnInterval = 30f;

    void Start()
    {
        SpawnInitialCoins();
        StartCoroutine(SpawnCoinRoutine());
    }

    void SpawnInitialCoins()
    {
        for (int i = 0; i < initialCoinCount; i++)
        {
            Vector3 randomPos = GetRandomPositionAround(Vector3.zero, mapRadius);
            Instantiate(coinPrefab, randomPos, Quaternion.identity);
        }
    }

    IEnumerator SpawnCoinRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int coinsToSpawn = Random.Range(1, 3); // 1개 또는 2개
            for (int i = 0; i < coinsToSpawn; i++)
            {
                Vector3 spawnPos = GetRandomPositionAround(playerTransform.position, spawnRadius);
                Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    Vector3 GetRandomPositionAround(Vector3 center, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return new Vector3(center.x + randomCircle.x, center.y + randomCircle.y, center.z); // z는 고정
    }

}
