﻿using System;
using OrbitVR.Components.Linkers;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.AffectOthers {
  public enum AffectDirection {
    AffectsOthers,
    OthersAffectThis,
    Both,
  }

  /// <summary>
  /// Attracts or repels nodes that it affects.
  /// </summary>
  [Info(UserLevel.User, "Attracts or repels nodes that it affects.", CompType)]
  public class Gravity : Component, ILinkable, IMultipliable //, IRadius
  {
    public enum Mode {
      Normal,
      Strong,
      ConstantForce,
      //Logarithmic,
    }

    public const mtypes CompType = mtypes.affectother | mtypes.draw;

    private float drawscale;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// Distance at which other nodes are attracted/repelled from this node
    /// </summary>
    [Info(UserLevel.User, "Distance at which other nodes are attracted/repelled from this node")]
    public float radius { get; set; }

    /// <summary>
    /// Represents minimum distance taken into account when calculating grav force strength.
    /// </summary>
    [Info(UserLevel.Advanced, "Represents minimum distance taken into account when calculating grav force strength.")]
    public int lowerbound { get; set; }

    /// <summary>
    /// If enabled, gravity strength is constant regardless of other nodes' distance;
    /// </summary>
    [Info(UserLevel.Advanced, "If enabled, gravity strength is constant regardless of other nodes' distance;")]
    public bool isConstant { get; set; }

    /// <summary>
    /// If enabled, this node only affects other nodes with a gravity component
    /// </summary>
    [Info(UserLevel.Advanced, "If enabled, this node only affects other nodes with a gravity component")]
    public bool AffectsOnlyGravity { get; set; }

    /// <summary>
    /// If enabled, this node not only pulls or pushes other nodes; It itself is pushed and pulled by the nodes it's affecting.
    /// </summary>
    [Info(UserLevel.Advanced,
      "If enabled, this node not only pulls or pushes other nodes; It itself is pushed and pulled by the nodes it's affecting."
      )]
    public bool AffectBoth { get; set; }

    /// <summary>
    /// Adds an angle to the gravitational pull
    /// </summary>
    [Info(UserLevel.User, "Adds an angle to the gravitational pull")]
    public int angle { get; set; }

    /// <summary>
    /// The mode that gravity will operate under.
    /// Normal: The normal gravity strength.
    /// Strong: The gravity strength is much stronger; it is squared.
    /// ConstantForce: Applies the same gravity force, regardless of the target's distance from this node. 
    /// </summary>
    [Info(UserLevel.User, "The mode that gravity will operate under.")]
    public Mode mode { get; set; }

    /// <summary>
    /// The direction in which this component takes effect.
    /// </summary>
    [Info(UserLevel.User, "The direction in which this component takes effect.")]
    public AffectDirection affectDirection { get; set; }

    /// <summary>
    /// Causes the node to repulse other nodes, pushing them away.
    /// </summary>
    [Info(UserLevel.User, "Causes the node to repulse other nodes, pushing them away.")]
    public bool Repulsive { get; set; }

    /// <summary>
    /// Draws the gravitational rings around the node.
    /// </summary>
    [Info(UserLevel.User, "Draws the gravitational rings around the node.")]
    public bool ShowRings { get; set; }

    /// <summary>
    /// Draws the lines on the nodes that it is exerting a force on, in the direction of the force.
    /// </summary>
    [Info(UserLevel.User, "Draws the lines on the nodes that it is exerting a force on, in the direction of the force.")
    ]
    public bool ShowForceLines { get; set; }

    /// <summary>
    /// If the distance to the node is less than the deadZone, no gravity is applied.
    /// </summary>
    [Info(UserLevel.User, "If the distance to the node is less than the deadZone, no gravity is applied.")]
    public Toggle<float> deadZone { get; set; }

    public Gravity() : this(null) {}

    public Gravity(Node parent) {
      if (parent != null) this.parent = parent;
      affectDirection = AffectDirection.AffectsOthers;
      multiplier = 40f;
      radius = 800f;
      lowerbound = 20;
      angle = 0;
      mode = Mode.Strong;
      Repulsive = false;
      deadZone = new Toggle<float>(10, true);
      ShowForceLines = false;
      ShowRings = true;
    }

    [Info(UserLevel.Developer)]
    public Link link { get; set; }

    //public bool EveryOther = false;
    //public int counter = 0;

    public override void AffectOther(Node other) {
      if (!active) {
        return;
      }
      if (exclusions.Contains(other)) return;

      //if (EveryOther && counter++ % 2 == 0) return;

      if (AffectsOnlyGravity && !other.HasComp<Gravity>()) return;

      float distVects = Vector2R.DistanceSquared(other.body.pos, parent.body.pos);
      Node affector = parent;
      Node affected = other;
      if (affectDirection == AffectDirection.OthersAffectThis) {
        affector = other;
        affected = parent;
      }

      if (distVects < radius*radius) {
        distVects = (float) Math.Sqrt(distVects);
        if (deadZone.enabled && distVects < deadZone.value) return;
        if (distVects < lowerbound) distVects = lowerbound;
        double angletemp = Math.Atan2((affector.body.pos.Y - affected.body.pos.Y),
                                      (affector.body.pos.X - affected.body.pos.X));

        float gravForce = (multiplier*affector.body.mass*affected.body.mass);

        switch (mode) {
          case Mode.Normal:
            gravForce /= distVects*distVects;
            break;
          case Mode.Strong:
            gravForce /= distVects;
            break;
          case Mode.ConstantForce:
            gravForce /= 100; //#magicnumber
            break;
        }
        if (Repulsive) gravForce *= -1;

        if (angle != 0) {
          angletemp = (angletemp + Math.PI + (Math.PI*(float) (angle/180.0f))%(Math.PI*2)) - Math.PI;
          //test for validity
        }

        //float gravForce = gnode1.GravMultiplier;
        float velX = (float) Math.Cos(angletemp)*gravForce;
        float velY = (float) Math.Sin(angletemp)*gravForce;
        Vector2R delta = new Vector2R(velX, velY);

        /*
                delta /= other.transform.mass;
                other.transform.velocity += delta;
                //*/
        //*
        if (affectDirection == AffectDirection.Both) {
          delta /= 2;
          affected.body.velocity += delta*other.body.invmass;
          affector.body.velocity -= delta*parent.body.invmass;
        }
        else {
          affected.body.ApplyForce(delta);
        }

        if (ShowForceLines) {
          room.Camera.DrawLine(other.body.pos, other.body.pos + (delta*100), 2, parent.body.color, (int)Layers.Over4);
        }
        //other.body.velocity += delta;
        //other.body.velocity /= other.body.mass; //creates snakelike effect when put below increments
      }
    }

    public override void Draw() {
      if (!ShowRings) return;
      float deadzone = 5f;
      if (!Repulsive) {
        if (multiplier > deadzone) {
          DrawOutwards();
        }
        else if (multiplier < -deadzone) {
          DrawInwards();
        }
      }
      else {
        if (multiplier > -deadzone) {
          DrawInwards();
        }
        else if (multiplier < deadzone) {
          DrawOutwards();
        }
      }
    }

    /// <summary>
    /// Strength of gravity, use negative to repel.
    /// </summary>
    [Info(UserLevel.User, "Strength of gravity, use negative to repel.")]
    public float multiplier { get; set; }

    public void DrawOutwards() {
      DrawCircle();
      drawscale -= 2f;
      if (drawscale < 10) drawscale = radius/10;
    }

    public void DrawInwards() {
      DrawCircle();
      drawscale += 2f;
      if (drawscale > radius/10) drawscale = 5f;
    }

    public void DrawCircle() {
      //room.camera.Draw(textures.ring, parent.body.pos, parent.body.color * 0.2f, drawscale / 50f, Layers.Under2);
      room.Camera.AddPermanentDraw(Textures.Ring, parent.body.pos, parent.body.color*0.2f, drawscale/50f, 0, 50);
    }
  }
}