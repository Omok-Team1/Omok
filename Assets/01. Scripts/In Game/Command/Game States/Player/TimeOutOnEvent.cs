using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeOutOnEvent", menuName = "IOnEventSO/TimeOutOnEvent")]
public class TimeOutOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var listenerObj = msg.GetParameter<GameObject>().GetComponent<StateMachine>();
        
        //Player Time-out!
        //GameManager.Instance.BoardManager.RecordDrop((-INF, -INF));
        
        //GameManager.Instance.TimerController.EndTurn();
        
        listenerObj.ChangeState<ChangeTurnState>();
    }

    private readonly int INF = (int)1e9;
}
