using System;
using System.Collections.Concurrent;
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
    public class MeshCamera : CameraBase
    {
    
    private Buffer<SpriteVertex> Mesh;
      private ConcurrentQueue<List<SpriteVertex>> pendingVertexQueue;
    //private Buffer<SpriteVertex> Perms;
    
    private Effect effect;
    private VertexInputLayout layout;
    private Texture2D texture;

    private EffectParameter mvpParam;
    private EffectParameter textureParam;
    private EffectPass effectPass;
    private List<SpriteVertex> pendingVertices;
    private List<SpriteVertex> drawingVertices;
    //private List<SpriteVertex> permVertices;
    private GraphicsDevice device;
    private EffectParameter spriteCountParam;
      private EffectParameter textureSamplerParameter;

      public MeshCamera(Room room, float zoom, Vector2R? pos) : base(room, zoom, pos) {
      pendingVertices = new List<SpriteVertex>();
      //permVertices = new List<SpriteVertex>();
      //Perms = new HashSet<SpriteVertex>();
      pendingVertexQueue = new ConcurrentQueue<List<SpriteVertex>>();
      device = OrbIt.Game.GraphicsDevice;
      Mesh = Buffer.Vertex.New<SpriteVertex>(OrbIt.Game.GraphicsDevice, 16 * 1024);
      layout = VertexInputLayout.FromBuffer(0, Mesh);
      
      effect = OrbIt.Game.Content.Load<Effect>("Effects/MixedShaders2");
      texture = OrbIt.Game.Content.Load<Texture2D>("Textures/spritesheet");
      
      mvpParam = effect.Parameters["mvp"];
      spriteCountParam = effect.Parameters["SpriteCount"];
      textureParam = effect.Parameters["ModelTexture"];

      textureSamplerParameter = effect.Parameters["_sampler"];
      effectPass = effect.Techniques["Render"].Passes[0];
      textureParam.SetResource(texture);

      textureSamplerParameter.SetResource(device.SamplerStates.LinearClamp);
      spriteCountParam.SetValue((float)Enum.GetValues(typeof(Textures)).Length);

    }
    public override void AddPermanentDraw(Textures texture, Vector2R position, Color color, Vector2 scalevect, float rotation, int life) {
      var v = new SpriteVertex((Vector3)position,scalevect,rotation,(int)texture, color);
      pendingVertices.Add(v);//TODO:PERMS
    }

    public override void AddPermanentDraw(Textures texture, Vector2R position, Color color, float scale, float rotation, int life) {
      AddPermanentDraw(texture, position, color, Vector2.One * scale, rotation, life);
    }

    public override void Draw(Textures texture, Vector2R position, Color color, Vector2 scalevect, float rotation, float depth) {
      var v = new SpriteVertex(new Vector3(position.X, position.Y, depth), scalevect*128, rotation, (int)texture, color);
      //Mesh.SetData(ref v);
      pendingVertices.Add(v);
    }

    public override void Draw(Textures texture, Vector2R position, Color color, float scale, float depth) {
      Draw(texture,position,color,Vector2.One * scale, 0, depth);
    }

    public override void Draw(Textures texture, Vector2R position, Color color, float scale, float rotation, float depth) {
      Draw(texture, position, color, Vector2.One * scale, rotation, depth);
    }

    public override void DrawLine(Vector2R start, Vector2R end, float thickness, Color color, float depth) {
      if (thickness * zoom < 1) thickness = 1 / zoom;
      Vector2R diff = (end - start); // *mapzoom;
      Vector2R centerpoint = (end + start) / 2;
      //centerpoint *= mapzoom;
      float len = diff.Length();
      //thickness *= 2f * mapzoom;
      Vector2 scalevect = new Vector2(len, thickness)/128;
      float angle = (float)(Math.Atan2(diff.Y, diff.X));
      Draw(Textures.Whitecircle, centerpoint, color, scalevect, angle, depth);
    }

    public override void DrawLinePermanent(Vector2R start, Vector2R end, float thickness, Color color, int life) {
      if (thickness * zoom < 1) thickness = 1 / zoom;
      Vector2R diff = (end - start); // *mapzoom;
      Vector2R centerpoint = (end + start) / 2;
      //centerpoint *= mapzoom;
      float len = diff.Length();
      //thickness *= 2f * mapzoom;
      Vector2 scalevect = new Vector2(len, thickness);
      float angle = (float)(Math.Atan2(diff.Y, diff.X));
      //Draw(textures.whitepixel, centerpoint, null, color, angle, Assets.textureCenters[textures.whitepixel], scalevect, Layer);
      AddPermanentDraw(Textures.Whitecircle, centerpoint, color, scalevect, angle, life);
    }

    public override void DrawStringScreen(string text, Vector2R position, Color color, Color? color2 = null, float scale = 0.5f,
                                          bool offset = true, Layers layer = Layers.Over5) {
      return;
    }

    public override void DrawStringWorld(string text, Vector2R position, Color color, Color? color2 = null, float scale = 0.5f,
                                         bool offset = true, Layers layer = Layers.Over5) {

      return;
    }

    public override void removePermanentDraw(Textures texture, Vector2R position, Color color, float scale) {
      //Perms.RemoveWhere(x => x.Pos.toV2() == position);
    }

    public override void Update()
    {
      base.Update();
      pendingVertices.Clear();
    }

    
    public override void Draw(Matrix world)
    {
      List<SpriteVertex> newVerts;
      var gotNewVerts = pendingVertexQueue.TryDequeue(out newVerts);
      if (gotNewVerts) drawingVertices = newVerts;
      if (drawingVertices == null)
        return; 
      //pendingVertices.Add(new SpriteVertex(
      //  new Vector3(OrbIt.ScreenWidth / 2, OrbIt.ScreenHeight / 2, 0),
      //  new Vector2(OrbIt.ScreenHeight, OrbIt.ScreenWidth)));
      mvpParam.SetValue(world * OrbIt.Game.view * OrbIt.Game.projection);
      //pendingVertices.AddRange(permVertices);
      var array = drawingVertices.Count == 0
        ? new[]
        {
          new SpriteVertex(Vector3.Right, Vector2.One, color: Color.Red),
          new SpriteVertex(Vector3.Zero, Vector2.One, (float) OrbIt.Game.Time.TotalGameTime.TotalSeconds,
            color: Color.Green),
          new SpriteVertex(Vector3.Left, Vector2.One, color: Color.Blue),
          new SpriteVertex(Vector3.Up, Vector2.One, color: Color.Yellow),
          new SpriteVertex(Vector3.Down, Vector2.One, color: Color.Brown),

          new SpriteVertex(Vector3.Down + Vector3.Left, Vector2.One, color: Color.Brown*Color.Blue),
          new SpriteVertex(Vector3.Down + Vector3.Right, Vector2.One, color: Color.Brown*Color.Red),
          new SpriteVertex(Vector3.Up + Vector3.Left, Vector2.One, color: Color.Yellow*Color.Blue),
          new SpriteVertex(Vector3.Up + Vector3.Right, Vector2.One, color: Color.Yellow*Color.Red),
        }
        : drawingVertices.ToArray();
      Mesh.SetData(array);
      device.SetVertexBuffer(Mesh);
      device.SetVertexInputLayout(layout);
      device.SetBlendState(device.BlendStates.AlphaBlend);
      device.SetDepthStencilState(device.DepthStencilStates.None);
      effectPass.Apply();

      //device.DrawAuto(PrimitiveType.PointList);
      device.Draw(PrimitiveType.PointList, array.Length);
      device.SetDepthStencilState(null);
      device.SetBlendState(null);
      effectPass.UnApply();
    }

      public void EndDrawing()
      {
        pendingVertexQueue.Enqueue(pendingVertices);
        pendingVertices = new List<SpriteVertex>();
      }
    }
}