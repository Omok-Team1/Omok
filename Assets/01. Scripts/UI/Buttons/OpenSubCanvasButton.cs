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
        if (childrenComponent == null || childrenComponent.Count == 0)
        {
            Debug.LogError($"오류: {gameObject.name}의 childrenComponent가 비어 있습니다!");
        }

        return childrenComponent;
    }


    [SerializeField] private bool isThisCanvasHide = false;
}