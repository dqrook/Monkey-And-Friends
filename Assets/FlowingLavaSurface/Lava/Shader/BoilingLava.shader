Shader "Flowing Lava Surface/Boiling Lava" {
	Properties {
		_NoiseTex3D ("Noise", 3D) = "black" {}
		_FlameTex ("Flame", 2D) = "black" {}
		_NoiseScale ("Noise Scale", Range(0, 1)) = 0.3
		_MorphRate ("Morph Rate", Range(0, 0.2)) = 0.05
		_BubbleRate ("Bubble Rate", Range(0, 0.5)) = 0.2
		_BubbleScale ("Bubble Scale", Range(0, 16)) = 8
		_FlameScale ("Flame Scale", Range(0, 2)) = 0.82
		_FlameBias ("Flame Bias", Range(-1, 1)) = -0.12
		_MaskTex ("Mask", 2D) = "white" {}
		_AlbedoTex ("Albedo", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler3D _NoiseTex3D;
			sampler2D _FlameTex, _MaskTex, _AlbedoTex;
			fixed4 _MaskTex_ST, _AlbedoTex_ST;
			float _NoiseScale, _MorphRate, _BubbleRate, _BubbleScale, _FlameScale, _FlameBias;

			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed4 uv2 : TEXCOORD1;
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.uv2.xy = TRANSFORM_TEX(v.texcoord, _MaskTex);
				o.uv2.zw = TRANSFORM_TEX(v.texcoord, _AlbedoTex);
				return o;
			}
			half4 frag (v2f input) : SV_TARGET
			{
				float3 uvw;
				uvw.xy = input.uv * _BubbleScale;
				uvw.z = _BubbleRate * _Time.y;

				float noiseX = tex3D(_NoiseTex3D, uvw).r - 0.5;
				float noiseY = tex3D(_NoiseTex3D, uvw + 0.5).r - 0.5;

				uvw.x = input.uv.x + noiseX * _NoiseScale;
				uvw.y = input.uv.y + noiseY * _NoiseScale;
				uvw.z = _Time.y * _MorphRate;

				float base = tex3D(_NoiseTex3D, uvw).r;
				half4 lava = tex2D(_FlameTex, float2(_FlameScale * base + _FlameBias, 0));
				half4 mask = tex2D(_MaskTex, input.uv2.xy);
				half4 albedo = tex2D(_AlbedoTex, input.uv2.zw);
				return lerp(albedo, lava, mask.r);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
