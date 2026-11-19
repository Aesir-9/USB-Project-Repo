#ifndef __SDPARTICLES_H__
#define __SDPARTICLES_H__

// this number must match the MaxParticles in VFXPortalController
#define MAX_PARTICLES 100

float4 _ParticlesPosition[MAX_PARTICLES];
float _ParticlesSize[MAX_PARTICLES];
float _ParticlesAmount;

float sdCircle(float2 p, float2 center, float radius)
{
    return length(p - center) - radius;
}

// cubic polynomial by Inigo Quilez
float smin(float a, float b, float k)
{
    k *= 6.0;
    float h = max(k - abs(a - b), 0.0 ) / k;
    return min(a, b) - h * h * h * k * (1.0 / 6.0);
}

void SDParticles_float(in float3 PositionWS, in float MaxRadius, in float MaxBlend, out float Out)
{
    float2 p = PositionWS.xy;   

    float d = 1000.0;
    
    for (int i = 0; i < _ParticlesAmount; i++)
    {
        float circle = sdCircle(p, _ParticlesPosition[i], _ParticlesSize[i] * MaxRadius);
        d = smin(d, circle, _ParticlesSize[i] * MaxBlend);
    }
    
    Out = smoothstep(d - 0.01, d + 0.01, 0.0);
}

#endif  