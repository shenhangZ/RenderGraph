#ifndef INPUT_HLSL
#define INPUT_HLSL

cbuffer UnityPerDraw
{
    float4x4 unity_ObjectToWorld;
    float4x4 unity_WorldToObject;
}
cbuffer UnityPerCamera
{
    float4x4 glstate_matrix_projection_inv;
    float4x4 glstate_matrix_projection;
    float4x4 unity_MatrixV;
    float3 _CameraPosW;
}

inline float4x4 GetCameraView()
{
    return unity_MatrixV;
}

inline float4x4 GetCameraProj()
{
    return glstate_matrix_projection;
}
inline float4x4 GetCameraProjInv()
{
    return glstate_matrix_projection_inv;
}
inline float4x4 GetCameraProjView()
{
    return mul(GetCameraProj(), GetCameraView());
}
inline float4x4 GetObjectToWorld()
{
    return unity_ObjectToWorld;
}
inline float4x4 GetWorldToObject()
{
    return unity_WorldToObject;
}


//////////////////////
/// SamplerState Define
//////////////////////
SamplerState my_linear_clamp_sampler;

inline SamplerState GetLinearClampSampler()
{
    return my_linear_clamp_sampler;
}
#endif