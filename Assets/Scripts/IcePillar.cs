using UnityEngine;
using System.Collections;

public class IcePillar : MonoBehaviour
{
    public float damage = 5f;
    public float lifetime = 2f;

    private void OnEnable()
    {
        // 일정 시간 후 자동 비활성화
        StartCoroutine(DisableAfterTime());
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyMove>().EnemyHurt(damage);
            Debug.Log($"[IcePillar] {other.name}에게 {damage} 데미지");
            gameObject.SetActive(false);
        }
    }
}
