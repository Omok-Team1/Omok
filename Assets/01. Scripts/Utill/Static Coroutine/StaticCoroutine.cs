using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCoroutine : Singleton<StaticCoroutine>
{
    new void Awake()
    {
        base.Awake();
        
        SceneLoader.OnAnySceneLoadedStarts += () =>
        {
            if (_staticCoroutine is not null) StopCoroutine(_staticCoroutine);
            _staticCoroutine = null;
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
        return Instance.StartCoroutine(Instance.DoCoroutine(coroutine));
    }
    
    private Coroutine _staticCoroutine;
}

