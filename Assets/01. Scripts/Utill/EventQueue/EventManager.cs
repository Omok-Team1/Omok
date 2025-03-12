using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    private new void Awake()
    {
        base.Awake();

        SceneLoader.OnAnySceneLoadedStarts += _listeners.Clear;
    }
    
    public void AddListener(string eventName, IOnEventSO listener)
    {
        if (_listeners.TryGetValue(eventName, out List<IOnEventSO> eventListener))
        {
            eventListener.Add(listener);
        }
        else
        {
            eventListener = new List<IOnEventSO>();
            eventListener.Add(listener);
            _listeners.Add(eventName, eventListener);
        }
    }

    public void PushEventMessageEvent(EventMessage eventMessage)
    {
        _eventQueue.Enqueue(eventMessage);
    }

    public void ProcessEvent(EventMessage eventMessage)
    {
        if (_listeners.TryGetValue(eventMessage.EventName, out List<IOnEventSO> eventListener))
        {
            foreach (IOnEventSO listener in eventListener)
            {
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
    
    private readonly Dictionary<string, List<IOnEventSO>> _listeners = new Dictionary<string, List<IOnEventSO>>();
    private Queue<EventMessage> _eventQueue = new Queue<EventMessage>();
}
