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

 /**
  * Hey, it's already specified in the license lines above, but nobody reads it,
  * so again, PLEASE credit me if you copy paste those, it took me some time to
  * do the math here.
  */

#define PI 3.141592654

float4 sampleCube(float x, float y, float z, sampler2D tex, float b, out float2 px, float myScale)
{
	float scale = 1.0 / x * b;

	px.x = (z * scale + 1.0) / 2.0f;
	px.y = 1 - (y * scale + 1.0) / 2.0f;

	float wz = smoothstep(b, 1, abs(z) * scale);
	float wy = smoothstep(b, 1, abs(y) * scale);
	float4 color;
	float2 pos = px; //  0.5 + (px - 0.5) * myScale;
	 pos.x = 0.5 + (pos.x - 0.5) * myScale;
	 pos.y = 0.5 + (pos.y - 0.5) * myScale;
	color.a = 1 - max(wy, wz);
//	if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y> 1)
//		return float4(0.75, 0.5, 0.5, 1);
	color.rgb = tex2D(tex, pos);// *color.a;
	return color;
}

fixed4 equirectangular(
	v2f i,
	sampler2D faceABot,
	sampler2D faceAMid,
	sampler2D faceATop,
	sampler2D faceBBot,
	sampler2D faceBMid,
	sampler2D faceBTop,
	sampler2D faceCBot,
	sampler2D faceCMid,
	sampler2D faceCTop,
	sampler2D seamTex,
	float useSeamTex,
#ifdef ORIENT_CUBE
	float3x3 orientMatrix,
#endif
	float beta,
	float hfov,
	float vfov,
	float frontFov,
	out float2 px,
	float hideSize,
	float hideSeamsSize,
	float offset,
	float eyeInfo,
	float4 hideColor)
{
	float theta = (i.uv.x - 0.5) * hfov;
	float phi = -(i.uv.y - 0.5) * vfov;

	float x = cos(phi) * sin(theta);
	float y = sin(phi);
	float z = cos(phi) * cos(theta);

#ifdef ORIENT_CUBE
	float3 orientedDir = mul(orientMatrix, float3(x, y, z));
	x = orientedDir.x;
	y = orientedDir.y;
	z = orientedDir.z;
#endif
	px = float2(0, 0);
	float4 color = float4(0, 0, 0, 0);

	float seamsProx = abs(theta) - frontFov/2 + offset * eyeInfo * (PI / 2) * (theta > 0 ? 1 : -1);
	float seamsProx2 = i.uv.x * hfov + offset * eyeInfo * (PI / 2);
	float seamsProx3 = 2 * PI - i.uv.x * hfov - offset * eyeInfo * (PI / 2);
	if (abs(seamsProx) < hideSeamsSize || (hfov > 0.95 * 2 * PI && (seamsProx2 < hideSeamsSize || seamsProx3 < hideSeamsSize)))
	{
		if (useSeamTex)
			return tex2D(seamTex, float2(seamsProx / hideSeamsSize, i.uv.y)) + (hideColor * 2 - 0.5);
		else
			return hideColor;
	}

	if (i.uv.y < hideSize || i.uv.y > 1 - hideSize)
		return hideColor;

	float sideFov = (2 * PI - frontFov)/2;
	float rot = (frontFov + sideFov) / 2;
	float ratio = tan((PI - frontFov) / 2); // 1/sqrt(3) = tan(PI/6) = tan ((180°-120°)/2)
	float sideratio = tan((PI - sideFov) / 2);
	if (z / abs(x) >= ratio) // front
	{
		if (z / abs(y) >= ratio)
			color += sampleCube(z, y, x, faceAMid, beta, px, ratio);
		else if (y > 0)
			color += sampleCube(y, -z, x, faceABot, beta, px, ratio);
		else
			color += sampleCube(-y, z, x, faceATop, beta, px, ratio);
	}
	else if (x > 0) // right
	{
		float ax = cos(rot) * x - sin(rot) * z;
		z = sin(rot) * x + cos(rot) * z;
		if (z / abs(y) >= sideratio)
			color += sampleCube(z, y, ax, faceBMid, beta, px, sideratio);
		else if (y > 0)
			color += sampleCube(y, -z, ax, faceBBot, beta, px, sideratio);
		else
			color += sampleCube(-y, z, ax, faceBTop, beta, px, sideratio);
	}
	else // left
	{
		float ax = cos(-rot) * x - sin(-rot) * z;
		z = sin(-rot) * x + cos(-rot) * z;
		if (z / abs(y) >= sideratio)
			color += sampleCube(z, y, ax, faceCMid, beta, px, sideratio);
		else if (y > 0)
			color += sampleCube(y, -z, ax, faceCBot, beta, px, sideratio);
		else
			color += sampleCube(-y, z, ax, faceCTop, beta, px, sideratio);
	}

	color.a = 1.0;

#ifdef SHOW_STITCH_LINES
	color = lerp(color, 1 - color, max(smoothstep(0.49 * beta, 0.5 * beta, abs(px.x - 0.5)), smoothstep(0.49 * beta, 0.5 * beta, abs(px.y - 0.5))));
#endif

	px = (px - 0.5) / beta + 0.5; // for double render fusion, px must be in [0, 1]

	return color;
}