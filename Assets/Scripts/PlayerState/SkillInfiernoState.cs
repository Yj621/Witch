using UnityEngine;

public class SkillInfiernoState : IState
{
    PlayerController player;
    public SkillInfiernoState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("Infierno");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }
    public string GetTrigger()
    {
        return "Infierno";
    }
}