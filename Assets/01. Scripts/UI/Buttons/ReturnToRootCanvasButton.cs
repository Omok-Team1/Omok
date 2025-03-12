using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[
    RequireComponent(typeof(Button))
]
public class ReturnToRootCanvasButton : UserFriendlyComponent
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
        UIManager.Instance.CloseAllChildrenCanvas();
    }
}
