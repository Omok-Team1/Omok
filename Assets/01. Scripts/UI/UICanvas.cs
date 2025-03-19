using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public abstract class UICanvas : MonoBehaviour, IUIComponent
{
    /* 원래코드
    public virtual void Init()
    {
        _children = GetComponentsInChildren<IUIComponent>().
            Where(c => !ReferenceEquals(c, this)).ToList();
        
        foreach (var child in _children)
        {
            child.Init();
        }
    }
    */
    
    public virtual void Init()
    {
        // 자신을 제외한 모든 자식 컴포넌트 가져오기
        _children = GetComponentsInChildren<IUIComponent>(true)
            .Where(c => c != (IUIComponent)this)
            .ToList();

        // 중복 초기화 방지
        foreach (var child in _children)
        {
            if (child is MonoBehaviour childMono && childMono.gameObject.activeInHierarchy)
            {
                child.Init();
            }
        }
    }

    public virtual void Show() // virtual 추가
    {
        gameObject.SetActive(true);
        
        foreach (var child in _children)
        {
            child.Show();
        }
    }

    public virtual void Hide() // virtual 추가
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
}