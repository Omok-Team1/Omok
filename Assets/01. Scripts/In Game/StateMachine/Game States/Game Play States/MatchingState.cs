using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. UI에 플레이어 본인의 정보를 업데이트 한다.
/// 2. Matching 시스템을 구현하여, 상대방을 Matching 시켜준다.
/// </summary>
public class MatchingState : IState
{
    public MatchingState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
        
        _actions.AddCommand(new PushMatchingEventCommand());
    }
    
    public void EnterState()
    {
        _actions.ExecuteCommands();
            
        StateMachine.ChangeState<StartState>();
    }

    public void UpdateState()
    {
        //TODO: Matching 기능이 필요함. 기능이 구현되면 ChangeState를 여기서 호출해야한다.
    }

    public void ExitState()
    {

    }

    private readonly Invoker _actions = new();

    public StateMachine StateMachine { get; set; }
}
