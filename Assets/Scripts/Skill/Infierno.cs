using UnityEngine;
using System.Collections;

public class Infierno : MonoBehaviour
{
    public SkillData skillData;

    private void OnEnable()
    {
        //  SkillManager에 있는 최신 클론으로 덮어쓰기
        skillData = SkillManager.Instance.GetRuntimeSkillData("Infierno");

        //범위 데미지
        DealAreaDamage();

        StartCoroutine(DisableAfterTime());
    }
    private void DealAreaDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, skillData.radius, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            hit.GetComponent<EnemyMove>().EnemyHurt(skillData.damage);
            Debug.Log($"[Infierno] {hit.name}에게 {skillData.damage} 데미지");
        }
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(skillData.duration);
    }
    public void SkillEnd()
    {
        gameObject.SetActive(false);
        AutoSKillPool.Instance.ReturnSkillObject(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        // 범위 확인용 기즈모
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, skillData.radius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyMove>().EnemyHurt(skillData.damage);
            Debug.Log($"[Infierno] {other.name}에게 {skillData.damage} 데미지");
        }
    }
}
