using System;
using System.Collections.Generic;
using UnityEngine;

public static class OmokAIController
{
    private const int SEARCH_DEPTH = 3;
    private static readonly (int, int)[] directions = { (1, 0), (0, 1), (1, 1), (1, -1) };
    private static Dictionary<ulong, float> zobristTable = new Dictionary<ulong, float>();

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
        if (zobristTable.TryGetValue(hash, out float cachedScore)) return cachedScore;

        if (CheckWin(Turn.PLAYER1)) return -1000 + depth;
        if (CheckWin(Turn.PLAYER2)) return 1000 - depth;
        if (depth == 0 || IsBoardFull()) return EvaluateBoard();

        float bestScore = isMaximizing ? float.MinValue : float.MaxValue;
        List<(int, int)> candidates = GetCandidateMoves();

        candidates.Sort((move1, move2) =>
        {
            float score1 = EvaluateMove(move1, isMaximizing);
            float score2 = EvaluateMove(move2, isMaximizing);
            return score2.CompareTo(score1);
        });

        foreach (var (row, col) in candidates)
        {
            if (isMaximizing)
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
                // 🔹 null 체크 추가
                if (_board[row, col] == null || _board[row, col].CellOwner != player) continue;

                foreach (var (dx, dy) in directions)
                {
                    int count = 1;
                    for (int i = 1; i < 5; i++)
                    {
                        int newRow = row + dx * i, newCol = col + dy * i;
                        
                        // 🔹 null 체크 추가
                        if (newRow < 0 || newRow >= Constants.BOARD_SIZE || newCol < 0 || newCol >= Constants.BOARD_SIZE || _board[newRow, newCol] == null)
                            break;

                        if (_board[newRow, newCol].CellOwner != player)
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
        HashSet<(int, int)> uniqueMoves = new HashSet<(int, int)>();

        for (int row = -7; row < Constants.BOARD_SIZE - 7; row++)
        {
            for (int col = -7; col < Constants.BOARD_SIZE - 7; col++)
            {
                if (_board[row, col].CellOwner != Turn.NONE) continue;

                foreach (var (dx, dy) in directions)
                {
                    int newRow = row + dx, newCol = col + dy;

                    if (newRow >= -7 && newRow < Constants.BOARD_SIZE - 7 &&
                        newCol >= -7 && newCol < Constants.BOARD_SIZE - 7 &&
                        _board[newRow, newCol].CellOwner != Turn.NONE)
                    {
                        uniqueMoves.Add((row, col));
                        break;
                    }
                }
            }
        }

        moves.AddRange(uniqueMoves);
        moves.Sort((a, b) => EvaluateMovePriority(b).CompareTo(EvaluateMovePriority(a)));
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
                    // 🔹 null 체크 추가
                    if (_board[row, col] == null || _board[row, col].CellOwner == Turn.NONE) continue;

                    int playerMultiplier = _board[row, col].CellOwner == Turn.PLAYER2 ? 1 : -1;
                    int count = 0;
                    bool isBlocked = false;
                    
                    for (int i = 0; i < 5; i++)
                    {
                        int newRow = row + dx * i, newCol = col + dy * i;
                        
                        // 🔹 null 체크 추가
                        if (newRow < 0 || newRow >= Constants.BOARD_SIZE || newCol < 0 || newCol >= Constants.BOARD_SIZE || _board[newRow, newCol] == null)
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
            hash ^= (ulong)((cell._coordinate.Item1 * Constants.BOARD_SIZE + cell._coordinate.Item2) * (int)cell.CellOwner);
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

    private static float EvaluateMove((int row, int col) move, bool isMaximizing)
    {
        float score = 0f;

        _board.MarkingTurnOnCell(move, isMaximizing ? Turn.PLAYER2 : Turn.PLAYER1);

        if (isMaximizing)
        {
            if (CheckImmediateWinOrBlock(Turn.PLAYER1) != (-1, -1)) score -= 1000; // 방어 우선
        }
        else
        {
            if (CheckImmediateWinOrBlock(Turn.PLAYER2) != (-1, -1)) score += 1000; // 공격 우선
        }

        score += EvaluateDefense(move, isMaximizing);
        score += EvaluateAttack(move, isMaximizing);

        _board.MarkingTurnOnCell(move, Turn.NONE);  // 이동 취소

        return score;
    }

    private static int EvaluateDefense((int row, int col) move, bool isMaximizing)
    {
        int score = 0;
        Turn opponent = isMaximizing ? Turn.PLAYER1 : Turn.PLAYER2;

        foreach (var (dx, dy) in directions)
        {
            int count = 0;
            bool isBlockedStart = false;
            bool isBlockedEnd = false;

            for (int i = 1; i < 5; i++)
            {
                int newRow = move.row + dx * i, newCol = move.col + dy * i;
                if (newRow < -7 || newRow >= Constants.BOARD_SIZE - 7 || newCol < -7 || newCol >= Constants.BOARD_SIZE - 7)
                {
                    isBlockedEnd = true;
                    break;
                }
                if (_board[newRow, newCol].CellOwner == Turn.PLAYER2)
                {
                    isBlockedEnd = true;
                    break;
                }
                if (_board[newRow, newCol].CellOwner == Turn.PLAYER1) count++;
            }

            for (int i = 1; i < 5; i++)
            {
                int newRow = move.row - dx * i, newCol = move.col - dy * i;
                if (newRow < -7 || newRow >= Constants.BOARD_SIZE - 7 || newCol < -7 || newCol >= Constants.BOARD_SIZE - 7)
                {
                    isBlockedStart = true;
                    break;
                }
                if (_board[newRow, newCol].CellOwner == Turn.PLAYER2)
                {
                    isBlockedStart = true;
                    break;
                }
                if (_board[newRow, newCol].CellOwner == Turn.PLAYER1) count++;
            }

            if (!isBlockedStart && !isBlockedEnd)
            {
                if (count == 3) score += 500;  // 상대가 3목을 만들면 방어할 점수
                if (count == 4) score += 1000; // 상대가 4목을 만들면 방어할 점수
            }
        }

        return score;
    }

    private static int EvaluateAttack((int row, int col) move, bool isMaximizing)
    {
        int score = 0;
        Turn player = isMaximizing ? Turn.PLAYER2 : Turn.PLAYER1;

        foreach (var (dx, dy) in directions)
        {
            int count = 1;
            bool isBlockedStart = false;
            bool isBlockedEnd = false;

            for (int i = 1; i < 5; i++)
            {
                int newRow = move.row + dx * i, newCol = move.col + dy * i;
                if (newRow < -7 || newRow >= Constants.BOARD_SIZE - 7 || newCol < -7 || newCol >= Constants.BOARD_SIZE - 7 || _board[newRow, newCol].CellOwner == Turn.PLAYER1)
                {
                    isBlockedEnd = true;
                    break;
                }
                if (_board[newRow, newCol].CellOwner == Turn.PLAYER2) count++;
            }

            for (int i = 1; i < 5; i++)
            {
                int newRow = move.row - dx * i, newCol = move.col - dy * i;
                if (newRow < -7 || newRow >= Constants.BOARD_SIZE - 7 || newCol < -7 || newCol >= Constants.BOARD_SIZE - 7 || _board[newRow, newCol].CellOwner == Turn.PLAYER1)
                {
                    isBlockedStart = true;
                    break;
                }
                if (_board[newRow, newCol].CellOwner == Turn.PLAYER2) count++;
            }

            if (!isBlockedStart && !isBlockedEnd && count >= 5)
            {
                score += 1000;  // 공격이 승리를 만들 수 있으면 점수 증가
            }
        }

        return score;
    }

    private static int EvaluateMovePriority((int row, int col) move)
    {
        int score = 0;

        foreach (var (dx, dy) in directions)
        {
            int count = 1;
            bool isBlockedStart = false, isBlockedEnd = false;

            for (int i = 1; i < 5; i++)
            {
                int newRow = move.row + dx * i, newCol = move.col + dy * i;
                if (newRow < -7 || newRow >= Constants.BOARD_SIZE - 7 ||
                    newCol < -7 || newCol >= Constants.BOARD_SIZE - 7)
                {
                    isBlockedEnd = true;
                    break;
                }
                if (_board[newRow, newCol].CellOwner == Turn.PLAYER2) count++;
                else if (_board[newRow, newCol].CellOwner == Turn.PLAYER1) 
                {
                    isBlockedEnd = true;
                    break;
                }
            }

            for (int i = 1; i < 5; i++)
            {
                int newRow = move.row - dx * i, newCol = move.col - dy * i;
                if (newRow < -7 || newRow >= Constants.BOARD_SIZE - 7 ||
                    newCol < -7 || newCol >= Constants.BOARD_SIZE - 7)
                {
                    isBlockedStart = true;
                    break;
                }
                if (_board[newRow, newCol].CellOwner == Turn.PLAYER2) count++;
                else if (_board[newRow, newCol].CellOwner == Turn.PLAYER1) 
                {
                    isBlockedStart = true;
                    break;
                }
            }

            if (count >= 5) return 1000;  // 즉시 승리 가능
            if (!isBlockedStart && !isBlockedEnd)
            {
                if (count == 4) score += 500; // 열린 4
                if (count == 3) score += 100; // 열린 3
                if (count == 2) score += 10;  // 열린 2
            }
        }

        return score;
    }

}
