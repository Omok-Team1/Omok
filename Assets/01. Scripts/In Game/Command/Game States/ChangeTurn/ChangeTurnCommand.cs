using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTurnCommand : ICommand
{
    public bool Execute()
    {
        _boardManager.ChangeTurn();

        EventMessage message;
        
        if(_boardManager.GameData.currentTurn == Turn.PLAYER1) 
            message = new EventMessage("PlayerTurn");
        else
            message = new EventMessage("OpponentTurn");
        
        EventManager.Instance.PushEventMessageEvent(message);
        EventManager.Instance.PublishMessageQueue();
        
        Debug.Log(_boardManager.GameData.currentTurn);

        return true;
    }

    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}