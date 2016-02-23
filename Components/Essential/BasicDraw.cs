using OrbitVR.Components.Drawers;
using OrbitVR.Framework;
using OrbitVR.Physics;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Essential {
  /// <summary>
  /// Basic Draw Component, ensures that you can see the node.
  /// </summary>
  [Info(UserLevel.User, "Basic Draw Component, ensures that you can see the node.", CompType)]
  public class BasicDraw : Component {
    public enum Initial {
      Deviant,
      Random,
      Managed,
      CloseToPermanent,
    }

    public const mtypes CompType = mtypes.essential | mtypes.draw;

    private Initial _InitialColor = Initial.Managed;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// Determines whether the color will be random or set by the Red, Green and Blue properties initially.
    /// </summary>
    [Info(UserLevel.User,
      "Determines whether the color will be random or set by the Red, Green and Blue properties initially.")]
    public Initial InitialColor {
      get { return _InitialColor; }
      set {
        _InitialColor = value;
        Colorize();
      }
    }

    /// <summary>
    /// Red color component
    /// </summary>
    [Info(UserLevel.User, "Red color component")]
    public int Red { get; set; }

    /// <summary>
    /// Green color component
    /// </summary>
    [Info(UserLevel.User, "Green color component")]
    public int Green { get; set; }

    /// <summary>
    /// Blue color component
    /// </summary>
    [Info(UserLevel.User, "Blue color component")]
    public int Blue { get; set; }

    /// <summary>
    /// Alpha color component
    /// </summary>
    [Info(UserLevel.User, "Alpha color component")]
    public float AlphaPercent { get; set; }

    /// <summary>
    /// The layer that the node will draw on.
    /// </summary>
    [Info(UserLevel.User, "The layer that the node will draw on.")]
    public Layers DrawLayer { get; set; }

    public int threshold { get; set; }

    public bool DrawSparkles { get; set; }
    public BasicDraw() : this(null) {}

    public BasicDraw(Node parent = null) {
      if (parent != null) this.parent = parent;
      UpdateColor();
      AlphaPercent = 100f;
      DrawLayer = Layers.Under1;
      DrawSparkles = true;
      threshold = 20;
    }

    public void UpdateColor() {
      if (parent == null) return;
      Color c = parent.body.color;
      Red = c.R;
      Green = c.G;
      Blue = c.B;
    }

    public override void OnSpawn() {
      Colorize();
    }

    public void Colorize() {
      if (InitialColor == Initial.Random) {
        RandomizeColor();
      }
      else if (InitialColor == Initial.Managed) {
        SetColor();
      }
      else if (InitialColor == Initial.Deviant) {
        Deviate();
      }
      else if (InitialColor == Initial.CloseToPermanent) {
        CloseToPermanent();
      }
    }

    public void SetColor() {
      if (parent != null) {
        parent.body.color = new Color(Red, Green, Blue);
        parent.body.permaColor = parent.body.color;
      }
    }

    [Clickable]
    public void RandomizeColor() {
      if (parent != null) {
        parent.body.color = Utils.randomColor();
        parent.body.permaColor = parent.body.color;
      }
    }

    public void Deviate() {
      if (parent != null) {
        if (OrbIt.GlobalGameMode != null) {
          parent.body.color = Color.Lerp(OrbIt.GlobalGameMode.globalColor, Utils.randomColor(), 0.1f);
          parent.body.permaColor = parent.body.color;
          OrbIt.GlobalGameMode.globalColor = parent.body.color;
        }
      }
      else SetColor();
    }

    public void CloseToPermanent() {
      if (parent != null) {
        SetColor();
        Color c = parent.body.permaColor;
        int r = Utils.random.Next(threshold) - (threshold/2);
        int g = Utils.random.Next(threshold) - (threshold/2);
        int b = Utils.random.Next(threshold) - (threshold/2);
        parent.body.color = new Color(c.R + r, c.G + g, c.B + b);
        parent.body.permaColor = parent.body.color;
        UpdateColor();
      }
    }

    public override void Draw() {
      //it would be really cool to have some kind of blending effects so that every combination of components will look diff

      if (parent.body.shape is Polygon) {
        parent.body.shape.Draw();
        if (!parent.body.DrawPolygonCenter) return;
      }

      Layers layer = parent.IsPlayer ? Layers.Player : DrawLayer;

      if (parent.HasComp<Shader>())
        room.Camera.Draw(parent.body.texture, parent.body.pos, parent.body.color*(AlphaPercent/100f), parent.body.scale,
                         parent.body.orient, layer, parent.Comp<Shader>().shaderPack);
      else
        room.Camera.Draw(parent.body.texture, parent.body.pos, parent.body.color*(AlphaPercent/100f), parent.body.scale,
                         parent.body.orient, layer);

      if (parent.body.texture == Textures.Boulder1 && DrawSparkles)
        room.Camera.Draw(Textures.BoulderShine, parent.body.pos, Utils.randomColor(), parent.body.scale,
                         parent.body.orient, layer);
    }
  }
}