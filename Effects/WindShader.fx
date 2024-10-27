sampler uImage0 : register(s0); // 风的贴图

float Rot;
float4 WindShader(float2 coords : TEXCOORD0,float4 inputColor : COLOR0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 pos = float2(-0.5, -0.5);
    float2x2 rot = float2x2(cos(Rot), -sin(Rot), sin(Rot), cos(Rot));
    float2 uv = mul(coords + pos, rot);
    uv -= pos;
    return tex2D(uImage0, uv) * inputColor;
}

technique Technique1
{
    pass WindShader
    {
        PixelShader = compile ps_3_0 WindShader();
    }
}
