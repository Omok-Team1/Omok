using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingState : IState
{
    public MatchingState(StateMachine stateMachine)
    {
        stateMachine = stateMachine;
    }
    
    public void EnterState()
    {
        GameManager.Instance.BoardManager.GameData.currentTurn = Turn.PLAYER1;
        StateMachine.ChangeState<PlayerState>();
    }

    public void UpdateState()
    {

    }

    public void ExitState()
    {

    }

    public StateMachine StateMachine { get; set; }
}
