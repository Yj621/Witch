using UnityEngine;

public class AutoSkillManager : MonoBehaviour
{
    public void LearnInfiernoAuto()
    {
        SkillManager.Instance.LearnNewSkill("Infierno", true);
    }

    public void LearnThunderAuto()
    {
        SkillManager.Instance.LearnNewSkill("Thunder", true);
    }
    public void LearnIcePillarAuto()
    {
        SkillManager.Instance.LearnNewSkill("IcePillar", true);
    }
}
