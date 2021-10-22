using UnityEngine;

namespace LavaSurface
{
	public class Demo : MonoBehaviour
	{
		public GameObject[] m_Objects;
		public Material[] m_Mats;
		public Texture2D m_PlaneMask, m_PlaneAlbedo;
		public float m_PlaneAlbedoScale = 3f;
		int m_PrevLavaType = 0, m_CurrLavaType = 0;
		Noise3D m_Noise3D = new Noise3D();

		void Start()
		{
			QualitySettings.antiAliasing = 8;
			Camera.main.depthTextureMode = DepthTextureMode.Depth;
			m_Noise3D.Create(64, 2);
		}
		void Update()
		{
			for (int i = 0; i < m_Objects.Length; i++)
			{
				Renderer rd = m_Objects[i].GetComponent<Renderer>();
				if (m_CurrLavaType != m_PrevLavaType)
				{
					rd.material = m_Mats[m_CurrLavaType];
					m_PrevLavaType = m_CurrLavaType;
				}
				rd.material.SetTexture("_NoiseTex3D", m_Noise3D.Get());
				rd.material.SetFloat("_BubbleScale", 6f);
				rd.material.SetFloat("_BubbleRate", 0.3f);
				rd.material.SetFloat("_NoiseScale", 0.6f);
				if (m_Objects[i].name == "Plane")
				{
					rd.material.SetTexture("_MaskTex", m_PlaneMask);
					rd.material.SetTexture("_AlbedoTex", m_PlaneAlbedo);
					rd.material.SetTextureScale("_AlbedoTex", new Vector2(m_PlaneAlbedoScale, m_PlaneAlbedoScale));
				}
			}
		}
		void OnGUI()
		{
			GUI.Box(new Rect(10, 10, 200, 25), "Lava Surface Demo");
			string[] names = { "Layered Lava", "Flowing Map", "Fire Ball", "Boiling Lava", "Lava Flow" };
			m_CurrLavaType = GUI.SelectionGrid(new Rect(10, 40, 130, 200), m_CurrLavaType, names, 1);
		}
	}
}