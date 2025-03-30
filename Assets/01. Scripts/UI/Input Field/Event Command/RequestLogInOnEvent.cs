using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequestLogInOnEvent", menuName = "IOnEventSO/RequestLogInOnEvent")]
public class RequestLogInOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var message = new EventMessage("RequestLogIn");

        var listener = msg.GetParameter<GameObject>().GetComponent<EventInputField>();
        
        foreach (var inputField in listener.InputField)
        {
            _inputFieldsDatas.Add(inputField.text);
        }
        
        message.AddParameter<List<string>>(_inputFieldsDatas);
        
        EventManager.Instance.PushEventMessageEvent(message);
        EventManager.Instance.PublishMessageQueue();
    }
    
    private readonly List<string> _inputFieldsDatas = new List<string>();
}
