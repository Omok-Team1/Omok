using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentState : IState
{
    public OpponentState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
    
    public void EnterState()
    {
        var bestCoordi = OmokAIController.GetBestMove();
        
        GameManager.Instance.BoardManager.RecordDrop(bestCoordi);
        
        StateMachine.ChangeState<OnDropState>();
    }

    public void UpdateState()
    {
        //Do nothing at Unity Update event loop...
    }

    public void ExitState()
    {

    }

    private readonly Invoker _actions;
    public StateMachine StateMachine { get; set; }
}
