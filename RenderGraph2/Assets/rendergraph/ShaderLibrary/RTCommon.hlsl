#ifndef RT_COMMON_HLSL
#define RT_COMMON_HLSL

////////////////////
/// Structure Define
////////////////////
#define M_PI 3.1415926

struct RayIntersection
{
    int isHit;
    float3 data3; // if hit object , the data3 is brdf,else if hit miss, the data3 is color
    RayDesc newRay;

    float brdfPdf;
    uint rngState;
};
////////////////////
/// Data Input
////////////////////
cbuffer CameraData
{
    float4x4 _InvCameraViewProj;
    float3 _CamPosW;
    float _CamPad0;
};

RayDesc GenPinholeCameraRay(float2 uvInNDC)
{
    RayDesc ray;
    ray.Origin = _CamPosW;
    ray.TMin = 0.f;
    ray.TMax = 1e+9;

    ray.Direction = normalize(mul(_InvCameraViewProj, float4(uvInNDC, 0,1))).xyz;
    return ray;

}


////////////////////
/// Random Number
////////////////////
uint jenkinsHash(uint x)
{
    x += x << 10;
    x ^= x >> 6;
    x += x << 3;
    x ^= x >> 11;
    x += x << 15;
    return x;
}
uint InitRNG(uint2 pixel,uint2 resolution,uint frames)
{
    uint rngState = dot(pixel,uint2(1,resolution.x)) ^ jenkinsHash(frames);
    return jenkinsHash(rngState);
}
float uintToFloat(uint x)
{
    return asfloat(0x3f800000 | (x >> 9)) - 1.f;
}
uint xorShift(inout uint rngState)
{
    rngState ^= rngState << 13;
    rngState ^= rngState >> 17;
    rngState ^= rngState << 5;
    return rngState;
}

float rand(inout uint rngState){
    return uintToFloat(xorShift(rngState));
}
#endif