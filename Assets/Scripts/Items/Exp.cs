using UnityEngine;

public class Exp : MonoBehaviour
{
    [Header("자석 이동 속도")]
    public float magnetStrength = 5f;
    public float distanceStrength = 10f;

    [Header("자석 유지 여부")]
    public bool looseMagnet = true;

    [Header("획득 거리 (현재 사용 안 함)")]
    [SerializeField] private float collectDistance = 0.2f;

    private Transform magnetTrans;
    private bool magnetInZone;

    private bool canBeCollected = false;
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        // 생성 직후 바로 획득 방지
        Invoke(nameof(EnableCollect), 0.2f);
    }

    private void EnableCollect()
    {
        canBeCollected = true;
    }

    private void Update()
    {
        if (!magnetInZone || magnetTrans == null)
            return;

        // 자석 흡수 중에는 Collider 끔 (물리 비용 제거)
        if (col.enabled)
            col.enabled = false;

        Vector2 from = transform.position;
        Vector2 to = magnetTrans.position;
        Vector2 dir = to - from;
        float dist = dir.magnitude;

        float speed = (distanceStrength / dist) * magnetStrength;
        transform.position = Vector2.MoveTowards(from, to, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeCollected)
            return;

        if (collision.CompareTag("Magnet"))
        {
            magnetTrans = collision.transform;
            magnetInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnet") && looseMagnet)
            ClearMagnet();
    }

    public void ClearMagnet()
    {
        magnetInZone = false;
        magnetTrans = null;

        // 다시 Collider 켜주기
        if (col != null)
            col.enabled = true;
    }
}
