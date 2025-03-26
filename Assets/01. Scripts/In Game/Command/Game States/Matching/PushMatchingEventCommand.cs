using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushMatchingEventCommand : ICommand
{
    public bool Execute()
    {
        var message = new EventMessage("Matching");
        
        //TODO: 상대방 이름, 프로필 사진 설정해주기. 여기서는 임시로 고정된 값으로 설정함
        message.AddParameter<string>("Omok is Love");
        //_boardManager.GameData.player1Sprite = ;
        message.AddParameter<Sprite>(null);

        EventManager.Instance.PushEventMessageEvent(message);
        EventManager.Instance.PublishMessageQueue();
        
        return true;
    }
    
    private readonly BoardManager _boardManager = GameManager.Instance.BoardManager;
}