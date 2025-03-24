using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IOnEventSO : ScriptableObject, IOnEvent
{
    public abstract void OnEvent(EventMessage msg);

    /// <summary>
    /// 여러 Event Message를 기다려야 하는 경우 반드시 Override 해야합니다.
    /// </summary>
    /// <param name="msg"></param>
    public virtual void WaitForAllMessages()
    {
        return;
    }
    
    protected static Queue<EventMessage> WaitMessages = new();
}
