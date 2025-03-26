using UnityEngine;

public class IdleState : IState
{
    PlayerController player;
    public IdleState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("Idle");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }

}
