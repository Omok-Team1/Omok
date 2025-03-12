using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResponseLogInOnEvent", menuName = "IOnEventSO/ResponseLogInOnEvent")]
public class ResponseLogInOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        resultCanvas.SetActive(true);

        //KeyValueException!! Just For Test
        Debug.Log(msg.GetParameter<int>());
    }

    [SerializeField] private GameObject resultCanvas;
}
