using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushStartEventCommand : ICommand
{
    public bool Execute()
    {
        EventMessage profileMsg = new EventMessage("StartGame");
        EventMessage turnInfoMsg;
        
        //TODO: 상대방 이름, 프로필 사진 설정해주기. 여기서는 임시로 고정된 값으로 설정함
        profileMsg.AddParameter<string>("Stranger");
        //_boardManager.GameData.player2Sprite = ;
        profileMsg.AddParameter<Sprite>(null);
        
        if(_boardManager.GameData.currentTurn == Turn.PLAYER1) 
            turnInfoMsg = new EventMessage("PlayerTurn");
        else
            turnInfoMsg = new EventMessage("OpponentTurn");
        
        EventManager.Instance.PushEventMessageEvent(turnInfoMsg);
        
        //StartState에서는 상대방 정보만 업데이트 해주면 된다.
        //Player 본인의 정보는 MatchingState에서 업데이트 해줬기 떄문에.
        //따라서 상대방 정보만 Event Message로 넣어서 Publish 해줘도 괜찮다.
        EventManager.Instance.PushEventMessageEvent(profileMsg);
        EventManager.Instance.PublishMessageQueue();
        
        return true;
    }
    
    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}
