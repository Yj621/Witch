using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private float magnetDuration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.EnableMagnet(magnetRadius, magnetDuration);
            Destroy(gameObject);
        }
    }
}
