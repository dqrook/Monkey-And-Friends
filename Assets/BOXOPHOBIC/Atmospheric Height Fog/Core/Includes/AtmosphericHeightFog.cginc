/*

// Add the following directive

		#include "Assets/BOXOPHOBIC/Atmospheric Height Fog/Core/Library/AtmosphericHeightFog.cginc"

// Apply Atmospheric Height Fog to transparent shaders like this
// Where finalColor is the shader output color, fogParams.rgb is the fog color and fogParams.a is the fog mask

		float4 fogParams = GetAtmosphericHeightFog(i.worldPos);
		return ApplyAtmosphericHeightFog(finalColor, fogParams);

*/

#ifndef ATMOSPHERIC_HEIGHT_FOG_INCLUDED
#define ATMOSPHERIC_HEIGHT_FOG_INCLUDED

#include "UnityCG.cginc"
#include "UnityShaderVariables.cginc"

uniform half _DirectionalCat;
uniform half _SkyboxCat;
uniform half _FogCat;
uniform half _NoiseCat;
uniform half _FogAxisMode;
uniform half4 AHF_FogColorStart;
uniform half4 AHF_FogColorEnd;
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
uniform half AHF_FogIntensity;
float4 mod289(float4 x)
{
	return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float4 perm(float4 x)
{
	return mod289(((x * 34.0) + 1.0) * x);
}

float SimpleNoise3D(float3 p)
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

// Returns the fog color and alpha based on world position
float4 GetAtmosphericHeightFog(float3 positionWS)
{
	float4 finalColor;

	float3 WorldPosition = positionWS;

	float3 WorldPosition2_g915 = WorldPosition;
	float temp_output_7_0_g920 = AHF_FogDistanceStart;
	half FogDistanceMask12_g915 = pow(abs(saturate(((distance(WorldPosition2_g915, _WorldSpaceCameraPos) - temp_output_7_0_g920) / (AHF_FogDistanceEnd - temp_output_7_0_g920)))), AHF_FogDistanceFalloff);
	float3 lerpResult258_g915 = lerp((AHF_FogColorStart).rgb, (AHF_FogColorEnd).rgb, (saturate((FogDistanceMask12_g915 - 0.5)) * AHF_FogColorDuo));
	float3 normalizeResult318_g915 = normalize((WorldPosition - _WorldSpaceCameraPos));
	float dotResult145_g915 = dot(normalizeResult318_g915, AHF_DirectionalDir);
	float temp_output_319_0_g915 = pow(abs(((dotResult145_g915*0.5 + 0.5) * AHF_DirectionalIntensity)), AHF_DirectionalFalloff);
	float DirectionalMask30_g915 = temp_output_319_0_g915;
	float3 lerpResult40_g915 = lerp(lerpResult258_g915, (AHF_DirectionalColor).rgb, DirectionalMask30_g915);
#ifdef AHF_DISABLE_DIRECTIONAL
	float3 staticSwitch442_g915 = lerpResult258_g915;
#else
	float3 staticSwitch442_g915 = lerpResult40_g915;
#endif
	float3 temp_output_2_0_g918 = staticSwitch442_g915;
	float3 gammaToLinear3_g918 = GammaToLinearSpace(temp_output_2_0_g918);
#ifdef UNITY_COLORSPACE_GAMMA
	float3 staticSwitch1_g918 = temp_output_2_0_g918;
#else
	float3 staticSwitch1_g918 = gammaToLinear3_g918;
#endif
	float3 temp_output_256_0_g915 = staticSwitch1_g918;
	float3 temp_output_94_86_g914 = temp_output_256_0_g915;
	half3 AHF_FogAxisOption181_g915 = AHF_FogAxisOption;
	float3 break159_g915 = (WorldPosition2_g915 * AHF_FogAxisOption181_g915);
	float temp_output_7_0_g917 = AHF_FogHeightEnd;
	half FogHeightMask16_g915 = pow(abs(saturate((((break159_g915.x + break159_g915.y + break159_g915.z) - temp_output_7_0_g917) / (AHF_FogHeightStart - temp_output_7_0_g917)))), AHF_FogHeightFalloff);
	float lerpResult328_g915 = lerp((FogDistanceMask12_g915 * FogHeightMask16_g915), saturate((FogDistanceMask12_g915 + FogHeightMask16_g915)), AHF_FogLayersMode);
	float mulTime204_g915 = _Time.y * 2.0;
	float3 temp_output_197_0_g915 = ((WorldPosition2_g915 * (1.0 / AHF_NoiseScale)) + (-AHF_NoiseSpeed * mulTime204_g915));
	float3 p1_g921 = temp_output_197_0_g915;
	float localSimpleNoise3D1_g921 = SimpleNoise3D(p1_g921);
	float temp_output_7_0_g919 = AHF_NoiseDistanceEnd;
	half NoiseDistanceMask7_g915 = saturate(((distance(WorldPosition2_g915, _WorldSpaceCameraPos) - temp_output_7_0_g919) / (0.0 - temp_output_7_0_g919)));
	float lerpResult198_g915 = lerp(1.0, (localSimpleNoise3D1_g921*0.5 + 0.5), (NoiseDistanceMask7_g915 * AHF_NoiseIntensity));
	half NoiseSimplex3D24_g915 = lerpResult198_g915;
#ifdef AHF_DISABLE_NOISE3D
	float staticSwitch42_g915 = lerpResult328_g915;
#else
	float staticSwitch42_g915 = (lerpResult328_g915 * NoiseSimplex3D24_g915);
#endif
	float temp_output_43_0_g915 = (staticSwitch42_g915 * AHF_FogIntensity);
	float temp_output_94_87_g914 = temp_output_43_0_g915;
	float4 appendResult26 = (float4(temp_output_94_86_g914, temp_output_94_87_g914));


	finalColor = appendResult26;
	return finalColor;
}

// Applies the fog
float3 ApplyAtmosphericHeightFog(float3 color, float4 fog)
{
	return float3(lerp(color.rgb, fog.rgb, fog.a));
}

float4 ApplyAtmosphericHeightFog(float4 color, float4 fog)
{
	return float4(lerp(color.rgb, fog.rgb, fog.a), color.a);
}

#endif
