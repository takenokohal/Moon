Shader "Takenokohal/InstancedShader"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white"
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        // 各Passでcbufferが変わらないようにここに定義する
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
        CBUFFER_END
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;

                float2 uv : TEXCOORD0;

                uint instanceID : SV_InstanceID;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;

                float4 color : COLOR;

                float2 uv : TEXCOORD0;
            };

            StructuredBuffer<float3> _Positions;
            StructuredBuffer<float4> _Colors;

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
            // The following line declares the _BaseMap_ST variable, so that you
            // can use the _BaseMap variable in the fragment shader. The _ST 
            // suffix is necessary for the tiling and offset function to work.
            float4 _BaseMap_ST;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;


                const float scale = 1;
                float3 pos = (v.vertex.xyz * scale) + _Positions[v.instanceID];
                o.vertex = mul(UNITY_MATRIX_VP, float4(pos, 1.0));
                o.normal = v.normal;

                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);


                //    float4 col = _Colors[instanceID];
                //     const Light light = GetMainLight();

                //     float t = dot(v.normal, light.direction);
                //    t = max(0, t);

                // float3 diffuseLight = light.color * t;
                // const float3 ambientLight = {0.5f, 0.5f, 0.5f};
                // col.rgb *= diffuseLight + ambientLight;

                float4 blue = {0, 0, 100, 1};
                //   col = lerp(col, blue, t);
                o.color = blue;


                return o;
            }


            float4 frag(v2f i) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                return color * i.color;
            }
            ENDHLSL
        }
    }
}