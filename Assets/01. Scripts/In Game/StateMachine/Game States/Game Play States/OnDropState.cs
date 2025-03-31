using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 플레이어 (혹은 AI, 상대)가 착수 했을 때 돌을 두고 승리 판정 혹은 게임 종료 조건을 검사하는 상태이다.
/// 1. Player, Opponent가 착수 했을 때 착수된 돌의 정보를 BoardManager의 Queue에 저장한다. (리플레이에도 활용)
/// 2. Queue의 최상단에는 이번 수의 착수된 돌이 기록되어 있을 것이니 Peek()해서 정보를 얻는다.
/// 3. Peek()된 정보를 가지고 Marking을 시도한다.
/// </summary>
public class OnDropState : IState
{
    public OnDropState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
        
        _actions.AddCommand(new TryOnDropCommand());
        _actions.AddCommand(new CheckEndGameCommand());
    }

    public void EnterState()
    {
        //OnDrop 효과음 재생
        EventManager.Instance.PublishSingleMessage(new EventMessage("OnDrop"));

        if (_actions.ExecuteCommands() is true)
        {
            StateMachine.ChangeState<ChangeTurnState>();
        }
        else
        {
            StateMachine.ChangeState<EndGameState>();
        }
    }

    public void UpdateState()
    {
        //Do nothing at UpdateState...
    }

    public void ExitState()
    {
        
    }

    private readonly Invoker _actions = new();
    public StateMachine StateMachine { get; set; }
}