Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _AlbedoFactor("Albedo", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _RoughnessFactor("Smoothness", Range(0,1)) = 0.5
        _MetallicFactor("Metallic", Range(0,1)) = 0.0
    }
        SubShader
        {
           Pass{
                Name "GBufferPass"
                Tags   {"LightMode" = "GBufferPass"}
                HLSLPROGRAM

                #pragma vertex GBufferVS
                #pragma fragment GBufferPS

                #include "../ShaderLibrary/GBufferPass.hlsl"

                #pragma enable_d3d11_debug_symbols


                ENDHLSL
            }
            Pass{
                Name "RayTracing"
                Tags   {"LightMode" = "RayTracing"}
                HLSLPROGRAM

                #pragma raytracing test

                #include "../ShaderLibrary/RTCommon.hlsl"
                #include "../ShaderLibrary/SampleTransformUtil.hlsl"
                #include "UnityRaytracingMeshUtils.cginc"

                struct AttributeData
                {
                    float2 barycentrics;
                };

                struct IntersectionVertex
                {
                    // Object space normal of the vertex
                    float3 normalOS;
                };

                void FetchIntersectionVertex(uint vertexIndex, out IntersectionVertex outVertex)
                {
                    outVertex.normalOS = UnityRayTracingFetchVertexAttribute3(vertexIndex, kVertexAttributeNormal);
                }

#define INTERPOLATE_RAYTRACING_ATTRIBUTE(v0,v1,v2,b) v0 * b.x + v1 * b.y + v2 * b.z 
                

                [shader("closesthit")]
                void ClosestHitShader(inout RayIntersection rayIntersection : SV_RayPayload, AttributeData attributeData : SV_IntersectionAttributes)
                {
                    rayIntersection.isHit = 1;

                    // Fetch the indices of the currentr triangle
                    uint3 triangleIndices = UnityRayTracingFetchTriangleIndices(PrimitiveIndex());

                    // Fetch the 3 vertices
                    IntersectionVertex v0, v1, v2;
                    FetchIntersectionVertex(triangleIndices.x, v0);
                    FetchIntersectionVertex(triangleIndices.y, v1);
                    FetchIntersectionVertex(triangleIndices.z, v2);

                    // Compute the full barycentric coordinates
                    float3 barycentricCoordinates = float3(1.0 - attributeData.barycentrics.x - attributeData.barycentrics.y, attributeData.barycentrics.x, attributeData.barycentrics.y);

                    float3 normalOS = INTERPOLATE_RAYTRACING_ATTRIBUTE(v0.normalOS, v1.normalOS, v2.normalOS, barycentricCoordinates);
                    float3x3 objectToWorld = (float3x3)ObjectToWorld3x4();
                    float3 normalWS = normalize(mul(objectToWorld, normalOS));
                    
                    float2 u2 = float2(rand(rayIntersection.rngState),rand(rayIntersection.rngState));
                    float4 sampleBRDF = SampleCosineWeightWithNormal(u2,normalWS);
                    rayIntersection.data3 = 1.0f / M_PI;
                    rayIntersection.brdfPdf = sampleBRDF.z;
                    
                    float3 positionWS = WorldRayOrigin() + WorldRayDirection() * RayTCurrent();
                    rayIntersection.newRay.Origin = positionWS + 0.001f * normalWS;
                    rayIntersection.newRay.TMin = 1e-5f;
                    rayIntersection.newRay.TMax = 1e+9;
                    rayIntersection.newRay.Direction = sampleBRDF.xyz;
                                            
                }       

                ENDHLSL
            }
        }
     FallBack "Diffuse"
}
