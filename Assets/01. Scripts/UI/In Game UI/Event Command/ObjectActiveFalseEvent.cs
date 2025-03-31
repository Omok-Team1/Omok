using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectActiveFalseEvent", menuName = "IOnEventSO/ObjectActiveFalseEvent")]
public class ObjectActiveFalseEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var listenerObj = msg.GetParameter<GameObject>();
        
        listenerObj.SetActive(false);
    }
}
