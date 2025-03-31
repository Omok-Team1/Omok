using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class InGameInitializer : MonoBehaviour
{
    [System.Serializable]
    public class ExcludedEntry
    {
        public GameObject targetObject;
        public string eventName;
        public bool excludeObject;
        public bool excludeEvent;
    }

    [Header("자동 수집 설정")]
    [SerializeField] private bool autoCollectOnAwake = true;
    [SerializeField] private bool includeInactiveObjects = false;

    [Header("제외 목록 설정")]
    [SerializeField] private List<ExcludedEntry> exclusionList = new List<ExcludedEntry>();

    [Header("초기화 설정")]
    [SerializeField] private bool clearAllListeners = false;

    private List<GameObject> allSceneObjects = new List<GameObject>();
    private List<string> allEventNames = new List<string>();

    private void Awake()
    {
        if (autoCollectOnAwake)
        {
            CollectSceneObjects();
            CollectAllEvents();
        }

        InitializeEventSystem();
    }

    private void CollectSceneObjects()
    {
        allSceneObjects.Clear();
        GameObject[] objects = includeInactiveObjects ? 
            Resources.FindObjectsOfTypeAll<GameObject>() : 
            FindObjectsOfType<GameObject>(true);

        foreach (var obj in objects)
        {
            if (obj.scene == gameObject.scene && !EditorUtility.IsPersistent(obj))
            {
                allSceneObjects.Add(obj);
            }
        }
    }

    private void CollectAllEvents()
    {
        allEventNames.Clear();
        var eventComponents = FindObjectsOfType<MonoBehaviour>(true).OfType<IEventContainer>();
        foreach (var component in eventComponents)
        {
            allEventNames.AddRange(component.GetEventNames());
        }
        allEventNames = allEventNames.Distinct().ToList();
    }

    private void InitializeEventSystem()
    {
        // 기존 싱글톤 초기화 로직 유지
        if (GameEndEventDispatcher.Instance != null)
        {
            GameEndEventDispatcher.Instance.ClearEventListeners();
            GameEndEventDispatcher.Instance.ResetDispatchState();
        }

        if (EventManager.Instance != null)
        {
            EventManager.Instance.ClearEventQueue();

            if (clearAllListeners)
            {
                EventManager.Instance.ClearAllListeners();
            }
            else
            {
                ApplyFilteredObjects();
                ApplyFilteredEvents();
            }
        }
    }

    private void ApplyFilteredObjects()
    {
        var objectsToClear = allSceneObjects.Where(obj => 
            !exclusionList.Any(e => e.excludeObject && e.targetObject == obj)).ToList();
        EventManager.Instance.ClearListenersForGameObjects(objectsToClear);
    }

    private void ApplyFilteredEvents()
    {
        var eventsToClear = allEventNames.Where(e => 
            !exclusionList.Any(ex => ex.excludeEvent && ex.eventName == e)).ToList();
        EventManager.Instance.ClearListenersForEvents(eventsToClear);
    }

    // 에디터 전용 기능
#if UNITY_EDITOR
    [CustomEditor(typeof(InGameInitializer))]
    public class InGameInitializerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InGameInitializer initializer = (InGameInitializer)target;

            if (GUILayout.Button("씬 오브젝트 새로고침"))
            {
                initializer.CollectSceneObjects();
                initializer.CollectAllEvents();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("제외 목록 관리", EditorStyles.boldLabel);

            for (int i = 0; i < initializer.exclusionList.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                initializer.exclusionList[i].targetObject = (GameObject)EditorGUILayout.ObjectField(
                    "Target Object", 
                    initializer.exclusionList[i].targetObject, 
                    typeof(GameObject), 
                    true);

                initializer.exclusionList[i].eventName = EditorGUILayout.TextField(
                    "Event Name", 
                    initializer.exclusionList[i].eventName);

                initializer.exclusionList[i].excludeObject = EditorGUILayout.Toggle(
                    "Exclude Object", 
                    initializer.exclusionList[i].excludeObject);

                initializer.exclusionList[i].excludeEvent = EditorGUILayout.Toggle(
                    "Exclude Event", 
                    initializer.exclusionList[i].excludeEvent);

                if (GUILayout.Button("제거"))
                {
                    initializer.exclusionList.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("새 제외 항목 추가"))
            {
                initializer.exclusionList.Add(new ExcludedEntry());
            }
        }
    }
#endif
}

public interface IEventContainer
{
    IEnumerable<string> GetEventNames();
}