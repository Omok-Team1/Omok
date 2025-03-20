using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubUICanvas : UICanvas
{
    private void Start()
    {
        Init();
    }
    
    public override void Init()
    {
        base.Init();
    
        // 모든 자식 UI를 숨기지 않음 (슬라이드 효과를 위해)
        foreach (var uiComponent in _children)
        {
            // 기본적으로 비활성화하지 않고, OpenChildrenCanvas에서 관리
        }
    }

}