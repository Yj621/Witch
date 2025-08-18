using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class IcePillar : SkillEffect
{
    protected override string SkillName => "IcePillar";
    protected override Color GizmoColor => Color.cyan;

    [SerializeField] private float freezeDuration = 5f;

    // 스폰되자마자 1회 스냅샷 판정 + 수명 끝나면 정리
    protected override IEnumerator SkillRoutine()
    {
        var mask = LayerMask.GetMask(TargetLayer);
        var hits = Physics2D.OverlapCircleAll(transform.position, skillData.radius, mask);

        var seen = new HashSet<EnemyMove>();
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyMove>();
            if (enemy == null || seen.Contains(enemy)) continue;

            seen.Add(enemy);
            enemy.Freeze(freezeDuration);
            enemy.EnemyHurt(skillData.damage);
        }

        yield return new WaitForSeconds(skillData.lifetime);
        Cleanup();
    }

    // IcePillar 콜라이더를 isTrigger=true 로 두면, 들어오는 적도 동결/데미지
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(TargetTag)) return;
        var enemy = other.GetComponent<EnemyMove>();
        if (enemy == null) return;

        enemy.Freeze(freezeDuration);
        enemy.EnemyHurt(skillData.damage);
    }
}