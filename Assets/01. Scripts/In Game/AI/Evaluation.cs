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

        float score = CalculateScore(count, isBlockedStart, isBlockedEnd, player == Turn.PLAYER2);

        float centerBonus = 1.0f + 0.1f * (7 - Math.Abs(row - Constants.BOARD_SIZE / 2)) +
                            0.1f * (7 - Math.Abs(col - Constants.BOARD_SIZE / 2));

        return score * centerBonus;
    }

    private static float CalculateScore(int count, bool isBlockedStart, bool isBlockedEnd, bool isAI)
    {
        if (count == 5) return 100000; // 오목 완성

        if (count == 4)
        {
            if (!isBlockedStart && !isBlockedEnd) return 7000;  // 열린 4 (OOOO_)
            if (isBlockedStart != isBlockedEnd) return 20000;   // 한쪽만 막힌 4
        }

        if (count == 3)
        {
            if (!isBlockedStart && !isBlockedEnd) return 500;   // 열린 3 (OOO__)
            if (isBlockedStart != isBlockedEnd) return 100;     // 한쪽만 막힌 3
        }

        if (count == 2)
        {
            if (!isBlockedStart && !isBlockedEnd) return 50;   // 열린 2 (OO__)
        }

        return 1; // 기타 경우 (1목 등)
    }

    public static float EvaluateMove(BoardGrid board, (int row, int col) move, bool isMaximizing)
    {
        float score = 0f;
        float proximityBonus = 0f;

        foreach (var (dx, dy) in directions)
        {
            score += GetPatternScoreWithMove(board, move.row, move.col, dx, dy, isMaximizing ? Turn.PLAYER2 : Turn.PLAYER1);
        }

        foreach (var (dx, dy) in directions)
        {
            int newRow = move.row + dx, newCol = move.col + dy;
            if (IsValid(newRow, newCol) && board[newRow, newCol]?.CellOwner != Turn.NONE)
            {
                proximityBonus += 10f;
            }
        }

        return score + proximityBonus;
    }

    private static float GetPatternScoreWithMove(BoardGrid board, int row, int col, int dx, int dy, Turn player)
    {
        int count = 1;
        bool isBlockedStart = false, isBlockedEnd = false;

        for (int i = 1; i < 5; i++)
        {
            int newRow = row + dx * i, newCol = col + dy * i;
            if (!IsValid(newRow, newCol)) { isBlockedEnd = true; break; }
            if (board[newRow, newCol]?.CellOwner == Turn.NONE) break;
            if (board[newRow, newCol].CellOwner != player) { isBlockedEnd = true; break; }
            count++;
        }

        for (int i = 1; i < 5; i++)
        {
            int newRow = row - dx * i, newCol = col - dy * i;
            if (!IsValid(newRow, newCol)) { isBlockedStart = true; break; }
            if (board[newRow, newCol]?.CellOwner == Turn.NONE) break;
            if (board[newRow, newCol].CellOwner != player) { isBlockedStart = true; break; }
            count++;
        }

        if (count == 4 && (isBlockedStart || isBlockedEnd))
        {
            Debug.Log($"🚨 4목 방어 필요! at ({row}, {col})");
            return 100000;
        }

        return CalculateScore(count, isBlockedStart, isBlockedEnd, player == Turn.PLAYER2);
    }

    private static bool IsValid(int row, int col)
    {
        return row >= -7 && row < Constants.BOARD_SIZE - 7 && col >= -7 && col < Constants.BOARD_SIZE - 7;
    }
}
