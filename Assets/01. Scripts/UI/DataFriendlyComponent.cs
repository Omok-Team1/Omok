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

    /// <summary>
    /// 인스펙터를 통해 전달 받은 Event Method를 실행합니다.
    /// </summary>
    public virtual void EventMethod()
    {
        EventManager.Instance.AddListener(eventName, eventMethod, gameObject);
    }
    
    [Header("Only UICanvas Object")]
    [Tooltip("필수로 채워야하는 값이 아닙니다.")]
    [SerializeField] protected List<GameObject> childrenObject;

    [Header("수신 받을 Event")]
    [Tooltip("필수로 채워야하는 값이 아닙니다.")]
    [SerializeField] private string eventName;
    
    [Header("Event Name 수신시 실행할 메소드")]
    [Tooltip("필수로 채워야하는 값이 아닙니다.")]
    [SerializeField] private IOnEventSO eventMethod;
    
    protected List<IUIComponent> childrenComponent = new();
}
