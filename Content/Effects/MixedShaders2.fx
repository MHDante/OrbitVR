float4x4 mvp;
sampler _sampler : register(s0);
Texture2D ModelTexture;
float SpriteCount = 45;
static const float PI = 3.14159265f;
static const float DEG2RAD = PI / 180.0f;
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
  float2 tex : TEXCOORD;
};

VSGS_IN VS(VSGS_IN input)
{ 
  return input;
}

[maxvertexcount(4)]
void GS(point VSGS_IN sprite[1], inout TriangleStream<PS_IN> triStream)
{

  PS_IN output;
  output.color = sprite[0].color;


  float x = sprite[0].pos.x;
  float y = sprite[0].pos.y;
  float z = sprite[0].pos.z;

  float4 base = float4(0, 0, 500, 1);
  float t = sprite[0].rot;
  float2x2 trans = { cos(t), sin(t),-sin(t),cos(t) };
  float2 wh = { sprite[0].size.x / 2, sprite[0].size.y / 2 };
  float2 ab2 = { -wh.x, wh.y };
  float2 ooo = mul(wh, trans);
  float2 ooo2 = mul(ab2, trans);
  float4 oo = float4(ooo.x, ooo.y, 0, 0);
  float4 oo2 = float4(ooo2.x, ooo2.y, 0, 0);

  float tx = x * DEG2RAD;
  float ty = y * DEG2RAD;
  float s = sin(tx);
  float c = cos(tx);
  float4x4 rotx = 
  { 1,0,0,0 ,
   0,c,s,0 ,
   0,-s,c,0 ,
   0,0,0,1 };
  s = sin(ty);
  c = cos(ty);
  float4x4 roty = 
  { c,0,-s,0 ,
   0,1,0,0 ,
   s,0,c,0 ,
   0,0,0,1 };

  float4x4 rot = mul(rotx, roty);


  float4 o = mul(oo,rot);
  float4 o2 = mul(oo2, rot);
  float4 rbase = mul(base, rot);


  float texTop = (1 / SpriteCount) * (float)sprite[0].textureIndex;
  float texBot = (1 / SpriteCount) * (float)(sprite[0].textureIndex + 1);
  


  //bottom left
  output.pos = mul(float4(rbase.x - o.x, rbase.y - o.y, rbase.z - o.z, 1), mvp);
  output.tex = float2(0, texBot);
  triStream.Append(output);
  //top left
  output.pos = mul(float4(rbase.x + o2.x, rbase.y + o2.y, rbase.z + o2.z, 1), mvp);
  output.tex = float2(0, texTop);
  triStream.Append(output);
  //bottom right
  output.pos = mul(float4(rbase.x - o2.x, rbase.y - o2.y, rbase.z - o2.z, 1), mvp);
  output.tex = float2(1, texBot);
  triStream.Append(output);
  //top right
  output.pos = mul(float4(rbase.x + o.x, rbase.y + o.y, rbase.z + o.z, 1), mvp);
  output.tex = float2(1, texTop);
  triStream.Append(output);

	
};



float4 PS(PS_IN input) : SV_Target
{

  	float4 tex = ModelTexture.Sample(_sampler, input.tex);
	return tex * input.color;
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
