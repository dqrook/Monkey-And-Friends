using Neon2.SlimeSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EnemyWaveTool : EditorWindow
{
	private UnityEngine.SceneManagement.Scene openedScene;

	[MenuItem("Tools/Enemy Wave Configurator")]
	static void Init()
	{
		EnemyWaveTool window = EditorWindow.GetWindow<EnemyWaveTool>("Enemy Wave Configurator");
		window.Show();
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Configure Built-in"))
		{
			Neon2.Neon2Utils.SaveSelectedSRP(MaterialSearcher.SRP.Builtin);
			ConfigurePackage();
		}

		if (GUILayout.Button("Configure URP / HDRP"))
		{
			Neon2.Neon2Utils.SaveSelectedSRP(MaterialSearcher.SRP.URP);
			ConfigurePackage();
		}
	}

	private void ConfigurePackage()
	{
		openedScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

		SetUpSlimeMaterials();
		SetUpBatchingSampleScene();



		string openedScenePath = openedScene.path;
		if (string.IsNullOrEmpty(openedScenePath))
		{
			SceneAsset openedSceneAsset = Neon2.EditorUtils.SearchAsset<SceneAsset>(openedScene.name);
			openedScenePath = AssetDatabase.GetAssetPath(openedSceneAsset);
		}
		
		UnityEditor.SceneManagement.EditorSceneManager.OpenScene(openedScenePath, OpenSceneMode.Single);
	}

	private void SetUpSlimeMaterials()
	{
		SlimeVisualDatabase slimeVisualDatabase = Neon2.EditorUtils.SearchAsset<SlimeVisualDatabase>();

		for (int i = 0; i < slimeVisualDatabase.slimesVisuals.Length; i++)
		{
			slimeVisualDatabase.slimesVisuals[i].UpdateMaterials();
		}
	}

	private void SetUpBatchingSampleScene()
	{
		string scenePath = "Assets/Enemy Wave/Sample Scenes/Scenes/Batching Example.unity";
		SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

		EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

		BatchingSceneController batchingSceneController = FindObjectOfType<BatchingSceneController>();
		batchingSceneController.SetUpMaterials();

		UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
		UnityEditor.SceneManagement.EditorSceneManager.SaveScene(activeScene);
	}
}