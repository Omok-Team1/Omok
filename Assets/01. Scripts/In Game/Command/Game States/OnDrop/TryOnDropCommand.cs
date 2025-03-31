using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryOnDropCommand : ICommand
{
    public bool Execute() => _boardManager.OnDropMarker();
    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}