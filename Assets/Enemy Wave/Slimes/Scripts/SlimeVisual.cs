using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimeVisual : MonoBehaviour
	{
		public SlimeVisualConfig visualConfig;

		private const string MATPROPERTY_FACE = "_Face";
		private const string MATPROPERTY_SLIME_COLOR = "_TopColor";
		private const string MATPROPERTY_SECOND_COLOR = "_SecondColor";

		private MaterialPropertyBlock bodyPropBlock;
		private MaterialPropertyBlock hatPropBlock;

		[Header("Renderers")]
		public Renderer meshRendererBody;
		public MeshRenderer meshRendererHat;
		public MeshRenderer meshRendererShadow;
		public MeshFilter meshFilterHat;



		public void Init()
		{
			bodyPropBlock = new MaterialPropertyBlock();
			hatPropBlock = new MaterialPropertyBlock();
		}

		public void SetupVisual(SlimeVisualConfig visualConfig)
		{
			if (visualConfig != null)
			{
				this.visualConfig = visualConfig;
			}

			UpdateMaterials();
			UpdatePropertyBlock();
		}

		public void UpdateMaterials()
		{
			if (visualConfig != null)
			{
				meshRendererBody.sharedMaterial = visualConfig.matSlimeBody;
				meshRendererHat.sharedMaterial = visualConfig.matHat;
				meshRendererShadow.sharedMaterial = visualConfig.matShadow;
			}
		}

		private void RefreshVisualBuiltIn()
		{
			if (bodyPropBlock == null)
			{
				bodyPropBlock = new MaterialPropertyBlock();
				hatPropBlock = new MaterialPropertyBlock();
			}

			if (visualConfig != null)
			{
				meshFilterHat.sharedMesh = visualConfig.hatMesh;

				meshRendererBody.GetPropertyBlock(bodyPropBlock);
				bodyPropBlock.SetVector(MATPROPERTY_FACE, visualConfig.GetFaceCoordinates());
				bodyPropBlock.SetColor(MATPROPERTY_SLIME_COLOR, visualConfig.slimeColor);
				bodyPropBlock.SetColor(MATPROPERTY_SECOND_COLOR, visualConfig.slimeSecondColor);
				meshRendererBody.SetPropertyBlock(bodyPropBlock);
			}
		}

		private void RefreshVisualURP()
		{
			if (visualConfig != null)
			{
				meshFilterHat.sharedMesh = visualConfig.hatMesh;


				Material mat = null;
				if (!Application.isPlaying)
				{
					mat = meshRendererBody.sharedMaterial;
				}
				else
				{
					mat = meshRendererBody.material;
				}

				mat.SetVector(MATPROPERTY_FACE, visualConfig.GetFaceCoordinates());
				if (!visualConfig.IsTextured())
				{
					mat.SetColor(MATPROPERTY_SLIME_COLOR, visualConfig.slimeColor);
					mat.SetColor(MATPROPERTY_SECOND_COLOR, visualConfig.slimeSecondColor);
				}
			}
		}

		public void UpdatePropertyBlock()
		{
			if (!Neon2Utils.IsUsingRenderPipeline())
			{
				RefreshVisualBuiltIn();
			}
			else
			{
				RefreshVisualURP();
			}
		}

		public SlimeVisualConfig GetVisualConfig()
		{
			return visualConfig;
		}

		public void SetEnabledRenderers(bool value)
		{
			meshRendererBody.enabled = value;
			meshRendererHat.enabled = value;
			meshRendererShadow.enabled = value;
		}
	}
}