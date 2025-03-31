using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitState : IState
{
    public InitState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
    public void EnterState()
    {

    }

    public void UpdateState()
    {
        //Do nothing at Unity Update Loop...
        StateMachine.ChangeState<MatchingState>();
    }

    public void ExitState()
    {

    }

    public StateMachine StateMachine { get; set; }
}
