using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListenEventText : DataFriendlyComponent
{
    public override void Init()
    {
        if (TryGetComponent(out _text))
        {
            EventMethod();
        }
        else
        {
            throw new Exception("TextMeshProUGUI is missing.");
        }
    }
    
    private TextMeshProUGUI _text;
}
