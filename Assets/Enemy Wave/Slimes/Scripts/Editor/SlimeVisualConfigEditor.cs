using UnityEditor;
using UnityEngine;
using Neon2.SlimeSystem;

[CustomEditor(typeof(SlimeVisualConfig))]
[CanEditMultipleObjects]
public class SlimeVisualConfigEditor : Editor
{
	private SerializedProperty spVisualType;
	private SerializedProperty spSlimeID;
	private SerializedProperty spFace;
	private SerializedProperty spHatMesh;
	private SerializedProperty spMatSlimeBody;
	private SerializedProperty spMatHat;
	private SerializedProperty spMathShadow;
	private SerializedProperty spSlimeColor;
	private SerializedProperty spSlimeSecondColor;

	private SlimeVisualConfig slimeVisualConfig;

	private void OnEnable()
	{
		InitializeSerializedProperties();


		slimeVisualConfig = (SlimeVisualConfig)target;
	}

	public override void OnInspectorGUI()
	{

		bool changesOnVisualType = false;
		serializedObject.Update();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(spVisualType);
		if (EditorGUI.EndChangeCheck())
		{
			changesOnVisualType = true;
		}


		EditorGUILayout.PropertyField(spSlimeID);
		EditorGUILayout.PropertyField(spFace);
		EditorGUILayout.PropertyField(spHatMesh);
		EditorGUILayout.PropertyField(spMatSlimeBody);
		EditorGUILayout.PropertyField(spMatHat);
		EditorGUILayout.PropertyField(spMathShadow);



		
		if (slimeVisualConfig.visualType == SlimeVisualConfig.VisualType.SIMPLE_COLOR)
		{
			EditorGUILayout.PropertyField(spSlimeColor);
		}
		else if (slimeVisualConfig.visualType == SlimeVisualConfig.VisualType.BICOLOR)
		{
			EditorGUILayout.PropertyField(spSlimeColor);
			EditorGUILayout.PropertyField(spSlimeSecondColor);
		}

		Neon2.EditorUtils.Space(4);
		if (slimeVisualConfig.matSlimeBody == null)
		{
			EditorGUILayout.HelpBox(string.Format("You must asign a material to '{0}' field", spMatSlimeBody.displayName), MessageType.Error);
		}
		if (string.IsNullOrEmpty(slimeVisualConfig.slimeID))
		{
			EditorGUILayout.HelpBox(string.Format("'{0}' is empty. This could lead to problems if you're looking slimes by ID",
				spSlimeID.displayName), MessageType.Warning);
		}



		if (GUILayout.Button("Update Materials References"))
		{
			RefreshMaterials();
		}

		if(GUILayout.Button("Refresh Visual"))
		{
			SlimeVisual[] slimeVisuals =  FindObjectsOfType<SlimeVisual>();

			for(int i = 0; i < slimeVisuals.Length; i++)
			{
				if(slimeVisuals[i].visualConfig == slimeVisualConfig)
				{
					slimeVisuals[i].UpdateMaterials();
					slimeVisuals[i].UpdatePropertyBlock();
				}
			}
		}

		serializedObject.ApplyModifiedProperties();

		if (changesOnVisualType)
		{
			RefreshMaterials();
		}
	}

	private void RefreshMaterials()
	{
		UpdateMaterials();
		EditorUtility.SetDirty(slimeVisualConfig);
		AssetDatabase.SaveAssets();
	}

	private void UpdateMaterials()
	{
		MaterialReferences materialReferneces = Neon2.EditorUtils.SearchAsset<MaterialReferences>();

		Material slimeBodyMaterial = materialReferneces.GetSlimeBodyMaterial(slimeVisualConfig.visualType);
		Material matHat = materialReferneces.GetHatMaterial();
		Material matShadow = materialReferneces.GetShadowMaterial();

		slimeVisualConfig.SetMaterials(slimeBodyMaterial, matHat, matShadow);
	}

	private void InitializeSerializedProperties()
	{
		spVisualType = serializedObject.FindProperty("visualType");
		spSlimeID = serializedObject.FindProperty("slimeID");
		spFace = serializedObject.FindProperty("face");
		spHatMesh = serializedObject.FindProperty("hatMesh");
		spMatSlimeBody = serializedObject.FindProperty("matSlimeBody");
		spMatHat = serializedObject.FindProperty("matHat");
		spMathShadow = serializedObject.FindProperty("matShadow");
		spSlimeColor = serializedObject.FindProperty("slimeColor");
		spSlimeSecondColor = serializedObject.FindProperty("slimeSecondColor");
	}
}