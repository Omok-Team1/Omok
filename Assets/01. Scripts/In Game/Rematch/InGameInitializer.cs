using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameInitializer : MonoBehaviour
{
    private void Awake()
    {
        // 싱글톤 인스턴스 강제 재설정
        if (GameEndEventDispatcher.Instance != null)
        {
            GameEndEventDispatcher.Instance.ClearEventListeners();
            GameEndEventDispatcher.Instance.ResetDispatchState();
        }

        if (EventManager.Instance != null)
        {
            EventManager.Instance.ClearEventQueue();
        }
    }
}
