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
    float4 color = tex2D(uImage0, coords);
    float4 color1 = tex2D(uImage1, float2(coords.x, 0.35 + coords.y * 0.5));
    if (color.r > 0 && color.b > 0 && color.g > 0)
    {
        color *= uColorChange;
        color *= color1;
    }
    color.a = alpha;
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
    pass StarSpinBladeShader
    {
        PixelShader = compile ps_2_0 SwingShader();
        VertexShader = compile vs_2_0 VertexShaderFunction();
    }
}