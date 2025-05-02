using UnityEngine;

public interface ISkill
{
    // 스킬 오브젝트(이펙트, 콜라이더 등)의 크기를 radius 값에 맞춰 초기화
    void InitializeRange(float radius);
    // 스킬 실제 실행 로직 진입점
    void StartSkill();
}
