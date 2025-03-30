using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommand : ICommand
{
    public bool Execute()
    {
        if (InputManager.GetInput() is true)
            _raycastHit2D = InputManager.TryRaycastHit(nameof(Cell));
        else
            return false;

        if (_raycastHit2D.collider is null)
            return false;
        else
        {
            _boardManager.RecordDrop(_raycastHit2D.collider.TryGetComponent<Cell>(out Cell cell) ? cell : null);
            
            GameManager.Instance.TimerController.EndTurn();
            
            return true;
        }
    }
    
    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
    private RaycastHit2D _raycastHit2D;
}