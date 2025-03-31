using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TempRequestSignUpEvent", menuName = "IOnEventSO/TempRequestSignUpEvent")]
public class TempRequestSignUpEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var inputStrings = msg.GetParameter<List<string>>();

        SignUpData inputData = new SignUpData(inputStrings[0], inputStrings[1], inputStrings[2], inputStrings[3]);

        TempNetworkManager.Instance.RequestSignUp(inputData);
    }
}
