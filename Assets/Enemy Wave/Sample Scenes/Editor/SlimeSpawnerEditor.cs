using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(SlimeSpawner))]
public class SlimeSpawnerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}

	void OnSceneGUI()
	{
		SlimeSpawner slimeSpawner = (SlimeSpawner)target;
		
		if (slimeSpawner == null)
		{
			return;
		}

		Color handleColor = Color.red;
		if(slimeSpawner.tag == "SlimePositioner")
		{
			handleColor = Color.blue;
		}

		Handles.color = handleColor;
		Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

		Quaternion circleRotation = Quaternion.Euler(90f, 0f, 0f) * slimeSpawner.transform.rotation;
		Handles.CircleHandleCap(0, slimeSpawner.transform.position, circleRotation, slimeSpawner.radius, EventType.Repaint);
	}
}