using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static MonsterPool;

public class EnemyMove : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2f;
    SpriteRenderer spriteRenderer;
    public float MaxHp = 20f;
    private float CurrentHp = 20f;
    Animator ani;
    public MonsterType type;
    private bool isDie = false;
    public int Exp = 50;
    public GameObject ExpCandyPrefab;

    public void Init(Transform player, MonsterType type)
    {
        target = player;
        this.type = type;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }
    void Update()
    {
        Vector2 direction = (target.position - transform.position).normalized;

        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        if (direction.x > 0)
            spriteRenderer.flipX = false; // 오른쪽
        else if (direction.x < 0)
            spriteRenderer.flipX = true;  // 왼쪽
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Skill"))
        {
            //EnemyHurt();
            if(CurrentHp < 0)
            {
                Die();
            }
        }
    }

    public void EnemyHurt(float Damage)
    {
        ani.SetTrigger("Hurt");
        CurrentHp -= Damage;
    }

    void Die()
    {
        ani.SetTrigger("Die");
        isDie = true;
        MonsterPool.Instance.Return(type, this.gameObject);
        StartCoroutine(DropExpCandies());
    }

    private IEnumerator DropExpCandies()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject candy = Instantiate(ExpCandyPrefab, transform.position, Quaternion.identity);
        candy.GetComponent<ExpCandy>();
    }
}
