#ifndef CUSTOM_BOLT
#define CUSTOM_BOLT

float3 antialiasing(float3 g, float e)
{
    float3 roc = fwidth(g);
    float3 le = e - roc;
    float3 ue = e + roc;
    float3 s = (g - le) / (ue - le);
    return saturate(s);
}

float _Long;

void CustomBolt_float(
    in float2 UV,
    in bool OneMinus,
    in float Time,
    in float Speed,
    in float Threshold,
    in float Frequency,
    in float Thickness,
    in float Noise,
    out float Out)
{    
    float time = Time * Speed;
    float n = Noise;
    float x = UV.x + time * 0.15;
    float q = Frequency * 10.0; //_Long

    if (OneMinus)
    {
        UV.y = 1.0 - UV.y;        
    }
    
    UV.x = UV.x * (cos(x / 0.08) * sin(x * q + n) * 0.5 + 0.5) * (1.0 - UV.x);

    float t = fmod(time, Threshold);
    float u = UV.x - 0.5;
    float v = UV.y - 0.5;
    float f = max(0.0, -(u * u) * 4.0 * t + t);

    float r = abs(v - f);
    float arcThickness = Thickness;
    
    Out = 1.0 - antialiasing(r, arcThickness);
}

#endif