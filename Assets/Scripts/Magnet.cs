using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Magnet : MonoBehaviour
{
    private Transform playerTrans;
    private CircleCollider2D circle;
    private Rigidbody2D rb;

    public void Initialize(Transform player)
    {
        playerTrans = player;
        // Awake에서 circle 참조 후, radius 설정은 이미 PlayerController가 한 상태
    }

    private void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        if (playerTrans == null) return;

        // FixedUpdate 타이밍에 정확히 플레이어 위치로 이동
        rb.MovePosition(playerTrans.position);
    }


    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (circle != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, circle.radius);
        }
#endif
    }
}
