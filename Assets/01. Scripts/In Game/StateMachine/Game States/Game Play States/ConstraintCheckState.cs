using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstraintCheckState : IState
{
    public ConstraintCheckState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
        
        _actions.AddCommand(new CheckDoubleThree());
        _actions.AddCommand(new CheckDoubleFour());
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
        
        bool branchFlag = _actions.ExecuteCommand(new BranchPlayerOpponent());

        //TODO: 테스트를 위해 잠시 주석처리
        //true : Player 상태로 전환
        //false : Opponent 상태로 전환
        // if (branchFlag is true)
        // {
        //     StateMachine.ChangeState<PlayerState>();
        // }
        // else
        // {
        //     StateMachine.ChangeState<OpponentState>();
        // }
        
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
