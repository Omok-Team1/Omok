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

        //메세지를 만들어야 할 때
        if (requestEventName is not null && requestEventName.Length > 0)
        {
            var message = new EventMessage(requestEventName);

            foreach (var param in MessageParamsInfo)
            {
                message.AddParameter(param.Item1, param.Item2);
            }
            
            EventManager.Instance.PushEventMessageEvent(message);

            if (isSingleMessage is true)
            {
                EventManager.Instance.PublishSingleMessage(message);
                return;
            }
        }
        
        EventManager.Instance.PublishMessageQueue();
    }
    
    public override List<IUIComponent> GetChildren()
    {
        return childrenComponent;
    }

    [SerializeField] private bool isSingleMessage;
    
    [Tooltip("OnClik 이벤트 발생 시 메세지를 만들어야 한다면, 이벤트 이름과 필요한 데이터 파라미터들을 채워주세요")]
    [Header("발신할 이벤트 이름입니다.")]
    [SerializeField] private string requestEventName;
    
    [Tooltip("필수로 입력해야하는 값들이 아닙니다.")]
    [Header("이벤트 발신 시 메세지에 필요한 데이터들을 설정한 후 메세지를 발신합니다.")]
    
    [Header("수신 받을 이벤트 이름입니다.")]
    [SerializeField] private string responseEventName;
    
    [Header("이벤트를 수신 받았을 때 이벤트를 처리할 이벤트 함수입니다.")]
    [SerializeField] private IOnEventSO responseOnEvent;

    private readonly List<(Type, object)> _messageParamsInfo = new();
    public List<(Type, object)> MessageParamsInfo
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
