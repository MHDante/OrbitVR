﻿using System;
using OrbitVR.Components.Meta;
using OrbitVR.Framework;
using OrbitVR.Physics;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.AffectOthers {
  /// <summary>
  /// The spike hurts nodes.
  /// </summary>
  [Info(UserLevel.User, "The spike hurts nodes.")]
  public class Spike : Component //, IRadius
  {
    public enum DamageMode {
      Players,
      Nodes,
      Both,
      None,
    }

    public const mtypes CompType = mtypes.none;
    public Action<Node, Node> collisionAction;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// If activated the spike will affect others.
    /// </summary>
    [Info(UserLevel.User, "If activated the spike will affect others.")]
    public bool spikeActivated { get; set; }

    /// <summary>
    /// The amount of damage done.
    /// </summary>
    [Info(UserLevel.User, "The amount of damage done.")]
    public float damageMultiplier { get; set; }

    /// <summary>
    /// The amount the spike will push nodes.
    /// </summary>
    [Info(UserLevel.User, "The amount the spike will push nodes.")]
    public Toggle<float> pushBack { get; set; }

    /// <summary>
    /// The amount of seconds the spike will stun others.
    /// </summary>
    [Info(UserLevel.User, "The amount of seconds the spike will stun others.")]
    public Toggle<float> stunSeconds { get; set; }

    /// <summary>
    /// The damage mode of the spike.
    /// </summary>
    [Info(UserLevel.User, "The damage mode of the spike.")]
    public DamageMode damageMode { get; set; }

    public Spike() : this(null) {}

    public Spike(Node parent) {
      this.parent = parent;
      spikeActivated = true;
      damageMultiplier = 1f;
      pushBack = new Toggle<float>(10f, true);
      stunSeconds = new Toggle<float>(1f, true);
      damageMode = DamageMode.Players;
    }

    public override void OnSpawn() {
      collisionAction = (s, t) => {
                          if (t.IsPlayer) {
                            if (damageMode == DamageMode.Players || damageMode == DamageMode.Both) {
                              t.meta.CalculateDamage(s, damageMultiplier);
                            }
                            if (pushBack.enabled) {
                              Vector2R f = (t.body.pos - s.body.pos);
                              VMath.NormalizeSafe(ref f);
                              f *= pushBack.value;
                              t.body.velocity += f;
                            }
                            if (stunSeconds.enabled) {
                              if (t.movement.active) {
                                t.movement.active = false;
                                Action<Node> ad = (n) => { t.movement.active = true; };
                                room.Scheduler.AddAppointment(new Appointment(ad, (int) (stunSeconds.value*1000)));
                              }
                            }
                          }
                          else {
                            if (damageMode == DamageMode.Nodes || damageMode == DamageMode.Both) {
                              t.meta.CalculateDamage(s, damageMultiplier);
                            }
                            if (pushBack.enabled) {
                              Vector2R f = (t.body.pos - s.body.pos);
                              VMath.NormalizeSafe(ref f);
                              f *= pushBack.value;
                              t.body.velocity += f;
                            }
                            if (stunSeconds.enabled) {
                              if (t.movement.active) {
                                t.movement.active = false;
                                Action<Node> ad = (n) => t.movement.active = true;
                                t.scheduler.AddAppointment(new Appointment(ad, (int) (stunSeconds.value*1000)));
                              }
                            }
                          }
                        };


      Polygon poly = new Polygon();
      poly.body = parent.body;
      float dist = parent.body.radius*1.3f;
      Vector2R[] verts = new Vector2R[3];
      for (int i = 0; i < 3; i++) {
        verts[i] = VMath.AngleToVector(GMath.TwoPI/3f*i)*dist;
      }
      parent.body.shape = poly;
      poly.Set(verts, 3);
      parent.body.DrawPolygonCenter = true;
      parent.body.orient = parent.body.orient; //todo:set this every init of polys
      parent.body.OnCollisionEnter += collisionAction;

      parent.body.ExclusionCheck += (s, t) => { return Vector2R.Distance(s.pos, t.pos) > dist; };
    }
  }
}