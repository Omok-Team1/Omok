using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubUICanvas : UICanvas
{
    
    protected virtual void Awake()
    {
        // 자식 클래스에서 오버라이드 가능하도록 기본 구조만 제공
    }

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

        Hide(); // 추가: UI가 초기화될 때 숨겨지도록 함
    }
}