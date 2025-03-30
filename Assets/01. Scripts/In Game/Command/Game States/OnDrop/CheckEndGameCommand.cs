using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEndGameCommand : ICommand
{
    public bool Execute()
    {
        if(_boardManager.CheckForWin() is true)
            return false;
        
        if (_boardManager.IsGridFull() is true)
            return false;
        
        return true;
    }

    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}
