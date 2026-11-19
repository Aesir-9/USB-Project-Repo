#include "Common/Common.hlsl"

void GGX_float(
    in float3 Color,
    in float SpecularFactor,
    in float Roughness,
    in float3 Normal,
    in float3 ViewDirection,
    in float3 LightDirection,    
    out float3 Out)
{
    float3 h = normalize(ViewDirection + LightDirection);

    float NdotL = saturate(dot(Normal, LightDirection));
    float NdotV = saturate(dot(Normal, ViewDirection));
    float NdotH = saturate(dot(Normal, h));
    float VdotH = saturate(dot(ViewDirection, h));

    float a = Roughness * Roughness;  
   
    float D = distribution(a, NdotH);   
    float F = schlick_fresnel(SpecularFactor, VdotH); 
    float G = geometry(a, NdotV, NdotL);

    float3 specular = (D * F * G) / (4.0 * NdotV * NdotL);
    specular *= Color;
    specular = saturate(specular);
    
    Out = specular;
}