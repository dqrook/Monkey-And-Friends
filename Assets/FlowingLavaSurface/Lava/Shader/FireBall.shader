Shader "Flowing Lava Surface/Fire Ball" {
	Properties {
		_NoiseTex3D ("Noise", 3D) = "white" {}
		_Scale ("Scale", Float) = 0.02
		_Speed ("Speed", Range(0, 2)) = 1
		_Offset ("Offset", Range(0, 0.7)) = 0.35
		_MaskTex ("Mask", 2D) = "white" {}
		_AlbedoTex ("Albedo", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			sampler3D _NoiseTex3D;
			sampler2D _MaskTex, _AlbedoTex;
			half4 _MaskTex_ST, _AlbedoTex_ST;
			float _Scale, _Speed, _Offset;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 uvw : TEXCOORD0;
				half4 uv2 : TEXCOORD1;
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uvw = _Scale * (v.vertex.xyz + float3(0.0, 0.0, -_Time.y * _Speed));
				o.uv2.xy = TRANSFORM_TEX(v.texcoord, _MaskTex);
				o.uv2.zw = TRANSFORM_TEX(v.texcoord, _AlbedoTex);
				return o;
			}
			float Noise (float3 coord)
			{
				float n = tex3D(_NoiseTex3D, coord).x;
				n += 0.25   * tex3D(_NoiseTex3D, coord * 2.0).x;
				n += 0.25   * tex3D(_NoiseTex3D, coord * 4.0).x;
				n += 0.125  * tex3D(_NoiseTex3D, coord * 8.0).x;
				n += 0.0625 * tex3D(_NoiseTex3D, coord * 16.0).x;
				return n;
			}
			half4 frag (v2f input) : SV_TARGET
			{
				float n = Noise(input.uvw) - _Offset;
				half4 lava = half4(1.5 - n, 1.0 - n, 0.5 - n, 1.0);

				half4 mask = tex2D(_MaskTex, input.uv2.xy);
				half4 albedo = tex2D(_AlbedoTex, input.uv2.zw);
				return lerp(albedo, lava, mask.r);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
