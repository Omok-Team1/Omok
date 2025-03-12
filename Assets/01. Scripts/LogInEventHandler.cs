using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInEventHandler
{
    public delegate void OnUpdatePlayersPoint(GameData data);
    public event OnUpdatePlayersPoint onUpdatePlayersPoint;

    public delegate void OnUpdateTurn(GameData data);
    public event OnUpdateTurn onUpdateTurn;
    
    public void UpdatePlayersPoint(GameData data)
    {
        onUpdatePlayersPoint?.Invoke(data);
    }

    public void UpdateTurn(GameData data)
    {
        onUpdateTurn?.Invoke(data);
    }
}
