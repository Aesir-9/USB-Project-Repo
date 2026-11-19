#include "Common/Common.hlsl"

void CustomLambert_half(
    in float3 Normal,
    in float3 LightDirection,
    in float3 LightIntensity,
    in float3 BaseColor,
    out float3 Out)
{
    half NdotL = lambert(Normal, LightDirection);
    Out = NdotL * LightIntensity * BaseColor;
}