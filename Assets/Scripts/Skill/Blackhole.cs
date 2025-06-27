using System.Collections;
using UnityEngine;

public class BlackholeSkill : SkillEffect
{
    protected override string SkillName => "Blackhole";
    protected override Color GizmoColor => Color.magenta;

    protected override IEnumerator SkillRoutine()
    {
        float timer = 0f;
        while (timer < skillData.duration)
        {
            var cols = Physics2D.OverlapCircleAll(transform.position, skillData.radius, LayerMask.GetMask(TargetLayer));
            foreach (var col in cols)
            {
                var rb = col.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 dir = (transform.position - col.transform.position);
                    float distance = dir.magnitude;
                    if (distance > 0.1f)
                    {
                        // 거리 반비례 힘 (최대 힘 제한)
                        float force = skillData.force / Mathf.Max(distance, 0.5f);
                        force = Mathf.Min(force, skillData.force * 2f); // 최대 힘 제한
                        rb.AddForce(dir.normalized * force * Time.deltaTime, ForceMode2D.Impulse);
                    }
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }

        DealAreaDamage("Monster");
    }
}
