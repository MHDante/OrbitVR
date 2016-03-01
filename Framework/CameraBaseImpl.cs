using System;
using System.Collections.Generic;
using OrbitVR.Components.Drawers;
using SharpDX;

namespace OrbitVR.Framework {
  public struct SpriteVertex
  {
    public Vector3 Pos; // POSITION;
    public Vector2 Size; // SIZE;
    public float Rotation; // ROTATION;
    public int TextureIndex; // TEXIND;
    public Color Color; // COLOR;


    public SpriteVertex(Vector3 pos, Vector2 size, float rotation = 0, int textureIndex = 0, Color? color = null)
    {
      this.Pos = pos;
      this.Size = size;
      this.Rotation = rotation;
      this.TextureIndex = textureIndex;
      this.Color = color ?? Color.White;
    }
  }

  class CameraBaseImpl : CameraBase {

    public HashSet<SpriteVertex> Mesh;
    public HashSet<SpriteVertex> Perms; 
    public CameraBaseImpl(Room room, float zoom, Vector2? pos) : base(room, zoom, pos) {
      Mesh = new HashSet<SpriteVertex>();
      Perms = new HashSet<SpriteVertex>();
    }
    public override void AddPermanentDraw(Textures texture, Vector2 position, Color color, Vector2 scalevect, float rotation, int life) {
      var v = new SpriteVertex(position.toV3(),scalevect,rotation,(int)texture, color);
      Perms.Add(v);
    }

    public override void AddPermanentDraw(Textures texture, Vector2 position, Color color, float scale, float rotation, int life) {
      AddPermanentDraw(texture, position, color, Vector2.One * scale, rotation, life);
    }

    public override void Draw(Textures texture, Vector2 position, Color color, Vector2 scalevect, float rotation, Layers layer) {
      var v = new SpriteVertex(new Vector3(position.X, position.Y, (int)layer), scalevect, rotation, (int)texture, color);
      Perms.Add(v);
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
      Perms.RemoveWhere(x => x.Pos.toV2() == position);
    }


  }
}