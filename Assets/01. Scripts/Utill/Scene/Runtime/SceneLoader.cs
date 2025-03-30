using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static event Action OnAnySceneLoadedStarts;
    public static event Action OnAnySceneLoadedFinished;
    public static event Action<SceneId> OnSceneLoadingStarts;
    public static event Action<SceneId> OnSceneLoadingFinished;
    
    private static TaskQueue taskQueue;
    public static SceneId CurrentLoadingScene { get; private set; } = SceneId.Unknown;

    public static void OnLoadComplete(Scene scene, LoadSceneMode _)
    {
        SceneId sceneId = BI.NAME_TO_ID[scene.name];
        
        OnSceneLoadingFinished?.Invoke(sceneId);
        OnAnySceneLoadedFinished?.Invoke();

        if (CurrentLoadingScene is not SceneId.Unknown && sceneId == CurrentLoadingScene)
        {
            CurrentLoadingScene = SceneId.Unknown;
            taskQueue.DoneTask();
        }
    }
    public static void Load(this SceneId id, LoadSceneMode mode = LoadSceneMode.Single, bool isAsync = false)
    {
        if (taskQueue is null)
        {
            taskQueue = new TaskQueue();
            SceneManager.sceneLoaded += OnLoadComplete;
        }
        
        taskQueue.BeginTask(() => TaskLoad(id, mode, isAsync));
    }

    private static void TaskLoad(SceneId id, LoadSceneMode mode, bool isAsync)
    {
        string sceneName = BI.ID_TO_NAME[id];

        if (sceneName is null)
        {
            throw new NullReferenceException("Scene not found");
        }

        CurrentLoadingScene = id;
        OnAnySceneLoadedStarts?.Invoke();
        OnSceneLoadingStarts?.Invoke(id);

        if (isAsync) SceneManager.LoadSceneAsync(sceneName, mode);
        else SceneManager.LoadScene(sceneName, mode);
    }
    
}
