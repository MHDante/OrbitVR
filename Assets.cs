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
    Whitecircle,
    Orientedcircle,
    Blackorb,
    Whitesphere,
    Ring,
    Whiteorb,
    Blueorb,
    Colororb,
    Whitepixel,
    Whitepixeltrans,
    Sword,
    Randompixels,
    InnerL,
    InnerR,
    OuterL,
    OuterR,
    Pointer,
    ItemLight,
    ItemWhisper,
    Fist,
    Cage,
    Robot1,
    Shoveltip,
    Spiderhead,
    Spiderleg1,
    Rock1,
    Boulder1,
    Goat,
    Gradient1,
    Gradient2,
    RidgesR,
    RidgesL,
    BoulderShine,
    EndLight,
    Leaf,
    Whiteorb2,
  }

  static class Assets {
    public const string Filepath = "Presets//Nodes/";
    public const string LevelsFilepath = "Presets//Levels/";

    public static SpriteFont Font;
    public static Dictionary<Textures, Texture2D> TextureDict;
    public static Dictionary<Textures, Vector2> TextureCenters;
    public static ObservableCollection<object> NodePresets = new ObservableCollection<object>();
    //public static Effect shaderEffect; // Shader code

    public static void LoadAssets(ContentManager content) {
      if (!Directory.Exists(Filepath)) Directory.CreateDirectory(Filepath);
      TextureDict = new Dictionary<Textures, Texture2D>() {
        {Textures.Blueorb, /**/content.Load<Texture2D>("Textures/bluesphere")},
        {Textures.Whiteorb, /**/content.Load<Texture2D>("Textures/whiteorb")},
        {Textures.Colororb, /**/content.Load<Texture2D>("Textures/colororb")},
        {Textures.Whitepixel, /**/content.Load<Texture2D>("Textures/whitepixel")},
        {Textures.Whitepixeltrans, /**/content.Load<Texture2D>("Textures/whitepixeltrans")},
        {Textures.Whitecircle, /**/content.Load<Texture2D>("Textures/whitecircle")},
        {Textures.Whitesphere, /**/content.Load<Texture2D>("Textures/whitesphere")},
        {Textures.Blackorb, /**/content.Load<Texture2D>("Textures/blackorb")},
        {Textures.Ring, /**/content.Load<Texture2D>("Textures/ring")},
        {Textures.Orientedcircle, /**/content.Load<Texture2D>("Textures/orientedcircle")},
        {Textures.Sword, /**/content.Load<Texture2D>("Textures/sword")},
        {Textures.Randompixels, /**/content.Load<Texture2D>("Textures/randompixels")},
        {Textures.InnerL, /**/content.Load<Texture2D>("Textures/innerL")},
        {Textures.InnerR, /**/content.Load<Texture2D>("Textures/innerR")},
        {Textures.OuterL, /**/content.Load<Texture2D>("Textures/outerL")},
        {Textures.OuterR, /**/content.Load<Texture2D>("Textures/outerR")},
        {Textures.Pointer, /**/content.Load<Texture2D>("Textures/pointer")},
        {Textures.ItemLight, /**/content.Load<Texture2D>("Textures/itemLight")},
        {Textures.ItemWhisper, /**/content.Load<Texture2D>("Textures/itemWhisper")},
        {Textures.Cage, /**/content.Load<Texture2D>("Textures/cage")},
        {Textures.Fist, /**/content.Load<Texture2D>("Textures/fist")},
        {Textures.Goat, /**/content.Load<Texture2D>("Textures/Boulder_3")},
        {Textures.Robot1, /**/content.Load<Texture2D>("Textures/Robot1")},
        {Textures.Shoveltip, /**/content.Load<Texture2D>("Textures/ShovelTip")},
        {Textures.Spiderhead, /**/content.Load<Texture2D>("Textures/SpiderHead")},
        {Textures.Spiderleg1, /**/content.Load<Texture2D>("Textures/SpiderLeg1")},
        {Textures.Rock1, /**/content.Load<Texture2D>("Textures/RockTexture1")},
        {Textures.Boulder1, /**/content.Load<Texture2D>("Textures/Bolders")},
        {Textures.Gradient1, /**/content.Load<Texture2D>("Textures/gradient")},
        {Textures.Gradient2, /**/content.Load<Texture2D>("Textures/gradient2")},
        {Textures.RidgesL, /**/content.Load<Texture2D>("Textures/RidgesL")},
        {Textures.RidgesR, /**/content.Load<Texture2D>("Textures/RidgesR")},
        {Textures.BoulderShine, /**/content.Load<Texture2D>("Textures/boulderShine")},
        {Textures.EndLight, /**/content.Load<Texture2D>("Textures/endLight")},
        {Textures.Leaf, /**/content.Load<Texture2D>("Textures/leaf")},
        {Textures.Whiteorb2, /**/content.Load<Texture2D>("Textures/whiteorb2")},
      };


      for (int i = 0; i < 16; i++) {
        Textures rune = (Textures) i;
        string s = "Textures/Runes/" + (i + 1) + " symboli";
        TextureDict.Add(rune, content.Load<Texture2D>(s));
      }

      TextureCenters = new Dictionary<Textures, Vector2>();
      foreach (var tex in TextureDict.Keys) {
        Texture2D t = TextureDict[tex];
        TextureCenters[tex] = new Vector2(t.Width/2f, t.Height/2f);
      }

      Font = content.Load<SpriteFont>("Courier New");
    }

    public static Texture2D ClippedBitmap(Texture2D t2D, Point[] pointsArray, out Point position) {
      MemoryStream mStream = new MemoryStream();
      t2D.Save(mStream, ImageFileType.Png);
      Bitmap texture = new Bitmap(mStream);
      int minX = pointsArray.Min(x => x.X); //margin.X >= 0 ? x.X : x.X + margin.X);
      int maxX = pointsArray.Max(x => x.X); //margin.X <= 0 ? x.X : x.X + margin.X);
      int minY = pointsArray.Min(x => x.Y); //margin.Y >= 0 ? x.Y : x.Y + margin.Y);
      int maxY = pointsArray.Max(x => x.Y); //margin.Y <= 0 ? x.X : x.X + margin.X);
      position = new Point(minX, minY);
      if (maxX - minX <= 0 || maxY - minY <= 0) return TextureDict[Textures.Whitepixel];
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
}