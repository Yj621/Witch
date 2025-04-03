using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Animator skillAnimator;
    private PlayerInput playerInput;

    private void Awake()
    {
        skillAnimator = GetComponent<Animator>(); // Skill Animator 가져오기
        playerInput = GetComponentInParent<PlayerInput>(); // PlayerController 가져오기
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
}
