using OrbitVR.Components.Linkers;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.AffectOthers {
  /// <summary>
  /// When another node enters the radius of this node, it gets teleported to the opposite side of this node, relative to this node's origin. 
  /// </summary>
  [Info(UserLevel.User,
    "When another node enters the radius of this node, it gets teleported to the opposite side of this node, relative to this node's origin. ",
    CompType)]
  public class Transfer : Component, ILinkable, IMultipliable //, IRadius
  {
    public const mtypes CompType = mtypes.affectother;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// Distance at which the node is teleported, based on a scale of the node's radius.
    /// </summary>
    [Info(UserLevel.User, "Distance at which the node is teleported, based on a scale of the node's radius.")]
    public float radiusScale { get; set; }

    public Transfer() : this(null) {}

    public Transfer(Node parent = null) {
      if (parent != null) this.parent = parent;
      radiusScale = 2f;
    }

    public Link link { get; set; }

    public override void AffectOther(Node other) {
      if (!active) return;
      if (exclusions.Contains(other)) return;

      float distVects = Vector2R.DistanceSquared(other.body.pos, parent.body.pos);
      float r = parent.body.radius*radiusScale;
      if (distVects < r*r) {
        float newX = (parent.body.pos.X - other.body.pos.X)*2.05f;
        float newY = (parent.body.pos.Y - other.body.pos.Y)*2.05f;
        other.body.pos.X += newX;
        other.body.pos.Y += newY;
      }
    }

    public override void Draw() {}

    public float multiplier {
      get { return radiusScale*10f; }
      set { radiusScale = value/10f; }
    }

    public override void AffectSelf() {}
  }
}