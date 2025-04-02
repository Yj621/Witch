using UnityEngine;

public class ExpCandy : MonoBehaviour
{
    public int expValue = 0;

    public void SetExpValue(int value)
    {
        expValue = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어 경험치 추가
            other.GetComponent<Player>().GetExperience(expValue);
            Destroy(gameObject);
        }
    }
}
