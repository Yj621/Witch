public class StateMachine
{
    public IState CurrentState { get; private set; }
    PlayerController playerController;
    public IdleState idleState;
    public WalkState walkState;
    public DieState dieState;
    public SpinState spinState;
    public HurtState hurtState;
    public DashState dashState;
    public SkillFireSlashState skillFireSlashState;
    public SkillIcePillarState skillIcePillarState;
    public SkillThunderState skillThunderState;
    public SkillBlackHoleState skillBlackHoleState;


    public StateMachine(PlayerController player)
    {
        playerController = player;
        idleState = new IdleState(player);
        walkState = new WalkState(player);
        hurtState = new HurtState(player);
        dieState = new DieState(player);
        spinState = new SpinState(player);
        dashState = new DashState(player);
        skillFireSlashState = new SkillFireSlashState(player);
        skillIcePillarState = new SkillIcePillarState(player);
        skillThunderState = new SkillThunderState(player);
        skillBlackHoleState = new SkillBlackHoleState(player);
    }

    //따라서 최초 state를 받아 이를 CurrentState에 넣고 Enter
    public void Initialize(IState state)
    {
        CurrentState = state;
        state.Enter();
    }

    //바뀔 state를 받아 현재 state에 대해서는 Exit를 수행하고 CurrentState를 바꾸며 
    //바뀔 state의 Enter를 수행
    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }

    //현재 있는 state에서 Excute()를 부르는 부분.
    //물론 우리 State들에 Excute는 없다.
    public void Execute()
    {
        CurrentState.Execute();
    }
}