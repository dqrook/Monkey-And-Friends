Shader "Flowing Lava Surface/Cartoon Lava" {
	Properties {
		[Header(Main)]
		_Color       ("Tint Start", Color) = (0.5, 0.1, 0.1, 1)
		_Color2      ("Tint End", Color) = (0.6, 0.4, 0.1, 1)
		_Offset      ("Tint Offset", Range(0, 10)) = 1
		[NoScaleOffset]
		_MainTex     ("Main", 2D) = "white" {}
		_Scale       ("Scale", Range(0, 1)) = 0.3
		_SpeedMainX  ("Speed X", Range(-10, 10)) = 0.4
		_SpeedMainY  ("Speed Y", Range(-10, 10)) = 0.4
		_Strength    ("Brightness Under Lava", Range(0, 10)) = 2
		_StrengthTop ("Brightness Top Lava", Range(0, 10)) = 3

		[Space(10)]
		[Header(Edge)]
		_EdgeC    ("Edge Color", Color) = (1, 0.5, 0.2, 1)
		_EdgeBlur ("Edge Blur", Range(0, 1)) = 0.5
		_Edge     ("Edge Thickness", Range(0.01, 1)) = 0.1

		[Space(10)]
		[Header(Distortion)]
		[NoScaleOffset]
		_DistortTex    ("Distort", 2D) = "white" {}
		_ScaleDist     ("Scale", Range(0, 1)) = 0.5
		_SpeedDistortX ("Speed X", Range(-10, 10)) = 0.2
		_SpeedDistortY ("Speed Y", Range(-10, 10)) = 0.2
		_Distortion    ("Strength", Range(0, 1)) = 0.2
		
		[Space(10)]
		[Header(Vertex Animation)]
		_Speed  ("Wave Speed", Range(0, 1)) = 0.5
		_Amount ("Wave Amount", Range(0, 1)) = 0.6
		_Height ("Wave Height", Range(0, 1)) = 0.1
	}
	SubShader {
		Tags { "RenderType" = "Opaque"  "Queue" = "Geometry+1" }
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD1;
				float4 scrpos : TEXCOORD2;
				float4 wldpos : TEXCOORD3;
				UNITY_FOG_COORDS(4)
			};

			float4 _Color, _Color2, _EdgeC;
			sampler2D _MainTex, _DistortTex, _CameraDepthTexture;
			float _Speed, _Amount, _Height, _Edge, _Scale, _ScaleDist;
			float  _Offset, _Strength, _Distortion,  _StrengthTop, _EdgeBlur;
			float _SpeedDistortX, _SpeedDistortY;
			float _SpeedMainX, _SpeedMainY;


			v2f vert (appdata_full v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);

				v.vertex.y += (sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount)) * _Height);

				o.pos = UnityObjectToClipPos(v.vertex);
				o.wldpos = mul(unity_ObjectToWorld, v.vertex);
				o.scrpos = ComputeScreenPos(o.pos);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				// sample distort texture
				float u = _Time.x * _SpeedDistortX;
				float v = _Time.x * _SpeedDistortY;
				float2 uv = float2(u, v);

				float d1 = tex2D(_DistortTex, (i.wldpos.xz * _ScaleDist) + uv).r;
				float d2 = tex2D(_DistortTex, (i.wldpos.xz * (_ScaleDist * 0.5)) + uv).r;
				float layereddist = saturate((d1 + d2) * 0.5);

				// sample main texture
				uv = i.wldpos.xz * _Scale;
				uv += layereddist * _Distortion;
				uv += float2(_Time.x * _SpeedMainX, _Time.x * _SpeedMainY);
				half4 col = tex2D(_MainTex, uv);

				col += layereddist;

				// depth edge color
				float dpt = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrpos)).r);
				float diff = dpt - i.scrpos.w;
				half4 edgeLine = 1 - saturate(_Edge * diff);
				float edge = smoothstep(1 - col, 1 - col + _EdgeBlur, edgeLine);

				float4 c = lerp(_Color, _Color2, col * _Offset) * _Strength;
				c *= (1 - edge);
				c += (edge *_EdgeC) * _StrengthTop;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}
	}
	Fallback Off
}