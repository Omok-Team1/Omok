using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class RootUICanvas : UICanvas
{
    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        UIManager.Instance.RegisterRootCanvas(this);
    }

    private void OnDestroy()
    {
        UIManager.Instance.UnregisterRootCanvas(this);
    }
}