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

        // 🌟 중앙에서 너무 한쪽으로만 치우치지 않도록 가중치 적용
        float centerBonus = 1.0f + 0.1f * (7 - Math.Abs(row - Constants.BOARD_SIZE / 2)) +
                            0.1f * (7 - Math.Abs(col - Constants.BOARD_SIZE / 2));

        return score * centerBonus;
    }


  private static float CalculateScore(int count, bool isBlockedStart, bool isBlockedEnd, bool isAI)
    {
        float baseScore = count switch
        {
            5 => 100000,  // 오목 완성 (최고 점수)
            4 when !isBlockedStart && !isBlockedEnd => 5000,  // 열린 4
            4 => 500,  // 막힌 4
            3 when !isBlockedStart && !isBlockedEnd => 300,  // 열린 3
            3 => 30,  // 막힌 3
            2 when !isBlockedStart && !isBlockedEnd => 10,  // 열린 2
            _ => 1 // 나머지
        };

        return isAI ? baseScore * 1.2f : baseScore;  // AI 가중치 적용
    }


    public static float EvaluateMove(BoardGrid board, (int row, int col) move, bool isMaximizing)
    {
        float score = 0f;
        foreach (var (dx, dy) in directions)
        {
            score += GetPatternScoreWithMove(board, move.row, move.col, dx, dy, isMaximizing ? Turn.PLAYER2 : Turn.PLAYER1);
        }
        return score;
    }

    private static float GetPatternScoreWithMove(BoardGrid board, int row, int col, int dx, int dy, Turn player)
    {
        int count = 1;
        bool isBlockedStart = false, isBlockedEnd = false;

        for (int i = 1; i < 5; i++)
        {
            int newRow = row + dx * i, newCol = col + dy * i;
            if (!IsValid(newRow, newCol) || board[newRow, newCol]?.CellOwner == Turn.NONE) break;
            if (board[newRow, newCol].CellOwner != player) { isBlockedEnd = true; break; }
            count++;
        }

        for (int i = 1; i < 5; i++)
        {
            int newRow = row - dx * i, newCol = col - dy * i;
            if (!IsValid(newRow, newCol) || board[newRow, newCol]?.CellOwner == Turn.NONE) break;
            if (board[newRow, newCol].CellOwner != player) { isBlockedStart = true; break; }
            count++;
        }

        return CalculateScore(count, isBlockedStart, isBlockedEnd, player == Turn.PLAYER2);
    }


    private static bool IsValid(int row, int col)
    {
        return row >= -7 && row < Constants.BOARD_SIZE - 7 && col >= -7 && col < Constants.BOARD_SIZE - 7;
    }
}