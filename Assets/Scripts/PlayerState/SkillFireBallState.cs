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

}