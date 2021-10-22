Shader "Flowing Lava Surface/LavaFlow" {
	Properties {
		[HideInInspector]_MainTex ("", 2D) = "white" {}
		[NoScaleOffset]_NoiseTex ("Noise", 2D) = "white" {}
		_Color ("Color", Color) = (0.2, 0.07, 0.01, 1)
		_Scale ("Scale", Range(1, 8)) = 3
		_Disp ("Displacement Intensity", Range(1, 10)) = 1
		_NoiseIntensity ("Noise Intensity", Range(3, 18)) = 7
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _NoiseTex, _MainTex;
			float4 _MainTex_ST, _Color;
			half _Scale, _Disp, _NoiseIntensity;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			float2x2 makem2 (float theta) { float c = cos(theta); float s = sin(theta); return float2x2(c,-s,s,c); }
			float noise (float2 v)        { return tex2D(_NoiseTex, v * 0.01).x; }
			float2 gradn (float2 p)
			{
				float ep = 0.09;
				float gradx = noise(float2(p.x+ep, p.y)) - noise(float2(p.x-ep, p.y));
				float grady = noise(float2(p.x, p.y+ep)) - noise(float2(p.x, p.y-ep));
				return float2(gradx, grady);
			}
			
			#define TIME (_Time.y * 0.1)
			
			float flow (float2 p)
			{
				float z = 2.0;
				float ret = 0.0;
				float2 bp = p;
				for (float i = 1.0; i < 7.0; i++)
				{
					p += TIME * 0.6;   // primary flow speed
					bp += TIME * 1.9;  // secondary flow speed (speed of the perceived flow)

					float2 gr = gradn(i * p * 0.34 + TIME * _Disp);   // displacement field (try changing time multiplier)
					gr = mul(makem2(TIME * 6.0 - (0.05 * p.x + 0.03 * p.y) * 40.0), gr);   // rotation of the displacement field
					p += gr * 0.5;   // displace the system

					ret += (sin(noise(p) * _NoiseIntensity) * 0.5 + 0.5) / z;   // add noise octave

					p = lerp(bp, p, 0.77);   // blending displaced system with base system
					z *= 1.4;   // intensity scaling
					p *= 2.0;   // octave scaling
					bp *= 1.9;
				}
				return ret;
			}
			half4 frag (v2f input) : SV_TARGET
			{
				float2 p = input.tex - 0.5;
				p.x *= _ScreenParams.x / _ScreenParams.y;
				p *= _Scale;

				half3 col = _Color.rgb / flow(p);
				col = pow(col, 1.4);
				return half4(col, 1.0);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
