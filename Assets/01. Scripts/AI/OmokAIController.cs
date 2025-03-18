using System;
using System.Collections.Generic;
using UnityEngine;

public static class OmokAIController
{
    private const int SEARCH_DEPTH = 3;
    private static readonly (int, int)[] directions = { (1, 0), (0, 1), (1, 1), (1, -1) };
    private static Dictionary<ulong, float> zobristTable = new Dictionary<ulong, float>();

    public static (int row, int col) GetBestMove(Constants.PlayerType[,] board)
    {
        if (IsBoardEmpty(board))
        {
            int center = Constants.BOARD_SIZE / 2;
            return (center, center);
        }

        var blockMove = CheckImmediateWinOrBlock(board, Constants.PlayerType.PlayerA);
        if (blockMove != (-1, -1)) return blockMove;

        float bestScore = float.MinValue;
        (int, int) bestMove = (-1, -1);

        var candidates = GetCandidateMoves(board);
        foreach (var (row, col) in candidates)
        {
            board[row, col] = Constants.PlayerType.PlayerB;
            float score = DoMinimax(board, SEARCH_DEPTH - 1, false, float.MinValue, float.MaxValue);
            board[row, col] = Constants.PlayerType.None;

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = (row, col);
            }
        }
        return bestMove;
    }

    private static (int, int) CheckImmediateWinOrBlock(Constants.PlayerType[,] board, Constants.PlayerType opponent)
    {
        foreach (var (row, col) in GetCandidateMoves(board))
        {
            board[row, col] = opponent;
            if (CheckWin(opponent, board))
            {
                board[row, col] = Constants.PlayerType.None;
                return (row, col);
            }
            board[row, col] = Constants.PlayerType.None;
        }
        return (-1, -1);
    }

    private static float DoMinimax(Constants.PlayerType[,] board, int depth, bool isMaximizing, float alpha, float beta)
    {
        ulong hash = ComputeHash(board);
        if (zobristTable.ContainsKey(hash)) return zobristTable[hash];

        if (CheckWin(Constants.PlayerType.PlayerA, board)) return -1000 + depth;
        if (CheckWin(Constants.PlayerType.PlayerB, board)) return 1000 - depth;
        if (depth == 0 || IsBoardFull(board)) return EvaluateBoard(board);

        float bestScore = isMaximizing ? float.MinValue : float.MaxValue;
        foreach (var (row, col) in GetCandidateMoves(board))
        {
            board[row, col] = isMaximizing ? Constants.PlayerType.PlayerB : Constants.PlayerType.PlayerA;
            float score = DoMinimax(board, depth - 1, !isMaximizing, alpha, beta);
            board[row, col] = Constants.PlayerType.None;

            bestScore = isMaximizing ? Math.Max(bestScore, score) : Math.Min(bestScore, score);
            if (isMaximizing) alpha = Math.Max(alpha, score);
            else beta = Math.Min(beta, score);

            if (beta <= alpha) break;
        }

        zobristTable[hash] = bestScore;
        return bestScore;
    }

    private static bool CheckWin(Constants.PlayerType player, Constants.PlayerType[,] board)
    {
        for (int row = 0; row < Constants.BOARD_SIZE; row++)
        {
            for (int col = 0; col < Constants.BOARD_SIZE; col++)
            {
                if (board[row, col] != player) continue;

                foreach (var (dx, dy) in directions)
                {
                    int count = 1;
                    for (int i = 1; i < 5; i++)
                    {
                        int newRow = row + dx * i, newCol = col + dy * i;
                        if (newRow < 0 || newRow >= Constants.BOARD_SIZE || newCol < 0 || newCol >= Constants.BOARD_SIZE || board[newRow, newCol] != player)
                            break;
                        count++;
                    }
                    if (count >= 5) return true;
                }
            }
        }
        return false;
    }

    private static List<(int, int)> GetCandidateMoves(Constants.PlayerType[,] board)
    {
        List<(int, int)> moves = new List<(int, int)>();
        for (int row = 0; row < Constants.BOARD_SIZE; row++)
        {
            for (int col = 0; col < Constants.BOARD_SIZE; col++)
            {
                if (board[row, col] != Constants.PlayerType.None) continue;

                foreach (var (dx, dy) in directions)
                {
                    int newRow = row + dx, newCol = col + dy;
                    if (newRow >= 0 && newRow < Constants.BOARD_SIZE && newCol >= 0 && newCol < Constants.BOARD_SIZE && board[newRow, newCol] != Constants.PlayerType.None)
                    {
                        moves.Add((row, col));
                        break;
                    }
                }
            }
        }
        return moves;
    }

    private static bool IsBoardFull(Constants.PlayerType[,] board)
    {
        foreach (var cell in board)
        {
            if (cell == Constants.PlayerType.None) return false;
        }
        return true;
    }

    private static float EvaluateBoard(Constants.PlayerType[,] board)
    {
        float score = 0f;
        foreach (var (dx, dy) in directions)
        {
            for (int row = 0; row < Constants.BOARD_SIZE; row++)
            {
                for (int col = 0; col < Constants.BOARD_SIZE; col++)
                {
                    if (board[row, col] == Constants.PlayerType.None) continue;
                    int playerMultiplier = board[row, col] == Constants.PlayerType.PlayerB ? 1 : -1;
                    int count = 0;
                    bool isBlocked = false;
                    for (int i = 0; i < 5; i++)
                    {
                        int newRow = row + dx * i, newCol = col + dy * i;
                        if (newRow < 0 || newRow >= Constants.BOARD_SIZE || newCol < 0 || newCol >= Constants.BOARD_SIZE)
                        {
                            isBlocked = true;
                            break;
                        }
                        if (board[newRow, newCol] == board[row, col])
                            count++;
                        else if (board[newRow, newCol] != Constants.PlayerType.None)
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

    private static ulong ComputeHash(Constants.PlayerType[,] board)
    {
        ulong hash = 0;
        foreach (var cell in board)
        {
            hash = hash * 31 + (ulong)cell;
        }
        return hash;
    }

    private static bool IsBoardEmpty(Constants.PlayerType[,] board)
    {
        foreach (var cell in board)
        {
            if (cell != Constants.PlayerType.None) return false;
        }
        return true;
    }
}
