using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static MonsterPool;

public class EnemyMove : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2f;
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
    }
    void Update()
    {
        if (isDie) return;
        if (isFrozen) return;

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
        if (freezeRoutine != null) StopCoroutine(freezeRoutine);
        freezeRoutine = StartCoroutine(FreezeCoroutine(duration));

        // 진행 중 패턴이 있으면 중단
        if (currentPattern != null)
        {
            StopCoroutine(currentPattern);
            currentPattern = null;
            isPattern = false;
        }
    }

    // 실제 얼음 상태를 관리하는 코루틴
    private IEnumerator FreezeCoroutine(float duration)
    {
        isFrozen = true;
        //ani.SetBool("IsFrozen", true);       // Animator에 얼음 애니메이션이 있는 경우
        
        yield return new WaitForSeconds(duration);

        isFrozen = false;
        //ani.SetBool("IsFrozen", false);      // 얼음 풀리는 애니메이션
        freezeRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Skill"))
        {
            QESkill qESKill = other.GetComponent<QESkill>();

            QESkill.Instance.Attack(gameObject.GetComponent<Collider2D>());
        }
    }
    public void EnemyHurt(float Damage)
    {
        CurrentHp -= Damage;

        // 데미지 텍스트 띄우기
        DamageManager.Instance.Show(
            Damage,
            transform.position + Vector3.up * 0.5f  // 살짝 위쪽으로 띄우기
        );

        if (CurrentHp <= 0)
        {
            Die();
        }
        else
        {
            ani.SetTrigger("Hurt");
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

        float originalSpeed = moveSpeed;
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
            currentPattern = null;
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
