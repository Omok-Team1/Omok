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
    
    // 특정 GameObject의 리스너만 제거하는 메서드
    public void ClearListenersForGameObject(GameObject targetObject)
    {
        if (targetObject == null) return;
        
        // 이벤트 이름 목록을 가져옴 (반복문 중에 컬렉션 변경 방지)
        var eventNames = _listeners.Keys.ToList();
        
        foreach (var eventName in eventNames)
        {
            if (_listeners.TryGetValue(eventName, out var listeners))
            {
                // 특정 GameObject와 연결된 리스너만 제거
                int beforeCount = listeners.Count;
                listeners.RemoveAll(item => item.Item2 == targetObject);
                int removedCount = beforeCount - listeners.Count;
                
                // 리스너가 비어있으면 해당 이벤트 키 제거
                if (listeners.Count == 0)
                {
                    _listeners.Remove(eventName);
                }
                
                if (removedCount > 0)
                {
                    Debug.Log($"GameObject '{targetObject.name}'에 연결된 이벤트 '{eventName}'의 리스너 {removedCount}개 제거됨");
                }
            }
        }
    }
    
    // 특정 GameObject 목록의 리스너만 제거하는 메서드
    public void ClearListenersForGameObjects(List<GameObject> targetObjects)
    {
        foreach (var obj in targetObjects)
        {
            ClearListenersForGameObject(obj);
        }
    }
    
    // 특정 이벤트 타입에 대한 리스너만 제거하는 메서드
    public void ClearListenersForEvent(string eventName)
    {
        if (_listeners.ContainsKey(eventName))
        {
            int count = _listeners[eventName].Count;
            _listeners.Remove(eventName);
            Debug.Log($"이벤트 '{eventName}'에 대한 리스너 {count}개 제거됨");
        }
    }
    
    // 특정 이벤트 목록에 대한 리스너를 제거하는 메서드
    public void ClearListenersForEvents(List<string> eventNames)
    {
        foreach (var eventName in eventNames)
        {
            ClearListenersForEvent(eventName);
        }
    }
    
    // 특정 컴포넌트 타입을 가진 GameObject에 대한 리스너를 제거하는 메서드
    public void ClearListenersForComponentType<T>() where T : Component
    {
        // 씬에서 해당 타입의 모든 컴포넌트 찾기
        T[] components = GameObject.FindObjectsOfType<T>();
        foreach (var component in components)
        {
            ClearListenersForGameObject(component.gameObject);
        }
        
        Debug.Log($"{typeof(T).Name} 타입을 가진 {components.Length}개의 GameObject에 대한 리스너 초기화됨");
    }
}