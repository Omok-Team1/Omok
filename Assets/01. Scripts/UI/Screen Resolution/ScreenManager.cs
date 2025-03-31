using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    // Singleton instance
    public static ScreenManager Instance { get; private set; }

    [Header("Resolution Settings")]
    [SerializeField] private int baseWidth = 720;
    [SerializeField] private int baseHeight = 1280;
    [SerializeField] private int fullscreenWidth = 1920;
    [SerializeField] private int fullscreenHeight = 1080;

    [Header("Background Settings")]
    [SerializeField] private Material backgroundMaterial;
    [SerializeField] private GameObject backgroundContainer;
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private float scrollSpeed = 0.5f;

    // 버튼을 비활성화하기 위한 변수 추가
    [SerializeField] private Button fullscreenButton;

    private bool _isFullscreen = false;
    private Vector2 _offset = Vector2.zero;
    private Camera _mainCamera;
    private CanvasScaler _canvasScaler;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _mainCamera = Camera.main;
        _canvasScaler = FindObjectOfType<CanvasScaler>();
        
        // Initialize with base resolution
        SetResolution(baseWidth, baseHeight, false);
    }

    private void Update()
    {
        // Update background shader offset for diagonal scrolling effect
        if (backgroundMaterial != null)
        {
            _offset.x += scrollSpeed * Time.deltaTime;
            _offset.y += scrollSpeed * Time.deltaTime;
            backgroundMaterial.SetTextureOffset("_MainTex", _offset);
        }
    }

    public void ToggleFullscreen()
    {
        // 항상 풀스크린으로만 전환되도록 수정 (이전 상태와 상관없이)
        SetResolution(fullscreenWidth, fullscreenHeight, true);
        _isFullscreen = true;
        
        UpdateBackgroundVisibility();
        
        // 버튼 비활성화
        if (fullscreenButton != null)
        {
            fullscreenButton.interactable = false;
        }
    }

    private void SetResolution(int width, int height, bool fullscreen)
    {
        Screen.SetResolution(width, height, fullscreen);
        
        if (_canvasScaler != null)
        {
            // Update the Canvas Scaler to maintain UI proportions
            _canvasScaler.referenceResolution = new Vector2(baseWidth, baseHeight);
            _canvasScaler.matchWidthOrHeight = 1f; // Match height (vertical)
        }
        
        if (_mainCamera != null)
        {
            // Set the camera viewport to maintain game view proportions
            if (fullscreen)
            {
                // Calculate the aspect ratio
                float targetAspect = (float)baseWidth / baseHeight;
                float currentAspect = (float)width / height;
                
                if (currentAspect > targetAspect)
                {
                    // Wider screen than needed - letterbox sides
                    float viewportWidth = targetAspect / currentAspect;
                    float viewportX = (1f - viewportWidth) / 2f;
                    _mainCamera.rect = new Rect(viewportX, 0, viewportWidth, 1);
                }
                else
                {
                    // Taller screen than needed - letterbox top/bottom
                    float viewportHeight = currentAspect / targetAspect;
                    float viewportY = (1f - viewportHeight) / 2f;
                    _mainCamera.rect = new Rect(0, viewportY, 1, viewportHeight);
                }
            }
            else
            {
                // Reset to full viewport when in base resolution
                _mainCamera.rect = new Rect(0, 0, 1, 1);
            }
        }
    }

    private void UpdateBackgroundVisibility()
    {
        if (backgroundContainer != null)
        {
            backgroundContainer.SetActive(_isFullscreen);
        }
    }
}