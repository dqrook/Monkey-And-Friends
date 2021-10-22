Shader "Flowing Lava Surface/Layered Lava" {
	Properties {
		_LavaTex ("Lava", 2D) = "white" {}
		_NoiseTex ("Noise", 2D) = "white" {}
		_LavaTile ("Lava Tile", Range(1, 8)) = 2
		_LavaBright ("Lava Bright", Range(0.1, 5)) = 2
		_LavaDarkOffset ("Lava Dark Offset", Range(0.01, 1)) = 0.1
		_LavaFlowSpeed ("Lava Flow Speed", Range(0.01, 0.1)) = 0.01
		_NoiseTile ("Noise Tile", Range(1, 8)) = 2
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

			sampler2D _NoiseTex, _LavaTex, _MaskTex, _AlbedoTex;
			float4 _LavaTex_ST, _MaskTex_ST, _AlbedoTex_ST;
			float _LavaTile, _LavaBright, _LavaDarkOffset, _LavaFlowSpeed, _NoiseTile;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
				float4 tex2 : TEXCOORD1;
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = TRANSFORM_TEX(v.texcoord, _LavaTex);
				o.tex2.xy = TRANSFORM_TEX(v.texcoord, _MaskTex);
				o.tex2.zw = TRANSFORM_TEX(v.texcoord, _AlbedoTex);
				return o;
			}
			half4 frag (v2f input) : SV_TARGET
			{
				float4 noise = tex2D(_NoiseTex, input.tex);
				float2 t1 = input.tex + float2( 1.5, -1.5) * _Time.y * 0.02;
				float2 t2 = input.tex + float2(-0.5,  2.0) * _Time.y * _LavaFlowSpeed;

				t1.x += noise.x * 2.0;
				t1.y += noise.y * 2.0;
				t2.x -= noise.y * 0.2;
				t2.y += noise.z * 0.2;

				half p = tex2D(_NoiseTex, t1 * _NoiseTile).a;

				half4 color = tex2D(_LavaTex, t2 * _LavaTile);
				half4 lava = color * (p.xxxx * _LavaBright) + (color * color - _LavaDarkOffset);

				half4 mask = tex2D(_MaskTex, input.tex2.xy);
				half4 albedo = tex2D(_AlbedoTex, input.tex2.zw);
				return lerp(albedo, lava, mask.r);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
