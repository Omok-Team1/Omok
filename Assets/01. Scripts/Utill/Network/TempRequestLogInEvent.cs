using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TempRequestLogInEvent", menuName = "IOnEventSO/TempRequestLogInEvent")]
public class TempRequestLogInEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var inputStrings = msg.GetParameter<List<string>>();

        SignInData inputData = new SignInData(inputStrings[0], inputStrings[1]);
        
        TempNetworkManager.Instance.RequestLogIn(inputData);
    }
}
