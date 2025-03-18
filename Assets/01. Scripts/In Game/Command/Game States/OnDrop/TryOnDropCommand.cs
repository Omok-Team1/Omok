using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryOnDropCommand : ICommand
{
    public bool Execute()
    {
        if (_boardManager.OnDropMarker() is false)
            return false;
        else
            return true;
    }

    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}