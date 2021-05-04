// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNature/LeavesNoTra"
{
	Properties
	{
		_Color("Color ", Color) = (0.1942862,0.6981132,0.2443751,0)
		_TimeScale1("TimeScale", Float) = 0.1
		_Texture("Texture", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.91
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.49
		_LeavesScale("LeavesScale", Float) = 0
		_Strength1("Strength", Range( 0 , 1)) = 0
		_Scale1("Scale", Float) = 1.08
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _LeavesScale;
		uniform float _TimeScale1;
		uniform float _Scale1;
		uniform float _Strength1;
		uniform float4 _Color;
		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform float _Smoothness;
		uniform float _Cutoff = 0.91;


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
			float temp_output_21_0 = ( _LeavesScale * -1.0 );
			float2 appendResult22 = (float2(temp_output_21_0 , temp_output_21_0));
			float2 appendResult20 = (float2(_LeavesScale , _LeavesScale));
			float3 appendResult3 = (float3((appendResult22 + (v.texcoord.xy - float2( 0,0 )) * (appendResult20 - appendResult22) / (float2( 1,1 ) - float2( 0,0 ))) , 0.0));
			float3 break39 = mul( float4( mul( float4( appendResult3 , 0.0 ), UNITY_MATRIX_V ).xyz , 0.0 ), unity_ObjectToWorld ).xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float2 temp_cast_5 = (( _Time.y * _TimeScale1 )).xx;
			float2 uv_TexCoord66 = v.texcoord.xy * ase_worldPos.xy + temp_cast_5;
			float simplePerlin2D70 = snoise( uv_TexCoord66*_Scale1 );
			simplePerlin2D70 = simplePerlin2D70*0.5 + 0.5;
			float3 appendResult41 = (float3(( break39.x + ( ( simplePerlin2D70 - 0.5 ) * _Strength1 ) ) , break39.y , break39.z));
			v.vertex.xyz += appendResult41;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float4 tex2DNode11 = tex2D( _Texture, uv_Texture );
			o.Albedo = ( _Color * tex2DNode11 ).rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			clip( tex2DNode11.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "LeavesNoTra"
}
/*ASEBEGIN
Version=17800
200.8;73.6;985;439;865.6602;-466.4074;1.921117;True;False
Node;AmplifyShaderEditor.RangedFloatNode;19;-964.1958,264.5416;Inherit;False;Property;_LeavesScale;LeavesScale;5;0;Create;True;0;0;False;0;0;0.37;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-839.4448,365.6028;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;1;-808.8521,97.97748;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;22;-710.4448,364.6028;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-753.1958,239.5416;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-1080.31,1144.584;Inherit;False;Property;_TimeScale1;TimeScale;1;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;62;-1093.693,1237.07;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;64;-1099.197,803.2211;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-893.8092,1179.96;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;2;-593.4761,165.5667;Inherit;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;-2,-2;False;4;FLOAT2;2,2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;-390.875,245.3709;Inherit;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-522.2962,833.9082;Inherit;False;Property;_Scale1;Scale;7;0;Create;True;0;0;False;0;1.08;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;66;-538.3482,945.397;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewMatrixNode;5;-382.5562,353.3956;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-238.875,322.3709;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;70;-285.6812,825.5542;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;3.21;False;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode;7;-309.5989,464.6786;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;71;-25.4727,838.3352;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-33.34007,1085.066;Inherit;False;Property;_Strength1;Strength;6;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-118.0304,390.393;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;39;41.29199,572.3845;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;144.1033,879.9859;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;346.6931,791.4684;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-55.29709,-259.4928;Inherit;True;Property;_Texture;Texture;2;0;Create;True;0;0;False;0;-1;None;4de630452063de545a77d20d705bf334;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;9;-13.23969,-448.3964;Inherit;False;Property;_Color;Color ;0;0;Create;True;0;0;False;0;0.1942862,0.6981132,0.2443751,0;0.5080703,0.8396226,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;284.5652,-265.0581;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;41;508.1354,587.0592;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;18;132.5564,271.0642;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;False;0;0.49;0.156;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;444.4519,79.16443;Float;False;True;-1;2;LeavesNoTra;0;0;Standard;SNature/LeavesNoTra;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.91;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;3;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;21;0;19;0
WireConnection;22;0;21;0
WireConnection;22;1;21;0
WireConnection;20;0;19;0
WireConnection;20;1;19;0
WireConnection;65;0;62;0
WireConnection;65;1;63;0
WireConnection;2;0;1;0
WireConnection;2;3;22;0
WireConnection;2;4;20;0
WireConnection;3;0;2;0
WireConnection;66;0;64;0
WireConnection;66;1;65;0
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;70;0;66;0
WireConnection;70;1;67;0
WireConnection;71;0;70;0
WireConnection;6;0;4;0
WireConnection;6;1;7;0
WireConnection;39;0;6;0
WireConnection;74;0;71;0
WireConnection;74;1;75;0
WireConnection;56;0;39;0
WireConnection;56;1;74;0
WireConnection;10;0;9;0
WireConnection;10;1;11;0
WireConnection;41;0;56;0
WireConnection;41;1;39;1
WireConnection;41;2;39;2
WireConnection;0;0;10;0
WireConnection;0;4;18;0
WireConnection;0;10;11;4
WireConnection;0;11;41;0
ASEEND*/
//CHKSM=FDF08E819DE4732617F44DD70EF4D16CED909479