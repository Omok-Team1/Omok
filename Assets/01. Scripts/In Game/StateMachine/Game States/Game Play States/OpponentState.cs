using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentState : IState
{
    public OpponentState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;

        _eventSO = ScriptableObject.CreateInstance<TimeOutOnEvent>();
        
        EventManager.Instance.AddListener("Player2TimeOver", _eventSO, StateMachine.gameObject);
    }
    
    public void EnterState()
    {
        var bestCoordi = OmokAIController.GetBestMove();
        
        GameManager.Instance.BoardManager.RecordDrop(bestCoordi);
        
        StateMachine.ChangeState<OnDropState>();
        
        GameManager.Instance.TimerController.EndPlayerTurn();
    }

    public void UpdateState()
    {
        //Do nothing at Unity Update event loop...
    }

    public void ExitState()
    {
       
    }
    
    private readonly IOnEventSO _eventSO;

    private readonly Invoker _actions;
    public StateMachine StateMachine { get; set; }
}
