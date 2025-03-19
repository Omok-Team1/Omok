using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 입력을 대기 받고, 입력 후 착수 버튼을 누르는 이벤트를 기다린다.
/// 이벤트가 발생하면, OnDrop 상태로 전환한다.
/// </summary>
public class PlayerState : IState
{
    public PlayerState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
        
        _actions.AddCommand(new PlayerCommand());
    }
    
    public void EnterState()
    {
        
    }

    public void UpdateState()
    {
        //착수 버튼을 눌렀다면
        if (_actions.ExecuteCommands() is true)
        {
            StateMachine.ChangeState<OnDropState>();
        }
    }

    public void ExitState()
    {

    }

    private readonly Invoker _actions = new();
    
    public StateMachine StateMachine { get; set; }
}
