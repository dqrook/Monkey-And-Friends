Shader "Tzar/VerticalFog"
{
    Properties
    {
        [CurvedWorldBendSettings] _CurvedWorldBendSettings("0|1|1", Vector) = (0, 0, 0, 0)
       _Color("Main Color", Color) = (1, 1, 1, .5)
       _Intensity("Intensity", float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent"  }
  
        Pass
        {
           Blend SrcAlpha OneMinusSrcAlpha
           ZWrite Off
           CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
           #pragma multi_compile_fog
           #include "UnityCG.cginc"

            #define CURVEDWORLD_BEND_TYPE_CLASSICRUNNER_X_POSITIVE
            #define CURVEDWORLD_BEND_ID_1
            #pragma shader_feature_local CURVEDWORLD_DISABLED_ON
            #pragma shader_feature_local CURVEDWORLD_NORMAL_TRANSFORMATION_ON
            #include "Assets/Amazing Assets/Curved World/Shaders/Core/CurvedWorldTransform.cginc"
  
           struct appdata
           {
               float4 vertex : POSITION;
           };
  
           struct v2f
           {
               float4 scrPos : TEXCOORD0;
               UNITY_FOG_COORDS(1)
               float4 vertex : SV_POSITION;
           };

		 
  
           sampler2D _CameraDepthTexture;
           float4 _Color;
           float4 _IntersectionColor;
           float _Intensity;
  
           v2f vert(appdata v)
           {
               v2f o;
               #if defined(CURVEDWORLD_IS_INSTALLED) && !defined(CURVEDWORLD_DISABLED_ON)
                    CURVEDWORLD_TRANSFORM_VERTEX(v.vertex)
                #endif
               o.vertex = UnityObjectToClipPos(v.vertex);
               o.scrPos = ComputeScreenPos(o.vertex);
               UNITY_TRANSFER_FOG(o,o.vertex);
               return o;   
           }
  
  
            half4 frag(v2f i) : SV_TARGET
            {
               float depth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
               float diff = saturate(_Intensity * (depth - i.scrPos.w));
  
               fixed4 col = lerp(fixed4(_Color.rgb, 0.0), _Color, diff * diff * diff * (diff * (6 * diff - 15) + 10));
  
               UNITY_APPLY_FOG(i.fogCoord, col);
               return col;
            }
  
            ENDCG
        }
    }
}
