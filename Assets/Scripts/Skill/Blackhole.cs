using UnityEngine;
using System.Collections;
public class BlackholeSkill : SkillEffect
{
    protected override string SkillName => "Blackhole";
    protected override Color GizmoColor => Color.magenta;

    protected override IEnumerator SkillRoutine()
    {
        float timer = 0f;
        while (timer < skillData.duration)
        {
            PullEnemies();
            timer += Time.deltaTime;
            yield return null;
        }
        DealAreaDamage("Monster");
    }
}

