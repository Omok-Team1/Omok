using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PushEndGameEventCommand : ICommand
{
    
    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;

    public bool Execute()
    {
        
        var message = new EventMessage("EndGame");

        message.AddParameter<Turn>(_boardManager.GameData.winner);
        
        message.AddParameter<int>(_boardManager.GameData.victoryPoint);
        
        message.AddParameter<Stack<Cell>>(_boardManager.MatchRecord.Reverse());

        ReplayManager.Instance.SaveReplay(
            _boardManager, 
            _boardManager.GameData.winner
        );

        EventManager.Instance.PushEventMessageEvent(message);
        EventManager.Instance.PublishMessageQueue();

        return true;
    }
}