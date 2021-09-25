using UnityEngine;
namespace Neon2.SlimeSystem
{
	public class MaterialReferences : ScriptableObject
	{
		public Material matSlimeBody_SimpleColor_BuiltIn;
		public Material matSlimeBody_SimpleColor_URP;

		public Material matSlimeBody_Bicolor_BuiltIn;
		public Material matSlimeBody_Bicolor_URP;

		public Material matSlimeBody_TexturedRainbow01_BuiltIn;
		public Material matSlimeBody_TexturedRainbow01_URP;

		public Material matSlimeBody_TexturedRainbow02_BuiltIn;
		public Material matSlimeBody_TexturedRainbow02_URP;

		public Material matHat_BuiltIn;
		public Material matHat_URP;

		public Material matSlimeShadow_BuiltIn;
		public Material matSlimeShadow_URP;

		public Material GetSlimeBodyMaterial(SlimeVisualConfig.VisualType visualType)
		{
			Material res = null;

			if (!Neon2Utils.IsUsingRenderPipeline())
			{
				res = GetSlimeBodyMaterialBuiltIn(visualType);
			}
			else
			{
				res = GetSlimeBodyMaterialURP(visualType);
			}

			return res;
		}

		public Material GetHatMaterial()
		{
			Material res = null;

			if (!Neon2Utils.IsUsingRenderPipeline())
			{
				res = matHat_BuiltIn;
			}
			else
			{
				res = matHat_URP;
			}

			return res;
		}

		public Material GetShadowMaterial()
		{
			Material res = null;

			if (!Neon2Utils.IsUsingRenderPipeline())
			{
				res = matSlimeShadow_BuiltIn;
			}
			else
			{
				res = matSlimeShadow_URP;
			}

			return res;
		}

		private Material GetSlimeBodyMaterialURP(SlimeVisualConfig.VisualType visualType)
		{
			Material res = null;

			switch (visualType)
			{
				case SlimeVisualConfig.VisualType.SIMPLE_COLOR:
					res = matSlimeBody_SimpleColor_URP;
					break;
				case SlimeVisualConfig.VisualType.BICOLOR:
					res = matSlimeBody_Bicolor_URP;
					break;
				case SlimeVisualConfig.VisualType.TEXTURED_RAINBOW_01:
					res = matSlimeBody_TexturedRainbow01_URP;
					break;
				case SlimeVisualConfig.VisualType.TEXTURED_RAINBOW_02:
					res = matSlimeBody_TexturedRainbow02_URP;
					break;
			}

			return res;
		}

		private Material GetSlimeBodyMaterialBuiltIn(SlimeVisualConfig.VisualType visualType)
		{
			Material res = null;

			switch (visualType)
			{
				case SlimeVisualConfig.VisualType.SIMPLE_COLOR:
					res = matSlimeBody_SimpleColor_BuiltIn;
					break;
				case SlimeVisualConfig.VisualType.BICOLOR:
					res = matSlimeBody_Bicolor_BuiltIn;
					break;
				case SlimeVisualConfig.VisualType.TEXTURED_RAINBOW_01:
					res = matSlimeBody_TexturedRainbow01_BuiltIn;
					break;
				case SlimeVisualConfig.VisualType.TEXTURED_RAINBOW_02:
					res = matSlimeBody_TexturedRainbow02_BuiltIn;
					break;
			}

			return res;
		}
	}
}