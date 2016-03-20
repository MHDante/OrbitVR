using System;
using System.Collections.Generic;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Linkers {
  /// <summary>
  /// Linked nodes will orbit around the source node at a cosntant random distance and speed
  /// </summary>
  [Info(UserLevel.User, "Linked nodes will orbit around the source node at a cosntant random distance and speed",
    CompType)]
  public class Orbiter : Component, ILinkable {
    public const mtypes CompType = mtypes.exclusiveLinker;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    public Dictionary<Node, OrbiterData> orbiterDatas { get; set; }

    /// <summary>
    /// Maximum distance for orbiting nodes
    /// </summary>
    [Info(UserLevel.User, "Maximum distance for orbiting nodes")]
    public int maxRadius { get; set; }

    /// <summary>
    /// Maximum speed for orbiting nodes
    /// </summary>
    [Info(UserLevel.User, "Maximum speed for orbiting nodes")]
    public int maxSpeed { get; set; }

    /// <summary>
    /// Multiplies the speed by the given factor
    /// </summary>
    [Info(UserLevel.User, "Multiplies the speed by the given factor")]
    public float speedMult { get; set; }

    public Orbiter() : this(null) {}

    public Orbiter(Node parent = null) {
      orbiterDatas = new Dictionary<Node, OrbiterData>();
      if (parent != null) this.parent = parent;
      maxRadius = 500;
      maxSpeed = 50;
      speedMult = 1f;
    }

    [Info(UserLevel.Developer)]
    public Link link { get; set; }

    public override void AffectOther(Node other) {
      if (!active) {
        return;
      }
      if (exclusions.Contains(other)) return;

      if (link != null && link.formation != null && link.formation.AffectionSets.ContainsKey(parent)) {
        foreach (Node n in link.formation.AffectionSets[parent]) {
          if (orbiterDatas.ContainsKey(n)) {
            OrbiterData od = orbiterDatas[n];
            od.angle += od.angledelta*speedMult;
            if (od.angle > Math.PI)
              od.angle = od.angle - 2*(float) Math.PI;
            else if (od.angle < -Math.PI)
              od.angle = od.angle + 2*(float) Math.PI;

            //Console.WriteLine(od.angle + " : " + od.angle);

            float x = od.radius*(float) Math.Cos(od.angle);
            float y = od.radius*(float) Math.Sin(od.angle);

            //n.transform.position.X = (float)Math.Atan2(parent.transform.position.Y - y, parent.transform.position.X - x);
            n.body.pos = new Vector2R(parent.body.pos.X - x, parent.body.pos.Y - y);

            orbiterDatas[n] = od;
          }
          else {
            orbiterDatas[n] = new OrbiterData(maxSpeed, maxRadius);
          }
        }
      }
    }

    public override void Draw() {}

    public override void AffectSelf() {}

    public struct OrbiterData {
      public float angle;
      public float angledelta;
      public float speed;
      public float radius;

      public OrbiterData(int randSpeed, int randRadius) {
        angle = Utils.random.Next(360)*(float) (Math.PI/180);
        speed = Utils.random.Next(randSpeed)*0.1f;
        radius = Utils.random.Next(randRadius);
        angledelta = speed/radius;
      }
    }
  }
}