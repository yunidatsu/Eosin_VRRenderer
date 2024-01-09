

Shader "Hidden/LilyRender/EquriectangularPixelSlice"
{
	Properties
	{
		[Enum(Center, 0, Left, 1, Right, 2, Top, 3, Bottom, 4)] _EyeLocation("Eye Location", Float) = 0
		_BackgroundTex("BackgroundTex", 2D) = "white" {}
		_BotTex("BotTex", 2D) = "white" {}
		_MidTex("MidTex", 2D) = "white" {}
		_TopTex("TopTex", 2D) = "white" {}
		_VerticalFov("VerticalFov", Float) = 60
		_PixelColumn("Pixel Column", Float) = 0
		_HideSize("Hide Size", Float) = 0
		_MainTex("Main Tex", 2D) = "red" {}
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
			#pragma multi_compile ___ WHOLE_TEXTURE

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
			int _EyeLocation;
#endif
			sampler2D _BotTex;
			sampler2D _MidTex;
			sampler2D _TopTex;
			float _VerticalFov; // fov of each of the three cameras
#ifdef WHOLE_TEXTURE
			float _PixelColumn;
			sampler2D _MainTex;
#endif
			float _HideSize;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			#include "EquirectangularPixelSlice.cginc"

			fixed4 frag(v2f i) : SV_Target
			{
				if (i.uv.y < _HideSize || i.uv.y > 1 - _HideSize)
					return fixed4(0, 0, 0, 1);

				float4 color = float4(0, 0, 0, 0);
#ifdef WHOLE_TEXTURE
				if ((_PixelColumn - i.uv.x) > 0.000001f)
					color = tex2D(_MainTex, i.uv);
				else
#endif			
					color = equirectangular(i, _BotTex, _MidTex, _TopTex, _VerticalFov);

#ifdef USE_BACKGROUND
				float4 bg = float4(0, 0, 0, 0);
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
				float totalAlpha = min(1, color.a + bg.a);
				color.rgb = (color.a / totalAlpha) * color.rgb + (1 - color.a / totalAlpha) * bg.rgb;
				return fixed4(color.rgb, totalAlpha);
#endif
				return color;
			}

			ENDCG
		}
	}
}