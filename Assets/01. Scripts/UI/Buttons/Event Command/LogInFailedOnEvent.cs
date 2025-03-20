using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "LogInFailedOnEvent", menuName = "IOnEventSO/LogInFailedOnEvent")]
public class LogInFailedOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var eventObj = msg.GetParameter<GameObject>();
        
        eventObj.GetComponent<IUIComponent>().Show();

        var txt = eventObj.GetComponentInChildren<TextMeshProUGUI>();
        
        txt.text = msg.GetParameter<string>();
    }
}
