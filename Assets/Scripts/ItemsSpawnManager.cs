using UnityEngine;
using System.Collections;

public class ItemsSpawnManager : MonoBehaviour
{    
    private GameObject rangeObject;
    public float spawnTimer = 0;
    BoxCollider2D rangeCollider;
    public GameObject[] itemPrefabs;

    private void Awake()
    {
        rangeObject = this.gameObject;
        rangeCollider = rangeObject.GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        StartCoroutine(RandomRespawn_Coroutine());
    }

    IEnumerator RandomRespawn_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnTimer);

            // 랜덤으로 아이템 선택
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject selectedItem = itemPrefabs[randomIndex];

            Instantiate(selectedItem, Return_RandomPosition(), Quaternion.identity);
        }
    }

    Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = rangeObject.transform.position;
        float range_X = rangeCollider.bounds.size.x;
        float range_Y = rangeCollider.bounds.size.y;

        range_X = Random.Range(-range_X / 2, range_X / 2);
        range_Y = Random.Range(-range_Y / 2, range_Y / 2);
        Vector3 randomPosition = new Vector3(range_X, 0f, range_Y);

        return originPosition + randomPosition;
    }
}