using UnityEngine;
using UnityEngine.UI;

public class ScreenManagerSetup : MonoBehaviour
{
    [SerializeField] private Button fullscreenToggleButton;
    [SerializeField] private RawImage backgroundImage;
    
    [Header("Materials")]
    [SerializeField] private Material backgroundMaterial;
    
    void Start()
    {
        if (fullscreenToggleButton != null)
        {
            fullscreenToggleButton.onClick.AddListener(() => {
                ScreenManager.Instance.ToggleFullscreen();
            });
        }
        
        if (backgroundImage != null && backgroundMaterial != null)
        {
            backgroundImage.material = backgroundMaterial;
        }
    }
}