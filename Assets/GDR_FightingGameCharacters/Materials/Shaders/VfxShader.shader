// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32727,y:32846,varname:node_4795,prsc:2|emission-2635-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32235,y:32601,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32446,y:32756,varname:node_2393,prsc:2|A-6074-RGB,B-2053-RGB,C-797-RGB,D-9248-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32235,y:32772,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:32235,y:32920,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:32235,y:33081,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_Panner,id:2702,x:31476,y:33212,varname:node_2702,prsc:2,spu:1,spv:1|UVIN-3720-UVOUT,DIST-3542-OUT;n:type:ShaderForge.SFN_Panner,id:1468,x:31462,y:33447,varname:node_1468,prsc:2,spu:1,spv:1|UVIN-3720-UVOUT,DIST-3542-OUT;n:type:ShaderForge.SFN_TexCoord,id:3720,x:31268,y:33212,varname:node_3720,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:9209,x:32021,y:33202,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_9209,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7571-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8884,x:32192,y:33361,ptovrint:False,ptlb:Mask_Stregth,ptin:_Mask_Stregth,varname:node_8884,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Time,id:8399,x:31072,y:33545,varname:node_8399,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3542,x:31268,y:33456,varname:node_3542,prsc:2|A-5995-OUT,B-8399-T;n:type:ShaderForge.SFN_Slider,id:5995,x:30948,y:33456,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_5995,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_SwitchProperty,id:7571,x:31810,y:33202,ptovrint:False,ptlb:VerticleScroll,ptin:_VerticleScroll,varname:node_7571,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2702-UVOUT,B-5980-OUT;n:type:ShaderForge.SFN_Slider,id:4761,x:32095,y:33602,ptovrint:False,ptlb:Brightness,ptin:_Brightness,varname:node_386,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Add,id:5980,x:31656,y:33359,varname:node_5980,prsc:2|A-2702-UVOUT,B-1468-UVOUT;n:type:ShaderForge.SFN_Multiply,id:2635,x:32497,y:32933,varname:node_2635,prsc:2|A-2393-OUT,B-1515-OUT,C-4276-OUT,D-8884-OUT,E-4761-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5357,x:31855,y:33448,ptovrint:False,ptlb:Fresnel Exp,ptin:_FresnelExp,varname:node_3502,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.25;n:type:ShaderForge.SFN_Fresnel,id:9386,x:32055,y:33426,varname:node_9386,prsc:2|EXP-5357-OUT;n:type:ShaderForge.SFN_OneMinus,id:4276,x:32220,y:33426,varname:node_4276,prsc:2|IN-9386-OUT;n:type:ShaderForge.SFN_Multiply,id:962,x:32038,y:33012,varname:node_962,prsc:2|A-671-OUT,B-7572-A;n:type:ShaderForge.SFN_Tex2d,id:7572,x:31810,y:33012,ptovrint:False,ptlb:DetailMask,ptin:_DetailMask,varname:node_3633,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7571-OUT;n:type:ShaderForge.SFN_Multiply,id:1515,x:32235,y:33157,varname:node_1515,prsc:2|A-962-OUT,B-9209-A;n:type:ShaderForge.SFN_Slider,id:671,x:31653,y:32897,ptovrint:False,ptlb:DetailMask_Strength,ptin:_DetailMask_Strength,varname:node_671,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:4;proporder:7571-797-6074-5357-7572-9209-8884-5995-4761-671-611;pass:END;sub:END;*/

Shader "Shader Forge/VfxShader" {
    Properties {
        [MaterialToggle] _VerticleScroll ("VerticleScroll", Float ) = 0
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _FresnelExp ("Fresnel Exp", Float ) = 0.25
        _DetailMask ("DetailMask", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}
        _Mask_Stregth ("Mask_Stregth", Float ) = 1
        _Speed ("Speed", Range(0, 2)) = 1
        _Brightness ("Brightness", Range(0, 3)) = 1
        _DetailMask_Strength ("DetailMask_Strength", Range(1, 4)) = 1
        _Slider ("Slider", Range(0, 1)) = 0.1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Mask_Stregth;
            uniform float _Speed;
            uniform fixed _VerticleScroll;
            uniform float _Brightness;
            uniform float _FresnelExp;
            uniform sampler2D _DetailMask; uniform float4 _DetailMask_ST;
            uniform float _DetailMask_Strength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_8399 = _Time;
                float node_3542 = (_Speed*node_8399.g);
                float2 node_2702 = (i.uv0+node_3542*float2(1,1));
                float2 _VerticleScroll_var = lerp( node_2702, (node_2702+(i.uv0+node_3542*float2(1,1))), _VerticleScroll );
                float4 _DetailMask_var = tex2D(_DetailMask,TRANSFORM_TEX(_VerticleScroll_var, _DetailMask));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(_VerticleScroll_var, _Mask));
                float3 emissive = ((_MainTex_var.rgb*i.vertexColor.rgb*_TintColor.rgb*2.0)*((_DetailMask_Strength*_DetailMask_var.a)*_Mask_var.a)*(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelExp))*_Mask_Stregth*_Brightness);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
