using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BoardManager : MonoBehaviour
{
    void Awake()
    {
        _gameData = Addressables.LoadAssetAsync<GameData>("Assets/02. Prefabs/GameData/GameData.asset").WaitForCompletion();
        
        _grid ??= FindObjectOfType<BoardGrid>();
    }
    
    public bool OnDropMarker()
    {
        var selected = _matchRecord.Peek()._coordinate;
        
        if (_matchRecord.Peek() is not null && _grid[selected.Item1, selected.Item2].Marker == _gameData.emptySprite)
            return _grid.TryMarkingOnCell(selected);

        return false;
    }
    
    public bool OnDropMarker((int, int) coordi)
    {
        Cell opponentMove = _grid[coordi.Item1, coordi.Item2];
        
        if (opponentMove.Marker == _gameData.emptySprite)
            return _grid.TryMarkingOnCell(coordi);

        return false;
    }

    public void ChangeTurn()
    {
        _gameData.ChangeTurn();
    }
    
    public bool CheckForWin()
    {
        //clock-wise (n -> ne -> e -> se)
        //row
        int[] dy = { 1, 1, 0, -1};
        //col
        int[] dx = { 0, 1, 1, 1};

        var cell = _matchRecord.Peek();

        int row = cell._coordinate.Item1;
        int col = cell._coordinate.Item2;
        
        for (int dir = 0; dir < dy.Length; dir++)
        {
            int count = CheckWinRecursive(row, col, dy[dir], dx[dir], 0, cell) +
                        CheckWinRecursive(row, col, -dy[dir], -dx[dir], 0, cell) - 1;

            if (count == 5)
            {
                _gameData.SetWinner();
                return true;
            }
        }

        return false;
    }

    private int CheckWinRecursive(int row, int col, int dy, int dx, int count, Cell curPlayerSelect)
    {
        if (_grid[row, col] is null)
            return count;
        if (_grid[row, col].Marker != curPlayerSelect.Marker)
            return count;

        return CheckWinRecursive(row + dy, col + dx, dy, dx, count + 1, curPlayerSelect);
    }

    public bool IsGridFull()
    {
        return _grid.RemainCells;
    }

    public void RecordDrop(Cell data)
    {
        if (data is null)
            throw new NullReferenceException("스택에 Null이 기록 되었습니다.");
        else
            _matchRecord.Push(data);
    }

    public Cell GetRecentOnDrop()
    {
        return _matchRecord.Peek();
    }

    public void TempConstraintsCheck()
    {
        var currentTurnCells = _matchRecord.Where(c => c.CellOnwer == _gameData.currentTurn).ToList();

        //clock-wise (n -> ne -> e -> se -> s -> sw -> w -> nw)
        //row
        int[] dy = { 1, 1, 0, -1, -1, -1, 0, 1};
        //col
        int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1};
        
        foreach (Cell cell in currentTurnCells)
        {
            for (int dir = 0; dir < dy.Length; dir++)
            {
                int ny = cell._coordinate.Item1 + dy[dir];
                int nx = cell._coordinate.Item2 + dx[dir];

                if (_grid[ny, nx] is null) continue;
                
                if (_grid[ny, nx].CellOnwer != _gameData.currentTurn ||
                    _grid[ny, nx].CellOnwer != Turn.NONE)
                {
                    continue;
                }
                
                
            }
        }
    }

    private int TempConstraintsCheckRecursive(int row, int col, int dy, int dx, int count, bool isAppearNone, ref bool isOpenFour)
    {
        if (_grid[row, col] is null) return count;

        if (_grid[row, col].CellOnwer != _gameData.currentTurn ||
            _grid[row, col].CellOnwer != Turn.NONE)
        {
            isOpenFour = false;
            return count;
        }

        if (_grid[row, col].CellOnwer == Turn.NONE && isAppearNone is not false)
        {
            // 4-3은 3-3과 다르므로, 4-3은 검사하지 않는다.
            if (_grid[row + dy, col + dx] is null ||
                _grid[row + 2 * dy, col + 2 * dx] is null) isOpenFour = false;
            
            return count;
        }

        if (_grid[row, col].CellOnwer == Turn.NONE)
            isAppearNone = true;
        
        return TempConstraintsCheckRecursive(row + dy, col + dx, dy, dx, count + 1, isAppearNone, ref isOpenFour);
    }
    
    
    private BoardGrid _grid;
    public BoardGrid Grid => _grid;
    
    private GameData _gameData;

    private readonly Stack<Cell> _matchRecord = new();
}
