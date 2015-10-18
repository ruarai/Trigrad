kernel void Barycentric(
	global  read_only int2* p_p,
	global  read_only int2* p_a,
	global  read_only int2* p_b,
	global  read_only int2* p_c,
	global write_only float4* c,
	global write_only int* c_valid)
{
	int i = get_global_id(0);

	float v0[] = { p_b[i].x - p_a[i].x, p_b[i].y - p_a[i].y };
	float v1[] = { p_c[i].x - p_a[i].x, p_c[i].y - p_a[i].y };
	float v2[] = { p_p[i].x - p_a[i].x, p_p[i].y - p_a[i].y };

	float d00 = v0[0] * v0[0] + v0[1] * v0[1];
	float d01 = v0[0] * v1[0] + v0[1] * v1[1];
	float d11 = v1[0] * v1[0] + v1[1] * v1[1];
	float d20 = v2[0] * v0[0] + v2[1] * v0[1];
	float d21 = v2[0] * v1[0] + v2[1] * v1[1];

	float denom = d00 * d11 - d01 * d01;

	float v = ((d11 * d20 - d01 * d21) / denom);
	float w = ((d00 * d21 - d01 * d20) / denom);
	float u = (1 - v - w);

	c[i] = (float4)(u, v, w,0);

	if (u >= 0 && v >= 0 && w >= 0)
	{
		c_valid[i] = 1;
	}
	else
	{
		c_valid[i] = 0;
	}
}