using UnityEditor;
using UnityEngine;
using Neon2.SlimeSystem;

[CustomEditor(typeof(SlimeVisualDatabase))]
public class SlimeVisualDatabaseEditor : Editor
{
	public override void OnInspectorGUI()
	{
		SlimeVisualDatabase slimeVisualDatabase = (SlimeVisualDatabase)target;


		/*
		if (GUILayout.Button("Refresh References"))
		{
			slimeVisualDatabase.slimesVisuals = Neon2.EditorUtils.SearchAssets<SlimeVisualConfig>().ToArray();

			EditorUtility.SetDirty(slimeVisualDatabase);
			AssetDatabase.SaveAssets();
		}
		*/

		if(GUILayout.Button("Update Materials References"))
		{
			MaterialReferences materialReferences = Neon2.EditorUtils.SearchAsset<MaterialReferences>();
			for (int i = 0; i < slimeVisualDatabase.slimesVisuals.Length; i++)
			{
				Material matSlimeBody = materialReferences.GetSlimeBodyMaterial(slimeVisualDatabase.slimesVisuals[i].visualType);
				Material matHat = materialReferences.GetHatMaterial();
				Material matShadow = materialReferences.GetShadowMaterial();

				slimeVisualDatabase.slimesVisuals[i].SetMaterials(matSlimeBody, matHat, matShadow);
			}

			AssetDatabase.SaveAssets();
		}

		DrawDefaultInspector();
	}
}