using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTurnCommand : ICommand
{
    public bool Execute()
    {
        _boardManager.ChangeTurn();
        
        Debug.Log(_boardManager.GameData.currentTurn);

        return true;
    }

    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}