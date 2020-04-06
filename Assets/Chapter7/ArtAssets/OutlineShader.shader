﻿Shader "Hidden/Internal-OutlineTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _CameraDepthTexture;
            float4 _CameraDepthTexture_ST;
            sampler2D _CameraDepthNormalsTexture;
            float4 _CameraDepthNormalsTexture_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 depthnormal = tex2D(_CameraDepthNormalsTexture, i.uv);
                float3 normal;
                float depth;
                DecodeDepthNormal(depthnormal, depth, normal);

                fixed4 col = tex2D(_MainTex, i.uv);

                float2 uv[4];
                float dw = 1.0 / _ScreenParams.x;
                float dh = 1.0 / _ScreenParams.y;
                uv[0] = i.uv + float2(dw, dh);
                uv[1] = i.uv - float2(dw, dh);
                uv[2] = i.uv + float2(-dw, dh);
                uv[3] = i.uv + float2(dw, -dh);

                float depthSamples[4];
                float3 normalSamples[4], colorSamples[4];
                float t = 0;
                for(int i = 0; i < 4 ; i++)
                {
                    depthSamples[i] = tex2D(_CameraDepthTexture, uv[i]).r;
                    DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv[i]), t, normalSamples[i]);
                }

                // Depth
                float depthFiniteDifference0 = depthSamples[1] - depthSamples[0];
                float depthFiniteDifference1 = depthSamples[3] - depthSamples[2];
                float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;

                // Normals
                float3 normalFiniteDifference0 = normalSamples[1] - normalSamples[0];
                float3 normalFiniteDifference1 = normalSamples[3] - normalSamples[2];
                float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));

                float edge = max(edgeDepth, edgeNormal);
                col = ((1 - edge) * col) + (edge * lerp(col, float4(0, 0, 0, 0),  0.5));
                return col;
            }
            ENDCG
        }
    }
}
