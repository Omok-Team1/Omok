using System;
using UnityEngine;
public class RankManager
{
    // AI 난이도 설정 기준
    public enum Rank
    {
        Beginner,  // 초급 (18~10급)
        Intermediate, // 중급 (9~5급)
        Advanced // 고급 (4~1급)
    }

    // 플레이어의 급수를 기반으로 AI 난이도 결정
    public static Rank GetRank(int playerRank)
    {
        if (playerRank >= 10) return Rank.Beginner;
        if (playerRank >= 5) return Rank.Intermediate;
        return Rank.Advanced;
    }

    // 난이도에 따른 탐색 깊이 설정
    public static int GetDepth(Rank rank)
    {
        switch (rank)
        {
            case Rank.Beginner: return 1; // 초급 AI는 깊이 1
            case Rank.Intermediate: return 2; // 중급 AI는 깊이 2
            case Rank.Advanced: return 4; // 고급 AI는 깊이 4 이상
            default: return 1;
        }
    }

    // 난이도에 따른 후보 수 제한 설정
    public static int GetMoveLimit(Rank rank)
    {
        switch (rank)
        {
            case Rank.Beginner: return 0; // 초급 AI는 무작위 or 단순 후보 선정
            case Rank.Intermediate: return 5; // 중급 AI는 상위 5~7개 후보 선택
            case Rank.Advanced: return 7; // 고급 AI는 상위 7~10개 후보 + Move Ordering
            default: return 0;
        }
    }

    // 난이도별 랜덤성 추가 (비효율적인 수 선택 확률 조정)
    public static float GetRandomness(Rank rank)
    {
        switch (rank)
        {
            case Rank.Beginner: return 0.4f; // 초급 AI는 30~50% 확률로 실수 유도
            case Rank.Intermediate: return 0.15f; // 중급 AI는 10~20% 확률로 서브 최선의 수 선택
            case Rank.Advanced: return 0.0f; // 고급 AI는 완전히 최선의 수만 선택
            default: return 0.4f;
        }
    }
}
