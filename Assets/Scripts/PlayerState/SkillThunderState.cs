using UnityEngine;

public class SkillThunderState : IState
{
    PlayerController player;
    public SkillThunderState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("Thunder");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }
    public string GetTrigger()
    {
        return "Thunder"; // 해당 상태에서 사용할 트리거 값 반환
    }
}