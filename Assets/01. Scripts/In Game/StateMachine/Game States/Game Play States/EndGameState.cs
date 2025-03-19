using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameState : IState
{
    /*
     * 1. 결과 화면 활성화 하기
     * 2. 이번 대국의 정보를 메세지로 만들어서 큐를 Publish 해주기
     */
    public EndGameState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
        
        _actions.AddCommand(new PushEndGameEventCommand());
    }
    
    public void EnterState()
    {
        _actions.ExecuteCommands();
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        
    }

    private readonly Invoker _actions = new();
    public StateMachine StateMachine { get; set; }
}
