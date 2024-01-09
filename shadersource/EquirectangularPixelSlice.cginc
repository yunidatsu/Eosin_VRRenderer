

#define PI 3.141592654
float w = 0;

float4 sampleCube(float x, float y, sampler2D tex, float myScale)
{
	float texy = 1 - (y / x + 1.0) / 2.0f;
	texy = 0.5 + (texy - 0.5) * myScale;

	float4 color;
	//	if (pos.x < 0 || pos.x>1 || pos.y < 0 || pos.y>1)
//		return float4(1, 1, 0, 1);
	color = tex2D(tex, float2(0, texy));
//	if (color.a < 0.2)
//		return float4(1, 0, 1, 1);
	return color;
}

fixed4 equirectangular(
	v2f i,
	sampler2D botTex,
	sampler2D midTex,
	sampler2D topTex,
	float verticalFov)
{
	float phi = -(i.uv.y - 0.5) * PI;

	float y = sin(phi);
	float z = cos(phi);
	
	float4 color = float4(0, 0, 0, 0);

	float ratio = tan((PI - verticalFov)/2.0); //  tan((180°-60°)/2) = tan(PI/3)
	float radAng = verticalFov / 2.0; //
	if(abs(phi) < radAng) // mid
		color += sampleCube(z, y, midTex, ratio);
	else if (y > 0) // bot
	{
		float az = cos(-verticalFov) * z - sin(-verticalFov) * y;
		y = sin(-verticalFov) * z + cos(-verticalFov) * y;
		color += sampleCube(az, y, botTex, ratio);

		/*float az = cos(radAng) * z - sin(radAng) * y;
		y = sin(radAng) * z + cos(radAng) * y;
		color += sampleCube(y, -az, botTex, ratio);*/
	}
	else // top
	{
		float az = cos(verticalFov) * z - sin(verticalFov) * y;
		y = sin(verticalFov) * z + cos(verticalFov) * y;
		color += sampleCube(az, y, topTex, ratio);

		/*float az = cos(-radAng) * z - sin(-radAng) * y;
		y = sin(-radAng) * z + cos(-radAng) * y;
		color += sampleCube(-y, az, topTex, ratio);*/
	}

	// color.a = 1.0;

	return color;
}
