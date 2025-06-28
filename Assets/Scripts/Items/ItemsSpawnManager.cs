using UnityEngine;
using System.Collections;
public class ItemsSpawnManager : MonoBehaviour
{    
    [Header("코인 관련")]
    [SerializeField] private GameObject coinPrefab; // 생성할 코인 프리팹
    private int initialCoinCount = 10;              // 시작 시 생성할 코인 개수
    private float mapRadius = 20f;                  // 맵 반경
    [SerializeField] private Transform playerTransform; // 플레이어 위치 참조
    public float coinSpawnRadius = 10f;             // 플레이어 주변 코인 생성 반경
    public float coinSpawnInterval = 30f;           // 코인 생성 주기(초)

    [Header("아이템 관련")]
    public GameObject[] itemPrefabs;                // 생성할 아이템 프리팹 배열
    public float itemSpawnInterval = 10f;           // 아이템 생성 주기(초)
    private BoxCollider2D itemSpawnRange;           // 아이템 생성 범위(BoxCollider2D)

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        itemSpawnRange = GetComponent<BoxCollider2D>(); // BoxCollider2D 컴포넌트 가져오기
    }

    /// <summary>
    /// 초기 코인 생성 및 코루틴 시작
    /// </summary>
    void Start()
    {
        SpawnInitialCoins();
        StartCoroutine(CoinSpawnRoutine());
        StartCoroutine(ItemSpawnRoutine());
    }

    /// <summary>
    /// 게임 시작 시 맵 전체에 코인 배치
    /// </summary>
    void SpawnInitialCoins()
    {
        for (int i = 0; i < initialCoinCount; i++)
        {
            Vector3 randomPos = GetRandomPositionAround(Vector3.zero, mapRadius);
            Instantiate(coinPrefab, randomPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// 일정 시간마다 플레이어 주변에 코인 생성
    /// </summary>
    IEnumerator CoinSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(coinSpawnInterval);

            int coinsToSpawn = Random.Range(1, 3); // 1~2개 랜덤 생성
            for (int i = 0; i < coinsToSpawn; i++)
            {
                Vector3 spawnPos = GetRandomPositionAround(playerTransform.position, coinSpawnRadius);
                Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// 일정 시간마다 맵 내 랜덤 위치에 아이템 생성
    /// </summary>
    IEnumerator ItemSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(itemSpawnInterval);

            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject selectedItem = itemPrefabs[randomIndex];
            Vector3 spawnPos = GetRandomPositionInBox(itemSpawnRange);
            Instantiate(selectedItem, spawnPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// 주어진 중심점과 반경 내 랜덤 위치 반환
    /// </summary>
    Vector3 GetRandomPositionAround(Vector3 center, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return new Vector3(center.x + randomCircle.x, center.y + randomCircle.y, center.z);
    }

    /// <summary>
    /// BoxCollider2D 범위 내 랜덤 위치 반환
    /// </summary>
    Vector3 GetRandomPositionInBox(BoxCollider2D box)
    {
        Vector3 origin = box.transform.position;
        float rangeX = box.bounds.size.x;
        float rangeY = box.bounds.size.y;

        float randomX = Random.Range(-rangeX / 2, rangeX / 2);
        float randomY = Random.Range(-rangeY / 2, rangeY / 2);

        return origin + new Vector3(randomX, 0f, randomY);
    }
}