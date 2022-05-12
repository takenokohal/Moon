#ifndef UNIVERSAL_FORWARD_LIT_PASS_INCLUDED
#define UNIVERSAL_FORWARD_LIT_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// GLES2 has limited amount of interpolators
#if defined(_PARALLAXMAP) && !defined(SHADER_API_GLES)
#define REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR
#endif

#if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
#endif

// keep this file in sync with LitGBufferPass.hlsl

struct Attributes
{
    half4 positionOS : POSITION;
    //  float3 normalOS : NORMAL;
    //  float4 tangentOS : TANGENT;
    half2 texcoord : TEXCOORD0;
    //    float2 staticLightmapUV : TEXCOORD1;
    // float2 dynamicLightmapUV : TEXCOORD2;
    //  UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    half2 uv : TEXCOORD0;

    /*
    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    float3 positionWS : TEXCOORD1;
    #endif

    half3 normalWS : TEXCOORD2;
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
    half4 tangentWS : TEXCOORD3; // xyz: tangent, w: sign
    #endif
    float3 viewDirWS : TEXCOORD4;

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
    half4 fogFactorAndVertexLight   : TEXCOORD5; // x: fogFactor, yzw: vertex light
    #else
    half fogFactor : TEXCOORD5;
    #endif

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    float4 shadowCoord              : TEXCOORD6;
    #endif

    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirTS : TEXCOORD7;
    #endif

    DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 8);
    #ifdef DYNAMICLIGHTMAP_ON
    float2  dynamicLightmapUV : TEXCOORD9; // Dynamic lightmap UVs
    #endif*/

    half4 positionCS : SV_POSITION;

    half3 color : COLOR;
};


struct BulletData
{
    half2 position;
    half3 color;
    half scale;
};

StructuredBuffer<BulletData> _Bullets;


///////////////////////////////////////////////////////////////////////////////
//                  Vertex and Fragment functions                            //
///////////////////////////////////////////////////////////////////////////////

// Used in Standard (Physically Based) shader
Varyings LitPassVertex(Attributes input, const uint instanceID : SV_InstanceID)
{
    Varyings output;

    output.uv = input.texcoord;


    const half scale = _Bullets[instanceID].scale;
    half2 pos = (input.positionOS.xyz * scale) + _Bullets[instanceID].position - half2(scale / 4, scale / 4);
    output.positionCS = mul(UNITY_MATRIX_VP, half4(pos, 0, 1.0));

    output.color = _Bullets[instanceID].color;

    return output;
}


half4 LitPassFragment(Varyings input) : SV_Target
{
    half4 color = half4(input.color, 0);

    const half radius = 1;

    const half2 center = {0.5, 0.5};
    const half dist = distance(input.uv * 2, center) * 2;

    if (dist < radius)
    {
        color.a = (1 - dist) * (1 - dist);
    }

    return color;
}

#endif
