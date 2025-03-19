using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[
    RequireComponent(typeof(Button))
]
public class OpenSubCanvasButton : UserFriendlyComponent
{
    public override void Init()
    {
        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(EventMethod);
        }
        
        CastingChildren();
    }

    public override void EventMethod()
    {
        UIManager.Instance.OpenChildrenCanvas(this, isThisCanvasHide);
    }

    public override List<IUIComponent> GetChildren()
    {
        return childrenComponent;
    }

    [SerializeField] private bool isThisCanvasHide = false;
}