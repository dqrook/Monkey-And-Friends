Shader "Neon2/Enemy Wave/Built-in/SH_Multiply"
{
	Properties
	{
		_Texture ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		
		Tags { "Queue" = "Geometry" }
		


		LOD 100

		Pass
		{
			ZWrite Off
			Blend DstColor Zero

			CGPROGRAM
			#pragma multi_compile_instancing

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Texture;
			float4 _Texture_ST;
			

			v2f vert (appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _Texture);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_Texture, i.uv);
				return col;
			}
			ENDCG
		}
	}
}