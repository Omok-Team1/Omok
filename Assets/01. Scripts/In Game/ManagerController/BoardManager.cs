using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

public class BoardManager : MonoBehaviour
{
    void Awake()
    {
        _gameData = Addressables.LoadAssetAsync<GameData>("Assets/02. Prefabs/GameData/GameData.asset").WaitForCompletion();
        
        _grid ??= FindObjectOfType<BoardGrid>();
        
        _grid.Init(_gameData);
    }
    
    public bool OnDropMarker()
    {
        if (_isTimeOut is true)
        {
            //이전 턴의 플레이어가 시간 초과, 돌을 놓지 않고 true를 반환해 ChangeTurnState로 전환한다.
            _isTimeOut = false;
            return true;
        }
        
        var selected = _matchRecord.Peek()._coordinate;

        if (_matchRecord.Peek() is not null && _grid[selected.Item1, selected.Item2].Marker == _gameData.emptySprite)
            return _grid.TryMarkingOnCell(selected);
        //for debug
        else if (_grid[selected.Item1, selected.Item2].Marker != _gameData.emptySprite)
        {
            Debug.LogError("해당 좌표에 이미 돌이 놓여져 있습니다.");
            //TODO: AI 알고리즘 수정이 필요함
            //TODO: 테스트를 위해 임시로 에러를 무시함
            return true;
            //throw new Exception("Duplicated Marker found.");
        }
        
        return false;
    }
    
    public bool OnDropMarker((int, int) coordi, Sprite marker = null)
    {
        Cell selectedCell = _grid[coordi.Item1, coordi.Item2];
        
        selectedCell.SelectedCell(true);
        
        if (selectedCell.Marker == _gameData.emptySprite)
            return _grid.TryMarkingOnCell(coordi, marker);

        return false;
    }

    public void ChangeTurn()
    {
        _gameData.ChangeTurn();
    }
    
    public bool CheckForWin()
    {
        //첫 수에 플레이어가 돌을 놓지 않으면 승리 검사를 하지 않는다.
        if (_matchRecord.Count <= 0)
        {
            return false;
        }
        
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

    //Player 용
    public void RecordDrop(Cell data)
    {
        if (data is null)
            throw new NullReferenceException("스택에 Null이 기록 되었습니다.");
        else
        {
            data.SelectedCell(true);
            _matchRecord.Push(data);
        }
    }
    
    //Opponent 용
    public void RecordDrop((int, int)? data)
    {
        if (data is null)
            throw new NullReferenceException("스택에 Null이 기록 되었습니다.");
        //인공 지능이 제한 시간안에 연산을 끝내지 못하면 -무한대 튜플을 반환한다.
        else if (data.Value.Item1 == -INF && data.Value.Item2 == -INF)
        {
            _isTimeOut = true;
        }
        else
        {
            _grid[data.Value.Item1, data.Value.Item2].SelectedCell(true);
            _matchRecord.Push(_grid[data.Value.Item1, data.Value.Item2]);
        }
    }

    public Cell GetRecentOnDrop()
    {
        return _matchRecord.Peek();
    }

    //여러가지 제약 조건을 추가 할 수 있도록 분리시키자!
    public List<Cell> ConstraintsCheck()
    {
        var currentTurnCells = _matchRecord.Where(c => c.CellOwner == _gameData.currentTurn).ToList();
        
        List<Cell> cells = new List<Cell>();
        
        //clock-wise (n -> ne -> e -> se)
        //row
        int[] dy = { 1, 1, 0, -1, -1, -1, 0, 1};
        //col
        int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1};
        
        /*
         * 현재 놓여져 있는 돌들에 대해, 8 방향에 빈 칸이 존재하는지 확인한 후
         * 빈칸이 존재한다면, 해당 방향으로 2칸을 가상의 돌을 놓아가면서 재귀적으로 3-3 조건을 탐색한다.
         */
        foreach (var currentTurnCell in currentTurnCells)
        {
            int curr = currentTurnCell._coordinate.Item1;
            int curc = currentTurnCell._coordinate.Item2;
            
            for (int dir = 0; dir < dy.Length; dir++)
            {
                for (int step = 1; step <= 2; step++)
                {
                    int nr = curr + dy[dir] * step;
                    int nc = curc + dx[dir] * step;
                    
                    if (_grid[nr, nc] is null) break;
                
                    if (_grid[nr, nc].CellOwner != _gameData.currentTurn &&
                        _grid[nr, nc].CellOwner != Turn.NONE)
                    {
                        break;
                    }

                    if (_grid[nr, nc].CellOwner == _gameData.currentTurn)
                        break;
                    
                    int doubleThreeCounter = 0;

                    for (int virtualDir = 0; virtualDir < 4; virtualDir++)
                    {
                        int counter = 0;
                        bool isAppearNone = false;
                        bool isOpenFour = false;
                    
                        _grid[nr, nc].CellOwner = _gameData.currentTurn;
                        
                        counter = ConstraintsCheckRecursive(nr, nc, dy[virtualDir], dx[virtualDir], 0, isAppearNone, ref isOpenFour) + 
                                  ConstraintsCheckRecursive(nr, nc, -dy[virtualDir], -dx[virtualDir], 0, isAppearNone, ref isOpenFour) - 1;

                        _grid[nr, nc].CellOwner = Turn.NONE;
                        
                        if (counter == 3 && isOpenFour)
                        {
                            doubleThreeCounter++;
                        }

                        if (doubleThreeCounter == 2)
                        {
                            cells.Add(_grid[nr, nc]);
                            break;
                        }
                    }
                }
            }
        }

        if (cells.Count > 0)
            return cells;
        else
            return null;
    }

    private int ConstraintsCheckRecursive(int row, int col, int dy, int dx, int count, bool isAppearNone, ref bool isOpenFour)
    {
        //Base Condition
        //현재 보는 칸이 보드 밖이라면
        if (_grid[row, col] is null)
        {
            if(isAppearNone is true) isOpenFour = true;
            else isOpenFour = false;
            
            return count;
        }
        
        //Base Condition
        //현재 보는 칸이 상대방의 돌이라면 해당 방향은 더 이상 볼 필요가 없다.
        if (_grid[row, col].CellOwner != _gameData.currentTurn &&
            _grid[row, col].CellOwner != Turn.NONE)
        {
            isOpenFour = false;
            return count;
        }

        // 빈 칸을 처음 만나면 계속 탐색
        if (_grid[row, col].CellOwner == Turn.NONE)
        {
            if (isAppearNone is false)
            {
                isAppearNone = true;
                return ConstraintsCheckRecursive(row + dy, col + dx, dy, dx, count, isAppearNone, ref isOpenFour);
            }
            //Base Condition
            //이전에 빈 칸을 만났고 또 빈 칸을 만났다면 해당 방향은 열린 4이다.
            else
            {
                isOpenFour = true;
                return count;
            }
        }
        
        return ConstraintsCheckRecursive(row + dy, col + dx, dy, dx, count + 1, isAppearNone, ref isOpenFour);
    }
    
    public List<Cell> CheckDoubleFour()
    {
        var currentTurnCells = _matchRecord.Where(c => c.CellOwner == _gameData.currentTurn).ToList();
        
        List<Cell> cells = new List<Cell>();
        
        //clock-wise (n -> ne -> e -> se)
        //row
        int[] dy = { 1, 1, 0, -1, -1, -1, 0, 1};
        //col
        int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1};
        
        /*
         * 현재 놓여져 있는 돌들에 대해, 8 방향에 빈 칸이 존재하는지 확인한 후
         * 빈칸이 존재한다면, 해당 방향으로 2칸을 가상의 돌을 놓아가면서 재귀적으로 3-3 조건을 탐색한다.
         */
        foreach (var currentTurnCell in currentTurnCells)
        {
            int curr = currentTurnCell._coordinate.Item1;
            int curc = currentTurnCell._coordinate.Item2;
            
            for (int dir = 0; dir < dy.Length; dir++)
            {
                for (int step = 1; step <= 2; step++)
                {
                    int nr = curr + dy[dir] * step;
                    int nc = curc + dx[dir] * step;
                    
                    if (_grid[nr, nc] is null) break;
                
                    if (_grid[nr, nc].CellOwner != _gameData.currentTurn &&
                        _grid[nr, nc].CellOwner != Turn.NONE)
                    {
                        break;
                    }

                    if (_grid[nr, nc].CellOwner == _gameData.currentTurn)
                        break;
                    
                    int doubleFourCounter = 0;

                    for (int virtualDir = 0; virtualDir < 4; virtualDir++)
                    {
                        int counter = 0;
                        int counterOppositeDir = 0;
                        int duplicateNoneCnt = 0;
                        
                        bool isAppearNone = false;
                        bool isAppearNoneOppositeDir = false;
                    
                        _grid[nr, nc].CellOwner = _gameData.currentTurn;

                        counter = CheckDoubleFourRecursive(nr, nc, dy[virtualDir], dx[virtualDir], 0, ref isAppearNone, ref duplicateNoneCnt);

                        counterOppositeDir = CheckDoubleFourRecursive(nr, nc, -dy[virtualDir], -dx[virtualDir], 0, ref isAppearNoneOppositeDir, ref duplicateNoneCnt) - 1;
                        
                        _grid[nr, nc].CellOwner = Turn.NONE;
                        
                        //서로 다른 방향의 4-4를 체크, (ex. n와 ne 방향으로의 4-4 체크)
                        if (counter + counterOppositeDir == 4 && duplicateNoneCnt <= 1)
                        {
                            doubleFourCounter++;
                        }
                        //서로 반대 방향의 4-4를 체크, (ex. n와 s 방향으로의 4-4 체크)
                        //이 경우 직선으로 양 방향이 4-4 형태가 된다.
                        else if (counter + counterOppositeDir > 4 &&
                                 counter > 1 && counterOppositeDir + 1 > 1 &&
                                 isAppearNone is true && isAppearNoneOppositeDir is true)
                        {
                            cells.Add(_grid[nr, nc]);
                            break;
                        }
                        
                        if (doubleFourCounter == 2)
                        {
                            cells.Add(_grid[nr, nc]);
                            break;
                        }
                    }
                }
            }
        }

        if (cells.Count > 0)
            return cells;
        else
            return null;
    }

    private int CheckDoubleFourRecursive(int row, int col, int dy, int dx, int count, ref bool isAppearNone, ref int duplicateNoneCnt)
    {
        //Base Condition
        //현재 보는 칸이 보드 밖이라면
        if (_grid[row, col] is null)
            return count;
        
        //Base Condition
        //현재 보는 칸이 상대방의 돌이라면 해당 방향은 더 이상 볼 필요가 없다.
        if (_grid[row, col].CellOwner != _gameData.currentTurn &&
            _grid[row, col].CellOwner != Turn.NONE)
            return count;

        // 빈 칸을 처음 만나면 계속 탐색
        if (_grid[row, col].CellOwner == Turn.NONE)
        {
            if (isAppearNone is false)
            {
                isAppearNone = true;
                
                int cnt = CheckDoubleFourRecursive(row + dy, col + dx, dy, dx, count, ref isAppearNone, ref duplicateNoneCnt);

                duplicateNoneCnt++;
                
                return cnt;
            }
            //Base Condition
            else
            {
                //연속으로 None이 두 번 나온 경우
                if (_grid[row - dy, col - dx].CellOwner == Turn.NONE)
                {
                    duplicateNoneCnt--;
                }
                
                return count;
            }
        }

        return CheckDoubleFourRecursive(row + dy, col + dx, dy, dx, count + 1, ref isAppearNone, ref duplicateNoneCnt);
    }
    
    // 두 번째 코드에서 추가된 리셋 기능들
    public void SafeReset()
    {
        // 그리드 경계값 탐색 (예: -7 ~ 7)
        for (int row = -7; row < 8; row++) 
        {
            for (int col = -7; col < 8; col++)
            {
                if (_grid[row, col] != null) // null 체크로 유효 좌표 확인
                {
                    _grid.TryUnmarkingOnCell((row, col));
                }
            }
        }
    }
    
    public void ResetBoardForReplay()
    {
        if (_grid == null) return;

        // MatchRecord에 저장된 좌표만 초기화 (최적화)
        foreach (var cell in _matchRecord)
        {
            _grid.TryUnmarkingOnCell(cell._coordinate);
        }
        _matchRecord.Clear();

        if (_matchRecord.Count == 0)
        {
            for (int row = -7; row < 8; row++) // 15x15 그리드 가정
            {
                for (int col = -7; col < 8; col++)
                {
                    _grid.TryUnmarkingOnCell((row, col));
                }
            }
        }
    }
    
    private BoardGrid _grid;
    public BoardGrid Grid => _grid;

    private GameData _gameData;
    public GameData GameData => _gameData;

    private readonly Stack<Cell> _matchRecord = new();
    public Stack<Cell> MatchRecord => _matchRecord;

    private readonly int INF = (int)1e9;
    //AI가 시간 안에 연산을 못 끝낼 때 stack에 값을 기록하지 않는다 -> 시간 초과라는 의미이며 아무 것도 하지 않는다를 표현하기 위해
    private bool _isTimeOut = false;
}