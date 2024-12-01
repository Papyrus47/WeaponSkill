sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uIamge2 : register(s2);

float4x4 uTransform;
float uColorChange;
float uTime; // 刀光需要自然消失时候用

struct VSInput
{
    float2 Pos : POSITION0;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

float4 SwingShader(PSInput input) : COLOR0
{
    float2 coords = input.Texcoord.xy;
    float alpha = input.Texcoord.z;
    float2 offset = floor((coords - float2(0.5, 0.5)) * 150) / 150;

    float4 color = tex2D(uImage0, float2(0.5, 0.5) + offset);
    float4 color1 = tex2D(uImage1, coords);
    color *= uColorChange;
    color *= color1 * 2;
    color.a = alpha;
    float a = 0.5;
    if (color.r > a && color.b > a && color.g > a)
    {
        color.a = 1 - alpha;
    }
    return color;
}
float4 DisappearSwingShader(float2 texCoords : TEXCOORD0,float4 drawColor : COLOR0) : COLOR0
{
    // 注意,这里的 uIamge1 必须是柏林噪声图像
    float4 color = tex2D(uImage0,texCoords.xy);
    color *= drawColor;
    float4 PerlinColor = tex2D(uImage1, texCoords.xy); // 获取对应的纹理坐标上的柏林噪声图
    if (PerlinColor.r > uTime) // 截取
        return float4(0, 0, 0, 0);
    color.a = 0;
    return color;
}
float4 MixSwingShader(PSInput input) : COLOR0 // 混合方法
{
    float2 coords = input.Texcoord.xy;
    float alpha = input.Texcoord.z;
    float2 offset = floor((coords - float2(0.5, 0.5)) * 150) / 150;

    float4 color = tex2D(uImage0, float2(0.5, 0.5) + offset);
    float4 color1 = tex2D(uImage1, coords);
    if (color.r > 0 && color.b > 0 && color.g > 0)
    {
        color *= uColorChange;
        color *= color1;
    }
    float4 PerlinColor = tex2D(uIamge2, coords); // 获取对应的纹理坐标上的柏林噪声图
    if (PerlinColor.r > uTime) // 截取
        return float4(0, 0, 0, 0);
    return color;
}

PSInput VertexShaderFunction(VSInput input)
{
    PSInput output;
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

technique Technique1
{
    pass SwingEffect
    {
        PixelShader = compile ps_2_0 SwingShader();
        VertexShader = compile vs_2_0 VertexShaderFunction();
    }

    pass DisappearSwing // 这个是让自己随风飘散的shader(
    {
        PixelShader = compile ps_3_0 DisappearSwingShader();
    }
    
    pass MixSwingEffect // 混合的特效绘制
    {
        PixelShader = compile ps_2_0 MixSwingShader();
        VertexShader = compile vs_2_0 VertexShaderFunction();
    }
}
