using UnityEngine;

public class SkillFireSlahsState : IState
{
    PlayerController player;
    public SkillFireSlahsState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("FireSlahs");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }
    public string GetTrigger()
    {
        return "FireSlahs"; // 해당 상태에서 사용할 트리거 값 반환
    }
}