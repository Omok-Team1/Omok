using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;

                if (_instance is null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    _instance = obj.AddComponent<T>();
                }
            }
            
            return _instance;
        }

        protected set => _instance = value;
    }

    public virtual void Awake()
    {
        if (_instance is null)
        {
            _instance = this as T;
            DontDestroyOnLoad(_instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private static T _instance;
}
