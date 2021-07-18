// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hidden/BOXOPHOBIC/Atmospherics/Height Fog Global"
{
	Properties
	{
		[HideInInspector]_HeightFogGlobal("_HeightFogGlobal", Float) = 1
		[HideInInspector]_IsHeightFogShader("_IsHeightFogShader", Float) = 1
		[StyledBanner(Height Fog Global)]_Banner("[ Banner ]", Float) = 1

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Overlay" "Queue"="Overlay" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Front
		ColorMask RGBA
		ZWrite Off
		ZTest Always
		Stencil
		{
			Ref 222
			Comp NotEqual
			Pass Zero
		}
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" "PreviewType"="Skybox" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			//Atmospheric Height Fog Defines
			//#define AHF_DISABLE_NOISE3D
			//#define AHF_DISABLE_DIRECTIONAL


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _IsHeightFogShader;
			uniform half _HeightFogGlobal;
			uniform half _Banner;
			uniform half4 AHF_FogColorStart;
			uniform half4 AHF_FogColorEnd;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform half AHF_FogDistanceStart;
			uniform half AHF_FogDistanceEnd;
			uniform half AHF_FogDistanceFalloff;
			uniform half AHF_FogColorDuo;
			uniform half4 AHF_DirectionalColor;
			uniform half3 AHF_DirectionalDir;
			uniform half AHF_DirectionalIntensity;
			uniform half AHF_DirectionalFalloff;
			uniform half3 AHF_FogAxisOption;
			uniform half AHF_FogHeightEnd;
			uniform half AHF_FogHeightStart;
			uniform half AHF_FogHeightFalloff;
			uniform half AHF_FogLayersMode;
			uniform half AHF_NoiseScale;
			uniform half3 AHF_NoiseSpeed;
			uniform half AHF_NoiseDistanceEnd;
			uniform half AHF_NoiseIntensity;
			uniform half AHF_SkyboxFogOffset;
			uniform half AHF_SkyboxFogHeight;
			uniform half AHF_SkyboxFogFalloff;
			uniform half AHF_SkyboxFogFill;
			uniform half AHF_SkyboxFogIntensity;
			uniform half AHF_FogIntensity;
			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				    float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				    float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				    float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				    float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 UnStereo( float2 UV )
			{
				#if UNITY_SINGLE_PASS_STEREO
				float4 scaleOffset = unity_StereoScaleOffset[ unity_StereoEyeIndex];
				UV.xy = (UV.xy - scaleOffset.zw) / scaleOffset.xy;
				#endif
				return UV;
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float4 screenPos = i.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float2 UV235_g1 = ase_screenPosNorm.xy;
				float2 localUnStereo235_g1 = UnStereo( UV235_g1 );
				float2 break248_g1 = localUnStereo235_g1;
				float clampDepth227_g1 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
				#ifdef UNITY_REVERSED_Z
				float staticSwitch250_g1 = ( 1.0 - clampDepth227_g1 );
				#else
				float staticSwitch250_g1 = clampDepth227_g1;
				#endif
				float3 appendResult244_g1 = (float3(break248_g1.x , break248_g1.y , staticSwitch250_g1));
				float4 appendResult220_g1 = (float4((appendResult244_g1*2.0 + -1.0) , 1.0));
				float4 break229_g1 = mul( unity_CameraInvProjection, appendResult220_g1 );
				float3 appendResult237_g1 = (float3(break229_g1.x , break229_g1.y , break229_g1.z));
				float4 appendResult233_g1 = (float4(( ( appendResult237_g1 / break229_g1.w ) * half3(1,1,-1) ) , 1.0));
				float4 break245_g1 = mul( unity_CameraToWorld, appendResult233_g1 );
				float3 appendResult239_g1 = (float3(break245_g1.x , break245_g1.y , break245_g1.z));
				float3 WorldPositionFromDepth253_g1 = appendResult239_g1;
				float3 WorldPosition2_g1 = WorldPositionFromDepth253_g1;
				float temp_output_7_0_g939 = AHF_FogDistanceStart;
				half FogDistanceMask12_g1 = pow( abs( saturate( ( ( distance( WorldPosition2_g1 , _WorldSpaceCameraPos ) - temp_output_7_0_g939 ) / ( AHF_FogDistanceEnd - temp_output_7_0_g939 ) ) ) ) , AHF_FogDistanceFalloff );
				float3 lerpResult258_g1 = lerp( (AHF_FogColorStart).rgb , (AHF_FogColorEnd).rgb , ( saturate( ( FogDistanceMask12_g1 - 0.5 ) ) * AHF_FogColorDuo ));
				float3 normalizeResult318_g1 = normalize( ( WorldPosition2_g1 - _WorldSpaceCameraPos ) );
				float dotResult145_g1 = dot( normalizeResult318_g1 , AHF_DirectionalDir );
				float DirectionalMask30_g1 = pow( abs( ( (dotResult145_g1*0.5 + 0.5) * AHF_DirectionalIntensity ) ) , AHF_DirectionalFalloff );
				float3 lerpResult40_g1 = lerp( lerpResult258_g1 , (AHF_DirectionalColor).rgb , DirectionalMask30_g1);
				#ifdef AHF_DISABLE_DIRECTIONAL
				float3 staticSwitch442_g1 = lerpResult258_g1;
				#else
				float3 staticSwitch442_g1 = lerpResult40_g1;
				#endif
				float3 temp_output_2_0_g940 = staticSwitch442_g1;
				float3 gammaToLinear3_g940 = GammaToLinearSpace( temp_output_2_0_g940 );
				#ifdef UNITY_COLORSPACE_GAMMA
				float3 staticSwitch1_g940 = temp_output_2_0_g940;
				#else
				float3 staticSwitch1_g940 = gammaToLinear3_g940;
				#endif
				float3 temp_output_256_0_g1 = staticSwitch1_g940;
				half3 AHF_FogAxisOption181_g1 = AHF_FogAxisOption;
				float3 break159_g1 = ( WorldPosition2_g1 * AHF_FogAxisOption181_g1 );
				float temp_output_7_0_g937 = AHF_FogHeightEnd;
				half FogHeightMask16_g1 = pow( abs( saturate( ( ( ( break159_g1.x + break159_g1.y + break159_g1.z ) - temp_output_7_0_g937 ) / ( AHF_FogHeightStart - temp_output_7_0_g937 ) ) ) ) , AHF_FogHeightFalloff );
				float lerpResult328_g1 = lerp( ( FogDistanceMask12_g1 * FogHeightMask16_g1 ) , saturate( ( FogDistanceMask12_g1 + FogHeightMask16_g1 ) ) , AHF_FogLayersMode);
				float mulTime204_g1 = _Time.y * 2.0;
				float3 temp_output_197_0_g1 = ( ( WorldPosition2_g1 * ( 1.0 / AHF_NoiseScale ) ) + ( -AHF_NoiseSpeed * mulTime204_g1 ) );
				float3 p1_g936 = temp_output_197_0_g1;
				float localSimpleNoise3D1_g936 = SimpleNoise3D( p1_g936 );
				float temp_output_7_0_g934 = AHF_NoiseDistanceEnd;
				half NoiseDistanceMask7_g1 = saturate( ( ( distance( WorldPosition2_g1 , _WorldSpaceCameraPos ) - temp_output_7_0_g934 ) / ( 0.0 - temp_output_7_0_g934 ) ) );
				float lerpResult198_g1 = lerp( 1.0 , (localSimpleNoise3D1_g936*0.5 + 0.5) , ( NoiseDistanceMask7_g1 * AHF_NoiseIntensity ));
				half NoiseSimplex3D24_g1 = lerpResult198_g1;
				#ifdef AHF_DISABLE_NOISE3D
				float staticSwitch42_g1 = lerpResult328_g1;
				#else
				float staticSwitch42_g1 = ( lerpResult328_g1 * NoiseSimplex3D24_g1 );
				#endif
				float3 normalizeResult169_g1 = normalize( ( WorldPosition2_g1 - _WorldSpaceCameraPos ) );
				float3 break170_g1 = ( normalizeResult169_g1 * AHF_FogAxisOption181_g1 );
				float temp_output_7_0_g938 = AHF_SkyboxFogHeight;
				float saferPower309_g1 = max( abs( saturate( ( ( abs( ( ( break170_g1.x + break170_g1.y + break170_g1.z ) + -AHF_SkyboxFogOffset ) ) - temp_output_7_0_g938 ) / ( 0.0 - temp_output_7_0_g938 ) ) ) ) , 0.0001 );
				float lerpResult179_g1 = lerp( pow( saferPower309_g1 , AHF_SkyboxFogFalloff ) , 1.0 , AHF_SkyboxFogFill);
				half SkyboxFogHeightMask108_g1 = ( lerpResult179_g1 * AHF_SkyboxFogIntensity );
				float clampDepth118_g1 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
				#ifdef UNITY_REVERSED_Z
				float staticSwitch123_g1 = clampDepth118_g1;
				#else
				float staticSwitch123_g1 = ( 1.0 - clampDepth118_g1 );
				#endif
				half SkyboxMask95_g1 = ( 1.0 - ceil( staticSwitch123_g1 ) );
				float lerpResult112_g1 = lerp( staticSwitch42_g1 , SkyboxFogHeightMask108_g1 , SkyboxMask95_g1);
				float temp_output_43_0_g1 = ( lerpResult112_g1 * AHF_FogIntensity );
				float4 appendResult114_g1 = (float4(temp_output_256_0_g1 , temp_output_43_0_g1));
				
				
				finalColor = appendResult114_g1;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "HeightFogShaderGUI"
	
	
}
/*ASEBEGIN
Version=18800
1920;1;1906;1021;4019.594;5119.538;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;885;-2912,-4864;Half;False;Property;_IsHeightFogShader;_IsHeightFogShader;33;1;[HideInInspector];Create;False;0;0;0;True;0;False;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;879;-3136,-4864;Half;False;Property;_HeightFogGlobal;_HeightFogGlobal;32;1;[HideInInspector];Create;False;0;0;0;True;0;False;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;892;-3328,-4864;Half;False;Property;_Banner;[ Banner ];34;0;Create;True;0;0;0;True;1;StyledBanner(Height Fog Global);False;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1041;-3328,-4608;Inherit;False;Base;0;;1;13c50910e5b86de4097e1181ba121e0e;27,99,1,374,0,372,0,370,0,347,0,376,0,343,0,382,0,392,0,366,0,354,0,345,0,368,0,116,1,450,0,378,0,380,0,364,0,388,0,355,0,384,0,360,0,349,0,386,0,339,0,351,0,361,0;0;4;FLOAT4;113;FLOAT3;86;FLOAT;87;FLOAT;445
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;383;-3072,-4608;Float;False;True;-1;2;HeightFogShaderGUI;0;1;Hidden/BOXOPHOBIC/Atmospherics/Height Fog Global;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;True;0;False;-1;True;1;False;-1;True;True;True;True;True;0;False;-1;False;False;False;True;True;222;False;-1;255;False;-1;255;False;-1;6;False;-1;2;False;-1;0;False;-1;0;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;594;True;7;False;595;True;False;0;False;500;1000;False;500;True;2;RenderType=Overlay=RenderType;Queue=Overlay=Queue=0;True;2;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;LightMode=ForwardBase;PreviewType=Skybox;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
Node;AmplifyShaderEditor.CommentaryNode;880;-3328,-4992;Inherit;False;919.8825;100;Drawers;0;;1,0.475862,0,1;0;0
WireConnection;383;0;1041;113
ASEEND*/
//CHKSM=015C67B180477C22BC40A074542A32576B53CE03