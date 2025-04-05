using UnityEngine;

public class DefaultSkill : MonoBehaviour
{
    // 실제 velocity는 SkillManager에서 bullet.GetComponent<DefaultSkill>().velocity 로 동적 할당중
    public Vector2 velocity = new Vector2(5f, 0);
    public float defaultDamage = 2;
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
            DisableGameObj();
        }
    }

    // 스킬이 없어지게(애니메이션이 끝나면, 카메라 밖으로 벗어나면,)
    public void DisableGameObj()
    {
        gameObject.SetActive(false);
    }
    
    // 기본 공격 데미지
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            gameObject.SetActive(false);
        }
        else if (other.gameObject.tag == "Enemy")
        {
            other.GetComponent<EnemyMove>().EnemyHurt(defaultDamage);
            Debug.Log($"{other.gameObject}에게 {defaultDamage}만큼 데미지 입히기");
        }
    }
}
