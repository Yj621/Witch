using UnityEngine;

public class SkillFireBallState  : IState
{
    PlayerController player;
    public SkillFireBallState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("FireBall");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }
    public string GetTrigger()
    {
        return "FireBall"; // 해당 상태에서 사용할 트리거 값 반환
    }
}