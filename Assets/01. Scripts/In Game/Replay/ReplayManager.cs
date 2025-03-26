using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 리플레이 데이터 모델 (기존 코드 유지)
[Serializable]
public class CellData
{
    public int row;
    public int col;
    public Sprite markerSprite;
    public Turn cellOwner;
}

[Serializable]
public class ReplayData
{
    public string ReplayId;
    public string GameDateString;
    public DateTime GameDate 
    {
        get 
        {
            // 여러 형식의 날짜 파싱 시도
            string[] dateFormats = {
                "yyyy-MM-dd HH:mm:ss.fff",  // 밀리초 포함 형식
                "yyyy-MM-dd HH:mm:ss",      // 초 포함 형식
                "yyyy-MM-dd HH:mm"          // 기존 형식
            };
            
            if (DateTime.TryParseExact(
                    GameDateString, 
                    dateFormats, 
                    System.Globalization.CultureInfo.InvariantCulture, 
                    System.Globalization.DateTimeStyles.AssumeLocal, 
                    out DateTime parsedDate))
            {
                return parsedDate;
            }

            // 파싱 실패 시 현재 날짜/시간 반환 (디버깅용)
            Debug.LogWarning($"Failed to parse date: {GameDateString}");
            return DateTime.MinValue;
        }
    }
    public Turn Winner;
    public int TotalTurns;
    public List<CellData> GameMoves = new List<CellData>();
    
    public int ReplayNumber;
}

public class ReplayManager : MonoBehaviour
{
    private const string REPLAY_FOLDER = "Replays";

    private const int MAX_REPLAY_STORAGE = 10;

    public static ReplayManager Instance { get; private set; }
    private List<ReplayData> _replays = new List<ReplayData>();
    private int _currentReplayNumber = 1; // 현재 리플레이 번호

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadReplays();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveReplay(BoardManager boardManager, Turn winner)
    {

        string gameDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        // 리플레이 개수가 최대치를 초과하면 가장 오래된 리플레이 삭제
        if (_replays.Count >= MAX_REPLAY_STORAGE)
        {
            // 가장 오래된 리플레이 제거 (날짜 기준)
            var oldestReplay = _replays.OrderBy(r => r.GameDate).First();
            _replays.Remove(oldestReplay);
        }

        var usedNumbers = _replays.Select(r => r.ReplayNumber).ToList();
        int nextNumber = 1;
        
        for (int i = 1; i <= MAX_REPLAY_STORAGE; i++)
        {
            if (!usedNumbers.Contains(i))
            {
                nextNumber = i;
                break;
            }
        }
        
        if (usedNumbers.Count >= MAX_REPLAY_STORAGE)
        {
            nextNumber = _replays.OrderBy(r => r.GameDate).First().ReplayNumber;
        }

        int totalTurns = boardManager.MatchRecord.Count;
        var moves = boardManager.MatchRecord;
        var newReplay = new ReplayData
        {
            ReplayId = Guid.NewGuid().ToString(),
            ReplayNumber = nextNumber,
            GameDateString = gameDate, // 문자열로 저장
            Winner = winner,
            TotalTurns = totalTurns
        };

        foreach (var cell in moves.Reverse())
        {
            newReplay.GameMoves.Add(new CellData
            {
                row = cell._coordinate.Item1,
                col = cell._coordinate.Item2,
                markerSprite = cell.Marker,
                cellOwner = cell.CellOwner
            });
        }

        _replays.Add(newReplay);
        _currentReplayNumber = nextNumber;

        Debug.Log($"SaveReplay Method Called - Winner: {winner}, Turns: {totalTurns}");
        Debug.Log($"Match Record Count: {boardManager.MatchRecord.Count}");
        SaveToJson();

#if UNITY_EDITOR
    UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private void SaveToJson()
    {
        string fullPath = Path.Combine(Application.dataPath, REPLAY_FOLDER);

        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        // 중복 제거
        var uniqueReplays = _replays
            .GroupBy(r => r.ReplayId) // ReplayId로 중복 제거
            .Select(g => g.Last())
            .ToList();

        string fileName = "ReplayData.json";
        string filePath = Path.Combine(fullPath, fileName);

        string jsonData = JsonUtility.ToJson(new Serialization<ReplayData>(uniqueReplays), true);
        File.WriteAllText(filePath, jsonData);

        // 저장된 리플레이 목록 업데이트
        _replays = uniqueReplays;
    }


    private void LoadReplays()
    {
        string fullPath = Path.Combine(Application.dataPath, REPLAY_FOLDER);

        // 디렉토리 존재 확인 및 생성
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        // JSON 파일들 로드
        string[] replayFiles = Directory.GetFiles(fullPath, "*.json");

        _replays.Clear();
        foreach (string filePath in replayFiles)
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                var loadedReplays = JsonUtility.FromJson<Serialization<ReplayData>>(jsonData);

                // 로드된 리플레이 추가
                if (loadedReplays?.target != null)
                {
                    _replays.AddRange(loadedReplays.target);
                }
            }
        }

        if (_replays.Count > 0)
        {
            _currentReplayNumber = _replays.Max(r => r.ReplayNumber);
        }
        else
        {
            _currentReplayNumber = 1;
        }

        Debug.Log($"Loaded {_replays.Count} replays from {replayFiles.Length} files");

    }

    public List<ReplayData> GetReplays()
    {
        return _replays
            .Where(r => r.GameDate != DateTime.MinValue)
            .OrderByDescending(r => r.GameDate) // DateTime 전체 비교
            .ThenByDescending(r => r.ReplayNumber) // 동일 시간일 경우 번호로 추가 정렬
            .Take(MAX_REPLAY_STORAGE)
            .ToList();

        return _replays
            .OrderByDescending(r => r.GameDate.Ticks) // Ticks로 더 정밀한 정렬
            .Take(MAX_REPLAY_STORAGE)
            .ToList();
    }

    [Serializable]
    private class Serialization<T>
    {
        public List<T> target;

        public Serialization(List<T> target)
        {
            this.target = target;
        }

        public List<T> ToList() => target;
    }

}




