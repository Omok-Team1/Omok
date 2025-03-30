using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// 새로운 Scene으로 전환할 때 RootCanvas는 해당 함수를 호출하여 현재 Scene의 Canvas들을 추적한다.
    /// </summary>
    /// <param name="rootCanvas"></param>
    public void RegisterRootCanvas(IUIComponent rootCanvas)
    {
        _rootCanvas = rootCanvas as RootUICanvas;
        _canvasTrace.Push(new List<IUIComponent> { rootCanvas });
    }

    /// <summary>
    /// Scene 전환 시 현재 Scene의 모든 Canvas들을 비워야 다음 Scene의 Root Canvas가 Stack의 가장 아래 위치
    /// </summary>
    /// <param name="rootCanvas"></param>
    public void UnregisterRootCanvas(IUIComponent rootCanvas)
    {
        while(_canvasTrace.Count > 0) _canvasTrace.Pop();
    }
    
    /// <summary>
    /// 현재 Canvas의 모든 Children Canvas를 활성화 합니다.
    /// </summary>
    /// <param name="iuiComponent"></param>
    /// <param name="isThisCanvasHide"></param>
    public void OpenChildrenCanvas(IUIComponent iuiComponent, bool isThisCanvasHide = false)
    {
        List<IUIComponent> thisCanvas = _canvasTrace.Peek();
        List<IUIComponent> nextCanvasList = iuiComponent.GetChildren();

        _canvasTrace.Push(nextCanvasList);

        // Profile Canvas가 포함된 경우 숨기지 않도록 예외처리
        bool isProfileCanvas = thisCanvas.Any(canvas => canvas is RootUICanvas);

        if (isThisCanvasHide && !isProfileCanvas)
        {
            foreach (var subCanvas in thisCanvas)
            {
                subCanvas.Hide(); // Profile Canvas가 아닐 때만 숨김
            }
        }
    
        foreach (var nextCanvas in nextCanvasList)
        {
            nextCanvas.Show();
        }
    }

    /// <summary>
    /// 파라미터로 전달 받은 childCoponent(= Child Canvas)를 활성화 합니다.
    /// </summary>
    /// <param name="currentCanvas">현재 활성화 되어 있는 캔버스 "반드시 this를 전달해야 합니다."</param>
    /// <param name="childComponent">활성화 할 자식 캔버스</param>
    /// <param name="isThisCanvasHide"></param>
    public void OpenTargetChildCanvas(IUIComponent currentCanvas, IUIComponent childComponent, bool isThisCanvasHide = false)
    {
        if (isThisCanvasHide)
        {
            foreach (var subCanvas in currentCanvas.GetChildren())
            {
                subCanvas.Hide();
            }
        }
        
        _canvasTrace.Push(childComponent.GetChildren());
        
        foreach (var nextCanvas in _canvasTrace.Peek())
        {
            nextCanvas.Show();
        }
    }

    /// <summary>
    /// 현재 UICanvas의 부모 UICanvas를 활성화 시킴
    /// </summary>
    public void CloseChildrenCanvas()
    {
        if (_canvasTrace.Count <= 1)
        {
            Debug.LogWarning("UI 스택에 닫을 캔버스가 없습니다.");
            return;
        }

        var currentCanvas = _canvasTrace.Pop();
        bool foundActiveCanvas = false;

        foreach (var subCanvas in currentCanvas)
        {
            subCanvas.Hide();
            foundActiveCanvas = true;
        }

        if (_canvasTrace.Count > 0 && foundActiveCanvas)
        {
            foreach (var nextCanvas in _canvasTrace.Peek())
            {
                nextCanvas.Show();
            }
        }
    }
    
    /// <summary>
    /// Root Canvas를 제외한 지금까지 열렸던 Root의 모든 Children Canvas들을 닫고 Root Canvas로 돌아갑니다.
    /// </summary>
    public void CloseAllChildrenCanvas()
    {
        while (_canvasTrace.Count > 1)
        {
            var canvas = _canvasTrace.Pop();
            
            foreach (var subCanvas in canvas)
            {
                subCanvas.Hide();
            }
        
            foreach (var nextCanvas in _canvasTrace.Peek())
            {
                nextCanvas.Show();
            }
        }
    }
    
    /// <summary>
    /// 다른 Manager, Controller와 협업하기 위한 Method.
    /// </summary>
    /// <param name="command">해당 이벤트를 처리하기 위한 Command</param>
    public void ExecuteEvent(ICommand command)
    {
        command?.Execute();
    }

    /// <summary>
    /// 최상위의 UI Canvas
    /// </summary>
    private RootUICanvas _rootCanvas;
    
    private readonly Stack<List<IUIComponent>> _canvasTrace = new();
    
    //이벤트 처리를 위해 분리된 IOnEventSO에서 사용하기 위한, 이벤트를 발생시킨 주체에 대한 정보
    public IUIComponent triggeredEventUIComponent;
    
    /// <summary>
    /// ShopTotalPanel을 열 때 ProfileCanvas를 _canvasTrace에 추가
    /// </summary>
    public void OpenShopPanel()
    {
        // ProfileCanvas가 이미 스택에 있는지 확인
        if (_canvasTrace.Count == 0)
        {
            var profileCanvas = FindObjectOfType<RootUICanvas>();
            if (profileCanvas != null)
            {
                _canvasTrace.Push(new List<IUIComponent> { profileCanvas });
            }
        }
    
        // ShopTotalPanel 열기
        var shopPanel = FindObjectOfType<StorePanel>();
        if (shopPanel != null)
        {
            _canvasTrace.Push(new List<IUIComponent> { shopPanel });
            shopPanel.Show();
        }
    }
}