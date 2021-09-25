using UnityEngine;

namespace Neon2.SlimeSystem
{
	[CreateAssetMenu(menuName = "Neon2/Slime System/Slime Visual Config")]
	public class SlimeVisualConfig : ScriptableObject
	{
		public enum VisualType
		{
			SIMPLE_COLOR,
			BICOLOR,
			TEXTURED_RAINBOW_01,
			TEXTURED_RAINBOW_02,
		}

		public VisualType visualType;

		public string slimeID;
		public int face;
		public Mesh hatMesh;
		public Material matSlimeBody;
		public Material matHat;
		public Material matShadow;

		[Header("Color Config")]
		public Color slimeColor;
		public Color slimeSecondColor;

#if UNITY_EDITOR
		private void Reset()
		{
			visualType = VisualType.SIMPLE_COLOR;
			slimeColor = new Color(0.1843137f, 0.9921569f, 0.7176471f);
			slimeSecondColor = new Color(0.6509804f, 0f, 1f);
			face = 0;
			hatMesh = null;

			UpdateMaterials();
		}
#endif

		public string GetID()
		{
			return slimeID;
		}

		public Vector4 GetFaceCoordinates()
		{
			Vector4 res = Vector4.zero;

			res.x = 5 - Mathf.Floor((face - 1) / 6);
			res.y = (face - 1) % 6;

			return res;
		}

		public bool IsTextured()
		{
			bool res = (visualType == VisualType.TEXTURED_RAINBOW_01 || visualType == VisualType.TEXTURED_RAINBOW_02);
			return res;
		}

#if UNITY_EDITOR
		[ContextMenu("Update Materials")]
		public void UpdateMaterials()
		{
			Material slimeBodyMaterial = MaterialSearcher.SearchMaterial(GetMaterialBodyNameByVisualType(visualType), MaterialSearcher.MaterialFolder.Slime);
			Material matHat = MaterialSearcher.SearchMaterial("MAT_Slime_Hat", MaterialSearcher.MaterialFolder.Slime);
			Material matShadow = MaterialSearcher.SearchMaterial("MAT_Slime_Shadow", MaterialSearcher.MaterialFolder.Slime);

			SetMaterials(slimeBodyMaterial, matHat, matShadow);
		}

		private string GetMaterialBodyNameByVisualType(VisualType visualType)
		{
			string res = "";

			switch (visualType)
			{
				case VisualType.SIMPLE_COLOR:
					res = "MAT_Slime_SimpleColor";
					break;
				case VisualType.BICOLOR:
					res = "MAT_Slime_Bicolor";
					break;
				case VisualType.TEXTURED_RAINBOW_01:
					res = "MAT_Slime_TexturedRainbow01";
					break;
				case VisualType.TEXTURED_RAINBOW_02:
					res = "MAT_Slime_TexturedRainbow02";
					break;
			}

			return res;
		}

		public void SetMaterials(Material matSlimeBody, Material matHat, Material matShadow)
		{
			this.matSlimeBody = matSlimeBody;
			this.matHat = matHat;
			this.matShadow = matShadow;

			UnityEditor.EditorUtility.SetDirty(this);
		}
#endif
	}
}