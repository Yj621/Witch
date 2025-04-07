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
    public float Clean = 10f;
    public GameObject ExpCandyPrefab;

    public void Init(Transform player, MonsterType type)
    {
        isDie = false;
        target = player;
        this.type = type;
        CurrentHp = MaxHp;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }
    void Update()
    {
        if (isDie) return;

        Vector2 direction = (target.position - transform.position).normalized;

        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        if (direction.x > 0)
            spriteRenderer.flipX = false; // 오른쪽
        else if (direction.x < 0)
            spriteRenderer.flipX = true;  // 왼쪽
    }

    public void EnemyHurt(float Damage)
    {
        CurrentHp -= Damage;

        if (CurrentHp <= 0)
        {
            Die();
        }
        else
        {
            ani.SetTrigger("Hurt");
        }
    }


    void Die()
    {
        isDie = true;
        GameManager.Instance.currentClean += Clean;
        ani.SetTrigger("Die");
        StartCoroutine(DropExpCandies());
    }

    private IEnumerator DropExpCandies()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject candy = Instantiate(ExpCandyPrefab, transform.position, Quaternion.identity);
        candy.GetComponent<ExpCandy>();
        yield return new WaitForSeconds(0.5f);
        MonsterPool.Instance.Return(type, this.gameObject);
    }
}
