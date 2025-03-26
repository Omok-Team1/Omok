using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequestSignUpOnEvent", menuName = "IOnEventSO/RequestSignUpOnEvent")]
public class RequestSignUpOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var message = new EventMessage("RequestSignUp");

        var listener = msg.GetParameter<GameObject>().GetComponent<EventInputField>();
        
        //"아이디(이메일), 패스워드, 패스워드 확인" String 3개
        foreach (var inputField in listener.InputField)
        {
            _inputFieldsDatas.Add(inputField.text);
        }
        
        message.AddParameter<List<string>>(_inputFieldsDatas);
        
        EventManager.Instance.PushEventMessageEvent(message);
        //EventManager.Instance.PublishMessageQueue();
    }
    
    private readonly List<string> _inputFieldsDatas = new List<string>();
}
