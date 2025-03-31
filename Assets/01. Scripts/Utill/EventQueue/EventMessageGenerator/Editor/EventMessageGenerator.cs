using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMessageGenerator : MonoBehaviour
{
    [SerializeField] private MessagePrimitiveParams messageParameters;

    void Start()
    {
        if (messageParameters is not null)
        {
            Debug.Log("hi");

            foreach (var ele in typeof(MessagePrimitiveParams).GetFields())
            {
                Debug.Log(ele.Name);
                Debug.Log(ele.GetValue(messageParameters));
            }
            
        }
        else
        {
            Debug.Log("bye");
        }
    }
}
