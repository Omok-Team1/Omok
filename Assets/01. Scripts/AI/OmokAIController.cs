using System;
using System.Collections.Generic;
using UnityEngine;

public static class OmokAIController
{
    private const int SEARCH_DEPTH = 3; // 탐색 깊이 조절
    private static readonly (int, int)[] directions = { (1, 0), (0, 1), (1, 1), (1, -1) };

    public static (int row, int col) GetBestMove(Constants.PlayerType[,] board)
    {
        if (IsBoardEmpty(board))
        {
            int center = Constants.BOARD_SIZE / 2;
            return (center, center);
        }

        float bestScore = float.MinValue;
        (int, int) bestMove = (-1, -1);

        foreach (var (row, col) in GetCandidateMoves(board))
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

    private static float DoMinimax(Constants.PlayerType[,] board, int depth, bool isMaximizing, float alpha, float beta)
    {
        if (CheckWin(Constants.PlayerType.PlayerA, board)) return -1000 + depth;
        if (CheckWin(Constants.PlayerType.PlayerB, board)) return 1000 - depth;
        if (depth == 0 || IsBoardFull(board)) return EvaluateBoard(board);

        if (isMaximizing)
        {
            float bestScore = float.MinValue;
            foreach (var (row, col) in GetCandidateMoves(board))
            {
                board[row, col] = Constants.PlayerType.PlayerB;
                float score = DoMinimax(board, depth - 1, false, alpha, beta);
                board[row, col] = Constants.PlayerType.None;
                bestScore = Math.Max(bestScore, score);
                alpha = Math.Max(alpha, score);
                if (beta <= alpha) break;
            }
            return bestScore;
        }
        else
        {
            float bestScore = float.MaxValue;
            foreach (var (row, col) in GetCandidateMoves(board))
            {
                board[row, col] = Constants.PlayerType.PlayerA;
                float score = DoMinimax(board, depth - 1, true, alpha, beta);
                board[row, col] = Constants.PlayerType.None;
                bestScore = Math.Min(bestScore, score);
                beta = Math.Min(beta, score);
                if (beta <= alpha) break;
            }
            return bestScore;
        }
    }

    private static bool CheckWin(Constants.PlayerType player, Constants.PlayerType[,] board)
    {
        for (int row = 0; row < Constants.BOARD_SIZE; row++)
        {
            for (int col = 0; col < Constants.BOARD_SIZE; col++)
            {
                if (board[row, col] == player)
                {
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
                if (board[row, col] == Constants.PlayerType.None)
                {
                    bool hasNeighbor = false;
                    foreach (var (dx, dy) in directions)
                    {
                        int newRow = row + dx, newCol = col + dy;
                        if (newRow >= 0 && newRow < Constants.BOARD_SIZE && newCol >= 0 && newCol < Constants.BOARD_SIZE && board[newRow, newCol] != Constants.PlayerType.None)
                        {
                            hasNeighbor = true;
                            break;
                        }
                    }
                    if (hasNeighbor) moves.Add((row, col));
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

    private static bool IsBoardEmpty(Constants.PlayerType[,] board)
    {
        foreach (var cell in board)
        {
            if (cell != Constants.PlayerType.None) return false;
        }
        return true;
    }
}
