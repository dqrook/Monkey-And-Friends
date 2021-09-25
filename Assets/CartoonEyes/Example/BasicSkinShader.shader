Shader "Hidden/CartoonEyes/BasicSkinShader"
{
	Properties
	{
		_AmbientColor("AmbientColor", Color) = (0.609,0.5682353,0.3403235,1)
		_Color("Color", Color) = (1,0.9330629,0.5588235,1)
		_Gradients("Gradients", Int) = 4
		_LightDir("Light Dir", Vector) = (0, 0, 1, 0)
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
			};

			uniform float4 _AmbientColor;
			uniform float4 _Color;
			uniform int _Gradients;
			uniform float3 _LightDir;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldSpaceLightDir4 = normalize(-_LightDir);
				return lerp(_AmbientColor, _Color, round((((dot(worldSpaceLightDir4, i.worldNormal) + 1) * 0.5) * (float)_Gradients)) / (float)_Gradients);
			}
			ENDCG
		}
	}
}
