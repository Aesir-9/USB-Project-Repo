void CustomLambert_half(
    in float3 Normal,
    in float3 LightDirection,
    in float3 LightIntensity,
    in float3 BaseColor,
    out float3 Out)
{
    half NdotL = max(0.0, dot(Normal, LightDirection));
    Out = NdotL * LightIntensity * BaseColor;
}

float dot(float3 a, float3 b)
{
    return a.x * b.x + a.y * b.y + a.z * b.z;
}