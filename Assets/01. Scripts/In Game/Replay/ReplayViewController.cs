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
    [SerializeField] private TMP_Text _playerTurnText; 

    [Header("컨트롤 버튼")]
    
    [SerializeField] private Button _nextTurnButton;
    [SerializeField] private Button _previousTurnButton;

    private ReplayData _currentReplay;
    private int _currentTurn = 0;
    private bool _isPlaying;
    private Coroutine _playCoroutine;

    void OnEnable()
    {
        if (ReplaySceneManager.CurrentReplayData != null)
            Initialize(ReplaySceneManager.CurrentReplayData);
    }

    private void Start()
    {
        
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
		_playerTurnText.text = "";
        _isPlaying = false;
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }
        UpdateUI();
    }

    public void NextTurn()
    {
        if (_currentTurn >= _currentReplay.GameMoves.Count) return;

        var move = _currentReplay.GameMoves[_currentTurn];
        _stoneManager.PlaceStone(move.row, move.col, (int)move.cellOwner);
        _currentTurn++;
        UpdateUI();
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

    

	private IEnumerator SafePlayRoutine()
{
    while (_currentTurn < _currentReplay.GameMoves.Count)
    {
        try 
        {
            // 재생 중 리플레이 데이터가 삭제되거나 변경되는 것을 방지
            if (_currentReplay == null) 
            {
                Debug.LogWarning("재생 중 리플레이 데이터가 사라졌습니다.");
                break;
            }

            NextTurn();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"재생 중 오류 발생: {e.Message}");
            _isPlaying = false;
            break;
        }

        yield return new WaitForSeconds(0.5f);

        // 모든 턴을 재생했다면 자동 정지
        if (_currentTurn >= _currentReplay.GameMoves.Count)
        {
            _isPlaying = false;
            break;
        }
    }

    UpdateUI();
}

    private IEnumerator PlayRoutine()
    {
        while (_currentTurn < _currentReplay.GameMoves.Count)
        {
            NextTurn();
            yield return new WaitForSeconds(0.5f);

            // 모든 턴을 재생했다면 자동 정지
            if (_currentTurn >= _currentReplay.GameMoves.Count)
            {
                _isPlaying = false;
                break;
            }
        }
        
        UpdateUI();
    }

    

    private void UpdateUI()
{
    // 턴 표시 (1턴부터 시작)
    _turnText.text = $"{_currentTurn}";

    if (_currentReplay == null || _currentReplay.GameMoves == null)
    {
        _playerTurnText.text = "";
        return;
    }

    if (_currentTurn == 0) // 0턴일 때
    {
        _playerTurnText.text = "?";
        _playerTurnText.color = Color.gray; // 회색으로 표시
    }
    else if (_currentTurn <= _currentReplay.GameMoves.Count) // 1턴 이상
    {
        var currentMove = _currentReplay.GameMoves[_currentTurn - 1];
        _playerTurnText.text = currentMove.cellOwner == Turn.PLAYER1 ? "흑" : "백";
        _playerTurnText.color = currentMove.cellOwner == Turn.PLAYER1 
            ? Color.black 
            : Color.white;
    }
    else // 턴을 초과한 경우
    {
        _playerTurnText.text = "?";
        _playerTurnText.color = Color.gray;
    }

    // 버튼 상태 업데이트
    _nextTurnButton.interactable = _currentTurn < _currentReplay.GameMoves.Count;
    _previousTurnButton.interactable = _currentTurn > 0;
}
}