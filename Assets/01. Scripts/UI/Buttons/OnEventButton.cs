using System;
using System.Collections;
using System.Collections.Generic;
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
        EventManager.Instance.AddListener(eventName ,eventOnEvent, gameObject);
        
        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(EventMethod);
        }
        
        CastingChildren();
    }

    public override void EventMethod()
    {
        UIManager.Instance.triggeredEventUIComponent = this;
        EventManager.Instance.PublishMessageQueue();
    }
    
    public override List<IUIComponent> GetChildren()
    {
        return childrenComponent;
    }
    
    [Header("영어로 띄어쓰기 없는 카멜표기법을 권장합니다.")]
    [SerializeField] private string eventName;
    
    [Tooltip("")]
    [SerializeField] private IOnEventSO eventOnEvent;
}
