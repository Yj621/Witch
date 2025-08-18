using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static MonsterPool;

public class EnemyMove : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 0.4f;
    private float originalSpeed; // Inspector 값 저장
    SpriteRenderer spriteRenderer;
    public float MaxHp = 20f;
    private float CurrentHp = 20f;
    Animator ani;
    public MonsterType type;
    private bool isDie = false;
    public int Exp = 50;
    public float Clean = 10f;
    public GameObject ExpCandyPrefab;

    private bool isPattern = false;
    public float patternTimer;
    public float patternCooldown = 3f; // 패턴 행동 주기
    public float GhostPatternRange = 3f;

    private bool isFrozen = false;            // 얼음 상태 플래그
    private Coroutine freezeRoutine = null;   // 중복 호출 방지용
    private Coroutine currentPattern;


    [Header("Warning")]
    public LineRenderer warningLine;      // 경고선 라인렌더러
    public float warningDuration = 0.5f;  // 경고선 지속 시간

    public void Init(Transform player, MonsterType type)
    {
        isDie = false;
        target = player;
        this.type = type;
        CurrentHp = MaxHp;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        originalSpeed = moveSpeed;
    }
    void Update()
    {
        if (isDie) return; 
        if (isFrozen)return;

        EnemyMovement();
        EnemyPattern();
    }

    private void EnemyMovement()
    {
        // 플레이어 방향 및 이동
        Vector2 direction = (target.position - transform.position).normalized;
        spriteRenderer.flipX = direction.x < 0;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    private void EnemyPattern()
    {
        // 타이머 누적
        patternTimer += Time.deltaTime;

        // 패턴 준비 조건
        if (!isPattern && patternTimer >= patternCooldown)
        {
            patternTimer = 0f;
            float distanceToPlayer = Vector2.Distance(transform.position, target.position);

            switch (type)
            {
                case MonsterType.Ghost:
                    if (distanceToPlayer <= GhostPatternRange)
                        currentPattern = StartCoroutine(GhostPattern());
                    break;
                case MonsterType.Spider:
                    currentPattern = StartCoroutine(SpiderPattern());
                    break;
                case MonsterType.Skull:
                    currentPattern = StartCoroutine(SkullPattern());
                    break;
            }
        }
    }


    public void Freeze(float duration)
    {
        isFrozen = true;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        spriteRenderer.color = Color.cyan;
        moveSpeed = 0f;

        if (freezeRoutine != null) StopCoroutine(freezeRoutine);
        freezeRoutine = StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        isFrozen = false;
        if (ani != null) ani.speed = 1f;

        // Inspector에서 세팅해둔 원래 속도로 복구
        moveSpeed = originalSpeed;

        freezeRoutine = null;
    }
    // Frozen 중에는 Hurt 애니메이션 트리거 금지
    public void EnemyHurt(float Damage)
    {
        CurrentHp -= Damage;

        DamageManager.Instance.Show(Damage, transform.position + Vector3.up * 0.5f);

        if (CurrentHp <= 0)
        {
            Die();
        }
        else
        {
            // 얼어있는 동안엔 Hurt 애니메이션을 실행하지 않음
            if (!isFrozen && ani != null)
            {
                ani.SetTrigger("Hurt");
            }
        }
    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Skill"))
        {
            QESkill qESKill = other.GetComponent<QESkill>();

            QESkill.Instance.Attack(gameObject.GetComponent<Collider2D>());
        }
    }



    void Die()
    {
        isDie = true;
        GameManager.Instance.currentClean += Clean;
        ani.SetTrigger("Die");
        StartCoroutine(DropExpCandies());
        SoundManager.Instance.PlaySFX("MonsterHit");
    }

    private IEnumerator DropExpCandies()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject candy = Instantiate(ExpCandyPrefab, transform.position, Quaternion.identity);
        candy.GetComponent<ExpCandy>();
        yield return new WaitForSeconds(0.5f);
        MonsterPool.Instance.Return(type, this.gameObject);
    }

    IEnumerator GhostPattern()
    {
        isPattern = true;

        // 반투명화
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);

        // 순간이동 위치 계산 (플레이어 반대편 3단위)
        Vector2 offset = -(target.position - transform.position).normalized * 3f;
        Vector2 ghostTarget = target.position + (Vector3)offset;
        transform.position = ghostTarget;

        yield return new WaitForSeconds(0.5f); // 잠시 유지

        // 불투명화
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        isPattern = false;
        currentPattern = null;
    }

    IEnumerator SkullPattern()
    {
        isPattern = true;

        originalSpeed = moveSpeed;
        moveSpeed = 0f; // 정지
        yield return new WaitForSeconds(1f);

        moveSpeed = 6f; // 돌진
        yield return new WaitForSeconds(0.5f);

        moveSpeed = originalSpeed; // 원상복구
        isPattern = false;
        currentPattern = null;
    }
    IEnumerator SpiderPattern()
    {
        isPattern = true;

        // 방향/거리 계산
        Vector2 randDir = Random.insideUnitCircle.normalized;
        float jumpPower = 2f;
        Vector3 startPos = transform.position;
        Vector3 jumpTarget = startPos + (Vector3)randDir * jumpPower;

        // 경고선 초기 세팅 (끝부분만 반투명)
        warningLine.positionCount = 2;
        warningLine.SetPosition(0, startPos);
        warningLine.SetPosition(1, jumpTarget);
        // 시작은 완전 투명, 끝은 50% 투명
        warningLine.colorGradient = MakeRedAlphaGradient(0f, 0.5f);

        // 경고시간 동안 점진적으로 끝 알파 올리기
        float elapsed = 0f;
        while (elapsed < warningDuration)
        {
            // t=0 → 1 동안 끝 알파를 0.5 → 1.0으로 보간
            float t = elapsed / warningDuration;
            float endAlpha = Mathf.Lerp(0.5f, 1f, t);
            warningLine.colorGradient = MakeRedAlphaGradient(0f, endAlpha);

            elapsed += Time.deltaTime;
            yield return null;
        }
        // 공격 직전엔 끝 알파가 1(완전 불투명)
        warningLine.colorGradient = MakeRedAlphaGradient(0f, 1f);

        // 실제 점프
        float dur = 0.2f;
        elapsed = 0f;
        while (elapsed < dur)
        {
            transform.position = Vector3.Lerp(startPos, jumpTarget, elapsed / dur);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = jumpTarget;

        warningLine.positionCount = 0;
        isPattern = false;
        currentPattern = null;
    }
    Gradient MakeRedAlphaGradient(float startAlpha, float endAlpha)
    {
        var grad = new Gradient();
        // 전체 구간에서 색상은 빨강 그대로
        grad.colorKeys = new[]
        {
        new GradientColorKey(Color.red, 0f),
        new GradientColorKey(Color.red, 1f)
    };
        // 알파는 startAlpha→endAlpha
        grad.alphaKeys = new[]
        {
        new GradientAlphaKey(startAlpha, 0f),
        new GradientAlphaKey(endAlpha,   1f)
    };
        return grad;
    }


}
