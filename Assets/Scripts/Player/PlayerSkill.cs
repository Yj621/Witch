using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Animator skillAnimator;
    private PlayerInput playerInput;
    private SkillManager skillManager;

    private void Awake()
    {
        skillAnimator = GetComponent<Animator>(); 
        playerInput = GetComponentInParent<PlayerInput>();
        skillManager = GetComponentInParent<SkillManager>();
    }

    private void Update()
    {
    }

    public void SyncSkillAnimation()
    {
        if (playerInput == null || skillAnimator == null) return;

        string currentState = playerInput.GetCurrentTriggerName();

        // 스킬 애니메이터의 상태도 변경
        skillAnimator.SetTrigger(currentState);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        float damage = skillManager.GetSkillDamage(playerInput.GetCurrentTriggerName());
        if (other.gameObject.tag == "Ground")
        {
            Destroy(other.gameObject);     
        }
        else if (other.gameObject.tag == "Enemy")
        {
            gameObject.SetActive(false);
            //other.GetComponent<EnemyMove>().Hit(damage);
        }
    }
}
