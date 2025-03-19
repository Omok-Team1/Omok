using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListenEventText : DataFriendlyComponent
{
    public override void Init()
    {
        TryGetComponent(out _textMeshPro);
    }
    
    private TextMeshPro _textMeshPro;
}
