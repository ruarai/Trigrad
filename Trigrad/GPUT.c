kernel void Barycentric(
	global  read_only int* p_p_x,
	global  read_only int* p_p_y,
	global  read_only int* p_a_x,
	global  read_only int* p_a_y,
	global  read_only int* p_b_x,
	global  read_only int* p_b_y,
	global  read_only int* p_c_x,
	global  read_only int* p_c_y,
	global write_only float* c_u,
	global write_only float* c_v,
	global write_only float* c_w,
	global write_only int* c_valid)
{
	int i = get_global_id(0);

	float v0[] = { p_b_x[i] - p_a_x[i], p_b_y[i] - p_a_y[i] };
	float v1[] = { p_c_x[i] - p_a_x[i], p_c_y[i] - p_a_y[i] };
	float v2[] = { p_p_x[i] - p_a_x[i], p_p_y[i] - p_a_y[i] };

	float d00 = v0[0] * v0[0] + v0[1] * v0[1];
	float d01 = v0[0] * v1[0] + v0[1] * v1[1];
	float d11 = v1[0] * v1[0] + v1[1] * v1[1];
	float d20 = v2[0] * v0[0] + v2[1] * v0[1];
	float d21 = v2[0] * v1[0] + v2[1] * v1[1];

	float denom = d00 * d11 - d01 * d01;

	float v = ((d11 * d20 - d01 * d21) / denom);
	float w = ((d00 * d21 - d01 * d20) / denom);
	float u = (1 - v - w);

	c_u[i] = u;
	c_v[i] = v;
	c_w[i] = w;

	if (u >= 0 && v >= 0 && w >= 0)
	{
		c_valid[i] = 1;
	}
	else
	{
		c_valid[i] = 0;
	}
}