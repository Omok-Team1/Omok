using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReplayViewController : MonoBehaviour
{
    [SerializeField] private BoardManager _boardManager;
    [SerializeField] private TMP_Text turnInfoText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button rewindButton;
    [SerializeField] private Button nextTurnButton;
    [SerializeField] private Button previousTurnButton;
    private List<CellData> _replayMoves;
    private int _currentTurnIndex = 0;
    private bool _isPlaying = false;

    public void LoadReplay(ReplayData replayData)
    {
        _boardManager.Grid.Clear();
        _replayMoves = new List<CellData>(replayData.GameMoves);
        _currentTurnIndex = 0;
        _isPlaying = false;

        UpdateTurnInfoText(replayData);
        SetupButtons();
    }

    private void UpdateTurnInfoText(ReplayData replayData)
    {
        turnInfoText.text = $"Replay: {replayData.GameDate}\n" +
                            $"Winner: {replayData.Winner}\n" +
                            $"Current Turn: {_currentTurnIndex}/{_replayMoves.Count}\n" +
                            $"Total Turns: {replayData.TotalTurns}";
    }

    private void SetupButtons()
    {
        playButton.onClick.RemoveAllListeners();
        pauseButton.onClick.RemoveAllListeners();
        rewindButton.onClick.RemoveAllListeners();
        nextTurnButton.onClick.RemoveAllListeners();
        previousTurnButton.onClick.RemoveAllListeners();

        playButton.onClick.AddListener(PlayReplay);
        pauseButton.onClick.AddListener(PauseReplay);
        rewindButton.onClick.AddListener(RewindReplay);
        nextTurnButton.onClick.AddListener(NextTurn);
        previousTurnButton.onClick.AddListener(PreviousTurn);
    }
    
    public void NextTurn()
    {
        if (_currentTurnIndex < _replayMoves.Count)
        {
            var move = _replayMoves[_currentTurnIndex];
            _boardManager.OnDropMarker((move.row, move.col), move.markerSprite);
            _currentTurnIndex++;
            UpdateTurnInfoText(new ReplayData { 
                GameDateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Winner = Turn.NONE, 
                TotalTurns = _replayMoves.Count 
            });
        }
    }
    
    public void PreviousTurn()
    {
        if (_currentTurnIndex > 0)
        {
            _currentTurnIndex--;
            _boardManager.Grid.Clear();
            
            for (int i = 0; i < _currentTurnIndex; i++)
            {
                var move = _replayMoves[i];
                _boardManager.OnDropMarker((move.row, move.col), move.markerSprite);
            }

            UpdateTurnInfoText(new ReplayData { 
                GameDateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Winner = Turn.NONE, 
                TotalTurns = _replayMoves.Count 
            });
        }
    }

    public void PlayReplay()
    {
        if (!_isPlaying)
        {
            _isPlaying = true;
            StartCoroutine(PlayReplayCoroutine());
        }
    }

    private IEnumerator PlayReplayCoroutine()
    {
        while (_currentTurnIndex < _replayMoves.Count && _isPlaying)
        {
            PlayNextTurn();
            yield return new WaitForSeconds(1f);
        }
        _isPlaying = false;
    }

    private void PlayNextTurn()
    {
        if (_currentTurnIndex < _replayMoves.Count)
        {
            var move = _replayMoves[_currentTurnIndex];
            _boardManager.OnDropMarker((move.row, move.col), move.markerSprite);
            _currentTurnIndex++;
        }
    }

    public void PauseReplay()
    {
        _isPlaying = false;
        StopAllCoroutines();
    }

    public void RewindReplay()
    {
        _boardManager.Grid.Clear();
        _currentTurnIndex = 0;
    }
}
