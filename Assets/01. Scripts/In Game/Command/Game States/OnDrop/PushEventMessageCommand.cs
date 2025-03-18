using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushEventMessageCommand : ICommand
{
    public bool Execute()
    {
        var onDropMessage = new EventMessage("OnDrop");
        
        onDropMessage.AddParameter<Cell>(_boardManager.GetRecentOnDrop());
        
        EventManager.Instance.PushEventMessageEvent(onDropMessage);
        
        return true;
    }

    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}
