using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Magnet : MonoBehaviour
{
    private Transform playerTrans;
    private CircleCollider2D circle;

    public void Initialize(Transform player)
    {
        playerTrans = player;
        // Awake에서 circle 참조 후, radius 설정은 이미 PlayerController가 한 상태
    }

    private void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
        // Awake 시점에 radius나 isTrigger 세팅이 필요하다면 여기서도 처리 가능
    }

    private void Update()
    {
        if (playerTrans != null)
        {
            transform.position = playerTrans.position;
        }
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
