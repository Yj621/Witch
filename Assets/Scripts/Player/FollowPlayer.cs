using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target; // 플레이어의 Transform
    public Vector3 baseOffset; // 기본 위치 오프셋
    private Vector3 currentOffset; // 현재 적용 중인 오프셋


    private void LateUpdate()
    {
        if (target != null)
        {
            // Player의 방향에 따라 offset을 동적으로 반전
            float direction = Mathf.Sign(target.localScale.x); // 오른쪽: 1, 왼쪽: -1
            if (direction == -1)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            currentOffset = new Vector3(baseOffset.x * direction, baseOffset.y, baseOffset.z);

            // 위치 설정
            transform.position = target.position + currentOffset;
        }
    }
}

