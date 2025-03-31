using System;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "OpponentTimeOutOnEvent", menuName = "IOnEventSO/OpponentTimeOutOnEvent")]
public class OpponentTimeOutOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var token = msg.GetParameter<CancellationTokenSource>();

        try
        {
            if(token.IsCancellationRequested is not true)
                token.Cancel();
        }
        catch (Exception e)
        {
            Debug.LogWarning("이미 Dispose 되어 있는 Token 입니다..");
        }
    }
}