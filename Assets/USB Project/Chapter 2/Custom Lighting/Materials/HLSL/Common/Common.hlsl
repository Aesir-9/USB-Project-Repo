#ifndef COMMON_FUNCTIONS
#define COMMON_FUNCTIONS

float schlick_fresnel(float f0, float u)
{    
    return f0 + (1.0 - f0) * pow(1.0 - u, 5.0);
}

float distribution(float a, float u)
{
    float d = (u * u) * (a * a - 1.0) + 1.0;
    return (a * a) / (PI * d * d);
}

float geometry(float a, float u, float v)
{
    float k = (a + 1.0) * (a + 1.0) / 8.0;
    float gv = u / (u * (1.0 - k) + k);
    float gl = v / (v * (1.0 - k) + k);
    return gv * gl;
}

float easing(float t)
{
    return t * t * (3.0 - 2.0 * t);
}

float lambert(float3 n, float3 l)
{
    return max(0.0, dot(n, l));
}

#endif


