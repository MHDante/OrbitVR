using System;
using OrbitVR.Components.Drawers;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace OrbitVR.Framework {
  public abstract class CameraBase {
    protected static System.Drawing.Pen pen;
    public Color backgroundColor = Color.Black;
    private float backgroundHue = 180;
    public SpriteBatch batch;
    public bool phaseBackgroundColor = false;

    public Room room;
    private double x = 0;

    public bool TakeScreenshot { get; set; }

    public Vector2 virtualTopLeft {
      get { return pos - new Vector2(room.GridsystemAffect.gridWidth/2, room.GridsystemAffect.gridHeight/2)*1/zoom; }
    }

    public float zoom { get; set; }
    public Vector2 CameraOffsetVect { get; set; }
    public Vector2 pos { get; set; }

    static CameraBase() {
      pen = new System.Drawing.Pen(new System.Drawing.Color());
    }

    protected CameraBase(Room room, float zoom, Vector2? pos) {
      this.room = room;
      this.batch = new SpriteBatch(OrbIt.Game.GraphicsDevice);
      this.zoom = zoom;
      this.pos = pos ??
                 new Vector2(room.GridsystemAffect.position.X + room.GridsystemAffect.gridWidth/2,
                             10 + room.GridsystemAffect.position.Y + room.GridsystemAffect.gridHeight/2);
    }

    public abstract void AddPermanentDraw(Textures texture, Vector2 position, Color color, Vector2 scalevect,
                                          float rotation, int life);

    public abstract void AddPermanentDraw(Textures texture, Vector2 position, Color color, float scale, float rotation,
                                          int life);

    public abstract void Draw(Textures texture, Vector2 position, Color color, Vector2 scalevect, float rotation,
                              Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));

    public abstract void Draw(Textures texture, Vector2 position, Color color, float scale, Layers Layer,
                              ShaderPack? shaderPack = default(ShaderPack?), bool center = true);

    public abstract void Draw(Textures texture, Vector2 position, Color color, float scale, float rotation, Layers Layer,
                              ShaderPack? shaderPack = default(ShaderPack?));

    public abstract void Draw(Texture2D texture, Vector2 position, Color color, float scale, Layers Layer,
                              ShaderPack? shaderPack = default(ShaderPack?), bool center = true);

    public abstract void Draw(Texture2D texture, Vector2 position, Color color, float scale, float rotation,
                              Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));

    public abstract void Draw(Textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                              Vector2 origin, Vector2 scalevect, Layers Layer,
                              ShaderPack? shaderPack = default(ShaderPack?));

    public abstract void Draw(Textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                              Vector2 origin, float scale, Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));


    public abstract void DrawLine(Vector2 start, Vector2 end, float thickness, Color color, Layers Layer);
    public abstract void DrawLinePermanent(Vector2 start, Vector2 end, float thickness, Color color, int life);

    public abstract void DrawStringScreen(string text, Vector2 position, Color color, Color? color2 = default(Color?),
                                          float scale = 0.5F, bool offset = true, Layers Layer = Layers.Over5);

    public abstract void DrawStringWorld(string text, Vector2 position, Color color, Color? color2 = default(Color?),
                                         float scale = 0.5F, bool offset = true, Layers Layer = Layers.Over5);

    public abstract void removePermanentDraw(Textures texture, Vector2 position, Color color, float scale);
    public abstract void Screenshot();

    public virtual void Update() {
      if (phaseBackgroundColor) {
        x += Math.PI/360.0;
        backgroundHue = (backgroundHue + ((float) Math.Sin(x) + 1)/10f)%360;
        backgroundColor = ColorChanger.getColorFromHSV(backgroundHue, value: 0.2f);
      }
    }

    public void DrawRect(Vector2 min, Vector2 max, Color borderColor) {
      DrawLine(min, new Vector2(max.X, min.Y), 2, borderColor, Layers.Under5);
      DrawLine(min, new Vector2(min.X, max.Y), 2, borderColor, Layers.Under5);
      DrawLine(new Vector2(min.X, max.Y), max, 2, borderColor, Layers.Under5);
      DrawLine(new Vector2(max.X, min.Y), max, 2, borderColor, Layers.Under5);
    }
  }

  public struct Line {
    public Vector2 Start;
    public Vector2 End;

    public Line(int x1, int y1, int x2, int y2) : this() {
      Start = new Vector2(x1, y1);
      End = new Vector2(x2, y2);
    }
  }
}