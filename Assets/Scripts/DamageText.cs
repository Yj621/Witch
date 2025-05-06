using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float duration = 1f;
    private float elapsed = 0f;

    public void Setup(float damage)
    {
        damageText.text = damage.ToString("0.#");
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;  

        elapsed += Time.deltaTime;
        if(elapsed>=duration)
        {
            Destroy(gameObject);
        }
    }
}
