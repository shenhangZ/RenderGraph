#ifndef GBUFFER_PASS_HLSL
#define GBUFFER_PASS_HLSL

#include"Input.hlsl"
struct VertexIn
{
    float3 PosL : POSITION;
    float3 Normal : NORMAL;
    float3 Tangent : TANGENT;
    float2 texCoord : TEXCOORD;
};
struct VertexOut
{
    float4 PosH : SV_POSITION;
    float3 PosW : NORMAL0;
    float3 NormalW : NORMAL1;
    float2 texCoord : TEXCOORD;
};

VertexOut GBufferVS(VertexIn vIn)
{
    VertexOut o;
    o.PosW = mul(GetObjectToWorld(), float4(vIn.PosL, 1.0f)).xyz;
    o.NormalW = normalize(mul(transpose(GetWorldToObject()), float4(vIn.Normal, 0))).xyz;
    o.texCoord = vIn.texCoord;
    o.PosH = mul(GetCameraProjView(), float4(o.PosW, 1.0f));

    return o;
}

struct GBuffer
{
    float4 LitFromEnvironment : SV_TARGET0;
    float4 NorWM : SV_TARGET1;
    float4 Albedo : SV_TARGET2;
    float4 PosWR : SV_TARGET3;
};

cbuffer UnityPerMaterial
{
    float3 _AlbedoFactor;
    float _MetallicFactor;

    float _RoughnessFactor;
    float3 _pad3;
};
//Texture2D _Albedo;
//Texture2D _Albedo;
//Texture2D _Albedo;

GBuffer GBufferPS(VertexOut pIn)
{
    GBuffer gBuffer;

    float roughness = _RoughnessFactor;
    float metallic = _MetallicFactor;
    float3 albedo = _AlbedoFactor;
    gBuffer.PosWR = float4(pIn.PosW, roughness);
    gBuffer.NorWM = float4(pIn.NormalW, metallic);
    gBuffer.Albedo = float4(albedo,1.0f);
    gBuffer.LitFromEnvironment = float4(pIn.NormalW, 1.0f);
    return gBuffer;
}
#endif