using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "TempRequestSignUpEvent", menuName = "IOnEventSO/TempRequestSignUpEvent")]
public class TempRequestSignUpEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        WaitMessages.Enqueue(msg);
        
        if(coroutine is null)
            coroutine = StaticCoroutine.StartStaticCoroutine(WaitForAllMessages());
    }

    private IEnumerator WaitForAllMessages()
    {
        SignUpData signUpData;

        signUpData.username = null;
        signUpData.password = null;
        signUpData.nickname = null;
        
        while (true)
        {
            // if (signUpData.username is not null && signUpData.password is not null &&
            //     signUpData.image is not null)
            if (signUpData.username is not null && signUpData.password is not null &&
                signUpData.nickname is not null)
            {
                break;
            }

            if (WaitMessages.Count > 0)
            {
                var msg = WaitMessages.Dequeue();
                
                // if (msg.TryGetParameter(out byte[] rawImage) is true) 
                // {
                //     signUpData.image = rawImage;
                // }
                
                if (msg.TryGetParameter(out List<string> inputStrings) is true)
                {
                    signUpData.username = inputStrings[0]; 
                    signUpData.password = inputStrings[1];
                    signUpData.nickname = inputStrings[2];
                }
            }
            
            yield return null;
        }
        
        TempNetworkManager.Instance.RequestsignUp(signUpData);
    }
    
    private Coroutine coroutine;
}
