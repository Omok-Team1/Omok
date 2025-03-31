using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[CreateAssetMenu(fileName = "SetFilenameOfImageLoadedEvent", menuName = "IOnEventSO/SetFilenameOfImageLoadedEvent")]
public class SetFilenameOfImageLoaded : IOnEventSO
{
    public override void OnEvent(EventMessage msg)
    {
        var text = msg.GetParameter<GameObject>().GetComponent<TextMeshProUGUI>();
        text.text = msg.GetParameter<string>();
    }
}
