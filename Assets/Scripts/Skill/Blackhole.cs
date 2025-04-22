using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackhole : MonoBehaviour
{
    public SkillData skillData;

    private List<Rigidbody2D> affectedEnemies = new();


    private void OnEnable()
    {
        //  SkillManager에 있는 최신 클론으로 덮어쓰기
        skillData = SkillManager.Instance.GetRuntimeSkillData("Blackhole");


        StartCoroutine(BlackholeRoutine());
    }

    private IEnumerator BlackholeRoutine()
    {
        float timer = 0f;

        while (timer < skillData.duration)
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
                    enemy.EnemyHurt(skillData.damage);
                    Debug.Log($"[Blackhole] {enemy.name}에게 {skillData.damage} 데미지");
                }
            }
        }

        affectedEnemies.Clear();
    }

    public void SkillEnd()
    {
        gameObject.SetActive(false);
        AutoSKillPool.Instance.ReturnSkillObject(gameObject);
    }
    private void PullEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, skillData.radius, LayerMask.GetMask("Monster"));

        foreach (Collider2D col in enemies)
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (transform.position - rb.transform.position).normalized;
                rb.AddForce(dir * skillData.force * Time.deltaTime, ForceMode2D.Force);

                if (!affectedEnemies.Contains(rb))
                    affectedEnemies.Add(rb);
            }
        }
    }

    // 개발용 시각화
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
