﻿/**
 * Copyright (c) 2019 Elie Michel
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 *
 * This file is part of Lily Render 360, a unity tool for equirectangular
 * rendering, available at https://github.com/eliemichel/LilyRender360
 */

Shader "Hidden/LilyRender/EquirectangularAlpha"
{
	Properties
	{
		_Beta("Beta", Float) = 1 // 1 / Alpha, where Alpha is the margin factor (eg 1.1 to add 10% margin)
		[Enum(Center, 0, Left, 1, Right, 2, Top, 3, Bottom, 4)] _EyeLocation("Eye Location", Float) = 0
		_BackgroundTex("BackgroundTex", 2D) = "white" {}
		_FaceTexPX("FaceTexPX", 2D) = "white" {}
		_FaceTexNX("FaceTexNX", 2D) = "white" {}
		_FaceTexPY("FaceTexPY", 2D) = "white" {}
		_FaceTexNY("FaceTexNY", 2D) = "white" {}
		_FaceTexPZ("FaceTexPZ", 2D) = "white" {}
		_FaceTexNZ("FaceTexNZ", 2D) = "white" {}
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
			#pragma shader_feature ORIENT_CUBE
			#pragma shader_feature SHOW_STITCH_LINES
			#pragma shader_feature TWO_CUBES
			#pragma shader_feature SMOOTH_STITCHING

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

			float _Beta;
			float _HorizontalFov;
			float _VerticalFov;

#ifdef USE_BACKGROUND
			sampler2D _BackgroundTex;
			int _EyeLocation;
#endif
			sampler2D _FaceTexPX;
			sampler2D _FaceTexNX;
			sampler2D _FaceTexPY;
			sampler2D _FaceTexNY;
			sampler2D _FaceTexPZ;
			sampler2D _FaceTexNZ;

#ifdef ORIENT_CUBE
			float4x4 _OrientMatrix;
#endif

#ifdef TWO_CUBES
			sampler2D _FaceTexPX2;
			sampler2D _FaceTexNX2;
			sampler2D _FaceTexPY2;
			sampler2D _FaceTexNY2;
			sampler2D _FaceTexPZ2;
			sampler2D _FaceTexNZ2;
#  ifdef ORIENT_CUBE
			float4x4 _OrientMatrix2;
#  endif
#endif

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			#include "EquirectangularAlpha.cginc"

			fixed4 frag(v2f i) : SV_Target
			{
				float2 px, px2;
				fixed4 color =
#ifdef SMOOTH_STITCHING
					equirectangularSmooth(
#else
					equirectangular(
#endif
						i, _FaceTexPX, _FaceTexNX, _FaceTexPY, _FaceTexNY, _FaceTexPZ, _FaceTexNZ,

						_Beta, _HorizontalFov, _VerticalFov, px);
				

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