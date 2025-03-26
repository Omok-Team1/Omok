using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IconWobbleAnime : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("최대 회전 각도")]
    [Range(1f, 45f)]
    public float maxRotationAngle = 15f;
    
    [Tooltip("애니메이션 지속 시간 (초)")]
    [Range(0.1f, 2f)]
    public float duration = 0.5f;
    
    [Tooltip("좌우 흔들림 횟수")]
    [Range(1, 10)]
    public int vibrato = 4;
    
    [Tooltip("흔들림 무작위성 (0=규칙적, 1=무작위)")]
    [Range(0f, 1f)]
    public float randomness = 0.2f;
    
    [Tooltip("애니메이션 시작 지연 시간 (초)")]
    [Range(0f, 3f)]
    public float delay = 0f;
    
    [Tooltip("애니메이션 페이드아웃 여부")]
    public bool fadeOut = true;
    
    [Tooltip("애니메이션 반복 횟수 (0=무한반복)")]
    [Range(0, 10)]
    public int loops = 1;
    
    [Tooltip("애니메이션 시작 시 약간 확대 효과 사용")]
    public bool useScaleEffect = true;
    
    [Tooltip("확대 효과 크기 (원래 크기의 배수)")]
    [Range(1f, 1.5f)]
    public float scaleFactor = 1.2f;
    
    [Header("Activation Settings")]
    [Tooltip("부모 객체가 활성화될 때 애니메이션 재생")]
    public bool playOnParentEnabled = true;
    
    [Tooltip("활성화 감지 방식 선택")]
    public DetectionMethod detectionMethod = DetectionMethod.ParentActive;
    
    public enum DetectionMethod
    {
        ParentActive,    // 부모 객체의 활성화 상태로 감지
        SelfActive,      // 자기 자신의 활성화 상태로 감지
        Visibility       // 가시성으로 감지
    }

    private Vector3 originalScale;
    private Tween rotateTween;
    private Tween scaleTween;
    private RectTransform rectTransform;
    private Image image;
    private Canvas parentCanvas;
    private GameObject previousParent;
    private bool hasInitialized = false;
    private bool hasPlayed = false;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        
        // 부모 캔버스 찾기
        Transform parent = transform;
        while (parent != null && parentCanvas == null)
        {
            parentCanvas = parent.GetComponent<Canvas>();
            parent = parent.parent;
        }
        
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform 컴포넌트가 필요합니다.");
            enabled = false;
            return;
        }
        
        originalScale = rectTransform.localScale;
        previousParent = transform.parent.gameObject;
        hasInitialized = true;
    }
    
    void OnEnable()
    {
        if (!hasInitialized) return;
        
        // 초기화 시 애니메이션은 재생하지 않고 상태만 리셋
        KillTweens();
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = originalScale;
        
        // 첫 활성화시에는 애니메이션을 바로 재생하지 않고, Update에서 체크
        // 이를 통해 부모 패널이 활성화될 때까지 기다림
        hasPlayed = false;
        
        // 첫 프레임에 패널 전환을 감지하기 위해 딜레이 후 부모 확인
        Invoke("CheckParentChange", 0.1f);
    }
    
    void OnDisable()
    {
        if (!hasInitialized) return;
        
        KillTweens();
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = originalScale;
    }
    
    void CheckParentChange()
    {
        if (previousParent != transform.parent.gameObject)
        {
            previousParent = transform.parent.gameObject;
            // 부모가 변경되면 애니메이션 상태 리셋
            hasPlayed = false;
        }
    }
    
    void Update()
    {
        if (!hasInitialized) return;
        
        bool shouldPlay = false;
        
        switch (detectionMethod)
        {
            case DetectionMethod.ParentActive:
                // 부모 객체가 활성화된 경우 (패널 전환 시)
                if (transform.parent != null && transform.parent.gameObject.activeInHierarchy && !hasPlayed)
                {
                    shouldPlay = true;
                }
                break;
                
            case DetectionMethod.SelfActive:
                // 자기 자신이 활성화된 경우
                if (gameObject.activeInHierarchy && !hasPlayed)
                {
                    shouldPlay = true;
                }
                break;
                
            case DetectionMethod.Visibility:
                // 가시성으로 감지 (부모 Canvas의 활성화 여부 등)
                bool isVisible = IsVisibleInHierarchy();
                if (isVisible && !hasPlayed)
                {
                    shouldPlay = true;
                }
                break;
        }
        
        if (shouldPlay && playOnParentEnabled)
        {
            PlayWobbleAnimation();
            hasPlayed = true;
        }
    }
    
    bool IsVisibleInHierarchy()
    {
        // 오브젝트 자체가 비활성화 되어 있으면 false
        if (!gameObject.activeInHierarchy) return false;
        
        // 이미지가 없거나 비활성화되어 있으면 false
        if (image != null && !image.enabled) return false;
        
        // 부모를 따라가면서 모든 캔버스 그룹이나 이미지의 알파값 확인
        Transform current = transform;
        while (current != null)
        {
            // CanvasGroup의 알파값 확인
            CanvasGroup canvasGroup = current.GetComponent<CanvasGroup>();
            if (canvasGroup != null && canvasGroup.alpha < 0.01f)
                return false;
            
            // Image의 알파값 확인
            Image img = current.GetComponent<Image>();
            if (img != null && img.color.a < 0.01f)
                return false;
                
            // 부모 객체가 비활성화 되어 있으면 false
            if (!current.gameObject.activeInHierarchy)
                return false;
                
            current = current.parent;
        }
        
        return true;
    }
    
    // 모든 Tween 객체를 제거하는 헬퍼 메서드
    void KillTweens()
    {
        if (rotateTween != null && rotateTween.IsActive())
            rotateTween.Kill();
        if (scaleTween != null && scaleTween.IsActive())
            scaleTween.Kill();
    }
    
    public void PlayWobbleAnimation()
    {
        if (!hasInitialized || !gameObject.activeInHierarchy) return;
        
        // 이미 재생 중인 Tween이 있으면 제거
        KillTweens();
        
        // 지연 시간 후 애니메이션 시작
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(delay);
        
        // 확대 효과 설정
        if (useScaleEffect)
        {
            // 처음엔 작게 시작
            rectTransform.localScale = originalScale * 0.5f;
            
            // 원래 크기보다 약간 커졌다가 원래 크기로 돌아오는 애니메이션
            scaleTween = rectTransform.DOScale(originalScale * scaleFactor, duration * 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    rectTransform.DOScale(originalScale, duration * 0.3f).SetEase(Ease.InOutQuad);
                });
            
            sequence.Append(scaleTween);
        }
        
        // 회전 애니메이션 설정
        rotateTween = rectTransform.DOShakeRotation(duration, new Vector3(0, 0, maxRotationAngle), vibrato, randomness, fadeOut)
            .SetLoops(loops, LoopType.Restart)
            .SetEase(Ease.OutElastic);
            
        sequence.Append(rotateTween);
        
        // 시퀀스 실행
        sequence.Play();
    }
    
    // 부모 패널이 활성화될 때 호출하기 위한 공개 메서드
    public void OnParentPanelEnabled()
    {
        hasPlayed = false;
    }
    
    // 애니메이션을 강제로 재생하기 위한 공개 메서드
    public void ForcePlayAnimation()
    {
        PlayWobbleAnimation();
        hasPlayed = true;
    }
    
    // 인스펙터에서 애니메이션 테스트를 위한 버튼
    [ContextMenu("Test Wobble Animation")]
    public void TestWobbleAnimation()
    {
        // 현재 설정된 값으로 애니메이션 테스트
        PlayWobbleAnimation();
    }
    
    // 애니메이션 상태를 리셋하는 메서드
    [ContextMenu("Reset Animation State")]
    public void ResetAnimationState()
    {
        hasPlayed = false;
        KillTweens();
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = originalScale;
    }
}