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
            if (enemy == null)
                Debug.LogWarning($"IcePillar: EnemyMove 컴포넌트를 못 찾음 on {hit.name}");
            else
            {
                Debug.Log($"IcePillar: Freeze 호출 on {enemy.gameObject.name}");
                enemy.Freeze(5f);
                enemy.EnemyHurt(skillData.damage);
            }
        }
        yield return new WaitForSeconds(skillData.lifetime);
        Cleanup();
    }
}