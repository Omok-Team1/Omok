using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public static class OmokAIController
{
    // private const int SEARCH_DEPTH = 3;
    private static readonly (int, int)[] directions = { (1, 0), (0, 1), (1, 1), (1, -1) };
    private static Dictionary<ulong, (float score, int depth)> zobristTable = new();    
    private static ulong[,,] zobristKeys; // zobrist 해싱키
    public static BoardGrid _board; 

    public static UniTask<(int row, int col)> GetBestMove(CancellationToken token)
    {
        if (_board == null)
        {
            Debug.LogError("OmokAIController: _board is null in GetBestMove()");
        }

        if (zobristKeys == null)
        {
            InitializeAI();
        }
        if (IsBoardEmpty())
        {
            int center = Constants.BOARD_SIZE / 2;
            return new UniTask<(int, int)>((center, center));
        }

        var blockMove = CheckImmediateWinOrBlock(Turn.PLAYER1);
        if (blockMove != (-1, -1)) return new UniTask<(int, int)>(blockMove);

        float bestScore = float.MinValue;
        (int, int) bestMove = (-1, -1);
        var candidates = GetCandidateMoves();
        int dynamicDepth = GetDynamicDepth(candidates.Count);
        
        foreach (var (row, col) in candidates)
        {
            _board.MarkingTurnOnCell((row, col), Turn.PLAYER2);
            float score = DoMinimax(dynamicDepth - 1, false, float.MinValue, float.MaxValue);
            _board.MarkingTurnOnCell((row, col), Turn.NONE);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = (row, col);
            }
        }
        return new UniTask<(int, int)>(bestMove);
    }

   private static (int, int) CheckImmediateWinOrBlock(Turn opponent)
    {
        foreach (var (row, col) in GetCandidateMoves())
        {
            _board.MarkingTurnOnCell((row, col), Turn.PLAYER2);
            if (CheckWin(Turn.PLAYER2))
            {
                _board.MarkingTurnOnCell((row, col), Turn.NONE);
                return (row, col);
            }
            _board.MarkingTurnOnCell((row, col), Turn.NONE);
        }

        foreach (var (row, col) in GetCandidateMoves())
        {
            _board.MarkingTurnOnCell((row, col), opponent);
            if (CheckWin(opponent) || CanMakeFive(opponent, row, col) || IsThreateningMove(row, col, opponent))
            {
                _board.MarkingTurnOnCell((row, col), Turn.NONE);
                return (row, col);
            }
            _board.MarkingTurnOnCell((row, col), Turn.NONE);
        }
        return (-1, -1);
    }
    private static bool IsThreateningMove(int row, int col, Turn player)
    {
        foreach (var (dx, dy) in directions)
        {
            int count = 1;
            bool isBlockedStart = false, isBlockedEnd = false;

            for (int i = 1; i < 5; i++)
            {
                int newRow = row + dx * i, newCol = col + dy * i;
                if (!IsWithinBounds(newRow, newCol)) break;
                var cell = _board[newRow, newCol];
                if (cell.CellOwner == Turn.NONE) break;
                if (cell.CellOwner != player) { isBlockedEnd = true; break; }
                count++;
            }

            for (int i = 1; i < 5; i++)
            {
                int newRow = row - dx * i, newCol = col - dy * i;
                if (!IsWithinBounds(newRow, newCol)) break;
                var cell = _board[newRow, newCol];
                if (cell.CellOwner == Turn.NONE) break;
                if (cell.CellOwner != player) { isBlockedStart = true; break; }
                count++;
            }

            if (count == 4 && (isBlockedStart || isBlockedEnd))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsWithinBounds(int row, int col)
    {
        return row >= -7 && row < Constants.BOARD_SIZE - 7 &&
            col >= -7 && col < Constants.BOARD_SIZE - 7;
    }

    private static bool CanMakeFive(Turn player, int row, int col)
    {
        foreach (var (dx, dy) in directions)
        {
            int count = 1;
            int emptySpots = 0;

            for (int i = 1; i < 5; i++)
            {
                int newRow = row + dx * i, newCol = col + dy * i;
                if (!IsWithinBounds(newRow, newCol)) break;
                if (_board[newRow, newCol].CellOwner == player) count++;
                else if (_board[newRow, newCol].CellOwner == Turn.NONE) emptySpots++;
                else break;
            }

            for (int i = 1; i < 5; i++)
            {
                int newRow = row - dx * i, newCol = col - dy * i;
                if (!IsWithinBounds(newRow, newCol)) break;
                if (_board[newRow, newCol].CellOwner == player) count++;
                else if (_board[newRow, newCol].CellOwner == Turn.NONE) emptySpots++;
                else break;
            }

            if (count == 4 && emptySpots > 0) return true; // 4목 체크
        }
        return false;
    }

    private static int GetDynamicDepth(int candidateCount)
    {
        int stoneCount = 0;
        foreach (var cell in _board)
        {
            if (cell.CellOwner != Turn.NONE) stoneCount++;
        }

        if (stoneCount < 10) return 3;
        if (stoneCount < 20) return candidateCount > 10 ? 2 : 3;
        return 2;
    }    
    
    private static float DoMinimax(int depth, bool isMaximizing, float alpha, float beta)
    {
        ulong hash = ComputeHash();
        if (zobristTable.TryGetValue(hash, out var cached) && cached.depth >= depth)
        {
            return cached.score;
        }

        if (CheckWin(Turn.PLAYER1)) return -1000 + depth;
        if (CheckWin(Turn.PLAYER2)) return 1000 - depth;
        if (depth == 0 || IsBoardFull()) return Evaluation.EvaluateBoard(_board);

        float bestScore = isMaximizing ? float.MinValue : float.MaxValue;
        List<(int, int)> candidates = GetCandidateMoves();

        foreach (var (row, col) in candidates)
        {
            _board.MarkingTurnOnCell((row, col), isMaximizing ? Turn.PLAYER2 : Turn.PLAYER1);
            float score = DoMinimax(depth - 1, !isMaximizing, alpha, beta);
            _board.MarkingTurnOnCell((row, col), Turn.NONE);

            if (isMaximizing)
            {
                bestScore = Math.Max(bestScore, score);
                alpha = Math.Max(alpha, score);
            }
            else
            {
                bestScore = Math.Min(bestScore, score);
                beta = Math.Min(beta, score);
            }

            if (beta <= alpha) break;
        }

        zobristTable[hash] = (bestScore, depth);
        return bestScore;
    }

    private static bool CheckWin(Turn player)
    {
        for (int row = -7; row < Constants.BOARD_SIZE - 7; row++)
        {
            for (int col = -7; col < Constants.BOARD_SIZE - 7; col++)
            {
                if (_board[row, col] == null || _board[row, col].CellOwner != player) continue;

                foreach (var (dx, dy) in directions)
                {
                    int count = 1;
                    for (int i = 1; i < 5; i++)
                    {
                        int newRow = row + dx * i, newCol = col + dy * i;
                        
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
        HashSet<(int, int)> uniqueMoves = new HashSet<(int, int)>();
        List<(int, int)> sortedMoves = new List<(int, int)>();

        for (int row = -7; row < Constants.BOARD_SIZE - 7; row++)
        {
            for (int col = -7; col < Constants.BOARD_SIZE - 7; col++)
            {
                if (_board[row, col].CellOwner != Turn.NONE) continue;

                int surroundingStones = 0;
                foreach (var (dx, dy) in directions)
                {
                    int newRow = row + dx, newCol = col + dy;
                    if (IsWithinBounds(newRow, newCol) && _board[newRow, newCol].CellOwner != Turn.NONE)
                    {
                        surroundingStones++;
                    }
                }

                if (surroundingStones > 0)
                {
                    uniqueMoves.Add((row, col));
                }
            }
        }

        sortedMoves.AddRange(uniqueMoves);

        sortedMoves.Sort((a, b) =>
        {
            float scoreA = EvaluateMovePriority(_board, a);
            float scoreB = EvaluateMovePriority(_board, b);
            return scoreB.CompareTo(scoreA);
        });

        return sortedMoves.Count > 12 ? sortedMoves.GetRange(0, 12) : sortedMoves;
    }

    private static float EvaluateMovePriority(BoardGrid board, (int row, int col) move)
    {
        board.MarkingTurnOnCell(move, Turn.PLAYER2);
        if (CheckWin(Turn.PLAYER2)) 
        {
            board.MarkingTurnOnCell(move, Turn.NONE);
            return 1_500_000;
        }
        board.MarkingTurnOnCell(move, Turn.NONE);

        board.MarkingTurnOnCell(move, Turn.PLAYER1);
        if (CheckWin(Turn.PLAYER1) || CanMakeFive(Turn.PLAYER1, move.row, move.col)) 
        {
            board.MarkingTurnOnCell(move, Turn.NONE);
            return 1_000_000;
        }
        board.MarkingTurnOnCell(move, Turn.NONE);

        float score = Evaluation.EvaluateMove(board, move, true);
        float proximityBonus = 0f;
        foreach (var (dx, dy) in directions)
        {
            int newRow = move.row + dx, newCol = move.col + dy;
            if (IsWithinBounds(newRow, newCol) && board[newRow, newCol]?.CellOwner != Turn.NONE)
            {
                proximityBonus += 30f;
            }
        }

        return score + proximityBonus;
    }

    private static bool IsBoardFull()
    {
        foreach (var cell in _board)
        {
            if (cell.CellOwner == Turn.NONE) return false;
        }
        return true;
    }

    private static ulong ComputeHash()
    {
        ulong hash = 0;
        foreach (var cell in _board)
        {
            if (cell.CellOwner != Turn.NONE)
            {
                int row = cell._coordinate.Item1 + 7;
                int col = cell._coordinate.Item2 + 7;
                hash ^= zobristKeys[row, col, (int)cell.CellOwner];
            }
        }
        return hash;
    }

    private static void InitializeZobristKeys()
    {
        System.Random random = new System.Random();
        zobristKeys = new ulong[Constants.BOARD_SIZE, Constants.BOARD_SIZE, 3];

        foreach (var cell in _board)
        {
            int row = cell._coordinate.Item1 + 7;  // 음수 보정
            int col = cell._coordinate.Item2 + 7;  // 음수 보정

            if (row < 0 || row >= Constants.BOARD_SIZE || col < 0 || col >= Constants.BOARD_SIZE)
                continue;  // 유효하지 않은 좌표는 무시

            for (int player = 0; player < 3; player++)
            {
                ulong high = (ulong)(uint)random.Next() << 32;
                ulong low = (ulong)(uint)random.Next();
                zobristKeys[row, col, player] = high | low;
            }
        }
    }

    public static void InitializeAI()
    {
        InitializeZobristKeys(); // 해시 키 초기화
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
