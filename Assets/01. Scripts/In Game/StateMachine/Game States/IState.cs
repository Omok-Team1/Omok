public interface IState
{
    void EnterState();
    void UpdateState();
    void ExitState();

    StateMachine StateMachine { get; set; }
}