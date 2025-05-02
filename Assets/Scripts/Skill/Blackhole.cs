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
            // 매 프레임마다 주변 적 끌어당기기
            var cols = Physics2D.OverlapCircleAll(transform.position, skillData.radius, LayerMask.GetMask(TargetLayer));
            foreach (var col in cols)
                col.GetComponent<Rigidbody2D>()?.AddForce(
                    (transform.position - col.transform.position).normalized
                    * skillData.force * Time.deltaTime,
                    ForceMode2D.Impulse);

            timer += Time.deltaTime;
            yield return null;
        }

        DealAreaDamage("Monster");
    }
}
