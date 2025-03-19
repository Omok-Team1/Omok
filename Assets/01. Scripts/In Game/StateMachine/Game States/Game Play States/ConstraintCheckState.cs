using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstraintCheckState : IState
{
    public ConstraintCheckState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
        
        //TODO: 이전에는 쌍삼 자리였지만, 이번 턴에는 쌍삼 자리가 아니게 되었을 때 x 마커를 지워주는 커맨드가 필요
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
        
        bool branchFlag = _actions.ExecuteCommand(new BranchPlayerOpponent());

        //true : Player 상태로 전환
        //false : Opponent 상태로 전환
        if (branchFlag is true)
        {
            StateMachine.ChangeState<PlayerState>();
        }
        else
        {
            StateMachine.ChangeState<OpponentState>();
        }
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
