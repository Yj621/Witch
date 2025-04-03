using UnityEngine;

public class DefaultSkill : MonoBehaviour
{
    public Vector2 velocity = new Vector2(1, 0);
    void Start()
    {

    }

    private void FixedUpdate()
    {
        transform.Translate(velocity * Time.fixedDeltaTime);
    }
    void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        if (viewportPos.x < -0.1f || viewportPos.x > 1.1f || viewportPos.y < -0.1f || viewportPos.y > 1.1f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            gameObject.SetActive(false);
        }
        else if (other.gameObject.tag == "Enemy")
        {
            gameObject.SetActive(false);
            //other.GetComponent<EnemyController>().Hit(1);
        }
    }
}
