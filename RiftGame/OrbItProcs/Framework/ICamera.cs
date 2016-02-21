using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace OrbItProcs
{
  public interface ICamera
  {
    bool TakeScreenshot { get; set; }
    Vector2 virtualTopLeft { get; }
    float zoom { get; set; }
    Vector2 CameraOffsetVect { get; set; }
    Vector2 pos { get; set; }

    void AddPermanentDraw(textures texture, Vector2 position, Color color, Vector2 scalevect, float rotation, int life);
    void AddPermanentDraw(textures texture, Vector2 position, Color color, float scale, float rotation, int life);
    void Draw(textures texture, Vector2 position, Color color, Vector2 scalevect, float rotation, Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));
    void Draw(textures texture, Vector2 position, Color color, float scale, Layers Layer, ShaderPack? shaderPack = default(ShaderPack?), bool center = true);
    void Draw(textures texture, Vector2 position, Color color, float scale, float rotation, Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));
    void Draw(Texture2D texture, Vector2 position, Color color, float scale, Layers Layer, ShaderPack? shaderPack = default(ShaderPack?), bool center = true);
    void Draw(Texture2D texture, Vector2 position, Color color, float scale, float rotation, Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));
    void Draw(textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scalevect, Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));
    void Draw(textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, Layers Layer, ShaderPack? shaderPack = default(ShaderPack?));
    void drawGrid(List<Rectangle> linesToDraw, Color color);
    void DrawLine(Vector2 start, Vector2 end, float thickness, Color color, Layers Layer);
    void DrawLinePermanent(Vector2 start, Vector2 end, float thickness, Color color, int life);
    void DrawStringScreen(string text, Vector2 position, Color color, Color? color2 = default(Color?), float scale = 0.5F, bool offset = true, Layers Layer = Layers.Over5);
    void DrawStringWorld(string text, Vector2 position, Color color, Color? color2 = default(Color?), float scale = 0.5F, bool offset = true, Layers Layer = Layers.Over5);
    void removePermanentDraw(textures texture, Vector2 position, Color color, float scale);
    void Screenshot();
  }
}