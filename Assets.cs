using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
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
    
    public static ObservableCollection<object> NodePresets = new ObservableCollection<object>();

    public static void LoadAssets() {
      if (!Directory.Exists(Filepath)) Directory.CreateDirectory(Filepath);
      //Font = content.Load<SpriteFont>("Courier New");

    }

    public static Texture2D ClippedBitmap(Textures t2D, Point[] pointsArray, out Point position) {
      throw new NotImplementedException();
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