
// 공통 스킬 동작을 정의한 추상 클래스
using System.Collections;
using UnityEngine;
public abstract class SkillEffect : MonoBehaviour
{
    protected SkillData skillData;

    // 스킬 이름(데이터베이스 조회용)
    protected abstract string SkillName { get; }
    // 디폴트 타겟 태그
    protected virtual string TargetTag => "Enemy";
    // 디폴트 레이어 마스크
    protected virtual string TargetLayer => "Enemy";
    // 기즈모 색상
    protected abstract Color GizmoColor { get; }
    // 스킬 지속 시간 (lifetime 또는 duration)
    protected virtual float Duration => skillData.lifetime > 0 ? skillData.lifetime : skillData.duration;

    // 원본 반경/스케일 저장
    private float originalRadius;
    private Vector3 originalScale;

    // 최초 Awake 시 스케일 저장
    protected virtual void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (SkillManager.Instance == null) return;
        // 런타임 데이터 갱신
        skillData = SkillManager.Instance.GetRuntimeSkillData(SkillName);
        // 반경 기반 시각/충돌 크기 설정
        originalRadius = skillData.radius;
        UpdateRangeVisual();
        // 메인 코루틴 실행
        StartCoroutine(SkillRoutineWrapper());
    }

    private IEnumerator SkillRoutineWrapper()
    {
        yield return StartCoroutine(SkillRoutine());
        Cleanup();
    }

    // 각 스킬 고유의 동작을 구현
    protected abstract IEnumerator SkillRoutine();

    // 즉시 범위 데미지
    protected void DealAreaDamage(string layerName = null)
    {
        var mask = LayerMask.GetMask(layerName ?? TargetLayer);
        var hits = Physics2D.OverlapCircleAll(transform.position, skillData.radius, mask);
        foreach (var hit in hits)
        {
            var mover = hit.GetComponent<EnemyMove>();
            if (mover != null)
                mover.EnemyHurt(skillData.damage);
        }
    }

    // 끌어당김이 필요한 스킬용 헬퍼
    protected void PullEnemies()
    {
        var mask = LayerMask.GetMask(TargetLayer);
        var cols = Physics2D.OverlapCircleAll(transform.position, skillData.radius, mask);
        foreach (var col in cols)
        {
            var rb = col.GetComponent<Rigidbody2D>();
            if (rb == null) continue;
            Vector2 dir = (transform.position - rb.transform.position).normalized;
            rb.AddForce(dir * skillData.force * Time.deltaTime, ForceMode2D.Impulse);
        }
    }

    // 스킬 사용 후 정리
    protected virtual void Cleanup()
    {
        gameObject.SetActive(false);
        GameManager.Instance.autoSkillPool.ReturnSkillObject(gameObject);
    }

    // 기즈모로 반경 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawWireSphere(transform.position, skillData.radius);
    }

    // 충돌 시 단일 대상 데미지
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(TargetTag)) return;
        var mover = other.GetComponent<EnemyMove>();
        if (mover != null)
        {
            mover.EnemyHurt(skillData.damage);
            Debug.Log($"[{SkillName}] {other.name}에게 {skillData.damage} 데미지");
        }
    }

    // 범위 증가(업그레이드 적용)
    public void ModifyRange(float delta)
    {
        skillData.radius += delta;
        UpdateRangeVisual();
    }

    // 쿨타임 감소(업그레이드 적용)
    public void ModifyCooldown(float delta)
    {
        skillData.cooltime = Mathf.Max(0f, skillData.cooltime - delta);
    }

    // 시각 및 콜라이더 크기 동기화
    private void UpdateRangeVisual()
    {
        float scaleFactor = skillData.radius / originalRadius;
        transform.localScale = originalScale * scaleFactor;
        var cc = GetComponent<CircleCollider2D>();
        if (cc != null)
            cc.radius = skillData.radius;
    }
}