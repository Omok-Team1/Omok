using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileImageSelection : MonoBehaviour
{
    void Start()
    {
        selected = 0;
    }
 
    static readonly string[] filename = { "profile.png", "go.png", "go_1.png", "girl.png", "woman.png", "go_2.png" };
    int selected = 0;
    public void profileSelected(bool isOn){ if (isOn) { selected = 0; profileChanged(); } }
    public void goSelected(bool isOn) { if (isOn) { selected = 1; profileChanged(); } }
    public void go_1Selected(bool isOn) { if (isOn) { selected = 2; profileChanged(); } }
    public void girlSelected(bool isOn) { if (isOn) { selected = 3; profileChanged(); } }
    public void womanSelected(bool isOn) { if (isOn) { selected = 4; profileChanged(); } }
    public void go_2Selected(bool isOn) { if (isOn) { selected = 5; profileChanged(); } }

    public void profileChanged()
    {
        var message = new EventMessage("ProfileChanged");

        message.AddParameter<string>(filename[selected]);

        EventManager.Instance.PushEventMessageEvent(message);
        EventManager.Instance.PublishMessageQueue();
    }
}
