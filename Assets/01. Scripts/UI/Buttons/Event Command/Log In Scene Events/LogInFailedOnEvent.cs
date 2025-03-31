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

        IUIComponent component = UIManager.Instance.triggeredEventUIComponent;

        UIManager.Instance.OpenChildrenCanvas(component);

        var txt = eventObj.GetComponentInChildren<TextMeshProUGUI>();
        
        txt.text = msg.GetParameter<string>();
    }
}
