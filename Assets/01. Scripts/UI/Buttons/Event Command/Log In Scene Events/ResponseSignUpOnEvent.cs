using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResponseSignUpOnEvent", menuName = "IOnEventSO/ResponseSignUpOnEvent")]
public class ResponseSignUpOnEvent : IOnEventSO
{
    //서버를 열어야합니다.
    public override void OnEvent(EventMessage msg)
    {
        if (msg.GetParameter<string>() == "Success")
        {
            var child = UIManager.Instance.triggeredEventUIComponent.GetChildren()[0];
            UIManager.Instance.OpenTargetChildCanvas(UIManager.Instance.triggeredEventUIComponent, child);
        }
        else
        {
            var child = UIManager.Instance.triggeredEventUIComponent.GetChildren()[1];
            UIManager.Instance.OpenTargetChildCanvas(UIManager.Instance.triggeredEventUIComponent, child);
        }
    }
}
