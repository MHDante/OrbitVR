using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Toolkit.Graphics;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;
using VertexBufferBinding = SharpDX.Direct3D11.VertexBufferBinding;

namespace OrbitVR.Framework {

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct SpriteVertex
  {
    [VertexElement("POSITION")]
    public Vector3 Pos; // POSITION;
    [VertexElement("SIZE")]
    public Vector2 Size; // SIZE;
    [VertexElement("ROTATION")]
    public float Rotation; // ROTATION;
    [VertexElement("TEXIND")]
    public int TextureIndex; // TEXIND;
    [VertexElement("COLOR")]
    public Vector4 Color; // COLOR;

    public SpriteVertex(Vector3 pos, Vector2 size, float rotation = 0, int textureIndex = 0, Color? color = null)
    {
      this.Pos = pos;
      this.Size = size;
      this.Rotation = rotation;
      this.TextureIndex = textureIndex;
      this.Color = color?.ToVector4() ?? SharpDX.Color.White.ToVector4();
    }
  }
  class MeshCamera : CameraBase {
    
    private Buffer<SpriteVertex> Mesh;
    //private Buffer<SpriteVertex> Perms;
    
    private Effect effect;
    private VertexInputLayout layout;
    private Texture2D texture;

    private EffectParameter mvpParam;
    private EffectParameter textureParam;
    private EffectPass effectPass;
    private List<SpriteVertex> pendingVertices;
    private List<SpriteVertex> permVertices;
    private GraphicsDevice device;

    public MeshCamera(Room room, float zoom, Vector2? pos) : base(room, zoom, pos) {
      pendingVertices = new List<SpriteVertex>();
      permVertices = new List<SpriteVertex>();
      //Perms = new HashSet<SpriteVertex>();
      device = OrbIt.Game.GraphicsDevice;
      Mesh = Buffer.Vertex.New<SpriteVertex>(OrbIt.Game.GraphicsDevice, 16 * 1024);
      layout = VertexInputLayout.FromBuffer(0, Mesh);
      effect = OrbIt.Game.Content.Load<Effect>("Effects/MixedShaders");
      texture = OrbIt.Game.Content.Load<Texture2D>("Textures/spritesheet");
      
      mvpParam = effect.Parameters["mvp"];
      textureParam = effect.Parameters["ModelTexture"];
      effectPass = effect.Techniques["Render"].Passes[0];
      
    }
    public override void AddPermanentDraw(Textures texture, Vector2 position, Color color, Vector2 scalevect, float rotation, int life) {
      var v = new SpriteVertex(position.toV3(),scalevect,rotation,(int)texture, color);
      pendingVertices.Add(v);
    }

    public override void AddPermanentDraw(Textures texture, Vector2 position, Color color, float scale, float rotation, int life) {
      AddPermanentDraw(texture, position, color, Vector2.One * scale, rotation, life);
    }

    public override void Draw(Textures texture, Vector2 position, Color color, Vector2 scalevect, float rotation, Layers layer) {
      var v = new SpriteVertex(new Vector3(position.X, position.Y, (int)layer), scalevect, rotation, (int)texture, color);
      Mesh.SetData(ref v);
      pendingVertices.Add(v);
    }

    public override void Draw(Textures texture, Vector2 position, Color color, float scale, Layers layer) {
      Draw(texture,position,color,Vector2.One * scale, 0, layer);
    }

    public override void Draw(Textures texture, Vector2 position, Color color, float scale, float rotation, Layers layer) {
      Draw(texture, position, color, Vector2.One * scale, rotation, layer);
    }

    public override void DrawLine(Vector2 start, Vector2 end, float thickness, Color color, Layers layer) {
      if (thickness * zoom < 1) thickness = 1 / zoom;
      Vector2 diff = (end - start); // *mapzoom;
      Vector2 centerpoint = (end + start) / 2;
      //centerpoint *= mapzoom;
      float len = diff.Length();
      //thickness *= 2f * mapzoom;
      Vector2 scalevect = new Vector2(len, thickness);
      float angle = (float)(Math.Atan2(diff.Y, diff.X));
      Draw(Textures.Whitecircle, centerpoint, color, scalevect, angle, layer);
    }

    public override void DrawLinePermanent(Vector2 start, Vector2 end, float thickness, Color color, int life) {
      if (thickness * zoom < 1) thickness = 1 / zoom;
      Vector2 diff = (end - start); // *mapzoom;
      Vector2 centerpoint = (end + start) / 2;
      //centerpoint *= mapzoom;
      float len = diff.Length();
      //thickness *= 2f * mapzoom;
      Vector2 scalevect = new Vector2(len, thickness);
      float angle = (float)(Math.Atan2(diff.Y, diff.X));
      //Draw(textures.whitepixel, centerpoint, null, color, angle, Assets.textureCenters[textures.whitepixel], scalevect, Layer);
      AddPermanentDraw(Textures.Whitecircle, centerpoint, color, scalevect, angle, life);
    }

    public override void DrawStringScreen(string text, Vector2 position, Color color, Color? color2 = null, float scale = 0.5f,
                                          bool offset = true, Layers layer = Layers.Over5) {
      return;
    }

    public override void DrawStringWorld(string text, Vector2 position, Color color, Color? color2 = null, float scale = 0.5f,
                                         bool offset = true, Layers layer = Layers.Over5) {

      return;
    }

    public override void removePermanentDraw(Textures texture, Vector2 position, Color color, float scale) {
      //Perms.RemoveWhere(x => x.Pos.toV2() == position);
    }

    public override void Draw(Matrix world) {

      pendingVertices.Add(new SpriteVertex(
        new Vector3(OrbIt.ScreenWidth / 2, OrbIt.ScreenHeight / 2, 0),
        new Vector2(OrbIt.ScreenHeight, OrbIt.ScreenWidth)));
      mvpParam.SetValue(OrbIt.Game.view * OrbIt.Game.projection * world);
      pendingVertices.AddRange(permVertices);
      Mesh.SetData(pendingVertices.ToArray());
      device.SetVertexBuffer(Mesh);
      device.SetVertexInputLayout(layout);
      //device.SetBlendState(device.BlendStates.Additive);
      //device.SetDepthStencilState(device.DepthStencilStates.None);
      effectPass.Apply();

      //device.DrawAuto(PrimitiveType.PointList);
      device.Draw(PrimitiveType.PointList, pendingVertices.Count);
      //device.SetDepthStencilState(null);
      //device.SetBlendState(null);
      effectPass.UnApply();
      pendingVertices.Clear();
    }
  }
}