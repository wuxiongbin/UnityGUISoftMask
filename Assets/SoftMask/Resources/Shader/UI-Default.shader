Shader "UI/Default Soft Mask"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)


			_Min("Min",Vector) = (0,0,0,0)
			_Max("Max",Vector) = (1,1,0,0)
			_SoftEdge("SoftEdge", Vector) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;

				float4 worldPosition2 : COLOR1;
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			float2 _Min;
			float2 _Max;
			float4 _SoftEdge;
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = mul(UNITY_MATRIX_MVP, OUT.worldPosition);
				OUT.worldPosition2 = mul(_Object2World, IN.vertex);

				OUT.texcoord = IN.texcoord;
				
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0) * float2(-1,1) * OUT.vertex.w;
				#endif
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				
				float alpha = 0;
				if (IN.worldPosition2.x <= _Min.x || IN.worldPosition2.x >= _Max.x || IN.worldPosition2.y <= _Min.y || IN.worldPosition2.y >= _Max.y)
				{
					alpha = 0;
				}
				else // It's in the mask rectangle, so apply the alpha of the mask provided.
				{
					alpha = 1;
					if (IN.worldPosition2.x - _Min.x < _SoftEdge.x)
						alpha *= (IN.worldPosition2.x - _Min.x) / _SoftEdge.x;
					else if (_Max.x - IN.worldPosition2.x < _SoftEdge.z)
						alpha *= (_Max.x - IN.worldPosition2.x) / _SoftEdge.z;

					if (IN.worldPosition2.y - _Min.y < _SoftEdge.y)
						alpha *= (IN.worldPosition2.y - _Min.y) / _SoftEdge.y;
					else if (_Max.y - IN.worldPosition2.y < _SoftEdge.w)
						alpha *= (_Max.y - IN.worldPosition2.y) / _SoftEdge.w;
				}

				color.a *= alpha;

				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
}
