using System;
using System.Collections.Generic;
using UnityEngine;

public static class Evaluation
{
    private static readonly (int, int)[] directions = { (1, 0), (0, 1), (1, 1), (1, -1) };
    
    public static float EvaluateBoard(BoardGrid board)
    {
        float score = 0f;
        foreach (var (dx, dy) in directions)
        {
            for (int row = -7; row < Constants.BOARD_SIZE - 7; row++)
            {
                for (int col = -7; col < Constants.BOARD_SIZE - 7; col++)
                {
                    if (board[row, col] == null || board[row, col].CellOwner == Turn.NONE) continue;
                    score += GetPatternScore(board, row, col, dx, dy);
                }
            }
        }
        return score;
    }

    private static float GetPatternScore(BoardGrid board, int row, int col, int dx, int dy)
    {
        Turn player = board[row, col].CellOwner;
        int count = 0;
        bool isBlockedStart = false, isBlockedEnd = false;

        for (int i = 0; i < 5; i++)
        {
            int newRow = row + dx * i, newCol = col + dy * i;
            if (!IsValid(newRow, newCol) || board[newRow, newCol] == null)
            {
                isBlockedEnd = true;
                break;
            }
            if (board[newRow, newCol].CellOwner != player)
            {
                isBlockedEnd = true;
                break;
            }
            count++;
        }

        for (int i = 1; i < 5; i++)
        {
            int newRow = row - dx * i, newCol = col - dy * i;
            if (!IsValid(newRow, newCol) || board[newRow, newCol] == null)
            {
                isBlockedStart = true;
                break;
            }
            if (board[newRow, newCol].CellOwner != player)
            {
                isBlockedStart = true;
                break;
            }
            count++;
        }

        return CalculateScore(count, isBlockedStart, isBlockedEnd, player == Turn.PLAYER2);
    }

    private static float CalculateScore(int count, bool isBlockedStart, bool isBlockedEnd, bool isAI)
    {
        float baseScore = (float)Math.Pow(10, count);
        if (isBlockedStart && isBlockedEnd) return 0;
        if (!isBlockedStart && !isBlockedEnd) return baseScore * 1.5f;
        return baseScore;
    }

    public static float EvaluateMove(BoardGrid board, (int row, int col) move, bool isMaximizing)
    {
        board.MarkingTurnOnCell(move, isMaximizing ? Turn.PLAYER2 : Turn.PLAYER1);
        float score = EvaluateBoard(board);
        board.MarkingTurnOnCell(move, Turn.NONE);
        return score;
    }

    private static bool IsValid(int row, int col)
    {
        return row >= -7 && row < Constants.BOARD_SIZE - 7 && col >= -7 && col < Constants.BOARD_SIZE - 7;
    }
}