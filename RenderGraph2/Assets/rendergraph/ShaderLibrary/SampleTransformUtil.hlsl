#ifndef SAMPLE_TRANSFORM_UTIL_HLSL
#define SAMPLE_TRANSFORM_UTIL_HLSL
#include "RTCommon.hlsl"


float4 SampleCosineWeight(float2 u2)
{
    float radius = sqrt(u2.x);
    float phi = 2 * M_PI * u2.y;
    float x = radius * cos(phi);
    float y = radius * sin(phi);
    float z = sqrt(1.0f-u2.x);
    float pdf = z / M_PI;
    return float4(x,y,z,pdf);
}

float4 SampleCosineWeightWithNormal(float2 u2,float3 normal)
{
    float a = 1 - 2 * u2.x;
    float b = sqrt(1 - a * a);
    float phi = 2 * M_PI * u2.y;
    float x = normal.x + b * cos(phi);
    float y = normal.y + b * sin(phi);
    float z = normal.z + a;
    float pdf = a / M_PI;
    return float4(x,y,z,pdf);
}

#endif