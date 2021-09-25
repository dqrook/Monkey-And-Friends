using UnityEditor;
using Neon2.SlimeSystem;

[CustomEditor(typeof(SlimeController))]
public class SlimeControllerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();


		/*
		SlimeController slimeController = (SlimeController)target;


		string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
		bool tagFound = false;

		for (int i = 0; i < tags.Length; i++)
		{
			if (tags[i] == slimeController.tagWall)
			{
				tagFound = true;
				break;
			}
		}

		Neon2.EditorUtils.Space(4);
		if (!tagFound)
		{
			EditorGUILayout.HelpBox("The 'Tag Wall' does not exist!", MessageType.Warning);
		}
		*/

		Neon2.EditorUtils.Space(4);
	}
}