using UnityEngine;

public class Exp : MonoBehaviour
{
    // 자석의 세기
    public float magnetStrength = 5f;
    // 거리 보정 계수
    public float distanceStrength = 10f;
    // 인력 방향: 1이면 끌어당김, -1이면 밀어냄
    public int magnetDirection = 1;
    // 자석 필드에서 벗어났을 때도 힘을 유지할지 여부
    public bool looseMagnet = true;

    private Transform trans;
    private Rigidbody2D thisRb;
    private Transform magnetTrans;
    private bool magnetInZone;

    private void Awake()
    {
        trans = transform;
        thisRb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (magnetInZone && magnetTrans != null)
        {
            Vector2 dir = (Vector2)magnetTrans.position - (Vector2)trans.position;
            float distance = dir.magnitude;
            if (distance > 0.01f)
            {
                float forceMagnitude = (distanceStrength / distance) * magnetStrength;
                Vector2 force = dir.normalized * (forceMagnitude * magnetDirection);
                thisRb.AddForce(force, ForceMode2D.Force);
            }
        }
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
        {
            ClearMagnet();
        }
    }

    /// <summary>
    /// PlayerController에서 이미 범위 내 Exp에 대해 수동 호출
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
    }
}
