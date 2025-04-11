using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target; // 플레이어의 Transform
    public Vector3 offset;   // 플레이어에 대한 위치 오프셋 (필요한 경우)

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
