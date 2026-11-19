#include "Common/Common.hlsl"

void AshikhminShirley_float(
    in float3 Color,
    in float SpecularFactor,
    in float AU,
    in float AV,
    in float3 Normal,
    in float3 LightDirection,    
    in float3 ViewDirection,
    in float3 Tangent,
    in float3 Bitangent,
    out float3 Out)
{    
    float3 h = normalize(LightDirection + ViewDirection);
    float e = 1e-4;

    float NdotL = max(dot(Normal, LightDirection), e);
    float NdotV = max(dot(Normal, ViewDirection), e);
    float NdotH = max(dot(Normal, h), e);
    float VdotH = max(dot(ViewDirection, h), e);

    float HdotT = dot(h, Tangent);
    float HdotB = dot(h, Bitangent);

    float denom = max(1.0 - NdotH * NdotH, e);
    float exponent = (AU * HdotT * HdotT + AV * HdotB * HdotB) / denom;

    float3 specular = sqrt((AU + 1.0) * (AV + 1.0)) * pow(NdotH, exponent);
    specular /= (8.0 * PI) * VdotH * max(NdotL, NdotV);
    
    float fresnel = schlick_fresnel(SpecularFactor, VdotH);
    specular *= fresnel;   
    specular *= lambert(Normal, LightDirection);
    specular = saturate(specular);
    specular *= Color;    

    Out = specular;
}