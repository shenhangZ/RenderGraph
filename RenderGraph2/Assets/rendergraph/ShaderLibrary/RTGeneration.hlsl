#ifndef RAY_TRACING_HLSL
#define RAY_TRACING_HLSL

#include "RTCommon.hlsl"

RWTexture2D<float4> RenderTarget;
RWTexture2D<float4> AccumulationBuffer;
cbuffer RayTracingConfig
{
    int _MaxBounces;
    uint _AccumulatedFrames;
}
RaytracingAccelerationStructure scene;

#pragma max_recursion_depth 1

inline RayDesc GeneratePrimaryRay(inout uint rngState )
{
    float2 pixel = float2(DispatchRaysIndex().xy);
    float2 resolutions = float2(DispatchRaysDimensions().xy);

    float2 uvInNDC = ((pixel + 0.5) / resolutions) * 2.0f - 1.0f;

    return GenPinholeCameraRay(uvInNDC);
}
inline float3 TraceRay(inout uint rngState,int maxBounces)
{
    RayDesc ray = GeneratePrimaryRay(rngState);

    float3 throughput = 1.0f;
    float3 radiance = 0.0f;

    RayIntersection hitInfo;
    hitInfo.rngState = rngState;

    for (int i = 0; i < maxBounces; i++)
    {
        TraceRay(scene, RAY_FLAG_CULL_BACK_FACING_TRIANGLES, 0xFF, 0, 1, 0, ray, hitInfo);
        if(hitInfo.isHit)
        {
            ray = hitInfo.newRay;
            throughput *= hitInfo.data3 / hitInfo.brdfPdf;
        }
        else{
            radiance += throughput * hitInfo.data3;
            break;
        }
    }    
    rngState = hitInfo.rngState;
    return radiance;
}
inline void SetRT(float3 radiance)
{
    float2 pixel = float2(DispatchRaysIndex().xy);
    RenderTarget[pixel] = float4(radiance.x,radiance.y,radiance.z, 1.0);
}

[shader("raygeneration")]
void MyRaygenShader()
{
    uint rngState = InitRNG(DispatchRaysIndex().xy,DispatchRaysDimensions().xy,_AccumulatedFrames);
    
    // Trace a path for the current pixel
    float3 radiance = TraceRay(rngState,_MaxBounces);

    float3 previousRadiance = AccumulationBuffer[DispatchRaysIndex().xy].xyz;

    float3 accumulatedRadiance = previousRadiance + radiance;
    AccumulationBuffer[DispatchRaysIndex().xy] = float4(accumulatedRadiance,1.0f);

    SetRT(accumulatedRadiance / _AccumulatedFrames);
}

[shader("miss")]
void MissShader(inout RayIntersection rayIntersection: SV_RayPayload)
{
    rayIntersection.isHit = 0;
    rayIntersection.data3 = float3(1,1,1);
}
#endif