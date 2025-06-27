using UnityEngine;
using System.Collections;
public class IcePillar : SkillEffect
{
    protected override string SkillName => "IcePillar";
    protected override Color GizmoColor => Color.cyan;

    protected override IEnumerator SkillRoutine()
    {
        var mask = LayerMask.GetMask(TargetLayer);
        var hits = Physics2D.OverlapCircleAll(transform.position, skillData.radius, mask);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyMove>();
            if (enemy != null)
            {
                // 적에게 피해를 주고, 일정 시간 동안 얼음 기둥에 묶음
                enemy.EnemyHurt(skillData.damage);
                enemy.Freeze(5f); // 1.5초 동안 동결
            }
        }
        yield return new WaitForSeconds(skillData.lifetime);
        Cleanup();
    }
}