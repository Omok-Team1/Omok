using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequestServerEvent", menuName = "IOnEventSO/RequestServerEvent")]
public class RequestServerEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var inputStrings = msg.GetParameter<List<string>>();
        
        SigninData inputData = new SigninData(inputStrings[0], null);
        
        TempNetworkManager.Instance.RequestLogIn(inputData);
    }
}
