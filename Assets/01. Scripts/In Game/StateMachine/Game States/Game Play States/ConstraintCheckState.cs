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
        _actions.InDependentExecuteCommands();
        
        bool branchFlag = _actions.ExecuteCommand(new BranchPlayerOpponent());

        //TODO: For Debug 테스트 끝나면 여기 주석 풀기
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
        //TODO: For Debug 테슽 끝나면 아래 코드 지우기
        //StateMachine.ChangeState<PlayerState>();
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
