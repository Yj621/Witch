using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public abstract class SkillEffect : MonoBehaviour, ISkill
{
    protected SkillData skillData;

    // 스킬 이름(데이터베이스 조회용)
    protected abstract string SkillName { get; }
    // 디폴트 타겟 태그
    protected virtual string TargetTag => "Enemy";
    // 디폴트 레이어 마스크
    public string TargetLayer = "Monster";
    // 기즈모 색상
    protected abstract Color GizmoColor { get; }
    // 원본 데이터
    private float originalRadius;
    private Vector3 originalScale;
    private CircleCollider2D cc;

    // 최초 Awake 시 스케일 저장
    protected virtual void Awake()
    {
        originalScale = transform.localScale;
        cc = GetComponent<CircleCollider2D>(); 
    }

    private void OnEnable()
    {
        // SkillManager가 Dictionary에 캐싱해둔 복제본 데이터를 이름으로 찾아옴
        skillData = SkillManager.Instance.GetRuntimeSkillData(SkillName);
        // 가져온 데이터를 바탕으로 현재 스킬 오브젝트의 범위를 결정
        originalRadius = skillData.radius;
        UpdateRangeVisual();
        // 스킬 로직 시작
        StartCoroutine(SkillRoutineWrapper());
    }

    private IEnumerator SkillRoutineWrapper()
    {
        yield return StartCoroutine(SkillRoutine());
    }

    // 각 스킬 고유의 동작을 구현
    protected abstract IEnumerator SkillRoutine();

    // 즉시 범위 데미지
    protected void DealAreaDamage(string layerName = null)
    {
        var mask = LayerMask.GetMask(layerName ?? TargetLayer);
        var hits = Physics2D.OverlapCircleAll(transform.position, skillData.radius, mask);
        foreach (var hit in hits)
            hit.GetComponent<EnemyMove>()?.EnemyHurt(skillData.damage);
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

    public void InitializeRange(float radius)
    {
        skillData.radius = radius;
        UpdateRangeVisual();
    }

    public void StartSkill()
    {
    }
}