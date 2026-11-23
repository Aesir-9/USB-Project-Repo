float easing(float t)
{
    return t * t * (3.0 - 2.0 * t);
}

void CustomAmbient_float(
in half3 BaseColor,
in float3 Sky,
in float3 Equator,
in float3 Ground,
in half3 Normal,
out float3 Out)
{
    
    float t = Normal.y * 0.5 + 0.5;
    float t_AB = easing(saturate(t * 2.0));
    float t_BC = easing(saturate((t - 0.5) * 2.0));
    
    float3 color_AB = lerp(Ground, Equator, t_AB);
    float3 color_BC = lerp(color_AB, Sky, t_BC);
    color_BC *= BaseColor;

    Out = color_BC;
}