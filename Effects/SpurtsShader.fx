sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);

struct Input
{
    float2 Pos : POSITIONT0;
    float3 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

float4 PSShader(Input input) : COLOR0
{
    float2 coord = input.TexCoords.xy;
    float4 texColor = tex2D(uImage0, coord);
    float4 color = input.Color;
    color.rbg *= coord.x;
    texColor *= color;
    float4 tex1Color = tex2D(uImage1, coord);
    tex1Color *= 0.5;
    tex1Color *= color;
    color = max(tex1Color, texColor);
    color = tex2D(uImage2,float2(0.5,color.r));
    return color;
}

technique Technique1
{
    pass SpurtsShader
    {
        PixelShader = compile ps_2_0 PSShader();
    }
}