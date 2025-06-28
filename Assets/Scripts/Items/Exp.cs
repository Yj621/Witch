using UnityEngine;

public class Exp : MonoBehaviour
{
    // 자석의 세기
    public float magnetStrength = 5f;
    // 거리 보정 계수
    public float distanceStrength = 10f;
    // 자석 필드에서 벗어났을 때도 힘을 유지할지 여부
    public bool looseMagnet = true;

    private Rigidbody2D thisRb;
    private Transform magnetTrans;
    private bool magnetInZone;

    private void Awake()
    {
        thisRb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!magnetInZone || magnetTrans == null)
            return;

        // Rigidbody2D.position 사용
        Vector2 from = thisRb.position;
        Vector2 to = magnetTrans.position;
        Vector2 dir = to - from;
        float dist = dir.magnitude;

        if (dist <= 0.01f)
            return;

        float forceMag = (distanceStrength / dist) * magnetStrength;
        thisRb.AddForce(dir.normalized * forceMag, ForceMode2D.Force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

    /// <summary>
    /// PlayerController 등에서 수동 호출할 경우
    /// </summary>
    public void SetMagnet(Transform magnet)
    {
        magnetTrans = magnet;
        magnetInZone = true;
    }

    /// <summary>
    /// 자석 효과 해제
    /// </summary>
    public void ClearMagnet()
    {
        magnetInZone = false;
        magnetTrans = null;
        thisRb.linearVelocity = Vector2.zero;
    }
}
