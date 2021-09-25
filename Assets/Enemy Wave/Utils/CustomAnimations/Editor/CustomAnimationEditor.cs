using UnityEditor;
using UnityEngine;
using Neon2.SlimeSystem;

[CanEditMultipleObjects]
[CustomEditor(typeof(CustomAnimation))]
public class CustomAnimationEditor : Editor
{
	private CustomAnimation customAnimation;
	private float timeFactor;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		customAnimation = (CustomAnimation)target;

		EditorGUILayout.BeginHorizontal();
		timeFactor = EditorGUILayout.FloatField("Time factor", timeFactor);

		if (timeFactor != 0)
		{
			if (GUILayout.Button("Apply"))
			{
				ApplyTimeFactor();
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	public void ApplyTimeFactor()
	{
		customAnimation.ApplyTimeFactor(timeFactor);
	}
}
