using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TempRequestSignUpEvent", menuName = "IOnEventSO/TempRequestSignUpEvent")]
public class TempRequestSignUpEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var inputStrings = msg.GetParameter<List<string>>();

        foreach (var inputString in inputStrings)
        {
            foreach (var s in inputString)
            {
                Debug.Log(s);
            }
        }

        var rawImage = msg.GetParameter<byte[]>();
        
        SignUpData inputData = new SignUpData(inputStrings[0], inputStrings[1], rawImage);
        
        TempNetworkManager.Instance.requestsignup(inputData);
    }
}
