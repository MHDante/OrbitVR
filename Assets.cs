using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using Color = SharpDX.Color;
using Point = System.Drawing.Point;

namespace OrbitVR {
  public enum Textures {
    Rune1,
    Rune2,
    Rune3,
    Rune4,
    Rune5,
    Rune6,
    Rune7,
    Rune8,
    Rune9,
    Rune10,
    Rune11,
    Rune12,
    Rune13,
    Rune14,
    Rune15,
    Rune16,
    Boulder1,
    Robot1,
    Shoveltip,
    Blackorb,
    Blueorb,
    Cage,
    Colororb,
    Fist,
    Gradient1,
    Gradient2,
    InnerL,
    InnerR,
    ItemLight,
    ItemWhisper,
    Orientedcircle,
    OuterL,
    OuterR,
    PixelEdge,
    Pointer,
    Randompixels,
    Ring,
    SmoothEdge,
    Sword,
    Whitecircle,
    Whiteorb,
    Whiteorb2,
    Whitesphere
  }

  static class Assets {
    public const string Filepath = "Presets//Nodes/";

    public static SpriteFont Font;
    public static ObservableCollection<object> NodePresets = new ObservableCollection<object>();

    public static void LoadAssets(ContentManager content) {
      if (!Directory.Exists(Filepath)) Directory.CreateDirectory(Filepath);
      Font = content.Load<SpriteFont>("Courier New");

    }

    public static Texture2D ClippedBitmap(Textures t2D, Point[] pointsArray, out Point position) {
      MemoryStream mStream = new MemoryStream();
      t2D.GetTexture2D().Save(mStream, ImageFileType.Png);
      Bitmap texture = new Bitmap(mStream);
      int minX = pointsArray.Min(x => x.X); //margin.X >= 0 ? x.X : x.X + margin.X);
      int maxX = pointsArray.Max(x => x.X); //margin.X <= 0 ? x.X : x.X + margin.X);
      int minY = pointsArray.Min(x => x.Y); //margin.Y >= 0 ? x.Y : x.Y + margin.Y);
      int maxY = pointsArray.Max(x => x.Y); //margin.Y <= 0 ? x.X : x.X + margin.X);
      position = new Point(minX, minY);
      if (maxX - minX <= 0 || maxY - minY <= 0) return null;
      Bitmap bmp = new Bitmap(maxX - minX, maxY - minY);
      Point[] offset = new Point[pointsArray.Length];
      pointsArray.CopyTo(offset, 0);
      offset = Array.ConvertAll(offset, x => new Point(x.X - minX, x.Y - minY));
      Graphics g = Graphics.FromImage(bmp);
      TextureBrush tb = new TextureBrush(texture);
      g.FillPolygon(tb, offset);

      Color[] pixels = new Color[bmp.Width*bmp.Height];
      for (int y = 0; y < bmp.Height; y++) {
        for (int x = 0; x < bmp.Width; x++) {
          System.Drawing.Color c = bmp.GetPixel(x, y);
          pixels[(y*bmp.Width) + x] = new Color(c.R, c.G, c.B, c.A);
        }
      }

      Texture2D myTex = Texture2D.New(
                                      OrbIt.Game.GraphicsDevice,
                                      bmp.Width,
                                      bmp.Height, Format.B8G8R8A8_UNorm);

      myTex.SetData(pixels);
      return myTex;
    }
  }

  public static class TextureExtensions {
    public static float GetWidth(this Textures t)
    {
      return 128;
    }
    public static float GetHeight(this Textures t)
    {
      return 128;
    }
    public static Vector2 GetCenter(this Textures t)
    {
      return new Vector2(64, 64);
    }
    public static Texture2D GetTexture2D(this Textures t) {
      throw new NotImplementedException();
      return null;
    }
  }
}