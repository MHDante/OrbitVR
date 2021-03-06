﻿using System.Collections.Generic;
using OrbitVR.Components.Meta;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Tracers {
  /// <summary>
  /// Draws one or two waves behind the node in a trail. The wave is customizable in terms of amplitude, period, wave size, and so on.
  /// </summary>
  [Info(UserLevel.User,
    "Draws one or two waves behind the node in a trail. The wave is customizable in terms of amplitude, period, wave size, and so on.",
    CompType)]
  public class Waver : Component {
    public const mtypes CompType = mtypes.draw | mtypes.tracer;

    [Info(UserLevel.Developer)] public Queue<Vector2R> metapositions = new Queue<Vector2R>();

    private Vector2R previousMetaPos = Vector2R.Zero, previousRelectPos = Vector2R.Zero;
    private Queue<Vector2R> reflectpositions = new Queue<Vector2R>();

    private float vshift = 0f;
    private float yval = 0f;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// Sets the length of the wave.
    /// </summary>
    [Info(UserLevel.User, "Sets the length of the wave.")]
    public int Length { get; set; }

    /// <summary>
    /// Sets the scale of the circles left by the wave trail
    /// </summary>
    [Info(UserLevel.User, "Sets the scale of the circles left by the wave trail")]
    public float waveScale { get; set; }

    /// <summary>
    /// How wide the wave will be at its maximum point.
    /// </summary>
    [Info(UserLevel.User, "How wide the wave will be at its maximum point.")]
    public float amp { get; set; }

    /// <summary>
    /// The frequency of the hills and valleys of the wave.
    /// </summary>
    [Info(UserLevel.User, "The frequency of the hills and valleys of the wave.")]
    public float period { get; set; }

    /// <summary>
    /// If set, two waves will appear instead of one, each a reflection of the other across the axis that is the node's path.
    /// </summary>
    [Info(UserLevel.User,
      "If set, two waves will appear instead of one, each a reflection of the other across the axis that is the node's path."
      )]
    public bool reflective { get; set; }

    /// <summary>
    /// The composite level of the sine wave. Google "Composite Sine Wave"  to understand this.
    /// </summary>
    [Info(UserLevel.Advanced, "The composite level of the sine wave. Google 'Composite Sine Wave'  to understand this.")
    ]
    public int composite { get; set; }

    public bool drawLines { get; set; }
    public bool drawSpin { get; set; }
    public Waver() : this(null) {}

    public Waver(Node parent = null) {
      if (parent != null) {
        this.parent = parent;
      }
      amp = 20;
      period = 1000;
      composite = 1;
      waveScale = 0.3f;
      Length = 50;
      reflective = true;
      drawLines = true;
      drawSpin = false;
    }

    public override void AfterCloning() {
      parent.addComponent(typeof (Lifetime), true);
    }

    public override void InitializeLists() {
      metapositions = new Queue<Vector2R>();
      reflectpositions = new Queue<Vector2R>();
    }

    public override void Draw() {
      float time = 0;
      if (parent.HasComp<Lifetime>()) {
        time = parent.Comp<Lifetime>().lifetime;
      }
      else return;

      yval = DelegateManager.SineComposite(time, amp, period, vshift, composite);

      Vector2R metapos = new Vector2R(parent.body.velocity.Y, -parent.body.velocity.X);
      VMath.NormalizeSafe(ref metapos);
      metapos *= yval;
      Vector2R metaposfinal = parent.body.pos + metapos;
      Vector2R reflectfinal = parent.body.pos - metapos;

      if (drawLines) {
        if (previousMetaPos != Vector2R.Zero) {
          room.Camera.DrawLinePermanent(previousMetaPos, metaposfinal, 2f, parent.body.color, Length);
        }
        previousMetaPos = metaposfinal;
        //previousRelectPos = metaposfinal;
        if (reflective) {
          if (previousRelectPos != Vector2R.Zero) {
            room.Camera.DrawLinePermanent(previousRelectPos, reflectfinal, 2f, parent.body.color, Length);

            if (drawSpin) {
              room.Camera.DrawLinePermanent(previousMetaPos, reflectfinal, 2f, parent.body.color, Length);
              room.Camera.DrawLinePermanent(previousRelectPos, metaposfinal, 2f, parent.body.color, Length);
            }
          }
          previousRelectPos = reflectfinal;
          //previousMetaPos = reflectfinal; //whoa make this a flag
        }
      }
      else {
        room.Camera.AddPermanentDraw(parent.texture, metaposfinal, parent.body.color, parent.body.scale*waveScale, 0,
                                     Length);
        if (reflective) {
          room.Camera.AddPermanentDraw(parent.texture, reflectfinal, parent.body.color, parent.body.scale*waveScale, 0,
                                       Length);
        }
      }
    }
  }
}