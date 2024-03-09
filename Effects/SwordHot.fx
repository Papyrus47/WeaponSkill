sampler uImage0 : register(s0); // 需要扭曲的原图片
//sampler uImage1 : register(s1); // 扭曲用图片
texture2D tex;
sampler2D uImage1 = sampler_state
{
    Texture = <tex>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

float uTime;

float4 SwordHot(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 color1 = tex2D(uImage1, coords);
    if (color1.r <= 0.05 && color1.g <= 0.05 && color1.b <= 0.05)
    {
        return color;
    }
    color *= color1 * uTime;
    return color;
}

technique Technique1
{
    pass SwordHotShader
    {
        PixelShader = compile ps_2_0 SwordHot();

    }
}