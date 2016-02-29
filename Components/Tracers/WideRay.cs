using System;
using System.Collections.Generic;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Tracers {
  /// <summary>
  /// Replaces the basic draw with a set of lines that trail behind the node, perpendicular to its direction. is said to look like a caterpillar.
  /// </summary>
  [Info(UserLevel.User,
    "Replaces the basic draw with a set of lines that trail behind the node, perpendicular to its direction. is said to look like a caterpillar.",
    CompType)]
  public class WideRay : Component {
    public const mtypes CompType = mtypes.draw | mtypes.tracer;

    public int _rayLength = 10;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// Sets the length of the ray.
    /// </summary>
    [Info(UserLevel.User, "Sets the length of the ray. ")]
    public int rayLength {
      get { return _rayLength; }
      set {
        //if (parent != null && parent.HasComp<Queuer>() && parent.Comp<Queuer>().queuecount < value)
        //{
        //    parent.Comp<Queuer>().queuecount = value;
        //}
        _rayLength = value;
      }
    }

    /// <summary>
    /// Sets the thickness of each ray line.
    /// </summary>
    [Info(UserLevel.User, "Sets the thickness of each ray line.")]
    public float rayscale { get; set; }

    /// <summary>
    /// Sets the width (length) of each ray line.
    /// </summary>
    [Info(UserLevel.User, "Sets the width (length) of each ray line.")]
    public int width { get; set; }

    public WideRay() : this(null) {}

    public WideRay(Node parent = null) {
      if (parent != null) this.parent = parent;
      rayscale = 20;
      width = 3;
    }

    //public override void AfterCloning()
    //{
    //    if (!parent.HasComp<Queuer>()) parent.addComponent(comp.queuer, true);
    //    parent.Comp<Queuer>().qs = parent.Comp<Queuer>().qs | queues.scale | queues.position | queues.angle;
    //}

    public override void Draw() {
      Vector2 pos = parent.body.pos;
      Vector2 scalevect = new Vector2(rayscale, width);
      float angle = (float) (Math.Atan2(parent.body.velocity.Y, parent.body.velocity.X) + Math.PI/2);
      //room.Camera.AddPermanentDraw(Textures.Whitepixel, pos, parent.body.color, scalevect, angle, rayLength);
      //Todo:drawlines
    }

    public void onCollision(Dictionary<dynamic, dynamic> args) {}
  }
}