using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResponseLogInOnEvent", menuName = "IOnEventSO/ResponseLogInOnEvent")]
public class ResponseLogInOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        if (msg.GetParameter<string>() == "Success")
        {
            // TODO : DB���� �̹��� �޾ƿͼ� sprite�� ����
            //Sprite sprite;
            SceneId.Title.Load();
        }
    }
}
