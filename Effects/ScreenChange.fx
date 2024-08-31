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
float uIntensity; // 作为缩放比例
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 ScaleScreen(float2 coords : TEXCOORD0) : COLOR0
{
    //float2 size = uImageSize1 / uIntensity;
    float2 pos = float2(0.5, 0.5); // 取中心点
    float2 offset = coords - pos; // 计算距离差距
    //float2 rpos = offset * float2(uScreenResolution.x / uScreenResolution.y, 1);
    //float dis = length(rpos);
    float4 color = tex2D(uImage0, pos + offset * uIntensity); // 计算对应点的颜色
    return color;
}

technique Technique1
{
    pass ScaleScreen // 屏幕缩放
    {
        PixelShader = compile ps_2_0 ScaleScreen();
    }
}