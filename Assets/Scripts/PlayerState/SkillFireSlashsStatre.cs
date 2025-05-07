using UnityEngine;

public class SkillFireSlashState : IState
{
    PlayerController player;
    public SkillFireSlashState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.StartHurtRoutine();
        player.GetComponent<Animator>().SetTrigger("FireSlashs");
        Debug.Log("FireSlahs");
        SoundManager.Instance.PlaySFX("FireSlash");
    }
    public void Exit()
    {

    }
    public void Execute()
    {

    }
    public string GetTrigger()
    {
        return "FireSlashs"; // 해당 상태에서 사용할 트리거 값 반환
    }
}