using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;
    [Tooltip("스킬 데미지 수치")]
    public float damage;
    [Tooltip("스킬 범위")]
    public float radius;
    [Tooltip("스킬 지속 시간")]
    public float duration;

    [Header("특수 속성")]
    [Tooltip("적을 끌어당기는 힘 (Blackhole 전용)")]
    public float force;
    [Tooltip("생존 시간 (IcePillar 전용)")]
    public float lifetime;
    [Tooltip("쿨타임")]
    public float cooltime;
}