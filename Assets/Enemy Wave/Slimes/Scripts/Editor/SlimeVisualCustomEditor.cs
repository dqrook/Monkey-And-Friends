using UnityEditor;
using Neon2.SlimeSystem;

[CanEditMultipleObjects]
[CustomEditor(typeof(SlimeVisual))]
public class SlimeVisualCustomEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		SlimeVisual slimeVisual = (SlimeVisual)target;
		slimeVisual.UpdateMaterials();
		slimeVisual.UpdatePropertyBlock();
	}
}