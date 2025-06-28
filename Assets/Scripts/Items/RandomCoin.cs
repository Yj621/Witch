using UnityEngine;

public class RandomCoin : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return; // 이미 먹었으면 무시
        if (collision.CompareTag("Player"))
        {
            collected = true;
            Coin.coin++;
            Destroy(gameObject);
        }
    }
}
