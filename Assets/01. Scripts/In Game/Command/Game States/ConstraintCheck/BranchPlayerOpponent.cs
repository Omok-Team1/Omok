using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BranchPlayerOpponent : ICommand
{
    public bool Execute()
    {
        if (_boardManager.GameData.currentTurn == Turn.PLAYER1)
        {
            return true;
        }
        else if (_boardManager.GameData.currentTurn == Turn.PLAYER2)
        {
            return false;
        }
        else
            throw new InvalidDataException("유효한 데이터가 아니라서, 플레이어 상태 혹은 상대방 상태로 전환할 수 없었습니다.");    
        
        return true;
    }
    
    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}
