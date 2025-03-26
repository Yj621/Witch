using UnityEngine;

public class SpinState : IState
{
    PlayerController player;
    public SpinState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("Spin");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }

}
