using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EndGameOnEvent", menuName = "IOnEventSO/EndGameOnEvent")]
public class EndGameOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var listenerObj = msg.GetParameter<GameObject>();
        
        listenerObj.SetActive(true);
    }
}
