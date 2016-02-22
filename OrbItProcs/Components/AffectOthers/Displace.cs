using System;
using SharpDX;

namespace OrbItProcs {
  /// <summary>
  /// When another node enters this radius, it is displaced in the direction of the angle without affecting its velocity.
  /// </summary>
  [Info(UserLevel.User,
    "When another node enters this radius, it is displaced in the direction of the angle without affecting its velocity.",
    CompType)]
  public class Displace : Component, ILinkable, IMultipliable //, IRadius
  {
    public const mtypes CompType = mtypes.affectother;

    public Displace() : this(null) {}

    public Displace(Node parent) {
      if (parent != null) this.parent = parent;
      multiplier = 100f;
      lowerbound = 20;
      radius = 800f;
    }

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// Radius at which other nodes are affected.
    /// </summary>
    [Info(UserLevel.User, "Radius at which other nodes are affected.")]
    public float radius { get; set; }

    /// <summary>
    /// Represents minimum distance taken into account when calculating push away.
    /// </summary>
    [Info(UserLevel.Advanced, "Represents minimum distance taken into account when calculating push away.")]
    public int lowerbound { get; set; }

    /// <summary>
    /// Changes the angle at which the node displaces the incoming node:
    /// 0 pushes away and 180 pulls toward. 90 pushes rightwards and 270 pushes leftwards.
    /// </summary>
    [Info(UserLevel.User,
      "Changes the angle at which the node displaces the incoming node: 0 pushes away and 180 pulls toward. 90 pushes rightwards and 270 pushes leftwards."
      )]
    public int angle { get; set; }

    /// <summary>
    /// If disabled, the intencity of displacement will vary depending on the distance from the node.
    /// </summary>
    [Info(UserLevel.Advanced,
      "If disabled, the intensity of displacement will vary depending on the distance from the node.")]
    public bool ConstantPush { get; set; }

    [Info(UserLevel.Developer)]
    public Link link { get; set; }

    public override void AffectOther(Node other) {
      if (!active) {
        return;
      }
      if (exclusions.Contains(other)) return;

      float distVects = Vector2.Distance(other.body.pos, parent.body.pos);

      if (distVects < radius) {
        if (distVects < lowerbound) distVects = lowerbound;
        double aa = Math.Atan2((parent.body.pos.Y - other.body.pos.Y), (parent.body.pos.X - other.body.pos.X));
        //float counterforce = 100 / distVects;
        //float gravForce = multiplier / (distVects * distVects * counterforce);
        //Console.WriteLine(angle);
        //float gravForce = (multiplier * parent.transform.mass * other.transform.mass) / (distVects * distVects * counterforce);
        float gravForce;
        if (!ConstantPush) gravForce = multiplier/10f; // * 10;
        else gravForce = (multiplier/10f*parent.body.mass*other.body.mass)/(distVects);

        if (angle != 0)
          aa = (aa + Math.PI + (Math.PI*(float) (angle/180.0f))%(Math.PI*2)) - Math.PI;

        //float gravForce = gnode1.GravMultiplier;
        float velX = (float) Math.Cos(aa)*gravForce;
        float velY = (float) Math.Sin(aa)*gravForce;
        Vector2 delta = new Vector2(velX, velY);

        if (!ConstantPush) delta *= other.body.invmass;
        other.body.pos -= delta;
      }
    }

    /// <summary>
    /// The strength with which the other node will be moved away.
    /// </summary>
    [Info(UserLevel.User, "The strength with which the other node will be moved away.")]
    public float multiplier { get; set; }
  }
}