#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneInfo
{
	public int sceneIndex;

#if UNITY_EDITOR
	public SceneAsset sceneAsset;

	public void RefreshIndex()
	{
		EditorBuildSettingsScene[] scenesLoaded = EditorBuildSettings.scenes;
		string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
		sceneIndex = -1;
		for(int i = 0; i < scenesLoaded.Length; i++)
		{
			if(scenesLoaded[i].path == scenePath)
			{
				sceneIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
				break;
			}
		}
	}

	public void AddSceneToBuildSettings()
	{
		List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
		string scenePath = GetScenePath();

		if (!IsSceneAlreadyAdded(scenePath) && !string.IsNullOrEmpty(scenePath))
		{
			EditorBuildSettingsScene sceneToAdd = new EditorBuildSettingsScene(scenePath, true);
			newScenes.Add(sceneToAdd);
		}

		EditorBuildSettings.scenes = newScenes.ToArray();
	}

	private string GetScenePath()
	{
		string res = AssetDatabase.GetAssetPath(sceneAsset); ;
		return res;
	}

	private bool IsSceneAlreadyAdded(string scenePath)
	{
		bool res = false;
		EditorBuildSettingsScene[] scenesLoaded = EditorBuildSettings.scenes;
		for(int i = 0; i < scenesLoaded.Length; i++)
		{
			if(scenesLoaded[i].path == scenePath)
			{
				res = true;
				break;
			}
		}

		return res;
	}
#endif
}