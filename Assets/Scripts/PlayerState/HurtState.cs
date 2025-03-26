using UnityEngine;

public class HurtState : IState
{
    PlayerController player;
    public HurtState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("Hurt");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }

}
