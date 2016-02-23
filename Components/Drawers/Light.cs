using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Drawers {
  /// <summary>
  /// Shows a transparent light that can shift in size.
  /// </summary>
  [Info(UserLevel.User, "Shows a transparent light that can shift in size.")]
  public class Light : Component {
    public const mtypes CompType = mtypes.draw;
    private Color _color;
    private float scale;
    private float scaleRateTemp;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// If enabled, the color will be random, otherwise it will pull from the nodes color.
    /// </summary>
    [Info(UserLevel.User, "If enabled, the color will be random, otherwise it will pull from the nodes color.")]
    public bool randomColor { get; set; }

    /// <summary>
    /// The range that the scale will ossilate
    /// </summary>
    [Info(UserLevel.User, "The range that the scale will ossilate")]
    public float scaleRange { get; set; }

    /// <summary>
    /// The middle that the scale will phase around.
    /// </summary>
    [Info(UserLevel.User, "The middle that the scale will phase around.")]
    public float scaleMiddle { get; set; }

    /// <summary>
    /// The rate that the scale will phase.
    /// </summary>
    [Info(UserLevel.User, "The rate that the scale will phase.")]
    public float scaleRate { get; set; }

    /// <summary>
    /// The percent of transparency that the light will be. Lower is more transparent. (0 to 100)
    /// </summary>
    [Info(UserLevel.User, "The percent of transparency that the light will be. Lower is more transparent. (0 to 100)")]
    public float transparencyPercent { get; set; }

    private Color color {
      get { return randomColor ? _color : parent.body.color; }
      set { }
    }

    /// <summary>
    /// If enabled, the given amount of 'shadows' of the same color will be overlaid on the light with reducing scale.
    /// </summary>
    [Info(UserLevel.User,
      "If enabled, the given amount of 'shadows' of the same color will be overlaid on the light with reducing scale.")]
    public Toggle<int> shadowCount { get; set; }

    public Layers drawLayer { get; set; }
    public Light() : this(null) {}

    public Light(Node parent = null) {
      if (parent != null) this.parent = parent;
      scaleRange = 0.1f;
      scaleMiddle = 1f;
      scaleRate = 0.01f;
      transparencyPercent = 25f;
      scaleRateTemp = scaleRate;
      randomColor = true;
      shadowCount = new Toggle<int>(1, false);
      drawLayer = Layers.Under5;
    }

    public override void OnSpawn() {
      _color = Utils.randomColor();
    }

    public override void Draw() {
      scale += scaleRateTemp;
      float max = scaleMiddle + scaleRange/2f;
      float min = scaleMiddle - scaleRange/2f;
      if (scale > max) {
        scaleRateTemp = -scaleRate;
        scale = max;
      }
      else if (scale < min) {
        scaleRateTemp = scaleRate;
        scale = min;
      }
      room.Camera.Draw(Textures.Whitecircle, parent.body.pos, color*(transparencyPercent/100f), scale, drawLayer);
      if (shadowCount.enabled) {
        float totalScaleDifference = 0.5f;
        float singleScaleDifference = totalScaleDifference/shadowCount.value;
        for (int i = 0; i < shadowCount.value; i++) {
          room.Camera.Draw(Textures.Whitecircle, parent.body.pos, color*(transparencyPercent/100f),
                           scale*(1f - (singleScaleDifference*(i + 1))), drawLayer);
        }
      }
    }
  }

  //public class LightData
  //{
  //    
  //}
}