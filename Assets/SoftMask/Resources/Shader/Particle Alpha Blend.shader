Shader "Particles/Alpha Blended Soft Mask" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0

		_Min("Min",Vector) = (0,0,0,0)
		_Max("Max",Vector) = (1,1,0,0)
		_SoftEdge("SoftEdge", Vector) = (1,1,1,1)
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				//#ifdef SOFTPARTICLES_ON
				//float4 projPos : TEXCOORD2;
				//#endif

				float4 worldPosition : TEXCOORD1;
				float4 worldPosition2 : COLOR1;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.worldPosition = v.vertex;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.worldPosition2 = mul(_Object2World, v.vertex);

				//#ifdef SOFTPARTICLES_ON
				//o.projPos = ComputeScreenPos (o.vertex);
				//COMPUTE_EYEDEPTH(o.projPos.z);
				//#endif
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;

			float2 _Min;
			float2 _Max;
			float4 _SoftEdge;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
				UNITY_APPLY_FOG(i.fogCoord, col);

				float alpha = 0;
				//return color;
				if (i.worldPosition2.x <= _Min.x || i.worldPosition2.x >= _Max.x || i.worldPosition2.y <= _Min.y || i.worldPosition2.y >= _Max.y)
				{
					alpha = 0;
				}
				else // It's in the mask rectangle, so apply the alpha of the mask provided.
				{
					alpha = 1;
					if (i.worldPosition2.x - _Min.x < _SoftEdge.x)
						alpha *= (i.worldPosition2.x - _Min.x) / _SoftEdge.x;
					else if (_Max.x - i.worldPosition2.x < _SoftEdge.z)
						alpha *= (_Max.x - i.worldPosition2.x) / _SoftEdge.z;

					if (i.worldPosition2.y - _Min.y < _SoftEdge.y)
						alpha *= (i.worldPosition2.y - _Min.y) / _SoftEdge.y;
					else if (_Max.y - i.worldPosition2.y < _SoftEdge.w)
						alpha *= (_Max.y - i.worldPosition2.y) / _SoftEdge.w;
				}

				col *= alpha;

				return col;
			}
			ENDCG 
		}
	}	
}
}
