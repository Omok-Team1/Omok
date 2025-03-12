using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[
    RequireComponent(typeof(Button))
]
public class SwitchSceneButton : UserFriendlyComponent
{
    public override void Init()
    {
        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(EventMethod);
        }
    }
    
    public override void EventMethod()
    {
        targetScene.Load();
    }

    [SerializeField] private SceneId targetScene;
}
