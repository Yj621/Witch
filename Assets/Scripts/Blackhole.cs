using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackhole : MonoBehaviour
{
    public float radius = 5f;              // 끌어당기는 범위
    public float force = 10f;              // 끌어당기는 힘
    public float duration = 3f;            // 지속 시간
    public float damage = 3f;              // 범위 데미지 (선택)

    private List<Rigidbody2D> affectedEnemies = new();

    void OnEnable()
    {
        StartCoroutine(BlackholeRoutine());
    }

    private IEnumerator BlackholeRoutine()
    {
        float timer = 0f;

        while (timer < duration)
        {
            PullEnemies();
            timer += Time.deltaTime;
            yield return null;
        }

        // 데미지 줄 수도 있음
        foreach (var rb in affectedEnemies)
        {
            if (rb != null)
            {
                var enemy = rb.GetComponent<EnemyMove>();
                if (enemy != null)
                {
                    enemy.EnemyHurt(damage);
                    Debug.Log($"[Blackhole] {enemy.name}에게 {damage} 데미지");
                }
            }
        }

        affectedEnemies.Clear();
        gameObject.SetActive(false);
    }

    private void PullEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy"));

        foreach (Collider2D col in enemies)
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (transform.position - rb.transform.position).normalized;
                rb.AddForce(dir * force * Time.deltaTime, ForceMode2D.Force);

                if (!affectedEnemies.Contains(rb))
                    affectedEnemies.Add(rb);
            }
        }
    }

    // 개발용 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
