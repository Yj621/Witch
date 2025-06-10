using UnityEngine;

public class Magnet : MonoBehaviour
{
    public Vector2 newPosition;
    private Transform trans;

    private void Awake()
    {
        trans = transform;
    }
    void Start()
    {
        
    }

    void Update()
    {
        trans.position = Vector2.Lerp(trans.position, newPosition, Time.deltaTime * 1.5f);

        if(Mathf.Abs(newPosition.x - trans.position.x) < 0.05)
        {
            trans.position = newPosition;    
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "EXP")
        {
            Destroy(collision.gameObject);
        }
    }
}
