using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTurnCommand : ICommand
{
    public bool Execute()
    {
        _boardManager.ChangeTurn();

        return true;
    }

    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}