using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDoubleThree : ICommand
{
    public bool Execute()
    {
        var doubleCell = _boardManager.ConstraintsCheck();
        
        if (doubleCell is not null)
        {
            foreach (Cell cell in doubleCell)
            {
                _boardManager.OnDropMarker(cell._coordinate, _boardManager._gameData.constraintMarker);
                Debug.Log(cell._coordinate);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}
