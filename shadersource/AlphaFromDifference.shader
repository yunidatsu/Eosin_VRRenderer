

Shader "Hidden/AlphaFromDifference"
{
	Properties
	{
		[Enum(Center, 0, Left, 1, Right, 2, Top, 3, Bottom, 4)] _EyeLocation("Eye Location", Float) = 0
		_BackgroundTex("Background Tex", 2D) = "black" {}
		_BlackBgTex("Black BG Tex", 2D) = "gray" {}
		_WhiteBgTex("White BG Tex", 2D) = "red" {}
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" "PreviewType" = "Plane"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile ___ USE_BACKGROUND // have to multi compile, otherwise optimized away

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

#ifdef USE_BACKGROUND
			sampler2D _BackgroundTex;
			float _EyeLocation;
#endif
			sampler2D _BlackBgTex;
			sampler2D _WhiteBgTex;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 cLow = tex2D(_BlackBgTex, i.uv);
				float3 cHigh = tex2D(_WhiteBgTex, i.uv);
				float alpha = 1 - ((cHigh.r + cHigh.g + cHigh.b) - (cLow.r + cLow.g + cLow.b)) / 3;
				//float3 colorA = cLow / alpha; // unpremultiply
				//float3 colorB = (cHigh - 1) / alpha + 1; // unpremultiply
				//float3 color = (colorA + colorB) / 2; // unpremultiply
				float3 color = cLow / alpha; // unpremultiply

				
#ifdef USE_BACKGROUND
				float4 bg = float4 (0, 0, 0, 0);
				if (_EyeLocation == 0)
					bg = tex2D(_BackgroundTex, i.uv);
				else if (_EyeLocation == 1)
					bg = tex2D(_BackgroundTex, float2(i.uv.x / 2.0, i.uv.y));
				else if (_EyeLocation == 2)
					bg = tex2D(_BackgroundTex, float2(i.uv.x / 2.0 + 0.5, i.uv.y));
				else if (_EyeLocation == 3)
					bg = tex2D(_BackgroundTex, float2(i.uv.x, i.uv.y / 2.0));
				else if (_EyeLocation == 4)
					bg = tex2D(_BackgroundTex, float2(i.uv.x, i.uv.y / 2.0 + 0.5));
				float totalAlpha = min(1, alpha + bg.a);
				color = (alpha / totalAlpha) * color + (1 - alpha / totalAlpha) * bg.rgb;
				return float4(color, totalAlpha);
#endif
				return float4(color, alpha);
			}
			ENDCG
		}
	}
}