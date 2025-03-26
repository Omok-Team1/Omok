using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "EndGameText", menuName = "IOnEventSO/EndGameText")]
public class EndGameText : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var listenerObj = msg.GetParameter<GameObject>();
        
        var text = listenerObj.GetComponent<TextMeshProUGUI>();
        
        var matchPoint = msg.GetParameter<int>();

        text.text = msg.GetParameter<Turn>() + "가 " + text.text;

        //승리 혹은 무승부
        if (matchPoint >= 0)
        {
            text.text = text.text.Replace("{point}", matchPoint.ToString());
            text.text = text.text.Replace("{result}", "증가 했습니다.");
        }
        //패배
        else
        {
            text.text = text.text.Replace("{point}", matchPoint.ToString()) ;
            text.text = text.text.Replace("{result}", "감소 했습니다.");
        }
    }
}
