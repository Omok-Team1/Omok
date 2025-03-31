using System.Collections;
using UnityEngine;
using System.Collections.Generic; 

[CreateAssetMenu(fileName = "TempRequestSignUpEvent", menuName = "IOnEventSO/TempRequestSignUpEvent")]
public class TempRequestSignUpEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        WaitMessages.Enqueue(msg);
        
        if (coroutine is null)
            coroutine = StaticCoroutine.StartStaticCoroutine(WaitForMessagesCoroutine());
    }

    public override void WaitForAllMessages()
    {
        // 의도적으로 비워둠
    }

    private IEnumerator WaitForMessagesCoroutine()
    {
        SignUpData signUpData = new SignUpData
        {
            username = null,
            password = null,
            nickname = null
        };

        while (true)
        {
            if (signUpData.username != null && 
                signUpData.password != null && 
                signUpData.nickname != null)
            {
                break;
            }

            if (WaitMessages.Count > 0)
            {
                var msg = WaitMessages.Dequeue();
                
                if (msg.TryGetParameter(out List<string> inputStrings))
                {
                    signUpData.username = inputStrings[0]; 
                    signUpData.password = inputStrings[1];
                    signUpData.nickname = inputStrings[2];
                }
            }

            yield return null;
        }

        TempNetworkManager.Instance.RequestSignUp(signUpData);
        coroutine = null;
    }

    private Coroutine coroutine;
}