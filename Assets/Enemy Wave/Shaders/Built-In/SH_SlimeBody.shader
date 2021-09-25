Shader "Neon2/Enemy Wave/Built-in/SH_SphereSlime"
{
    Properties
    {
		_MainTex ("Texture", 2D) = "white" {}
			 
		[PerRendererData]_Face("Face", Vector) = (0, 0, 0, 0)
		[PerRendererData]_TopColor("Top Color", Color) = (1, 1, 1, 1)
		[PerRendererData]_SecondColor("Second Color", Color) = (1, 1, 1, 1)

		_Velocity("Body Velocity", Vector) = (0, 0, 0, 0)

		_BodyTexture("Slime Body Tex", 2D) = "white" {}
		_RimWidth("Rim Width", Range(0, 2)) = 0.7

		_Cube("Cubemap", Cube) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
			#pragma multi_compile_instancing

			#pragma shader_feature_local COLOR_TEXTURE
			#pragma shader_feature_local BICOLOR

            #pragma vertex vert
            #pragma fragment frag

			#include "UnityCG.cginc"

            struct appdata
            {
				float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal: NORMAL;
				fixed4 color: COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 faceUV : FACE_UV;

#if COLOR_TEXTURE
				float2 uvDisplaced : UV_DISPLACED;
#endif

                float4 vertex : SV_POSITION;

#if !COLOR_TEXTURE
				fixed4 bodyColor : BODY_COLOR;
#endif

				half rimPower : RIM_POWER;
				half3 reflectedDir : REFLECTED_DIR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
            };


            sampler2D _MainTex;
            float4 _MainTex_ST;

			
			samplerCUBE _Cube;
			half _RimWidth;
			


#if COLOR_TEXTURE
			sampler2D _BodyTexture;
			fixed4 _Velocity;
#endif

			static const fixed _Rows = 6;
			static const fixed _Cols = 6;
			static const half horizontalSpace = 1 / _Rows;
			static const half verticalSpace = 1 / _Cols;

			UNITY_INSTANCING_BUFFER_START(InstanceProperties)

				UNITY_DEFINE_INSTANCED_PROP(half4, _Face)
				//#define _Face_arr InstanceProperties

#if !COLOR_TEXTURE
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _TopColor)
				//#define _TopColor_arr InstanceProperties
#endif

#if BICOLOR
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _SecondColor)
				//#define _SecondColor_arr InstanceProperties
#endif

			UNITY_INSTANCING_BUFFER_END(InstanceProperties)
			

            v2f vert (appdata v)
            {
                v2f o;

				UNITY_SETUP_INSTANCE_ID(v); 
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);


                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

#if COLOR_TEXTURE
				o.uvDisplaced = v.uv;
				o.uvDisplaced.x += _Time.x * _Velocity.x; 
				o.uvDisplaced.y += _Time.y * _Velocity.y;
#endif


#if !COLOR_TEXTURE
				o.bodyColor = UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _TopColor);
#endif

#if BICOLOR
				o.bodyColor = lerp(
					UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _TopColor),
					UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _SecondColor),
					v.color.g);
#endif
					
				o.faceUV = v.uv;

				
				o.faceUV.x = (horizontalSpace * v.uv.x) + (UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _Face).y * horizontalSpace);
				o.faceUV.y = (verticalSpace * v.uv.y) + (UNITY_ACCESS_INSTANCED_PROP(InstanceProperties, _Face).x * verticalSpace);

				half3 vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);
				half3 viewDir = normalize(_WorldSpaceCameraPos - vertexWorldPos);
				half3 normal = UnityObjectToWorldNormal(v.normal);

				o.reflectedDir = reflect(viewDir, normal);
			

				o.rimPower = saturate(dot(viewDir, normal));
				o.rimPower = smoothstep(1  - _RimWidth, 1, o.rimPower);
		

				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				UNITY_SETUP_INSTANCE_ID(i);

				
#if COLOR_TEXTURE
				fixed4 res = tex2D(_BodyTexture, i.uvDisplaced);
#else
				fixed4 res = i.bodyColor;
				
#endif
				
				fixed4 faceTex = tex2D(_MainTex, i.faceUV);
				fixed4 cube = texCUBE(_Cube, i.reflectedDir) * 1.5 * (1 - i.rimPower);
				
				res += cube;

				res = lerp(res, faceTex, faceTex.a);






				return res;
            }
            ENDCG
        }
    }

	CustomEditor "SHSphereSlimeCustomEditor"
}