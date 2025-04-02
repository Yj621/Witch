using UnityEngine;

public class SkillIcePillarState  : IState
{
    PlayerController player;
    public SkillIcePillarState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("IcePillar");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }
    public string GetTrigger()
    {
        return "IcePillar"; // 해당 상태에서 사용할 트리거 값 반환
    }
}