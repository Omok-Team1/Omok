using System;
using System.Collections.Generic;
using UnityEngine;

public static class OmokAIController
{
    private const int SEARCH_DEPTH = 3;
    private static readonly (int, int)[] directions = { (1, 0), (0, 1), (1, 1), (1, -1) };
    private static Dictionary<ulong, float> zobristTable = new Dictionary<ulong, float>();
    
    //종한 추가
    private static BoardGrid _board = GameManager.Instance.BoardManager.Grid;

    public static (int row, int col) GetBestMove()
    {
        if (IsBoardEmpty())
        {
            int center = Constants.BOARD_SIZE / 2;
            return (center, center);
        }

        var blockMove = CheckImmediateWinOrBlock(Turn.PLAYER1);
        if (blockMove != (-1, -1)) return blockMove;

        float bestScore = float.MinValue;
        (int, int) bestMove = (-1, -1);

        var candidates = GetCandidateMoves();
        foreach (var (row, col) in candidates)
        {
            _board.MarkingTurnOnCell((row, col), Turn.PLAYER2);
            float score = DoMinimax(SEARCH_DEPTH - 1, false, float.MinValue, float.MaxValue);
            _board.MarkingTurnOnCell((row, col), Turn.NONE);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = (row, col);
            }
        }
        return bestMove;
    }

    private static (int, int) CheckImmediateWinOrBlock(Turn opponent)
    {
        foreach (var (row, col) in GetCandidateMoves())
        {
            _board.MarkingTurnOnCell((row, col), opponent);
            if (CheckWin(opponent))
            {
                _board.MarkingTurnOnCell((row, col), Turn.NONE);
                return (row, col);
            }
            _board.MarkingTurnOnCell((row, col), Turn.NONE);
        }
        return (-1, -1);
    }

    private static float DoMinimax(int depth, bool isMaximizing, float alpha, float beta)
    {
        ulong hash = ComputeHash();
        if (zobristTable.ContainsKey(hash)) return zobristTable[hash];

        if (CheckWin(Turn.PLAYER1)) return -1000 + depth;
        if (CheckWin(Turn.PLAYER2)) return 1000 - depth;
        if (depth == 0 || IsBoardFull()) return EvaluateBoard();

        float bestScore = isMaximizing ? float.MinValue : float.MaxValue;
        foreach (var (row, col) in GetCandidateMoves())
        {
            //기존 코드
            //_board[row, col] = isMaximizing ? Turn.PLAYER2 : Turn.PLAYER1;
            
            //종한 수정
            if(isMaximizing)
                _board.MarkingTurnOnCell((row, col), Turn.PLAYER2);
            else
                _board.MarkingTurnOnCell((row, col), Turn.PLAYER1);
            
            float score = DoMinimax(depth - 1, !isMaximizing, alpha, beta);
            _board.MarkingTurnOnCell((row, col), Turn.NONE);

            bestScore = isMaximizing ? Math.Max(bestScore, score) : Math.Min(bestScore, score);
            if (isMaximizing) alpha = Math.Max(alpha, score);
            else beta = Math.Min(beta, score);

            if (beta <= alpha) break;
        }

        zobristTable[hash] = bestScore;
        return bestScore;
    }

    private static bool CheckWin(Turn player)
    {
        for (int row = -7; row < Constants.BOARD_SIZE - 7; row++)
        {
            for (int col = -7; col < Constants.BOARD_SIZE - 7; col++)
            {
                if (_board[row, col].CellOwner != player) continue;

                foreach (var (dx, dy) in directions)
                {
                    int count = 1;
                    for (int i = 1; i < 5; i++)
                    {
                        int newRow = row + dx * i, newCol = col + dy * i;
                        if (newRow < 0 || newRow >= Constants.BOARD_SIZE / 2 || newCol < 0 || newCol >= Constants.BOARD_SIZE / 2 || _board[newRow, newCol].CellOwner != player)
                            break;
                        count++;
                    }
                    if (count >= 5) return true;
                }
            }
        }
        return false;
    }

    private static List<(int, int)> GetCandidateMoves()
    {
        List<(int, int)> moves = new List<(int, int)>();
        for (int row = -7; row < Constants.BOARD_SIZE - 7; row++)
        {
            for (int col = -7; col < Constants.BOARD_SIZE - 7; col++)
            {
                if (_board[row, col].CellOwner != Turn.NONE) continue;

                foreach (var (dx, dy) in directions)
                {
                    int newRow = row + dx, newCol = col + dy;
                    
                    if (newRow >= 0 && newRow < Constants.BOARD_SIZE / 2 && newCol >= 0 && newCol < Constants.BOARD_SIZE / 2 && _board[newRow, newCol].CellOwner != Turn.NONE)
                    {
                        moves.Add((row, col));
                        break;
                    }
                }
            }
        }
        return moves;
    }

    private static bool IsBoardFull()
    {
        foreach (var cell in _board)
        {
            if (cell.CellOwner == Turn.NONE) return false;
        }
        return true;
    }

    private static float EvaluateBoard()
    {
        float score = 0f;
        foreach (var (dx, dy) in directions)
        {
            for (int row = -7; row < Constants.BOARD_SIZE - 7; row++)
            {
                for (int col = -7; col < Constants.BOARD_SIZE - 7; col++)
                {
                    if (_board[row, col].CellOwner == Turn.NONE) continue;
                    int playerMultiplier = _board[row, col].CellOwner == Turn.PLAYER2 ? 1 : -1;
                    int count = 0;
                    bool isBlocked = false;
                    for (int i = 0; i < 5; i++)
                    {
                        int newRow = row + dx * i, newCol = col + dy * i;
                        if (newRow < 0 || newRow >= Constants.BOARD_SIZE / 2 || newCol < 0 || newCol >= Constants.BOARD_SIZE / 2)
                        {
                            isBlocked = true;
                            break;
                        }
                        if (_board[newRow, newCol] == _board[row, col])
                            count++;
                        else if (_board[newRow, newCol].CellOwner != Turn.NONE)
                        {
                            isBlocked = true;
                            break;
                        }
                    }
                    if (!isBlocked)
                    {
                        score += (float)Math.Pow(10, count) * playerMultiplier;
                    }
                }
            }
        }
        return score;
    }

    private static ulong ComputeHash()
    {
        ulong hash = 0;
        foreach (var cell in _board)
        {
            hash = hash * 31 + (ulong)cell.CellOwner;
        }
        return hash;
    }

    private static bool IsBoardEmpty()
    {
        foreach (var cell in _board)
        {
            if (cell.CellOwner != Turn.NONE) return false;
        }
        return true;
    }
}
