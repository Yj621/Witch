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

    public string GetTrigger()
    {
        return "Spin"; // 해당 상태에서 사용할 트리거 값 반환
    }
}
