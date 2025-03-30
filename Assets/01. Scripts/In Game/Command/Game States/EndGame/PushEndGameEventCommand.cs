using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PushEndGameEventCommand : ICommand
{
    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;

    public bool Execute()
    {
        var message = new EventMessage("EndGame");

        // 승자 정보 (턴에 따른 이름)
        string winner = _boardManager.GameData.winner == Turn.PLAYER1 ?
            _boardManager.GameData.player1Name : _boardManager.GameData.player2Name;
        
        // 메시지에 승자 이름 추가
        message.AddParameter<string>(winner);
        
        // 승점 (두 번째 버전의 접근 방식)
        // TODO: 플레이어 등급에 따른 더 동적인 점수 시스템 구현 필요
        message.AddParameter<int>(10);
        
        // 리플레이를 위한 대국 기록
        message.AddParameter<Stack<Cell>>(_boardManager.MatchRecord.Reverse());

        // 리플레이 저장
        ReplayManager.Instance.SaveReplay(
            _boardManager, 
            _boardManager.GameData.winner
        );

        // 게임 타이머 비활성화
        GameManager.Instance.TimerController.gameObject.SetActive(false);

        // 이벤트 푸시 및 발행
        EventManager.Instance.PushEventMessageEvent(message);
        EventManager.Instance.PublishMessageQueue();
        GameEndEventDispatcher.Instance.OnGameEnded.Invoke();
        return true;
    }
}