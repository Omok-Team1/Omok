using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenManager : MonoBehaviour
{
    [Header("해상도 설정")]
    [SerializeField] private Vector2 targetResolution = new Vector2(1280, 720);
    
    [Header("배경 애니메이션 설정")]
    [SerializeField] private RawImage leftBackground;
    [SerializeField] private RawImage rightBackground;
    [SerializeField] private float scrollSpeed = 0.5f;
    
    [Header("UI 참조")]
    [SerializeField] private Button fullscreenToggleButton;
    
    private bool isFullscreen = false;
    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;
    
    private void Start()
    {
        // 사용 가능한 해상도 가져오기
        resolutions = Screen.resolutions;
        
        // 초기 창 모드 설정
        SetWindowedMode();
        
        // 버튼 이벤트 연결
        if (fullscreenToggleButton != null)
        {
            fullscreenToggleButton.onClick.AddListener(ToggleFullscreen);
        }
    }
    
    private void Update()
    {
        // 배경 텍스처 스크롤링 효과
        if (leftBackground != null && rightBackground != null)
        {
            Vector2 uvOffset = new Vector2(Time.time * scrollSpeed, -Time.time * scrollSpeed);
            leftBackground.uvRect = new Rect(uvOffset, Vector2.one);
            rightBackground.uvRect = new Rect(uvOffset, Vector2.one);
        }
    }
    
    // 창 모드 설정
    private void SetWindowedMode()
    {
        Screen.SetResolution((int)targetResolution.x, (int)targetResolution.y, false);
        isFullscreen = false;
        
        // 배경 비활성화
        SetBackgroundsActive(false);
    }
    
    // 전체화면 모드 설정
    private void SetFullscreenMode()
    {
        // 현재 화면에 적합한 해상도 찾기 (예: 1920x1080)
        int bestResIndex = FindBestResolutionIndex();
        
        // 전체화면으로 전환
        Screen.SetResolution(
            resolutions[bestResIndex].width, 
            resolutions[bestResIndex].height, 
            true
        );
        
        isFullscreen = true;
        
        // 배경 활성화
        SetBackgroundsActive(true);
        
        // 레터박스 설정
        ConfigureLetterboxing();
    }
    
    // 전체화면 토글
    public void ToggleFullscreen()
    {
        if (isFullscreen)
        {
            SetWindowedMode();
        }
        else
        {
            SetFullscreenMode();
        }
    }
    
    // 현재 화면에 적합한 해상도 인덱스 찾기
    private int FindBestResolutionIndex()
    {
        int bestResIndex = 0;
        
        // 기본값으로 가장 큰 해상도 사용
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width >= resolutions[bestResIndex].width &&
                resolutions[i].height >= resolutions[bestResIndex].height)
            {
                bestResIndex = i;
            }
        }
        
        return bestResIndex;
    }
    
    // 레터박스 설정 (화면 비율 유지를 위한 설정)
    private void ConfigureLetterboxing()
    {
        // 카메라에 레터박스 설정
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float targetAspect = targetResolution.x / targetResolution.y;
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float scaleHeight = screenAspect / targetAspect;
            
            if (scaleHeight < 1.0f)
            {
                Rect rect = mainCamera.rect;
                
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
                
                mainCamera.rect = rect;
            }
            else
            {
                float scaleWidth = 1.0f / scaleHeight;
                
                Rect rect = mainCamera.rect;
                
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;
                
                mainCamera.rect = rect;
            }
        }
    }
    
    // 배경 이미지 활성화/비활성화
    private void SetBackgroundsActive(bool active)
    {
        if (leftBackground != null)
            leftBackground.gameObject.SetActive(active);
            
        if (rightBackground != null)
            rightBackground.gameObject.SetActive(active);
    }
}