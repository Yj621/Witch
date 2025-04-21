using UnityEngine;

public class AutoSkillManager : MonoBehaviour
{
    public void LearnInfiernoAuto()
    {
        SkillManager.Instance.LearnNewSkill("Infierno", true);
    }

    public void LearnBlackholeAuto()
    {
        SkillManager.Instance.LearnNewSkill("Blackhole", true);
    }
    public void LearnIcePillarAuto()
    {
        SkillManager.Instance.LearnNewSkill("IcePillar", true);
    }
}
