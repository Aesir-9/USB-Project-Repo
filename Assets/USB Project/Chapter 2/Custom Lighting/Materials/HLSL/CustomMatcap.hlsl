void CustomMatcap_half ( 
    in UnityTexture2D Matcap,
    in float3 Mask,
    in float3 Normal,
    in float3 Position,    
    in SamplerState SS,   
    out float3 Out)
{    
    float3 projectedNormal = cross(Normal, normalize(Position));
    float2 matcapUV = float2(projectedNormal.y, projectedNormal.x * -1.0);
    matcapUV *= 0.5;
    matcapUV += 0.5;
    half4 matcapColor = SAMPLE_TEXTURE2D(Matcap, Matcap.samplerstate, matcapUV);
    matcapColor.rgb *= Mask;
    Out = matcapColor.rgb;
}