using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubUICanvas : UICanvas
{
    private void Start()
    {
        Init();
    }
    
    public override void Init()
    {
        base.Init();
        
        foreach (var uiComponent in _children)
        {
            uiComponent.Hide();
        }

        Hide();
    }
}