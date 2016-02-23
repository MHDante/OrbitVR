using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Drawers {
  /// <summary>
  /// Draws a rune image after the node's basic draw.
  /// </summary>
  [Info(UserLevel.User, "Draws a rune image after the node's basic draw.", CompType)]
  public class Rune : Component {
    public const mtypes CompType = mtypes.draw;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// The rune texture to draw.
    /// </summary>
    [Info(UserLevel.User, "The rune texture to draw.")]
    public Textures runeTexture { get; set; }

    /// <summary>
    /// Toggles whether runes are randomly generated upon spawning.
    /// </summary>
    [Info(UserLevel.User, "Toggles whether runes are randomly generated upon spawning.")]
    public bool randomRune { get; set; }

    public Rune() : this(null) {}

    public Rune(Node parent) {
      this.parent = parent;
      randomRune = false;
      runeTexture = Textures.Rune1;
    }

    public override void OnSpawn() {
      if (!randomRune) return;
      int r = Utils.random.Next(16);
      runeTexture = (Textures) r;
    }

    public override void Draw() {
      Color col = parent.body.color.ContrastColor();
      room.Camera.Draw(runeTexture, parent.body.pos, col, parent.body.scale, parent.body.orient, Layers.Over1);
    }
  }
}