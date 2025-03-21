using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCoroutine : Singleton<StaticCoroutine>
{
    new void Awake()
    {
        if(Instance is null)
            Instance = this as StaticCoroutine;
    }

    private IEnumerator DoCoroutine(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);
    }

    public static Coroutine StartStaticCoroutine(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(Instance.DoCoroutine(coroutine));
    }
}

