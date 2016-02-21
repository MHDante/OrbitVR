using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using SharpDX;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using Color = SharpDX.Color;
using Point = System.Drawing.Point;

namespace OrbItProcs {
  public enum textures {
    rune1,
    rune2,
    rune3,
    rune4,
    rune5,
    rune6,
    rune7,
    rune8,
    rune9,
    rune10,
    rune11,
    rune12,
    rune13,
    rune14,
    rune15,
    rune16,
    whitecircle,
    orientedcircle,
    blackorb,
    whitesphere,
    ring,
    whiteorb,
    blueorb,
    colororb,
    whitepixel,
    whitepixeltrans,
    sword,
    randompixels,
    innerL,
    innerR,
    outerL,
    outerR,
    pointer,
    itemLight,
    itemWhisper,
    fist,
    cage,
    robot1,
    shoveltip,
    spiderhead,
    spiderleg1,
    rock1,
    boulder1,
    goat,
    gradient1,
    gradient2,
    ridgesR,
    ridgesL,
    boulderShine,
    endLight,
    leaf,
    whiteorb2,
  }

  static class Assets {
    public const string filepath = "Presets//Nodes/";
    public const string levelsFilepath = "Presets//Levels/";

    public static SpriteFont font;
    public static Dictionary<textures, Texture2D> textureDict;
    public static Dictionary<textures, Vector2> textureCenters;
    public static Texture2D[,] btnTextures;
    public static ObservableCollection<object> NodePresets = new ObservableCollection<object>();
    //public static Effect shaderEffect; // Shader code

    public static void LoadAssets(ContentManager content) {
      if (!Directory.Exists(filepath)) Directory.CreateDirectory(filepath);
      textureDict = new Dictionary<textures, Texture2D>() {
        {textures.blueorb, content.Load<Texture2D>("Textures/bluesphere")},
        {textures.whiteorb, content.Load<Texture2D>("Textures/whiteorb")},
        {textures.colororb, content.Load<Texture2D>("Textures/colororb")},
        {textures.whitepixel, content.Load<Texture2D>("Textures/whitepixel")},
        {textures.whitepixeltrans, content.Load<Texture2D>("Textures/whitepixeltrans")},
        {textures.whitecircle, content.Load<Texture2D>("Textures/whitecircle")},
        {textures.whitesphere, content.Load<Texture2D>("Textures/whitesphere")},
        {textures.blackorb, content.Load<Texture2D>("Textures/blackorb")},
        {textures.ring, content.Load<Texture2D>("Textures/ring")},
        {textures.orientedcircle, content.Load<Texture2D>("Textures/orientedcircle")},
        {textures.sword, content.Load<Texture2D>("Textures/sword")},
        {textures.randompixels, content.Load<Texture2D>("Textures/randompixels")},
        {textures.innerL, content.Load<Texture2D>("Textures/innerL")},
        {textures.innerR, content.Load<Texture2D>("Textures/innerR")},
        {textures.outerL, content.Load<Texture2D>("Textures/outerL")},
        {textures.outerR, content.Load<Texture2D>("Textures/outerR")},
        {textures.pointer, content.Load<Texture2D>("Textures/pointer")},
        {textures.itemLight, content.Load<Texture2D>("Textures/itemLight")},
        {textures.itemWhisper, content.Load<Texture2D>("Textures/itemWhisper")},
        {textures.cage, content.Load<Texture2D>("Textures/cage")},
        {textures.fist, content.Load<Texture2D>("Textures/fist")},
        {textures.goat, content.Load<Texture2D>("Textures/Boulder_3")},
        {textures.robot1, content.Load<Texture2D>("Textures/Robot1")},
        {textures.shoveltip, content.Load<Texture2D>("Textures/ShovelTip")},
        {textures.spiderhead, content.Load<Texture2D>("Textures/SpiderHead")},
        {textures.spiderleg1, content.Load<Texture2D>("Textures/SpiderLeg1")},
        {textures.rock1, content.Load<Texture2D>("Textures/RockTexture1")},
        {textures.boulder1, content.Load<Texture2D>("Textures/Bolders")},
        {textures.gradient1, content.Load<Texture2D>("Textures/gradient")},
        {textures.gradient2, content.Load<Texture2D>("Textures/gradient2")},
        {textures.ridgesL, content.Load<Texture2D>("Textures/RidgesL")},
        {textures.ridgesR, content.Load<Texture2D>("Textures/RidgesR")},
        {textures.boulderShine, content.Load<Texture2D>("Textures/boulderShine")},
        {textures.endLight, content.Load<Texture2D>("Textures/endLight")},
        {textures.leaf, content.Load<Texture2D>("Textures/leaf")},
        {textures.whiteorb2, content.Load<Texture2D>("Textures/whiteorb2")},
      };


      for (int i = 0; i < 16; i++) {
        textures rune = (textures) i;
        string s = "Textures/Runes/" + (i + 1) + " symboli";
        textureDict.Add(rune, content.Load<Texture2D>(s));
      }

      textureCenters = new Dictionary<textures, Vector2>();
      foreach (var tex in textureDict.Keys) {
        Texture2D t = textureDict[tex];
        textureCenters[tex] = new Vector2(t.Width/2f, t.Height/2f);
      }

      font = content.Load<SpriteFont>("Courier New");
      // shaderEffect = content.Load<Effect>("Effects/Shader");
      //TODO: btnTextures = content.Load<Texture2D>("Textures/buttons").sliceSpriteSheet(2, 5);
    }


    public static void LoadNodes() {
      throw new NotImplementedException("We need to Rework Serialization.");
      foreach (string file in Directory.GetFiles(filepath, "*.xml")) {
        try {
          Node presetnode = (Node) OrbIt.game.serializer.Deserialize(file);
          NodePresets.Add(presetnode);
        }
        catch (Exception e) {
          Console.WriteLine("Failed to deserialize node: {0}", e.Message);
        }
      }
    }

    public static void saveNode(Node node, string name, bool overWrite = false) {
      throw new NotImplementedException("We need to Rework Serialization.");
      if (name.Equals("") || node == null) return;
      name = name.Trim();
      string filename = "Presets//Nodes//" + name + ".xml";
      Action completeSave = delegate {
        //OrbIt.ui.sidebar.inspectorArea.editNode.name = name;
        Node serializenode = new Node(node.room);
        //Node.cloneNode(OrbIt.ui.sidebar.inspectorArea.editNode, serializenode);
        OrbIt.game.serializer.Serialize(serializenode, filename);
        Assets.NodePresets.Add(serializenode);
      };

      if (File.Exists(filename)) {
        //we must be overwriting, therefore don't update the live presetList
        //PopUp.Prompt("OverWrite?", "O/W?", delegate(bool c, object a) { if (c) { completeSave(); PopUp.Toast("Node was overridden"); } return true; });
      }
      else {
        //PopUp.Toast("Node Saved"); completeSave();
      }
    }

    internal static void deletePreset(Node p) {
      throw new NotImplementedException("We need to Rework Serialization.");
      Console.WriteLine("Deleting file: " + p);
      File.Delete(Assets.filepath + p.name + ".xml");
      Assets.NodePresets.Remove(p);
    }


    public static Texture2D ClippedBitmap(Texture2D t2d, Point[] pointsArray, out Point position) {
      MemoryStream mStream = new MemoryStream();
      t2d.Save(mStream, ImageFileType.Png);
      Bitmap texture = new Bitmap(mStream);
      int minX = pointsArray.Min(x => x.X); //margin.X >= 0 ? x.X : x.X + margin.X);
      int maxX = pointsArray.Max(x => x.X); //margin.X <= 0 ? x.X : x.X + margin.X);
      int minY = pointsArray.Min(x => x.Y); //margin.Y >= 0 ? x.Y : x.Y + margin.Y);
      int maxY = pointsArray.Max(x => x.Y); //margin.Y <= 0 ? x.X : x.X + margin.X);
      position = new Point(minX, minY);
      if (maxX - minX <= 0 || maxY - minY <= 0) return Assets.textureDict[textures.whitepixel];
      Bitmap bmp = new Bitmap(maxX - minX, maxY - minY);
      Point[] offset = new Point[pointsArray.Length];
      pointsArray.CopyTo(offset, 0);
      offset = Array.ConvertAll(offset, x => x = new Point(x.X - minX, x.Y - minY));
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
        OrbIt.game.GraphicsDevice,
        bmp.Width,
        bmp.Height, PixelFormat.R32G32B32A32.SInt);

      myTex.SetData<Color>(pixels);
      return myTex;
    }
  }
}