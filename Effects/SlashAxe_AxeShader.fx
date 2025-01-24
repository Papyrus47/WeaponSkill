sampler uImage0 : register(s0); // 原图片
texture2D tex;
sampler2D uImage1 = sampler_state // 柏林噪声
{
    Texture = <tex>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

float2 uTime;
float4 uColor;

float4 AxeHot(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 color1 = tex2D(uImage1, coords * 0.1 + uTime);
    if (color1.r <= 0.05 && color1.g <= 0.05 && color1.b <= 0.05)
    {
        return color;
    }
    color *= color1;
    color *= uColor;
    return color;
}

technique Technique1
{
    pass AxeShader
    {
        PixelShader = compile ps_2_0 AxeHot();

    }
}