using UnityEngine;

public class DieState : IState
{
    PlayerController player;
    public DieState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("Die");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }

}
