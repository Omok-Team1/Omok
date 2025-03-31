using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceInitialResolution : MonoBehaviour
{
    [SerializeField] private int width = 720;  // 원하는 너비
    [SerializeField] private int height = 1280; // 원하는 높이
    [SerializeField] private bool isFullScreen = false;

    void Awake()
    {
        // 해상도 강제 설정 + 전체화면 여부
        Screen.SetResolution(width, height, isFullScreen);

        // 추가: 프레임 제한 (옵션)
        Application.targetFrameRate = 60;
    }
}