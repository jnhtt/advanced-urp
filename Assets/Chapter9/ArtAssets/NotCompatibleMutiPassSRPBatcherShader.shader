﻿Shader "Custom/NotCompatibleMultiPassSRPBatcher"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", float) = 0
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "Pass1"
            Tags { "LightMode" = "Pass1" }

            Cull Front

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            //CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Color;
            half _OutlineWidth;
            half4 _OutlineColor;
            //CBUFFER_END

            float DistanceToCamera(float3 pos)
            {
                return mul(unity_MatrixV, mul(unity_ObjectToWorld, float4(pos, 1.0))).z;
            }

            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
                float3 normal           : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv        : TEXCOORD0;
                float fogCoord  : TEXCOORD1;
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float d = DistanceToCamera(input.positionOS.xyz);
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz - d * _OutlineWidth * input.normal);
                output.vertex = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half2 uv = input.uv;
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                half3 color = texColor.rgb * _OutlineColor;
                half alpha = texColor.a;

                color = MixFog(color, input.fogCoord);

                return half4(color, alpha);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Pass2"
            Tags { "LightMode" = "Pass2" }

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            //CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Color;
            half _OutlineWidth;
            half4 _OutlineColor;
            //CBUFFER_END

            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv        : TEXCOORD0;
                float fogCoord  : TEXCOORD1;
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half2 uv = input.uv;
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;
                half3 color = texColor.rgb;
                half alpha = texColor.a;

                color = MixFog(color, input.fogCoord);

                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}