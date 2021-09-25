Shader "Neon2/SH_AlphaTextured"
{
	Properties
	{
		_Tex ("Texture", 2D) = "white" {}
		_Alpha ("Alpha", Range(0, 1)) = 1
		_Tint("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Tex;
			float4 _Tex_ST;
			half _Alpha;
			fixed4 _Tint;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _Tex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_Tex, i.uv) * _Tint;
				//col.a = _Alpha;
				return col;
			}
			ENDCG
		}
	}
}
