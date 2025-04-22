using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Scriptable Objects/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    public SkillData[] allSkills;

    // 이름으로 찾기
    public SkillData GetSkill(string skillName)
    {
        foreach(var skill in allSkills)
            if (skill.skillName == skillName)
                return skill;
        Debug.LogWarning($"Skill '{skillName}' not found!");
        return null;
    }
}
