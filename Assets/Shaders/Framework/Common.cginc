float MapRange(float value, float min1, float max1, float min2, float max2)
{
    return ((value - min1) / (max1 - min1)) * (max2 - min2) + min2;
}

float2 ffloat2(float xy)
{
    return float2(xy,xy);
}

float3 ffloat3(float2 v)
{
    return float3(v.x, v.y, 0);
}float3 ffloat3(float2 v, float z)
{
    return float3(v.x, v.y, z);
}
float3 ffloat3(float xyz)
{
    return float3(xyz,xyz,xyz);
}
float4 ffloat4(float2 v)
{
    return float4(v.x, v.y,0,0);
}
float4 ffloat4(float2 v, float z)
{
    return float4(v.x, v.y, z, 0);
}
float4 ffloat4(float2 v, float z, float w)
{
    return float4(v.x, v.y, z, w);
}
float4 ffloat4(float3 v)
{
    return float4(v.x, v.y, v.z, 0);
}
float4 ffloat4(float3 v, float w)
{
    return float4(v.x, v.y, v.z, w);
}
float4 ffloat4(float xyzw)
{
    return float4(xyzw,xyzw,xyzw,xyzw);
}

float4 debug_color(float brightness)
{
    return float4(brightness,brightness,brightness, 1);
}
float4 debug_color(float2 v)
{
    return ffloat4(v.x, 0, 1);
}
float4 debug_color(float2 v, float b)
{
    return ffloat4(v.x, b, 1);
}
float4 debug_color(float3 v)
{
    return ffloat4(v, 1);
}