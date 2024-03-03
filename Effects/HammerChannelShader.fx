sampler uImage0 : register(s0);
float uTime;
float2 uImageSize;
float4 uColor;

float4 edge(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (any(color))
        return color;
    // 获取每个像素的正确大小
    float dx = 1 / uImageSize.x;
    float dy = 1 / uImageSize.y;
    bool flag = false;
    // 对周围进行判定
    for (int i = -2; i <= 2; i++)
    {
        for (int j = -2; j <= 2; j++)
        {
            float4 c = tex2D(uImage0, coords + float2(dx * i, dy * j));
            // 如果任何一个像素有颜色
            if (any(c))
            {
                // 不知道为啥，这里直接return会被编译器安排，所以只能打标记了
                flag = true;
            }
        }
    }
    if (flag)
    {
        float4 color1 = uColor;
        color1.rgb += cos(tan((uTime + coords.y) * 4));
        return color1;
    }
    return color;
}

technique Technique1
{
    pass HammerChannelShader
    {
        PixelShader = compile ps_3_0 edge();
    }
}