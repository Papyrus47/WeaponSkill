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

float uOffset;

float4 Offset(float2 coords : TEXCOORD0) : COLOR0
{
    // 以 r 为 偏移角度
    // 以 g 为 偏移大小
    float4 color = tex2D(uImage0, coords);
    float4 color1 = tex2D(uImage1, coords);
    if (!any(color1))
    {
        return color;
    }
    float2 vec = float2(0, 0);
    float rot = color1.r * 6.28; // 变成弧度制
    vec = float2(cos(rot), sin(rot)) * color1.g * uOffset;
    return tex2D(uImage0, coords + vec);
    //return float4(vec.x, vec.y, 0, 0);

}

technique Technique1
{
    pass OffsetShader
    {
        PixelShader = compile ps_2_0 Offset();

    }
}