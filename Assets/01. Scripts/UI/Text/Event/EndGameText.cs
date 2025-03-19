using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "EndGameText", menuName = "IOnEventSO/EndGameText")]
public class EndGameText : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var listenerObj = msg.GetParameter<GameObject>();
        
        var text = listenerObj.GetComponent<TextMeshPro>();

        text.text = msg.GetParameter<Turn>() + " " + text.text;
        
        text.text = text.text.Replace("{point}", msg.GetParameter<int>().ToString());
    }
}
