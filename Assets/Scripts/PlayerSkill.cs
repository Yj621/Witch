using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Animator playerAnimator;
    private AnimatorOverrideController overrideController;

    private void Awake()
    {
        playerAnimator = GetComponentInParent<Animator>(); // 부모 오브젝트에서 Animator 가져오기
        overrideController = playerAnimator.runtimeAnimatorController as AnimatorOverrideController;
    }

    public void PlayAnimation(string stateName)
    {
        // 트리거 파라미터로 애니메이션 실행
        if (playerAnimator.parameters.Length > 0)
        {
            bool hasTrigger = false;

            foreach (var param in playerAnimator.parameters)
            {
                if (param.name == stateName && param.type == AnimatorControllerParameterType.Trigger)
                {
                    hasTrigger = true;
                    break;
                }
            }

            if (hasTrigger)
            {
                playerAnimator.SetTrigger(stateName);
            }
            else
            {
                Debug.LogError($"[PlayerSkill] 트리거 '{stateName}'을(를) 찾을 수 없습니다! 애니메이터의 파라미터 이름을 확인하세요.");
            }
        }
    }
}
