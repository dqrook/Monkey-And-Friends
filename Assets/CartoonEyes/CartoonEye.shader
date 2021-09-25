Shader "CartoonEye"
{
    Properties
    {
        _Outline("Outline", 2D) = "white" {}
        _OutlineColor("OutlineColor", Color) = (0,0,0,1)
        _OutlineThickness("OutlineThickness", Range(0 , 0.2)) = 0.5
        _Open("Open", Range(0 , 0.99)) = 0.95
        _ScleraColor("ScleraColor", Color) = (1,1,1,1)
        [NoScaleOffset]_IrisGradient("IrisGradient", 2D) = "white" {}
        _IrisColor("IrisColor", Color) = (1,1,1,1)
        _IrisSize("IrisSize", Float) = 0.001
        _PupilSize("PupilSize", Range(0.01 , 1)) = 0.2
        _AspectRatio("AspectRatio", Float) = 1
        _Position("Position", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            uniform float _Open;
            uniform float _OutlineThickness;
            uniform float4 _OutlineColor;
            uniform float4 _ScleraColor;
            uniform float4 _IrisColor;
            uniform sampler2D _IrisGradient;
            uniform float2 _Position;
            uniform float _PupilSize;
            uniform float _IrisSize;
            uniform float _AspectRatio;
            uniform sampler2D _Outline;
            uniform float4 _Outline_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 GetOutlineColor(float Open, float Thickness, float4 OutlineColor, float4 EyeBallColor, float OutlineMask)
            {
                float clampedopen = lerp(Thickness, 1 - Thickness, Open);
                float maskmax = clampedopen - Thickness;
                float4 result = lerp(OutlineColor, EyeBallColor, step(OutlineMask, maskmax));

                float maskmin = clampedopen + Thickness;
                result.a *= step(OutlineMask, maskmin);

                return result;
            }

            float GetIrisDistance(float2 Position, float2 TexCoord, float PupilSize, float IrisSize, float AspectRatio)
            {
                float2 center = Position;
                center.x = -center.x;
                center = center * 0.5 + 0.5;
                center.x = center.x * AspectRatio;
                float2 offset = float2(TexCoord.x * AspectRatio, TexCoord.y) - center;
                float dist = length(offset);
                float a = PupilSize * IrisSize;
                float b = IrisSize;
                float t = dist;
                return (t - a) / (b - a); // inverse lerp
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float localGetIrisDistance = GetIrisDistance(_Position , i.uv , _PupilSize , _IrisSize , _AspectRatio);
                float2 appendResult37 = float2(localGetIrisDistance , 0.0);
                float4 tex2DNode13 = tex2D(_IrisGradient, appendResult37);
                float2 uv_Outline = i.uv * _Outline_ST.xy + _Outline_ST.zw;
                float4 color = GetOutlineColor(_Open , _OutlineThickness , _OutlineColor , lerp(_ScleraColor , (_IrisColor * tex2DNode13) , tex2DNode13.a) , tex2D(_Outline, uv_Outline).r);
                clip(color.a - 0.5);
                return color;
            }
            ENDCG
        }
    }
}
