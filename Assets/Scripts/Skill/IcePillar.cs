using UnityEngine;
using System.Collections;

public class IcePillar : MonoBehaviour
{
    public SkillData skillData;

    private void OnEnable()
    {
        if (SkillManager.Instance == null)
            return;              // 아직 준비 안 됐으면 그냥 스킵

        //  SkillManager에 있는 최신 클론으로 덮어쓰기
        skillData = SkillManager.Instance.GetRuntimeSkillData("IcePillar");

        // 사거리만큼 즉시 범위 데미지
        DealAreaDamage();

        StartCoroutine(DisableAfterTime());
    }

    private void DealAreaDamage()
    {
        var hits = Physics2D.OverlapCircleAll(
            transform.position,
            skillData.radius,
            LayerMask.GetMask("Enemy")
        );
        foreach (var hit in hits)
        {
            hit.GetComponent<EnemyMove>()
               .EnemyHurt(skillData.damage);
        }
        Debug.Log($"[IcePillar] 범위:{skillData.radius}, 데미지:{skillData.damage}");
    }

    // 씬 안에서 반경 확인용
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, skillData.radius);
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(skillData.lifetime);
        SkillEnd();
    }

    public void SkillEnd()
    {
        gameObject.SetActive(false);

        AutoSKillPool.Instance.ReturnSkillObject(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyMove>().EnemyHurt(skillData.damage);
            Debug.Log($"[IcePillar] {other.name}에게 {skillData.damage} 데미지");
        }
    }
}
