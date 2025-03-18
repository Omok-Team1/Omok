using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstraintCheckState : IState
{
    public ConstraintCheckState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
        
        _actions.AddCommand(new CheckDoubleThree());
    }
    
    public void EnterState()
    {
        if (_actions.ExecuteCommands() is true)
        {
            Debug.Log("쌍삼이 검출 되었습니다.");
        }
        else
        {
            Debug.Log("쌍삼이 검출 되지 않았습니다.");
        }
        
        StateMachine.ChangeState<PlayerState>();
    }

    public void UpdateState()
    {
        // Do nothing at Unity Update event loop...
    }

    public void ExitState()
    {

    }

    private readonly Invoker _actions = new();
    
    public StateMachine StateMachine { get; set; }
}
