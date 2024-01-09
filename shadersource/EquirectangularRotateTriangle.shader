/**
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

Shader "Hidden/LilyRender/EquirectangularRotateTriangle"
{
	Properties
	{
		_Beta("Beta", Float) = 1 // 1 / Alpha, where Alpha is the margin factor (eg 1.1 to add 10% margin)
		_Offset("Offset", Float) = 1 // 1 / Alpha, where Alpha is the margin factor (eg 1.1 to add 10% margin)
		_FrontFov("Front FOV", Float) = 1 // 1 / Alpha, where Alpha is the margin factor (eg 1.1 to add 10% margin)
		_UseSeamTex("Use Seam Tex", Float) = 0// 1 / Alpha, where Alpha is the margin factor (eg 1.1 to add 10% margin)
		_EyeInfo("Eye Info", Float) = 1 // 1 / Alpha, where Alpha is the margin factor (eg 1.1 to add 10% margin)
		_HideSize("Hide Size", Float) = 1
		_HideSeamsSize("Hide Seams Size", Float) = 1
		_HideColor("Hide Color", Color) = (0, 0, 0, 0)

		_FaceABot("FaceABot", 2D) = "white" {}
		_FaceAMid("FaceAMid", 2D) = "white" {}
		_FaceATop("FaceATop", 2D) = "white" {}
		_FaceBBot("FaceBBot", 2D) = "white" {}
		_FaceBMid("FaceBMid", 2D) = "white" {}
		_FaceBTop("FaceBTop", 2D) = "white" {}
		_FaceCBot("FaceCBot", 2D) = "white" {}
		_FaceCMid("FaceCMid", 2D) = "white" {}
		_FaceCTop("FaceCTop", 2D) = "white" {}
		_SeamTex("SeamTex", 2D) = "white" {}

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
			float _FrontFov;
			float _HideSize;
			float _HideSeamsSize;
			float4 _HideColor;
			float _Offset;
			float _EyeInfo;
			float _UseSeamTex;

			sampler2D _FaceABot;
			sampler2D _FaceAMid;
			sampler2D _FaceATop;
			sampler2D _FaceBBot;
			sampler2D _FaceBMid;
			sampler2D _FaceBTop;
			sampler2D _FaceCBot;
			sampler2D _FaceCMid;
			sampler2D _FaceCTop;
			sampler2D _SeamTex;

#ifdef ORIENT_CUBE
			float4x4 _OrientMatrix;
#endif

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			#include "EquirectangularRotateTriangle.cginc"

			fixed4 frag(v2f i) : SV_Target
			{
				float2 px, px2;
				fixed4 c =
					equirectangular(
						i, _FaceABot, _FaceAMid, _FaceATop, _FaceBBot, _FaceBMid, _FaceBTop, _FaceCBot, _FaceCMid, _FaceCTop, _SeamTex, _UseSeamTex,

						_Beta, _HorizontalFov, _VerticalFov, _FrontFov, px, _HideSize, _HideSeamsSize, _Offset, _EyeInfo, _HideColor);

				return c;
			}

			ENDCG
		}
	}
}