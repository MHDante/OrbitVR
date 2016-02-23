using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace OrbItProcs {
  public abstract class CameraBase {
    protected static System.Drawing.Pen pen;
    public Color backgroundColor = Color.Black;
    private float backgroundHue = 180;
    public SpriteBatch batch;
    public bool phaseBackgroundColor = false;

    public Room room;
    private double x = 0;

    static CameraBase() {
      pen = new System.Drawing.Pen(new System.Drawing.Color());
    }

    protected CameraBase(Room room, float zoom, Vector2? pos) {
      this.room = room;
      this.batch = new SpriteBatch(OrbIt.Game.GraphicsDevice);
      this.zoom = zoom;
      this.pos = pos ??
                 new Vector2(room.gridsystemAffect.position.X + room.gridsystemAffect.gridWidth/2,
                             10 + room.gridsystemAffect.position.Y + room.gridsystemAffect.gridHeight/2);
    }

    public bool TakeScreenshot { get; set; }

    public Vector2 virtualTopLeft {
      get { return pos - new Vector2(room.gridsystemAffect.gridWidth/2, room.gridsystemAffect.gridHeight/2)*1/zoom; }
    }

    public float zoom { get; set; }
    public Vector2 CameraOffsetVect { get; set; }
    public Vector2 pos { get; set; }

    public abstract void AddPermanentDraw(textures texture, Vector2 position, Color color, Vector2 scalevect,
                                          float rotation, int life);

    public abstract void AddPermanentDraw(textures texture, Vector2 position, Color color, float scale, float rotation,
                                          int life);

    public abstract void Draw(textures texture, Vector2 position, Color color, Vector2 scalevect, float rotation,
                              Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));

    public abstract void Draw(textures texture, Vector2 position, Color color, float scale, Layers Layer,
                              ShaderPack? shaderPack = default(ShaderPack?), bool center = true);

    public abstract void Draw(textures texture, Vector2 position, Color color, float scale, float rotation, Layers Layer,
                              ShaderPack? shaderPack = default(ShaderPack?));

    public abstract void Draw(Texture2D texture, Vector2 position, Color color, float scale, Layers Layer,
                              ShaderPack? shaderPack = default(ShaderPack?), bool center = true);

    public abstract void Draw(Texture2D texture, Vector2 position, Color color, float scale, float rotation,
                              Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));

    public abstract void Draw(textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                              Vector2 origin, Vector2 scalevect, Layers Layer,
                              ShaderPack? shaderPack = default(ShaderPack?));

    public abstract void Draw(textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                              Vector2 origin, float scale, Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));

    public abstract void drawGrid(List<Rectangle> linesToDraw, Color color);
    public abstract void DrawLine(Vector2 start, Vector2 end, float thickness, Color color, Layers Layer);
    public abstract void DrawLinePermanent(Vector2 start, Vector2 end, float thickness, Color color, int life);

    public abstract void DrawStringScreen(string text, Vector2 position, Color color, Color? color2 = default(Color?),
                                          float scale = 0.5F, bool offset = true, Layers Layer = Layers.Over5);

    public abstract void DrawStringWorld(string text, Vector2 position, Color color, Color? color2 = default(Color?),
                                         float scale = 0.5F, bool offset = true, Layers Layer = Layers.Over5);

    public abstract void removePermanentDraw(textures texture, Vector2 position, Color color, float scale);
    public abstract void Screenshot();

    public virtual void Update() {
      if (phaseBackgroundColor) {
        x += Math.PI/360.0;
        backgroundHue = (backgroundHue + ((float) Math.Sin(x) + 1)/10f)%360;
        backgroundColor = ColorChanger.getColorFromHSV(backgroundHue, value: 0.2f);
      }
    }
  }
}