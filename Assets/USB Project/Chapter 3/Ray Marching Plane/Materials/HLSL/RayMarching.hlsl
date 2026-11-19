#define MAX_ITERATIONS 128
#define MAX_DISTANCE 10.0
#define SURFACE_DISTANCE 0.001

float planeSDF(float3 position, float edge)
{
    return position.y - edge;
}

float render(in float3 rayOrigin, in float3 rayDirection, in float edge)
{
    float rayDistance = 0;
    
    for (int i = 0; i < MAX_ITERATIONS; i++)
    {
        float3 p = rayOrigin + rayDirection * rayDistance;
        float sceneDistance = planeSDF(p, edge);
        rayDistance += sceneDistance;
        
        if (sceneDistance < SURFACE_DISTANCE || rayDistance > MAX_DISTANCE)
        {
            break;
        }            
    }
    return rayDistance;
}

void RayMarching_float(in UnityTexture2D Albedo,
    in UnityTexture2D Intersection,
    in float2 UV,
    in float BorderRadius,
    in float3 BorderColor,
    in bool IsFrontFace,
    in float3 Position,
    in float3 CameraPosition,
    in float Edge,
    out float3 Out)
{
    float3 rayOrigin = CameraPosition;
    float3 rayDirection = normalize(Position - CameraPosition);  
    float hitDistance = render(rayOrigin, rayDirection, Edge);
    
    float3 albedo = Albedo.Sample(Albedo.samplerstate, UV).rgb;
    float3 intersection;
    float projectionCurve = pow(-abs(Edge), 2.0) + pow(-abs(Edge) - 1.0, 2.0);    
    
    if (hitDistance < MAX_DISTANCE)
    {
        float3 p = rayOrigin + hitDistance * rayDirection;       
        float2 uv_p = p.xz * (1.0 + abs(pow(Edge * projectionCurve, 2.0))) - 0.5;
        
        float circle = length(p.xz);
        float3 lemonBorder  = smoothstep(circle - 0.002, circle + 0.002, BorderRadius - abs(pow(Edge * (projectionCurve * 0.5), 2.0)));
        
        intersection = Intersection.Sample(Intersection.samplerstate, uv_p).rgb;
        intersection *= lemonBorder;
        intersection += (1.0 - lemonBorder) * BorderColor;
    }   

    if (Position.y > Edge) discard;           
        
    Out = IsFrontFace ? albedo : intersection;
}