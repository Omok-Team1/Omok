using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDoubleFour : ICommand
{
    public bool Execute()
    {
        // if (_constraintCoordinates is not null)
        // {
        //     foreach (Cell constraintCoordinate in _constraintCoordinates)
        //     {
        //         constraintCoordinate.EraseMarker();
        //     }
        //
        //     _constraintCoordinates = null;
        // }
        
        _constraintCoordinates = _boardManager.CheckDoubleFour();
        
        if (_constraintCoordinates is not null)
        {
            foreach (Cell cell in _constraintCoordinates)
            {
                Debug.Log("4-4 : " + cell._coordinate);
                _boardManager.OnDropMarker(cell._coordinate, _boardManager.GameData.constraintMarker);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private List<Cell> _constraintCoordinates = new();
    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}
