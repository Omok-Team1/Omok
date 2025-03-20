using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[
    RequireComponent(typeof(Button))
]
public class OnEventButton : UserFriendlyComponent
{
    public override void Init()
    {
        EventManager.Instance.AddListener(responseEventName ,responseOnEvent, gameObject);
        
        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(EventMethod);
        }
        
        CastingChildren();
    }

    public override void EventMethod()
    {
        UIManager.Instance.triggeredEventUIComponent = this;

        var message = new EventMessage(requestEventName);
        
        foreach (var param in _messageParamsInfo)
        {
            message.AddParameter(param.Item1, param.Item2);
        }
        
        EventManager.Instance.PushEventMessageEvent(message);
        EventManager.Instance.PublishMessageQueue();
    }
    
    public override List<IUIComponent> GetChildren()
    {
        return childrenComponent;
    }
    
    [Header("발신할 이벤트 이름입니다.")]
    [SerializeField] private string requestEventName;
    
    [Header("이벤트를 발신할 때 메세지에 필요한 데이터들을 설정한 후 메세지를 발신합니다.")]
    [SerializeField] private MessagePrimitiveParams messageParameters;
    
    [Header("수신 받을 이벤트 이름입니다.")]
    [SerializeField] private string responseEventName;
    
    [Header("이벤트를 수신 받았을 때 이벤트를 처리할 이벤트 함수입니다.")]
    [SerializeField] private IOnEventSO responseOnEvent;

    private static readonly List<(Type, object)> _messageParamsInfo = new();
    public static List<(Type, object)> MessageParamsInfo
    {
        get
        {
            if (_messageParamsInfo.Count == 0)
            {
                var fieldInfo = _messageParamsInfo.GetType().GetFields();

                foreach (var param in fieldInfo)
                {
                    _messageParamsInfo.Add((param.GetType(), param.GetValue(_messageParamsInfo)));
                }
            }

            return _messageParamsInfo;
        }
    }
}
