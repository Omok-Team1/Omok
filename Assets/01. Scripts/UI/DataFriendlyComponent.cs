using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataFriendlyComponent : MonoBehaviour, IUIComponent
{
    public abstract void Init();

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 해당 데이터와 상호작용하는 Canvas들의 집합을 반환, 자식이 없으면 빈 List를 반환함
    /// </summary>
    /// <returns></returns>
    public virtual List<IUIComponent> GetChildren()
    {
        return new List<IUIComponent>();
    }
    
    public virtual void CastingChildren()
    {
        if (childrenObject is not null && childrenObject.Count > 0)
        {
            foreach (var component in childrenObject)
            {
                childrenComponent.Add(component.GetComponent<IUIComponent>()); 
            }
        }
    }

    public abstract void OnChangedDataEvent(GameData data);
    
    [Header("Only UICanvas Object")]
    [SerializeField] protected List<GameObject> childrenObject;
    
    protected List<IUIComponent> childrenComponent = new();
}
