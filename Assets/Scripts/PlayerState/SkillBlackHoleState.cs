using UnityEngine;

public class SkillBlackHoleState : IState
{
    PlayerController player;
    public SkillBlackHoleState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("BlackHole");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }
    public string GetTrigger()
    {
        return "BlackHole"; // 해당 상태에서 사용할 트리거 값 반환
    }
}