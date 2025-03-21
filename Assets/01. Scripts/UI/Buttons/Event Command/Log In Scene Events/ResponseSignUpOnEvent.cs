using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResponseSignUpOnEvent", menuName = "IOnEventSO/ResponseSignUpOnEvent")]
public class ResponseSignUpOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        if (msg.GetParameter<string>() == "Success")
        {
            var child = UIManager.Instance.triggeredEventUIComponent.GetChildren()[0];
            UIManager.Instance.OpenTargetChildCanvas(UIManager.Instance.triggeredEventUIComponent, child);
        }
        else
        {
            Debug.Log(msg.GetParameter<string>());
            var child = UIManager.Instance.triggeredEventUIComponent.GetChildren()[1];
            UIManager.Instance.OpenTargetChildCanvas(UIManager.Instance.triggeredEventUIComponent, child);
        }
    }
}
