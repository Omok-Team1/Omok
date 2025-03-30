using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static partial class IndexRebuildController
{
    private const string INDEX_DATA_RESOURCE_NAME = "SceneIndexData";
    private const string INDEX_DATA_RESOURCE_PATH = "Assets/01. Scripts/Utill/Scene/Editor/Resources";
    private const string SCRIPT_GUID = "0c5a27969e7e0e54dabdd91d69c29ad2";
    private static SceneIndexData LoadOrCreateIndexData()
    {
        SceneIndexData sceneIndexData = Resources.Load<SceneIndexData>(INDEX_DATA_RESOURCE_NAME);

        if (sceneIndexData is null)
        {
            sceneIndexData = ScriptableObject.CreateInstance<SceneIndexData>();

            if (Directory.Exists(INDEX_DATA_RESOURCE_PATH) is false)
            {
                Directory.CreateDirectory(INDEX_DATA_RESOURCE_PATH);
                AssetDatabase.ImportAsset(INDEX_DATA_RESOURCE_PATH);
            }
            
            AssetDatabase.CreateAsset(sceneIndexData, INDEX_DATA_RESOURCE_PATH + "/" + INDEX_DATA_RESOURCE_NAME + ".asset");
            AssetDatabase.SaveAssets();
        }
        
        return sceneIndexData;
    }

    /*
     * 1. 이전에 존재하고 있었던 Scene들의 정보를 불러옴
     * 2. 현재 Build Setting에 활성화 되어 있는 Scene들을 불러옴
     * 3. (1), (2)의 중복된 Scene을 제거하고 새롭게 생긴 Scene에 새로운 Build Index를 달아준다.
     * 4. 새롭게 Enum Script를 만들어준다
     */
    [MenuItem("Scene Controll/Rebuild Index")]
    public static void RebuildIndex()
    {
        var indexData = LoadOrCreateIndexData();
        
        Dictionary<SceneAsset, SceneEntry> sceneDictionary = 
            indexData.sceneEntries.ToDictionary(entry => entry.asset, entry => entry);
        
        var scenesInBuild = EditorBuildSettings.scenes.
            Where(scene => scene.enabled).
            Select(scene => AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path)).
            ToList();

        var a = EditorBuildSettings.scenes[0];
        
        HashSet<SceneAsset> newScenesInBuild = scenesInBuild.Concat(sceneDictionary.Keys).ToHashSet();
        
        HashSet<string> enumNames = new HashSet<string>();
        List<SceneEntry> sceneEntries = new List<SceneEntry>();
        
        int sceneId = sceneDictionary.Values.Select(scene => scene.enumValue).DefaultIfEmpty(0).Max();
        
        foreach (SceneAsset sceneAsset in newScenesInBuild)
        {
            if (sceneDictionary.TryGetValue(sceneAsset, out SceneEntry sceneEntry) is false)
            {
                string sceneName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(sceneAsset));
                sceneEntry = new SceneEntry(sceneAsset, sceneName, sceneId);
                
                sceneDictionary.Add(sceneAsset, sceneEntry);
                sceneId++;
            }

            if (sceneEntries.Contains(sceneEntry) is true)
            {
                throw new Exception("중복된 이름을 가진 Scene이 있습니다.");
            }
            
            enumNames.Add(sceneEntry.enumName);
            sceneEntries.Add(sceneEntry);
        }
        string scriptPath = AssetDatabase.GUIDToAssetPath(SCRIPT_GUID);
        
        sceneEntries.Sort((e1, e2) => e1.enumValue.CompareTo(e2.enumValue));
        
        string enumEntities = String.Join("\n", sceneEntries.
            Select(scene => $"        {scene.enumName} = {scene.enumValue},"));
        
        string enumValues = String.Join(",\n", newScenesInBuild.Select(SelectBuildIndexEntry));
        
        string newEnumScripts = SCRIPT_TEMPLATE.Replace("SCENE_ID_ENTRIES", enumEntities)
            .Replace("BUILD_INDEX_ENTRIES", enumValues);
        
        File.WriteAllText(scriptPath, newEnumScripts);
        AssetDatabase.Refresh();
        
        indexData.sceneEntries = sceneEntries;
        EditorUtility.SetDirty(indexData);
        AssetDatabase.SaveAssets();
        return;
        
        string SelectBuildIndexEntry(SceneAsset asset)
        {
            var info = sceneDictionary[asset];
            var sceneName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(asset));
            return $"        (SceneId.{info.enumName}, \"{sceneName}\")";
        }
    }
}
