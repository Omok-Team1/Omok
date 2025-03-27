using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReplayViewController : MonoBehaviour
{
    [Header("스톤 관리")]
    [SerializeField] private ReplayStoneManager _stoneManager;
    
    [Header("UI 텍스트")]
    [SerializeField] private TMP_Text _turnText;
    
    [Header("컨트롤 버튼")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _rewindButton;
    [SerializeField] private Button _nextTurnButton;
    [SerializeField] private Button _previousTurnButton;

    private ReplayData _currentReplay;
    private int _currentTurn = 0;
    private bool _isPlaying;

    private void Start()
    {
        // 버튼 이벤트 연결
        _playButton.onClick.AddListener(PlayAll);
        _pauseButton.onClick.AddListener(Pause);
        _rewindButton.onClick.AddListener(ResetReplay);
        _nextTurnButton.onClick.AddListener(NextTurn);
        _previousTurnButton.onClick.AddListener(PreviousTurn);
    }

    public void Initialize(ReplayData replay)
    {
        if (replay == null || replay.GameMoves == null)
        {
            Debug.LogError("리플레이 데이터가 유효하지 않습니다.");
            return;
        }

        _currentReplay = replay;
        ResetReplay();
    }

    private void ResetReplay()
    {
        _stoneManager.ClearAllStones();
        _currentTurn = 0;
        UpdateUI();
    }

    public void NextTurn()
{
    if (_currentTurn >= _currentReplay.GameMoves.Count) return;

    var move = _currentReplay.GameMoves[_currentTurn];
    // 변환 없이 원본 좌표 바로 전달 (-7~7)
    _stoneManager.PlaceStone(move.row, move.col, (int)move.cellOwner);
    _currentTurn++;
    UpdateUI();
}

private (int, int) AdjustCoordinates(int row, int col)
{
    return (row + 7, col + 7); // 15x15 전용 단순화
}

    public void PreviousTurn()
    {
        if (_currentTurn <= 0) return;

        _stoneManager.ClearAllStones();
        _currentTurn--;
        
        for (int i = 0; i < _currentTurn; i++)
        {
            var move = _currentReplay.GameMoves[i];
            _stoneManager.PlaceStone(move.row, move.col, (int)move.cellOwner);
        }
        
        UpdateUI();
    }

    public void PlayAll()
    {
        if (!_isPlaying && _currentTurn < _currentReplay.GameMoves.Count)
        {
            _isPlaying = true;
            StartCoroutine(PlayRoutine());
        }
    }

    private IEnumerator PlayRoutine()
    {
        while (_currentTurn < _currentReplay.GameMoves.Count && _isPlaying)
        {
            NextTurn();
            yield return new WaitForSeconds(0.5f);
        }
        _isPlaying = false;
    }

    public void Pause()
    {
        _isPlaying = false;
        StopAllCoroutines();
    }

    private void UpdateUI()
    {
        _turnText.text = $"{_currentTurn}";
        
        // 버튼 상태 업데이트
        _nextTurnButton.interactable = _currentTurn < _currentReplay.GameMoves.Count;
        _previousTurnButton.interactable = _currentTurn > 0;
        _playButton.interactable = !_isPlaying && _currentTurn < _currentReplay.GameMoves.Count;
        _pauseButton.interactable = _isPlaying;
    }
}