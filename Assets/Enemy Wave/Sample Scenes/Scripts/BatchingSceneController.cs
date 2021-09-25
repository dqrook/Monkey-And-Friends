using UnityEngine;

public class BatchingSceneController : MonoBehaviour
{
	public MeshRenderer meshRenderer;


#if UNITY_EDITOR
	[ContextMenu("Setup Materials")]
	public void SetUpMaterials()
	{
		Material mat01 = MaterialSearcher.SearchMaterial("MAT_Scenario", MaterialSearcher.MaterialFolder.Samples);
		Material mat02 = MaterialSearcher.SearchMaterial("MAT_EmmisiveBorder", MaterialSearcher.MaterialFolder.Samples);

		meshRenderer.sharedMaterials = new Material[2] { mat01, mat02 };
	} 
#endif
}
