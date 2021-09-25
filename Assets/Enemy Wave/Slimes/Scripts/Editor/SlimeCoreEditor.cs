using UnityEditor;
using Neon2.SlimeSystem;

[CustomEditor(typeof(SlimeCore))]
public class SlimeCoreEditor : Editor
{
	private bool advancedOptionsEnabled;

	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("Nothing to see here...");


		Neon2.EditorUtils.Space(4);
		EditorGUILayout.HelpBox("Dangereous zone. Enter under your responsability", MessageType.Warning);
		advancedOptionsEnabled = EditorGUILayout.Toggle("Advanced Options", advancedOptionsEnabled);

		if (advancedOptionsEnabled)
		{
			DrawDefaultInspector();
		}
	}
}
