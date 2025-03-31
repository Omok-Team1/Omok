using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ProfileChangedOnEvent", menuName = "IOnEventSO/ProfileChangedOnEvent")]
public class ProfileChangedOnEvent : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        string filename = msg.GetParameter<string>();
        TempNetworkManager.Instance.RequestProfilePic(filename);
    }
}
