// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNature/Flower"
{
	Properties
	{
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_TimeScale1("TimeScale", Float) = 0.1
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Strength1("Strength", Range( 0 , 1)) = 0
		_PlantColor("Plant Color", Color) = (0.1942862,0.6981132,0.2443751,0)
		_Scale1("Scale", Float) = 1.08
		_FlowerColor("Flower Color", Color) = (0.1942862,0.6981132,0.2443751,0)
		_Texture1("Texture", 2D) = "white" {}
		_FlowerBase("Flower Base", 2D) = "white" {}
		_FlowerTint("Flower Tint", 2D) = "white" {}
		_Smoothness1("Smoothness", Range( 0 , 1)) = 0.49
		_Translucent1("Translucent", Range( 0 , 1)) = 0.49
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform float _TimeScale1;
		uniform float _Scale1;
		uniform float _Strength1;
		uniform float4 _PlantColor;
		uniform sampler2D _FlowerBase;
		uniform float4 _FlowerBase_ST;
		uniform float4 _FlowerColor;
		uniform sampler2D _FlowerTint;
		uniform float4 _FlowerTint_ST;
		uniform float _Smoothness1;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _Translucent1;
		uniform sampler2D _Texture1;
		uniform float4 _Texture1_ST;
		uniform float _Cutoff = 0.5;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 break11 = float3( 0,0,0 );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 appendResult45 = (float2(ase_worldPos.y , ase_worldPos.z));
			float2 temp_cast_0 = (( _Time.y * _TimeScale1 )).xx;
			float2 uv_TexCoord33 = v.texcoord.xy * appendResult45 + temp_cast_0;
			float simplePerlin2D37 = snoise( uv_TexCoord33*_Scale1 );
			simplePerlin2D37 = simplePerlin2D37*0.5 + 0.5;
			float3 appendResult16 = (float3(( break11.x + ( ( simplePerlin2D37 - 0.5 ) * _Strength1 ) ) , break11.y , break11.z));
			float3 lerpResult23 = lerp( float3( 0,0,0 ) , appendResult16 , v.texcoord.xy.y);
			v.vertex.xyz += lerpResult23;
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_FlowerBase = i.uv_texcoord * _FlowerBase_ST.xy + _FlowerBase_ST.zw;
			float2 uv_FlowerTint = i.uv_texcoord * _FlowerTint_ST.xy + _FlowerTint_ST.zw;
			o.Albedo = ( ( _PlantColor * tex2D( _FlowerBase, uv_FlowerBase ) ) + ( _FlowerColor * tex2D( _FlowerTint, uv_FlowerTint ) ) ).rgb;
			o.Smoothness = _Smoothness1;
			float3 temp_cast_1 = (_Translucent1).xxx;
			o.Translucency = temp_cast_1;
			o.Alpha = 1;
			float2 uv_Texture1 = i.uv_texcoord * _Texture1_ST.xy + _Texture1_ST.zw;
			clip( tex2D( _Texture1, uv_Texture1 ).a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "FlowerIn"
}
/*ASEBEGIN
Version=17800
420.8;108;998;555;2681.943;-390.2942;1.723182;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;29;-1793.409,1200.947;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1780.026,1108.461;Inherit;False;Property;_TimeScale1;TimeScale;7;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;31;-1936.999,863.3399;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;45;-1584.982,932.825;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1569.652,1146.043;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1360.098,894.027;Inherit;False;Property;_Scale1;Scale;11;0;Create;True;0;0;False;0;1.08;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-1376.15,1005.516;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;37;-1123.483,885.673;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;3.21;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-871.1418,1145.185;Inherit;False;Property;_Strength1;Strength;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;38;-863.2743,898.454;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-693.6982,940.105;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;11;-1148.284,597.8895;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;24;-559.7884,-962.9803;Inherit;True;Property;_FlowerBase;Flower Base;14;0;Create;True;0;0;False;0;-1;None;72a490c6e4057ca4abe3324b673d094c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;26;-737.5406,-756.8903;Inherit;False;Property;_FlowerColor;Flower Color;12;0;Create;True;0;0;False;0;0.1942862,0.6981132,0.2443751,0;1,0,0.6135116,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-770.2184,-1048.865;Inherit;False;Property;_PlantColor;Plant Color;10;0;Create;True;0;0;False;0;0.1942862,0.6981132,0.2443751,0;0.1942861,0.6981132,0.244375,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;25;-526.7042,-737.873;Inherit;True;Property;_FlowerTint;Flower Tint;15;0;Create;True;0;0;False;0;-1;None;392fbe978dad3004cb0385bba5d819af;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-530.0956,794.196;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-166.0485,-708.0363;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-676.5878,532.1805;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;16;-260.121,703.9733;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-160.6194,-872.6563;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;18;-794.0227,-259.7723;Inherit;True;Property;_Texture1;Texture;13;0;Create;True;0;0;False;0;-1;None;deaa79fe48136b449b5d435cf79d419d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;22;-614.089,187.2619;Inherit;False;Property;_Smoothness1;Smoothness;16;0;Create;True;0;0;False;0;0.49;0.49;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;86.37537,-731.6343;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;23;-220.1509,457.5331;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-646.3078,90.77482;Inherit;False;Property;_Translucent1;Translucent;17;0;Create;True;0;0;False;0;0.49;0.49;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;FlowerIn;0;0;Standard;SNature/Flower;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;8;0;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;45;0;31;2
WireConnection;45;1;31;3
WireConnection;43;0;29;0
WireConnection;43;1;30;0
WireConnection;33;0;45;0
WireConnection;33;1;43;0
WireConnection;37;0;33;0
WireConnection;37;1;34;0
WireConnection;38;0;37;0
WireConnection;41;0;38;0
WireConnection;41;1;42;0
WireConnection;15;0;11;0
WireConnection;15;1;41;0
WireConnection;27;0;26;0
WireConnection;27;1;25;0
WireConnection;16;0;15;0
WireConnection;16;1;11;1
WireConnection;16;2;11;2
WireConnection;21;0;17;0
WireConnection;21;1;24;0
WireConnection;28;0;21;0
WireConnection;28;1;27;0
WireConnection;23;1;16;0
WireConnection;23;2;19;2
WireConnection;0;0;28;0
WireConnection;0;4;22;0
WireConnection;0;7;20;0
WireConnection;0;10;18;4
WireConnection;0;11;23;0
ASEEND*/
//CHKSM=24F1CD82DC12D41FAFACEE73CADD354E77ED9242