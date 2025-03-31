using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    public new void Awake()
    {
        base.Awake();
        // 씬 로드 시작 시 이벤트 큐와 리스너 초기화
        SceneLoader.OnAnySceneLoadedStarts += () => 
        {
            ClearEventQueue();
            _listeners.Clear();
            Debug.Log("씬 로드 시작: 이벤트 큐 및 리스너 초기화 완료");
        };
    }
    
    /// <summary>
    /// 인자로 전달한 Event를 기다리고 수신합니다.
    /// </summary>
    /// <param name="eventName">수신 받을 이벤트 이름</param>
    /// <param name="listener">이벤트 함수</param>
    /// <param name="listenerObj">해당 이벤트 함수를 멤버 메소드로 가지는 오브젝트</param>
    public void AddListener(string eventName, IOnEvent listener, GameObject listenerObj = null)
    {
        if (_listeners.TryGetValue(eventName, out List<(IOnEvent, GameObject)> eventListener))
        {
            eventListener.Add((listener, listenerObj));
        }
        else
        {
            eventListener = new List<(IOnEvent, GameObject)>();
            eventListener.Add((listener, listenerObj));
            _listeners.Add(eventName, eventListener);
        }
    }

    public void RemoveListener(string eventName, IOnEventSO listener, GameObject listenerObj = null)
    {
        if (_listeners.TryGetValue(eventName, out List<(IOnEvent, GameObject)> eventListener))
        {
            eventListener.Remove((listener, listenerObj));
        }
    }

    public void PushEventMessageEvent(EventMessage eventMessage)
    {
        _eventQueue.Enqueue(eventMessage);
    }

    public void PopLastEventMessageEvent()
    {
        Queue<EventMessage> messageQueue = new Queue<EventMessage>();
        
        if(_eventQueue.Count == 0) return;
        else
        {
            int count = _eventQueue.Count;

            for (int i = 0; i < count - 1; i++)
            {
                messageQueue.Enqueue(_eventQueue.Dequeue());
            }

            _eventQueue.Dequeue();
            
            foreach (var eventMessage in _eventQueue)
            {
                Debug.LogError(eventMessage.EventName);
            }
            
            for (int i = 0; i < messageQueue.Count; i++)
            {
                _eventQueue.Enqueue(messageQueue.Dequeue());
            }
        }
    }

    public void ProcessEvent(EventMessage eventMessage)
    {
        if (_listeners.TryGetValue(eventMessage.EventName, out List<(IOnEvent, GameObject)> eventListener))
        {
            foreach (var (listener, listenerObj) in eventListener)
            {
                if (listener is null)
                    throw new Exception(listenerObj.name + "'s IOnEventSO is null!!!");
                //같은 이벤트를 기다리는 게임 오브젝트들이 여러개 일 때 값(게임 오브젝트)를 덮어 쓴다.
                eventMessage.AddParameter<GameObject>(listenerObj);
                
                listener.OnEvent(eventMessage);
            }
        }
    }

    public void PublishMessageQueue()
    {
        while (_eventQueue.Count > 0)
        {
            EventMessage eventMessage = _eventQueue.Dequeue();
            
            ProcessEvent(eventMessage);
        }
    }

    public void PublishSingleMessage(EventMessage eventMessage)
    {
        ProcessEvent(eventMessage);
    }
    
    private readonly IDictionary<string, List<(IOnEvent, GameObject)>> _listeners = new Dictionary<string, List<(IOnEvent, GameObject)>>();
    private Queue<EventMessage> _eventQueue = new Queue<EventMessage>();
    
    public void ClearEventQueue()
    {
        _eventQueue.Clear();
        Debug.Log("이벤트 큐가 초기화되었습니다.");
    }
    public void ClearAllListeners()
    {
        _listeners.Clear();
        Debug.Log("모든 이벤트 리스너 초기화됨");
    }
}