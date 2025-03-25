using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("EventManager").AddComponent<EventManager>();
            }
            return _instance;
        }
    }

    private Queue<EventMessage> eventQueue = new Queue<EventMessage>();

    public void PushEventMessageEvent(EventMessage message)
    {
        eventQueue.Enqueue(message);
        Debug.Log("Event Added: " + message.Message);
    }

    public EventMessage PopEventMessageEvent()
    {
        if (eventQueue.Count > 0)
        {
            return eventQueue.Dequeue();
        }
        return null;
    }

    public void ProcessEvents()
    {
        while (eventQueue.Count > 0)
        {
            EventMessage message = PopEventMessageEvent();
            // 이벤트 처리 로직
            Debug.Log("Processing Event: " + message.Message);
        }
    }
}

public class EventMessage
{
    public string Message { get; private set; }

    public EventMessage(string message)
    {
        Message = message;
    }
}