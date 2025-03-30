using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VolumeControl : MonoBehaviour, IPointerClickHandler
{
    public Image[] volumeBars;  // 볼륨 막대 배열 (Inspector에서 설정)
    public int maxVolume = 5;  // 최대 볼륨 단계 (5단계)
    private int currentVolume = 3;  // 현재 볼륨 단계 (초기값 3)
    private bool increasing = true; // 증가 방향인지 여부

    public Button volumeKeyButton; // 투명한 볼륨 키 버튼 (Inspector에서 할당)

    private void Start()
    {
        UpdateVolumeUI(); // UI 초기 설정
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeVolume(); // 클릭 시 볼륨 변경
    }

    private void ChangeVolume()
    {
        if (increasing)
        {
            currentVolume++; // 볼륨 증가
            if (currentVolume >= maxVolume)
            {
                currentVolume = maxVolume;
                increasing = false; // 최대 볼륨이면 감소로 전환
            }
        }
        else
        {
            currentVolume--; // 볼륨 감소
            if (currentVolume <= 0)
            {
                currentVolume = 0;
                increasing = true; // 최소 볼륨이면 증가로 전환
            }
        }

        UpdateVolumeUI();
    }

    private void UpdateVolumeUI()
    {
        for (int i = 0; i < volumeBars.Length; i++)
        {
            volumeBars[i].enabled = (i < currentVolume);  // 현재 볼륨 값에 맞게 활성화/비활성화
        }

        AudioListener.volume = (float)currentVolume / maxVolume;  // 실제 볼륨 적용
    }
}