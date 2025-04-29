using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Scriptable Objects/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    [Tooltip("모든 스킬 데이터 목록")]
    public SkillData[] allSkills;

    // 빠른 조회용 맵
    private Dictionary<string, SkillData> skillMap;

    private void OnEnable()
    {
        if (skillMap != null) return;
        skillMap = new Dictionary<string, SkillData>(allSkills.Length);
        foreach (var skill in allSkills)
        {
            skillMap[skill.skillName] = skill;
        }
    }

    /// <summary>
    /// 이름으로 스킬 데이터 반환 (없으면 경고 로그)
    /// </summary>
    public SkillData GetSkill(string skillName)
    {
        if (skillMap != null && skillMap.TryGetValue(skillName, out var data))
            return data;
        Debug.LogWarning($"Skill '{skillName}' not found!");
        return null;
    }
}