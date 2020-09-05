Shader "Custom/SimpleDecal"
{
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		[HDR]_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader 
	{
		Tags { "RenderType"= "Transparent" "Queue" = "Transparent" }
		Pass 
		{
            Tags { "LightMode" = "UniversalForward" }
			ZWrite Off		
			Lighting Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
		
			uniform half4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _CameraDepthNormalsTexture;		
			float4 _MainTex_ST;			

			struct VSInput 
			{
				float4 vertex : POSITION;
				float3 texcoord : TEXCOORD0;
			};

			struct VSOut 
			{
				float4 position :SV_POSITION;
				float4 screenPos : TEXCOORD0;		  		  
				float3 ray : TEXCOORD2;		  
			};
		
			VSOut vert (VSInput v)
			{
				VSOut o ;
				o.position = UnityObjectToClipPos(v.vertex);						
				o.screenPos = ComputeScreenPos(o.position);
				o.ray = UnityObjectToViewPos(v.vertex).xyz * float3(-1,-1,1);			

				// v.texcoord is equal to 0 when we are drawing 3D light shapes and
				// contains a ray pointing from the camera to one of near plane's
				// corners in camera space when we are drawing a full screen quad.
				o.ray = lerp(o.ray, v.texcoord, v.texcoord.z != 0);

				return o;
			}
		
			fixed4 frag(VSOut i): Color  
			{	    			
				// Get correct view direction
        		i.ray = i.ray * (_ProjectionParams.z / i.ray.z);

                float4 depthnormal = tex2D(_CameraDepthNormalsTexture, i.screenPos.xy / i.screenPos.w);
                float3 normal;
                float depth;
                DecodeDepthNormal(depthnormal, depth, normal);

				// Get depth in the current pixel		    			
				//float depth = Linear01Depth (SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.screenPos.xy / i.screenPos.w));			

				// Get new projection coordinates. It is almost like original o.position, 
				// except that Z axis is using depth information. Such taht we are ignoring our projected object, Z values			
				float4 prjPos = float4(i.ray * depth,1);
				float3 worldPos = mul(unity_CameraToWorld, prjPos).xyz;
				float4 objPos = mul(unity_WorldToObject, float4(worldPos, 1));

				clip(float3(0.5, 0.5, 0.5) - abs(objPos.xyz));
				half2 uv = _MainTex_ST.xy * (objPos.xz + 0.5);
				return tex2D(_MainTex, uv) * _Color;
			}
			ENDCG
		}
	} 
	FallBack "Unlit/Transparent"
}