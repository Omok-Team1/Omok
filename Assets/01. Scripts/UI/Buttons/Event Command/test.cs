using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    void Start()
    {
        var message = new EventMessage("ResponseLogIn");
        
        message.AddParameter<string>("Hello World");
        message.AddParameter<float>(2.5f);
        
        EventManager.Instance.PushEventMessageEvent(message);
    }

    private void OnGUI()
    {
        var style = new GUIStyle("button");
        style.fontSize = 40;
        style.normal.textColor = Color.white;

        if (GUILayout.Button("Publish Message Queue", style))
        {
            Debug.Log("Publish Message Queue");
            EventManager.Instance.PublishMessageQueue();
        }
    }
}
