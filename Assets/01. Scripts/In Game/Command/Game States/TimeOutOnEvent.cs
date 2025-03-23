using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeOutOnEvent", menuName = "IOnEventSO/TimeOutOnEvent")]
public class TimeOutOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var listenerObj = msg.GetParameter<GameObject>().GetComponent<StateMachine>();
        
        listenerObj.ChangeState<ChangeTurnState>();
    }
}
