using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public abstract class UICanvas : MonoBehaviour, IUIComponent
{
    public virtual void Init()
    {
        _children = GetComponentsInChildren<IUIComponent>(true)
            .Where(c => c != (IUIComponent)this)
            .ToList();

        if(String.IsNullOrEmpty(eventName) is false && uiEvent is not null)
            EventManager.Instance.AddListener(eventName, uiEvent, gameObject);
        
        foreach (var child in _children)
        {
            if (child is MonoBehaviour childMono && childMono.gameObject.activeInHierarchy)
            {
                child.Init();
            }
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        
        foreach (var child in _children)
        {
            child.Show();
        }
    }

    public virtual void Hide()
    {
        foreach (var child in _children)
        {
            child.Hide();
        }
        
        gameObject.SetActive(false);
    }

    public List<IUIComponent> GetChildren()
    {
        return _children;
    }
    
    protected List<IUIComponent> _children = new();
    
    [SerializeField] private string eventName;
    [SerializeField] private IOnEventSO uiEvent;
}