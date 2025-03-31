using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ResponseSignUpOnEvent", menuName = "IOnEventSO/ResponseSignUpOnEvent")]
public class ResponseSignUpOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        if (msg.GetParameter<string>() == "Success")
        {
            var child = UIManager.Instance.triggeredEventUIComponent.GetChildren()[0];
            Debug.Log(child);
            UIManager.Instance.OpenTargetChildCanvas(UIManager.Instance.triggeredEventUIComponent, child);
        }
        else
        {
            var child = UIManager.Instance.triggeredEventUIComponent.GetChildren()[1];
            UIManager.Instance.OpenTargetChildCanvas(UIManager.Instance.triggeredEventUIComponent, child);
        }
    }
}
