using System.Collections;
using UnityEngine;
// 범위형 데미지 스킬 (IcePillar, ThunderStrike, Infierno, ThunderStrike)
public abstract class AreaDamageSkill : SkillEffect
{
    protected override IEnumerator SkillRoutine()
    {
        DealAreaDamage();
        yield return new WaitForSeconds(Duration);
    }
}
