float4x4 Model;
float4x4 View;
float4x4 Projection;
Texture2D ModelTexture;
float SpriteCount = 45;
// clamped mip map nearest neigbor sampling
SamplerState SampPointClamp
{
	AddressU = Clamp;
	AddressV = Clamp;
	Filter = MIN_MAG_MIP_POINT;
};
struct VSGS_IN
{
  float3 pos : POSITION;
  float2 size : SIZE;
  float rot : ROTATION;
  int textureIndex : TEXIND;
  float4 color : COLOR;
};

struct PS_IN
{
  float4 pos : SV_POSITION;
  float4 color : COLOR;
  float2 tex : TEX;
};

[maxvertexcount(4)]
void GS(point VSGS_IN sprite[1], inout TriangleStream<PS_IN> triStream)
{

	PS_IN output;
	output.color = sprite[0].color;
	float w = sprite[0].size.x / 2;
	float h = sprite[0].size.y / 2;
	float x = sprite[0].pos.x;
	float y = sprite[0].pos.y;
	float texTop = (1 / SpriteCount) * sprite[0].textureIndex;
	float texBot = (1 / SpriteCount) * sprite[0].textureIndex + 1;
	float4x4 mvp = Model * View * Projection;
    //bottom left
	output.pos = mul(mvp , float4(x - w, y - h, 1, 1));
	output.tex = float2(0, texBot);
	triStream.Append(output);
	//top left
	output.pos = mul(mvp , float4(x - w, y + h, 1, 1));
	output.tex = float2(0, texTop);
	triStream.Append(output);
	//bottom right
	output.pos = mul(mvp , float4(x + w, y - h, 1, 1));
	output.tex = float2(1, texBot);
	triStream.Append(output);
	//top right
	output.pos = mul(mvp , float4(x + w, y + h, 1, 1));
	output.tex = float2(1, texTop);
	triStream.Append(output);

	
};

VSGS_IN VS(VSGS_IN input)
{ 
  return input;
}

float4 PS(PS_IN input) : SV_Target
{

  return input.color;
}

technique10 Render
{
  pass P0
  {
		SetGeometryShader(CompileShader(gs_4_0, GS()));
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
  }
}
