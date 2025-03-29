using System.Collections.Generic;
using UnityEngine;

public class ReplayBoardManager : MonoBehaviour
{
    [Header("의존성 설정")]
    [SerializeField] private BoardManager _originalBoardManager;

    private Dictionary<(int, int), Sprite> _currentMoves = new Dictionary<(int, int), Sprite>();

    private void Awake()
    {
        if (_originalBoardManager == null)
        {
            _originalBoardManager = FindObjectOfType<BoardManager>();
            if (_originalBoardManager == null)
            {
                Debug.LogError("BoardManager를 찾을 수 없습니다!");
                enabled = false;
            }
        }
    }

    public void ResetReplayBoard()
    {
        if (_originalBoardManager == null) return;

        foreach (var pos in _currentMoves.Keys)
        {
            _originalBoardManager.OnDropMarker(pos, null);
        }
        _currentMoves.Clear();
        Debug.Log("리플레이 보드 초기화 완료");
    }

    public void ApplyMove(int row, int col, Sprite marker)
    {
        if (_originalBoardManager == null) return;

        var position = (row, col);
        _currentMoves[position] = marker;
        _originalBoardManager.OnDropMarker(position, marker);
        Debug.Log($"돌 추가: ({row}, {col})");
    }
}