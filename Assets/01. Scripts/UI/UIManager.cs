using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public void RegisterRootCanvas(IUIComponent rootCanvas)
    {
        _rootCanvas = rootCanvas as RootUICanvas;
        _canvasTrace.Push(new List<IUIComponent> { rootCanvas });
    }

    public void UnregisterRootCanvas(IUIComponent rootCanvas)
    {
        while(_canvasTrace.Count > 0) _canvasTrace.Pop();
    }
    
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
    
    public void ExecuteEvent(ICommand command)
    {
        command?.Execute();
    }

    private RootUICanvas _rootCanvas;
    private readonly Stack<List<IUIComponent>> _canvasTrace = new();
    
    public IUIComponent triggeredEventUIComponent;
  
    // ShopTotalPanel을 열 때 ProfileCanvas를 _canvasTrace에 추가
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
