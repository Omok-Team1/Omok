using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCoroutine : Singleton<StaticCoroutine>
{
    new void Awake()
    {
        base.Awake();
        
        // DontDestroyOnLoad 설정 추가 (기존 기능에 영향 없음)
        if (transform.parent == null) // 최상위 오브젝트인 경우에만 적용
        {
            DontDestroyOnLoad(gameObject);
        }
        
        SceneLoader.OnAnySceneLoadedStarts += () =>
        {
            if (_staticCoroutine is not null) 
            {
                StopCoroutine(_staticCoroutine);
                _staticCoroutine = null;
            }
        };
    }

    private IEnumerator DoCoroutine(IEnumerator coroutine)
    {
        if (_staticCoroutine is not null)
        {
            Debug.LogWarning("이전에 실행하던 Static 코루틴을 정지하고 새로운 코루틴을 실행합니다.");
            StopCoroutine(_staticCoroutine);
        }
        
        yield return _staticCoroutine = StartCoroutine(coroutine);
    }

    public static Coroutine StartStaticCoroutine(IEnumerator coroutine)
    {
        // 인스턴스가 없을 경우 자동 생성 (기존 동작 보장)
        if (Instance == null)
        {
            GameObject obj = new GameObject("StaticCoroutine");
            Instance = obj.AddComponent<StaticCoroutine>();
            DontDestroyOnLoad(obj);
        }
        return Instance.StartCoroutine(Instance.DoCoroutine(coroutine));
    }
    
    private Coroutine _staticCoroutine;
}