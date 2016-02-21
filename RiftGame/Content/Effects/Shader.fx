sampler TextureSampler : register(s0);
float2 Viewport;

int4 colour;
int enabled;

void VertexShaderFunction(inout float4 color : COLOR0, inout float2 texCoord : TEXCOORD0, inout float4 position : POSITION0)
{
	// Half pixel offset for correct texel centering. 
	position.xy -= 0.5;

	// Viewport adjustment. 
	position.xy = position.xy / Viewport;
	position.xy *= float2(2, -2);
	position.xy -= float2(1, -1);
}

float4 halfScreen(float2 texCoord, float4 screenSpace)
{
	float4 finalcolor;

	float4 tex = tex2D(TextureSampler, texCoord);

		float diff = texCoord.r;
	finalcolor = float4(diff*tex.a, diff * tex.a, diff * tex.a, tex.a);

	finalcolor *= (colour / 255.0f) * tex.rgba;

	if (screenSpace.r > (Viewport.r / 2.0f))
	{
		if (finalcolor.a > 0.1)
		{
			finalcolor.a = 0.5f;
		}
	}
	return finalcolor;
}

float4 PixelShaderFunction(float4 color : COLOR0, float2 texCoord : TEXCOORD0, float4 screenSpace : SV_Position) : COLOR0
{
	float4 tex = tex2D(TextureSampler, texCoord);
	if (enabled < 1)
	{
		return float4(tex.rgb, tex.a);
	}
	else
		return halfScreen(texCoord, screenSpace);
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}