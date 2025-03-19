using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTurnState : IState
{
    public ChangeTurnState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
        
        _actions.AddCommand(new ChangeTurnCommand());
    }
    
    public void EnterState()
    {
        if(_actions.ExecuteCommands() is true)
        {
            StateMachine.ChangeState<ConstraintCheckState>();
        }
        else
        {
            throw new Exception("Invalid Access");
        }
    }

    public void UpdateState()
    {
        //Do nothing at Unity Update event loop...
    }

    public void ExitState()
    {

    }

    private readonly Invoker _actions = new();
    
    public StateMachine StateMachine { get; set; }
}
