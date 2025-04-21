using UnityEngine;
using System.Collections;

public class Infierno : MonoBehaviour
{
    public float damage = 8f;
    public float duration = 1.5f;
    public float radius = 3f;

    private void OnEnable()
    {
        // 범위 데미지 적용
        DealAreaDamage();
        // 일정 시간 후 비활성화
        StartCoroutine(DisableAfterTime());
    }

    private void DealAreaDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            hit.GetComponent<EnemyMove>().EnemyHurt(damage);
            Debug.Log($"[Infierno] {hit.name}에게 {damage} 데미지");
        }
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        // 범위 확인용 기즈모
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
