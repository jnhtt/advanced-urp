Shader "Hidden/Internal-DistortionTexture"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DistortionPower("Distortion Power", Range(0.0, 0.1)) = 0.003
		_DistortionSpeed("Distortion Speed", Range(0.01, 2)) = 0.1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			fixed2 random2(fixed2 st) {
				st = fixed2(dot(st, fixed2(127.1, 311.7)),
				dot(st, fixed2(269.5, 183.3)));
				return -1.0 + 2.0*frac(sin(st)*43758.5453123);
			}
			float perlinNoise(fixed2 st)
			{
				fixed2 p = floor(st);
				fixed2 f = frac(st);
				fixed2 u = f * f*(3.0 - 2.0*f);
				float v00 = random2(p + fixed2(0, 0));
				float v10 = random2(p + fixed2(1, 0));
				float v01 = random2(p + fixed2(0, 1));
				float v11 = random2(p + fixed2(1, 1));
				return lerp(lerp(dot(v00, f - fixed2(0, 0)), dot(v10, f - fixed2(1, 0)), u.x),
					lerp(dot(v01, f - fixed2(0, 1)), dot(v11, f - fixed2(1, 1)), u.x),
					u.y) + 0.5f;
			}
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			sampler2D _MainTex;
			sampler2D _DistortionTex;
			float _DistortionPower;
			float _DistortionSpeed;
			fixed4 frag(v2f i) : SV_Target
			{
				float area = tex2D(_DistortionTex, i.uv).r;
				float2 uv = frac(i.uv + float2(0, _DistortionSpeed * _Time.y));
				float3 decode = tex2D(_DistortionTex, uv) - float3(0.5, 0.5, 0);
				uv = area * _DistortionPower * float2(decode.r * _ScreenParams.z, decode.g * _ScreenParams.w);
				fixed4 col = tex2D(_MainTex, i.uv + uv);
				return col;
			}
			ENDCG
		}
	}
}