void BlinnPhong_float(
    in float3 Color,
    in float SpecularFactor,
    in float Roughness,
    in float3 Normal,
    in float3 LightDirection,
    in float3 ViewDirection,
    out float3 Out)
{
    float3 h = normalize(LightDirection + ViewDirection);
    float m = lerp(256.0, 0.0, Roughness);
    float3 specular = pow(max(0.0, dot(Normal, h)), m);
    specular *= max(0.0, dot(Normal, LightDirection));    
    specular *= SpecularFactor;    
    specular *= Color;
    
    Out = specular;
}