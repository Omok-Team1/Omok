using System;
using System.Collections.Generic;
using UnityEngine;

public static class OmokAIController
{
    private const int SEARCH_DEPTH = 3;
    private static readonly (int, int)[] directions = { (1, 0), (0, 1), (1, 1), (1, -1) };
    private static Dictionary<ulong, float> zobristTable = new Dictionary<ulong, float>();
    private static ulong[,,] zobristKeys; // zobrist 해싱키
    private static BoardGrid _board => GameManager.Instance?.BoardManager?.Grid; // null 안전 처리 (?.)


    public static (int row, int col) GetBestMove()
    {
        if (_board == null)
        {
            Debug.LogError("OmokAIController: _board is null in GetBestMove()");
            return (-1, -1);  // 기본값 반환
        }
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
            
            // 5목 체크 (즉시 승리 or 방어)
            if (CheckWin(opponent))
            {
                _board.MarkingTurnOnCell((row, col), Turn.NONE);
                return (row, col);
            }

            // 4목(한 수 두면 승리) 체크
            if (CanMakeFive(opponent, row, col))
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

            // 한쪽 방향 체크
            for (int i = 1; i < 5; i++)
            {
                int newRow = row + dx * i, newCol = col + dy * i;
                if (!IsWithinBounds(newRow, newCol)) break; // 수정된 부분

                var cell = _board[newRow, newCol];
                if (cell.CellOwner == Turn.NONE) break;
                if (cell.CellOwner != player) { isBlockedEnd = true; break; }

                count++;
            }

            // 반대 방향 체크
            for (int i = 1; i < 5; i++)
            {
                int newRow = row - dx * i, newCol = col - dy * i;
                if (!IsWithinBounds(newRow, newCol)) break; // 수정된 부분

                var cell = _board[newRow, newCol];
                if (cell.CellOwner == Turn.NONE) break;
                if (cell.CellOwner != player) { isBlockedStart = true; break; }

                count++;
            }

            // 4목 이상이면 무조건 방어
            if (count >= 4) return true;

            // 열린 3목 (양쪽이 막히지 않음)도 방어해야 함
            if (count == 3 && !(isBlockedStart && isBlockedEnd)) return true;
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


    private static float DoMinimax(int depth, bool isMaximizing, float alpha, float beta)
    {
        ulong hash = ComputeHash();
        if (zobristTable.TryGetValue(hash, out float cachedScore)) return cachedScore;

        if (CheckWin(Turn.PLAYER1)) return -1000 + depth;
        if (CheckWin(Turn.PLAYER2)) return 1000 - depth;
        if (depth == 0 || IsBoardFull()) return Evaluation.EvaluateBoard(_board); // 평가 함수 사용

        float bestScore = isMaximizing ? float.MinValue : float.MaxValue;
        List<(int, int)> candidates = GetCandidateMoves();

        candidates.Sort((move1, move2) =>
        {
            float score1 = Evaluation.EvaluateMove(_board, move1, isMaximizing); // 평가 함수 사용
            float score2 = Evaluation.EvaluateMove(_board, move2, isMaximizing);
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

                foreach (var (dx, dy) in directions)
                {
                    int newRow = row + dx, newCol = col + dy;

                    // 🔥 기존 2칸까지 탐색하던 로직을 제거하여 1칸 거리 내 후보만 선택
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

        sortedMoves.AddRange(uniqueMoves);
        sortedMoves.Sort((a, b) => Evaluation.EvaluateMove(_board, b, true).CompareTo(Evaluation.EvaluateMove(_board, a, true))); 
        return sortedMoves;
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
                int row = cell._coordinate.Item1 + 7; // 좌표 변환
                int col = cell._coordinate.Item2 + 7; // 좌표 변환
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
