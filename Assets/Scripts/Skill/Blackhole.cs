using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackhole : MonoBehaviour
{
    public SkillData skillData;


    private void OnEnable()
    {
        if (SkillManager.Instance == null)
            return;              // 아직 준비 안 됐으면 그냥 스킵

        //  SkillManager에 있는 최신 클론으로 덮어쓰기
        skillData = SkillManager.Instance.GetRuntimeSkillData("Blackhole");
        Debug.Log($"[Blackhole] duration={skillData.duration}, radius={skillData.radius}, force={skillData.force}");

        StartCoroutine(BlackholeRoutine());
    }

    private IEnumerator BlackholeRoutine()
    {
        float timer = 0f;

        // 끌어당김
        while (timer < skillData.duration)
        {
            PullEnemies();
            timer += Time.deltaTime;
            yield return null;
        }

        DealAreaDamage();
        SkillEnd();
    }

    private void PullEnemies()
    {
        var cols = Physics2D.OverlapCircleAll(
            transform.position,
            skillData.radius,
            LayerMask.GetMask("Monster")
        );
        foreach (var col in cols)
        {
            var rb = col.GetComponent<Rigidbody2D>();
            if (rb == null) continue;
            Vector2 dir = (transform.position - rb.transform.position).normalized;
            rb.AddForce(dir * skillData.force * Time.deltaTime, ForceMode2D.Impulse);
        }
    }
    private void DealAreaDamage()
    {
        var cols = Physics2D.OverlapCircleAll(
            transform.position,
            skillData.radius,
            LayerMask.GetMask("Monster")
        );
        foreach (var col in cols)
        {
            col.GetComponent<EnemyMove>()
               .EnemyHurt(skillData.damage);
        }
        Debug.Log($"[Blackhole] 범위:{skillData.radius}, 데미지:{skillData.damage}");
    }

    public void SkillEnd()
    {
        gameObject.SetActive(false);
        GameManager.Instance.autoSkillPool.ReturnSkillObject(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, skillData.radius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyMove>().EnemyHurt(skillData.damage);
            Debug.Log($"[Blackhole] {other.name}에게 {skillData.damage} 데미지");
        }
    }
}
