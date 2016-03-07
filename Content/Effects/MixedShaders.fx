float4x4 mvp;
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
  //float3 float2 float int float4
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
  float h = sprite[0].size.y / 2;
  float x = sprite[0].pos.x;
  float y = sprite[0].pos.y;
  float texTop = (1 / SpriteCount) * sprite[0].textureIndex;
  float texBot = (1 / SpriteCount) * sprite[0].textureIndex + 1;
  float t = sprite[0].rot;
  float2x2 trans = { cos(t), -sin(t),sin(t),cos(t) };
  trans = transpose(trans);
  float2 wh = { sprite[0].size.x / 2, sprite[0].size.y / 2 };
  float2 ab2 = { -wh.x, wh.y };
  float2 o = mul(trans, wh);
  float2 o2 = mul(trans, ab2);
  //bottom left
  output.pos = mul(float4(x - o.x, y - o.y, 1, 1), mvp);
  output.tex = float2(0, texBot);
  triStream.Append(output);
  //top left
  output.pos = mul(float4(x + o2.x, y + o2.y, 1, 1), mvp);
  output.tex = float2(0, texTop);
  triStream.Append(output);
  //bottom right
  output.pos = mul(float4(x - o2.x, y - o2.y, 1, 1), mvp);
  output.tex = float2(1, texBot);
  triStream.Append(output);
  //top right
  output.pos = mul(float4(x + o.x, y + o.y, 1, 1), mvp);
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
