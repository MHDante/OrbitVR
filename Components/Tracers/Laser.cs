﻿using System;
using OrbitVR.Components.Drawers;
using OrbitVR.Components.Essential;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Tracers {
  /// <summary>
  /// Replaces the Basic Circle drawing with a thin and long laser that trails behind the node's current position.
  /// </summary>
  [Info(UserLevel.User,
    "Replaces the Basic Circle drawing with a thin and long laser that trails behind the node's current position.",
    CompType)]
  public class Laser : Component {
    public const mtypes CompType = mtypes.draw | mtypes.tracer;

    private int _laserLength = 10;
    private int counter = 0;
    private Vector2R prevPos = Vector2R.Zero;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// Sets the length of the laser.
    /// </summary>
    [Info(UserLevel.User, "Sets the length of the laser. ")]
    public int laserLength {
      get { return _laserLength; }
      set { _laserLength = value; }
    }

    /// <summary>
    /// Increasing this number increases performance, but causes flashing.
    /// </summary>
    [Info(UserLevel.Advanced, "Increasing this number increases performance, but causes flashing.")]
    public int onceEveryAmount { get; set; }

    /// <summary>
    /// Cool effect where the laser changes color depending on which way it's travelling. Consider using ColorChanger component and setting this to false.
    /// </summary>
    [Info(UserLevel.User,
      "Cool effect where the laser changes color depending on which way it's travelling. Consider using ColorChanger component and setting this to false."
      )]
    public bool IsColorByAngle { get; set; }

    /// <summary>
    /// If enabled, changes the brightness in a trippy way. Try setting it to decimal values for extra trippyness.
    /// </summary>
    [Info(UserLevel.Advanced,
      "If enabled, changes the brightness in a trippy way. Try setting it to decimal values for extra trippyness.")]
    public Toggle<float> brightness { get; set; }

    /// <summary>
    /// Sets the width of the line.
    /// </summary>
    [Info(UserLevel.User, "Sets the width of the line.")]
    public float thickness { get; set; }

    /// <summary>
    /// The ratio of the white core to the coloured outer parts of the laser.
    /// </summary>
    [Info(UserLevel.Advanced, "The ratio of the white core to the coloured outer parts of the laser.")]
    public float beamRatio { get; set; }

    /// <summary>
    /// The amount of laser beams to draw beside eachother, resulting in a thicker beam.
    /// </summary>
    [Info(UserLevel.User, "The amount of laser beams to draw beside eachother, resulting in a thicker beam.")]
    public int beamCount { get; set; }

    public Laser() : this(null) {}

    public Laser(Node parent = null) {
      if (parent != null) {
        this.parent = parent;
      }
      InitializeLists();
      brightness = new Toggle<float>(1f, false);
      thickness = 5f;
      beamRatio = 0.7f;
      onceEveryAmount = 1;
      IsColorByAngle = true;
      beamCount = 1;
    }

    public override void AfterCloning() {
      //if (!parent.HasComp<Queuer>()) parent.addComponent(comp.queuer, true);
      //parent.Comp<Queuer>().qs = parent.Comp<Queuer>().qs | queues.scale | queues.position;// | queues.angle;
    }

    public override void Draw() {
      if (++counter%onceEveryAmount != 0) return;
      Vector2R start = parent.body.pos;
      if (prevPos == Vector2R.Zero) {
        prevPos = start;
        return;
      }
      //don't draw lines from screen edge to edge if screenwrapping
      if (parent.movement.mode == movemode.screenwrap) {
        float diffx = prevPos.X - start.X;
        if (diffx > room.WorldWidth/2) {
          start.X += room.WorldWidth;
        }
        else if (diffx < -room.WorldWidth/2) {
          start.X -= room.WorldWidth;
        }
        float diffy = prevPos.Y - start.Y;
        if (diffy > room.WorldHeight/2) {
          start.Y += room.WorldHeight;
        }
        else if (diffy < -room.WorldHeight/2) {
          start.Y -= room.WorldHeight;
        }
      }

      Vector2R diff = (prevPos - start);
      Vector2R centerpoint = (prevPos + start)/2;
      float len = diff.Length();
      Vector2R scalevect;
      float xscale = len;
      float yscale = thickness;
      //float outerscale = yscale;
      //float beamdist = 1f;
      if (brightness) {
        xscale = brightness;
      }
      //if (thickness)
      //{
      //    yscale = thickness;
      //    outerscale = yscale * beamRatio;
      //}

      scalevect = new Vector2R(xscale, yscale);

      float testangle = (float) (Math.Atan2(diff.Y, diff.X));

      VMath.NormalizeSafe(ref diff);
      diff = new Vector2R(-diff.Y, diff.X);

      //uncommet later when not using direction based color shit
      Color coll;
      int alpha = 255; //i * (255 / min);
      //Console.WriteLine(alpha);
      if (IsColorByAngle) {
        coll = ColorChanger.getColorFromHSV((testangle + (float) Math.PI)*(float) (180/Math.PI));
      }
      else {
        coll = new Color(parent.body.color.R, parent.body.color.G, parent.body.color.B, alpha);
      }
      //room.Camera.AddPermanentDraw(Textures.Whitepixel, centerpoint, parent.body.color, scalevect, testangle,
      //                             laserLength);
      //Todo:drawlines
      prevPos = start;
    }
  }
}