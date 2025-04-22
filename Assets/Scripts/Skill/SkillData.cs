using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public float damage;     //데미지
    public float radius;     //범위
    public float duration;   //지속시간
    public float force;      // 블랙홀 전용
    public float lifetime;   // 아이스필라 전용
}
