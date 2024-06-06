sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // Automatically Images/Misc/Perlin via Force Shader testing option
sampler uImage2 : register(s2); // Automatically Images/Misc/noise via Force Shader testing option
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 color1 = tex2D(uImage1, float2(uTime + coords.x, coords.y * 2 + uTime * 0.7));
    //color1.r *= 0.3;
    //color1.g *= 0.5;
    //color1.b *= 0.4;
    color1 *= 1.2;
    if (color1.r <= 0.4)
    {
        color1.r *= uColor.r;
        color1.g *= uColor.g;
        color1.b *= uColor.b;
    }
    else
    {
        color = tex2D(uImage0, coords + float2(0.02 * color1.r, 0.003 * color.r));
    }
    color *= color1;
    return color;
}

technique Technique1
{
    pass ModdersToolkitShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}