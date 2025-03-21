using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResponseLogInOnEvent", menuName = "IOnEventSO/ResponseLogInOnEvent")]
public class ResponseLogInOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        if (msg.GetParameter<string>() == "Success")
        {
            SceneId.Title.Load();
        }
        else
        {
            Debug.Log(msg.GetParameter<string>());
            UIManager.Instance.OpenChildrenCanvas(UIManager.Instance.triggeredEventUIComponent);
        }
    }
}
