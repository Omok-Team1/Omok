using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventMessage
{
    public EventMessage(string eventName)
    {
        EventName = eventName;
        Parameters = new Dictionary<Type, object>();
    }

    public void AddParameter<T>(object value)
    {
        if(Parameters.ContainsKey(typeof(T)) is true) Parameters[typeof(T)] = value;
        else Parameters.Add(typeof(T), value);
    }
    
    public void AddParameter(Type type, object value)
    {
        if(Parameters.ContainsKey(type) is true) Parameters[type] = value;
        else Parameters.Add(type, value);
    }
    
    public bool TryGetParameter<T>(out T value)
    {
        if (Parameters.TryGetValue(typeof(T), out object val))
        {
            value = (T)val;
            return true;
        }
        else
        {
            value = default(T);
            return false;
        }
    }

    public T GetParameter<T>()
    {
        if(Parameters.TryGetValue(typeof(T), out object value)) return (T)value;
        else throw new KeyNotFoundException(typeof(T).Name + " is Invalid parameter type");
    }

    private IDictionary<Type, object> Parameters;
    public string EventName;
}
