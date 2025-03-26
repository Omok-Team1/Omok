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
    public DateTime GameDate => DateTime.Parse(GameDateString);
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
        

        // 리플레이 개수가 최대치를 초과하면 가장 오래된 리플레이 삭제
        if (_replays.Count >= MAX_REPLAY_STORAGE)
        {
            // 가장 오래된 리플레이 제거 (날짜 기준)
            var oldestReplay = _replays.OrderBy(r => r.GameDate).First();
            _replays.Remove(oldestReplay);
        }
        int totalTurns = boardManager.MatchRecord.Count;
        var moves = boardManager.MatchRecord;
        var newReplay = new ReplayData
        {
            ReplayId = Guid.NewGuid().ToString(),
            ReplayNumber = _currentReplayNumber,
            GameDateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm"), // 문자열로 저장
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
        _currentReplayNumber = _replays.Count + 1; // 저장된 리플레이 개수 기반 번호 할당
        newReplay.ReplayNumber = _currentReplayNumber;
        Debug.Log($"SaveReplay Method Called - Winner: {winner}, Turns: {totalTurns}");
        Debug.Log($"Match Record Count: {boardManager.MatchRecord.Count}");
        SaveToJson();
    
#if UNITY_EDITOR
    UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private void SaveToJson()
    {
        
        foreach (var replay in _replays)
        {
            Debug.Log($"Saving Replay - ID: {replay.ReplayId}, " +
                      $"Number: {replay.ReplayNumber}, " +
                      $"Date: {replay.GameDateString}, " +
                      $"Winner: {replay.Winner}, " +
                      $"Turns: {replay.TotalTurns}");
        }
        string fullPath = Path.Combine(Application.dataPath, REPLAY_FOLDER);
    
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        // 중복 제거
        var uniqueReplays = _replays
            .GroupBy(r => new { r.ReplayId, r.TotalTurns, r.Winner })
            .Select(g => g.First())
            .ToList();

        string fileName = $"Replay_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        string filePath = Path.Combine(fullPath, fileName);

        string jsonData = JsonUtility.ToJson(new Serialization<ReplayData>(uniqueReplays), true);
        File.WriteAllText(filePath, jsonData);

        // 저장된 리플레이 목록 업데이트
        _replays = uniqueReplays;

#if UNITY_EDITOR
    UnityEditor.AssetDatabase.Refresh();
#endif
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

        Debug.Log($"Loaded {_replays.Count} replays from {replayFiles.Length} files");
    }

    public List<ReplayData> GetReplays() 
    {
        // 날짜 기준 최신순으로 정렬 및 중복 제거
        List<ReplayData> validReplays = _replays
            .GroupBy(r => new { r.ReplayId, r.TotalTurns, r.Winner })
            .Select(g => g.OrderByDescending(r => r.GameDate).First())
            .OrderByDescending(r => r.GameDate)
            .ToList();

        Debug.Log($"Valid Replays After Deduplication: {validReplays.Count}");
        foreach (var replay in validReplays)
        {
            Debug.Log($"Replay Details: " +
                      $"No.{replay.ReplayNumber}, " +
                      $"Date: {replay.GameDate}, " +
                      $"Winner: {replay.Winner}, " +
                      $"Turns: {replay.TotalTurns}, " +
                      $"Moves: {replay.GameMoves.Count}");
        }

        return validReplays;
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



