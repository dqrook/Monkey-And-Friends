using UnityEngine.Rendering;

namespace Neon2
{
	public static class Neon2Utils
	{
		private const string KEY_SELECTED_SRP = "neon2.enemywave.srp";

		public static bool GetRandomBool()
		{
			int randomInt = UnityEngine.Random.Range(0, 2);
			bool res = randomInt == 1;

			return res;
		}

		public static bool IsUsingRenderPipeline()
		{
			bool res = false;

			if (GraphicsSettings.renderPipelineAsset != null)
			{
				res = true;
			}

			return res;
		}

#if UNITY_EDITOR
		public static void SaveSelectedSRP(MaterialSearcher.SRP srp)
		{
			UnityEditor.EditorPrefs.SetInt(KEY_SELECTED_SRP, (int)srp);
		}

		public static MaterialSearcher.SRP GetSelectedSRP()
		{
			MaterialSearcher.SRP res = (MaterialSearcher.SRP)UnityEditor.EditorPrefs.GetInt(KEY_SELECTED_SRP, 
				(int)MaterialSearcher.SRP.URP);
			return res;
		}
#endif
	}
}