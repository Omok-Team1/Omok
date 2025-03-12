using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMessage
{
    public EventMessage(string eventName)
    {
        EventName = eventName;
        Parameters = new Dictionary<Type, object>();
    }

    public void AddParameter<T>(object value)
    {
        Parameters.Add(typeof(T), value);
    }

    public T GetParameter<T>()
    {
        if(Parameters.TryGetValue(typeof(T), out object value)) return (T)value;
        else throw new KeyNotFoundException("Invalid parameter type");
    }

    public IDictionary<Type, object> Parameters;
    public string EventName;
}
