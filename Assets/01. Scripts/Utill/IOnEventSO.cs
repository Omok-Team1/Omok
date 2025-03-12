using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IOnEventSO : ScriptableObject, IOnEvent
{
    public abstract void OnEvent(EventMessage msg);
}
