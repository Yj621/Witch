using UnityEngine;

public class DashState : IState
{
    PlayerController player;
    public DashState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("Dash");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }

}
