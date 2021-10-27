// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Bumped shader. Differences from regular Bumped one:
// - no Main Color
// - Normalmap uses Tiling/Offset of the Base texture
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Amazing Assets/Curved World/Mobile/Bumped Cutout" 
{
    Properties 
    {
        [HideInInspector][CurvedWorldBendSettings]	  _CurvedWorldBendSettings("0|1|1", Vector) = (0, 0, 0, 0)

        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
    }

    SubShader 
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
        LOD 250

        CGPROGRAM
        #pragma surface surf Lambert noforwardadd vertex:vert alphatest:_Cutoff addshadow


#define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
#define CURVEDWORLD_BEND_ID_1
#pragma shader_feature_local CURVEDWORLD_DISABLED_ON
#pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
#include "../../Core/CurvedWorldTransform.cginc" 


        sampler2D _MainTex;
        sampler2D _BumpMap;
        fixed4 _Color;

        void vert (inout appdata_full v) 
        {
            #if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
                #ifdef CURVEDWORLD_NORMAL_TRANSFORMATION_ON
                    CURVEDWORLD_TRANSFORM_VERTEX_AND_NORMAL(v.vertex, v.normal, v.tangent)
                #else
                    CURVEDWORLD_TRANSFORM_VERTEX(v.vertex)
                #endif
            #endif
        }

        struct Input 
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) 
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
        }
        ENDCG
    }

    FallBack "Hidden/Amazing Assets/Curved World/Fallback/VertexLit"

    CustomEditor "AmazingAssets.CurvedWorldEditor.DefaultShaderGUI"
}
