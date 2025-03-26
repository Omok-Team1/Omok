using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListenEventComponent : DataFriendlyComponent
{
    public override void Init()
    {
        EventMethod();
    }
    
    private TextMeshProUGUI _text;
}
