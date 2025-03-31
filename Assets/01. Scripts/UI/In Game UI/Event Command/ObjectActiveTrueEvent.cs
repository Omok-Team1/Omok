using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectActiveTrueEvent", menuName = "IOnEventSO/ObjectActiveTrueEvent")]
public class ObjectActiveTrueEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var listenerObj = msg.GetParameter<GameObject>();
        
        var uiElement = listenerObj.GetComponent<IUIComponent>();

        uiElement.Show();
    }
}
