using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        // 싱글턴 패턴을 사용하여 인스턴스를 하나만 유지
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스가 생성되지 않도록 처리
        }
    }

    // 게임 종료 이벤트 처리
    public void PublishMessageQueue()
    {
        // 이벤트 처리 코드 추가: 예를 들어 패배 처리나 승리 처리 등의 메시지를 출력할 수 있습니다.
        Debug.Log("Publishing event: A player lost due to time limit.");
        
        // 이곳에서 다른 이벤트를 발행하거나, UI 업데이트를 할 수도 있습니다.
        NotifyGameOver();
    }

    private void NotifyGameOver()
    {
        // 예시로 게임 오버 상태를 UI로 업데이트하거나, 적절한 이벤트를 발생시킬 수 있습니다.
        Debug.Log("Game over due to time limit. A player lost.");
        // 추가적으로 UI 갱신이나 승패 처리 등을 할 수 있습니다.
    }
}