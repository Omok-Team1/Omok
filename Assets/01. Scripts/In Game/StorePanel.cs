using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System;

public class StorePanel : SubUICanvas
{
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Ease slideEase = Ease.OutBack;
    [SerializeField] private Canvas targetParentCanvas;
    
    private List<GameObject> _alwaysActiveObjects = new List<GameObject>();
    private RectTransform _rectTransform;
    private Vector2 _originalPosition;
    private bool _isAnimating;
    private Tween _currentTween;
    private Canvas _parentCanvas; // 부모 캔버스 추적

    public bool IsAnimating => _isAnimating;

    // 항상 활성화 상태를 유지할 오브젝트 등록 메서드
    public void RegisterAlwaysActiveObject(GameObject obj)
    {
        if (obj != null && !_alwaysActiveObjects.Contains(obj))
        {
            _alwaysActiveObjects.Add(obj);
        }
    }

    protected void Awake()
    {
        // base.Awake(); // 부모 클래스의 Awake 호출
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
        _rectTransform.anchoredPosition = new Vector2(Screen.width, _originalPosition.y);

        // 부모 캔버스 저장
        _parentCanvas = GetComponentInParent<Canvas>();
    }

    public override void Show()
    {
        // 부모 캔버스 활성화
        if (targetParentCanvas != null && !targetParentCanvas.gameObject.activeSelf)
        {
            targetParentCanvas.gameObject.SetActive(true);
        }
        else if (_parentCanvas != null && !_parentCanvas.gameObject.activeSelf)
        {
            _parentCanvas.gameObject.SetActive(true);
        }

        // 항상 활성화 상태를 유지해야 할 오브젝트들 활성화
        foreach (var obj in _alwaysActiveObjects)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }

        _currentTween?.Kill();
        _isAnimating = true;

        _currentTween = _rectTransform.DOAnchorPosX(_originalPosition.x, slideDuration)
            .SetEase(slideEase)
            .OnComplete(() => _isAnimating = false);

        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        if (_isAnimating || _rectTransform == null) return;
        if (!gameObject.activeSelf) return;

        _currentTween?.Kill();
        _isAnimating = true;

        _currentTween = _rectTransform.DOAnchorPosX(Screen.width, slideDuration)
            .SetEase(slideEase)
            .OnComplete(() =>
            {
                _isAnimating = false;
                gameObject.SetActive(false); // 패널 명시적으로 비활성화
    
                // 항상 활성화된 상태로 유지할 오브젝트들 처리
                foreach (var obj in _alwaysActiveObjects)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }
    
                // 예외 처리 없이 직접 호출
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.CloseChildrenCanvas();
                }
            })
            .OnKill(() =>
            {
                gameObject.SetActive(false);
                _isAnimating = false;
            });
    }

    // 애니메이션 완료 후 UIManager.CloseChildrenCanvas 호출
    public void HideAndCloseUI()
    {
        if (_isAnimating || _rectTransform == null) return;
        if (!gameObject.activeSelf) return;

        _currentTween?.Kill();
        _isAnimating = true;

        _currentTween = _rectTransform.DOAnchorPosX(Screen.width, slideDuration)
            .SetEase(slideEase)
            .OnComplete(() =>
            {
                _isAnimating = false;
                gameObject.SetActive(false); // 패널 명시적으로 비활성화
                
                // 항상 활성화된 상태로 유지할 오브젝트들 처리
                foreach (var obj in _alwaysActiveObjects)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);
                    }
                }
                
                try
                {
                    // UI 스택 관리 체계와 별개로 게임오브젝트 상태 명시적 제어
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.CloseChildrenCanvas();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"UI 스택 처리 중 오류 발생: {e.Message}");
                    // 오류가 발생해도 패널은 명시적으로 비활성화 상태 유지
                }
            })
            .OnKill(() => { 
                _isAnimating = false;
                gameObject.SetActive(false); // 트윈이 중단되더라도 패널 비활성화
            });
    }
    // UIManager가 애니메이션이 끝날 때까지 기다리도록 함
    public IEnumerator WaitForAnimation(UIManager uiManager)
    {
        while (_isAnimating)
        {
            yield return null; // 애니메이션이 끝날 때까지 대기
        }

        uiManager.CloseChildrenCanvas(); // 애니메이션 완료 후 다시 실행
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
    }
}