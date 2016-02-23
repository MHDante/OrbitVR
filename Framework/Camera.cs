using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace OrbitVR.Framework {
  public class Camera // : CameraBase
  {
    private static int _CameraOffset = 0;
    public static Vector2 CameraOffsetVect = new Vector2(0, 0);
    public static readonly SpriteFont font = Assets.font;
    public SpriteBatch batch;
    public Vector2 pos;

    public Room room;
    public float zoom;

    public static int CameraOffset {
      get { return _CameraOffset; }
      set {
        _CameraOffset = value;
        CameraOffsetVect = new Vector2(value, 0);
      }
    }

    public Camera(Room room, float zoom = 0.5f, Vector2? pos = null) {} // : base(room, zoom, pos) {}

    public virtual void Draw(textures texture, Vector2 position, Color color, float scale) {
      color *= ((float) color.A/255f);
      batch.Draw(Assets.textureDict[texture], ((position - pos)*zoom) + CameraOffsetVect, null, color, 0,
                 Assets.textureCenters[texture], scale*zoom, SpriteEffects.None, 0);
    }

    public virtual void Draw(textures texture, Vector2 position, Color color, float scale, float rotation) {
      color *= ((float) color.A/255f);
      batch.Draw(Assets.textureDict[texture], ((position - pos)*zoom) + CameraOffsetVect, null, color, rotation,
                 Assets.textureCenters[texture], scale*zoom, SpriteEffects.None, 0);
    }

    public virtual void Draw(textures texture, Vector2 position, Color color, Vector2 scalevect, float rotation) {
      color *= ((float) color.A/255f);
      batch.Draw(Assets.textureDict[texture], ((position - pos)*zoom) + CameraOffsetVect, null, color, rotation,
                 Assets.textureCenters[texture], scalevect*zoom, SpriteEffects.None, 0);
    }

    public virtual void Draw(textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                             Vector2 origin, float scale, SpriteEffects effects = SpriteEffects.None,
                             float layerDepth = 0) {
      color *= ((float) color.A/255f);
      batch.Draw(Assets.textureDict[texture], ((position - pos)*zoom) + CameraOffsetVect, sourceRect, color, rotation,
                 origin, scale*zoom, effects, layerDepth);
    }

    public virtual void Draw(textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                             Vector2 origin, Vector2 scalevect, SpriteEffects effects = SpriteEffects.None,
                             float layerDepth = 0) {
      color *= ((float) color.A/255f);
      batch.Draw(Assets.textureDict[texture], ((position - pos)*zoom) + CameraOffsetVect, sourceRect, color, rotation,
                 origin, scalevect*zoom, effects, layerDepth);
    }

    public virtual void DrawStringScreen(string text, Vector2 position, Color color, Color? color2 = null,
                                         float scale = 0.5f, bool offset = true) {
      Color c2 = Color.White;
      if (color2 != null) c2 = (Color) color2;
      Vector2 pos = position;
      if (offset) pos += CameraOffsetVect;
      batch.DrawString(font, text, pos, c2, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
      batch.DrawString(font, text, pos + new Vector2(1, -1), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
    }

    public virtual void DrawStringWorld(string text, Vector2 position, Color color, Color? color2 = null,
                                        float scale = 0.5f, bool offset = true) {
      Color c2 = Color.White;
      if (color2 != null) c2 = (Color) color2;
      Vector2 pos = position*zoom;
      if (offset) pos += CameraOffsetVect;
      batch.DrawString(font, text, pos, c2, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
      batch.DrawString(font, text, pos + new Vector2(1, -1), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
    }
  }
}