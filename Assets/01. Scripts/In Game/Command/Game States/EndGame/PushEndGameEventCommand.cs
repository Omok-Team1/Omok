using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PushEndGameEventCommand : ICommand
{
    public bool Execute()
    {
        var message = new EventMessage("EndGame");

        //승자 정보
        string winner = _boardManager.GameData.winner == Turn.PLAYER1 ?
            _boardManager.GameData.player1Name : _boardManager.GameData.player2Name;
        
        message.AddParameter<string>(winner);
        
        //플레이어 랭크 포인트
        //message.AddParameter<uint>(_boardManager.GameData.);
        
        //승자 급수에 따른 승점 정보
        //TODO: 일단은 테스트를 위해 10으로 설정, 후에 급수에 따라 점수를 주는 로직이 구현되어야한다.
        message.AddParameter<int>(10);
        
        //리플레이 기능을 위해 이번 대국의 착수 정보
        message.AddParameter<Stack<Cell>>(_boardManager.MatchRecord.Reverse());
        
        GameManager.Instance.TimerController.gameObject.SetActive(false);
        
        EventManager.Instance.PushEventMessageEvent(message);
        
        EventManager.Instance.PublishMessageQueue();
        
        return true;
    }
    
    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}
