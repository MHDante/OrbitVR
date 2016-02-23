using System;
using OrbitVR.Framework;
using OrbitVR.Interface;
using OrbitVR.Physics;
using SharpDX;

namespace OrbitVR.Components.Items {
  /// <summary>
  /// The fist allows you to punch other players and nodes.
  /// </summary>
  [Info(UserLevel.User, "The fist allows you to punch other players and nodes.", CompType)]
  public class Fist : Component {
    public enum fistmode {
      ready,
      punching,
      retracting,
    }

    public const mtypes CompType = mtypes.playercontrol | mtypes.draw | mtypes.item; // | mtypes.affectself;

    private bool movingStick = false;
    Vector2 oldstickpos = Vector2.Zero;
    fistmode state = fistmode.ready;

    Vector2 target;

    public override bool active {
      get { return base.active; }
      set {
        base.active = value;
        if (fistNode != null) {
          fistNode.active = value;
        }
      }
    }

    /// <summary>
    /// The fist node that will be held and swung.
    /// </summary>
    [Info(UserLevel.User, "The sword node that will be held and swung.")]
    [CopyNodeProperty]
    public Node fistNode { get; set; }

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// The distance from the player the fist can reach.
    /// </summary>
    [Info(UserLevel.User, "The distance from the player the sword will swing at.")]
    public float fistReach { get; set; }

    /// <summary>
    /// The radius of the fist.
    /// </summary>
    [Info(UserLevel.User, "The radius of the fist.")]
    public float fistRadius { get; set; }

    /// <summary>
    /// The amount of damage the fist will do.
    /// </summary>
    [Info(UserLevel.User, "The amount of damage the fist will do.")]
    public float damageMultiplier { get; set; }

    /// <summary>
    /// The force at which to push the other node back when clashing fist.
    /// </summary>
    [Info(UserLevel.User, "The force at which to push the other node back when clashing swords.")]
    public float parryKnockback { get; set; }

    /// <summary>
    /// The force at which to push the other node back after a direct hit to the other node.
    /// </summary>
    [Info(UserLevel.User, "The force at which to push the other node back after a direct hit to the other node.")]
    public float nodeKnockback { get; set; }

    public Fist() : this(null) {}

    public Fist(Node parent) {
      this.parent = parent;
      fistReach = 60;
      fistRadius = 15;
      damageMultiplier = 10f;
      parryKnockback = 20f;
      nodeKnockback = 500f;
      fistNode = new Node(room);
      fistNode.basicdraw.active = false;
      fistNode.name = "fist";
      fistNode.body.radius = fistRadius;
    }

    public override void AfterCloning() {
      if (fistNode == null) return;
      fistNode = fistNode.CreateClone(room);
      //sword = new Node(room, props);
    }

    public override void OnSpawn() {
      //Node.cloneNode(parent.Game1.ui.sidebar.ActiveDefaultNode, sword);
      //parent.body.texture = textures.orientedcircle;
      fistNode.dataStore["fistnodeparent"] = parent;
      fistNode.body.pos = parent.body.pos;


      room.Groups.Items.IncludeEntity(fistNode);
      fistNode.OnSpawn();
      fistNode.body.AddExclusionCheck(parent.body);
      fistNode.body.ExclusionCheck += delegate(Collider p, Collider o) { return !movingStick; };
      fistNode.body.OnCollisionEnter += (p, o) => {
                                          if (o.dataStore.ContainsKey("swordnodeparent")) {
                                            Node otherparent = o.dataStore["swordnodeparent"];
                                            Vector2 f = otherparent.body.pos - parent.body.pos;
                                            VMath.NormalizeSafe(ref f);
                                            f *= parryKnockback;
                                            otherparent.body.ApplyForce(f);
                                          }
                                          else if (o.dataStore.ContainsKey("fistnodeparent")) {
                                            Node otherparent = o.dataStore["fistnodeparent"];
                                            Vector2 f = otherparent.body.pos - parent.body.pos;
                                            VMath.NormalizeSafe(ref f);
                                            f *= parryKnockback;
                                            otherparent.body.ApplyForce(f);
                                          }
                                          else if (o.IsPlayer) {
                                            o.player.node.meta.CalculateDamage(parent, damageMultiplier);
                                          }
                                        };
      //sword.body.exclusionList.Add(parent.body);
      //
      //parent.body.exclusionList.Add(sword.body);
    }

    public override void PlayerControl(Input input) {
      //fistNode.movement.active = false;
      //fistNode.body.velocity = fistNode.body.effvelocity * nodeKnockback;
      Vector2 newstickpos = input.GetRightStick();
      Vector2 relVel = newstickpos - oldstickpos;

      if (state == fistmode.ready) {
        fistNode.body.pos = parent.body.pos;
        fistNode.body.orient = parent.body.orient;
        //if stick is moving away from center of stick
        if (newstickpos.LengthSquared() > oldstickpos.LengthSquared()) {
          //if stick is moving fast enough
          float len = relVel.Length();
          if (relVel.Length() > 0.2f) //deadzone
          {
            state = fistmode.punching;
            float power = (float) Math.Log((double) len + 2.0, 2.0)/2f;
            //Console.WriteLine(power);
            VMath.NormalizeSafe(ref relVel);
            relVel *= power;
            //Console.WriteLine(relVel.X + " : " + relVel.Y);
            //fistNode.body.ApplyForce(relVel);
            fistNode.body.velocity = relVel*10f;
            fistNode.body.orient = VMath.VectorToAngle(relVel);
          }
        }
      }
      else if (state == fistmode.punching) {
        //check if fully punched.
        if (Vector2.Distance(fistNode.body.pos, parent.body.pos) > fistReach) {
          state = fistmode.retracting;
        }
      }
      else if (state == fistmode.retracting) {
        //fistNode.body.pos = Vector2.Lerp(fistNode.body.pos, parent.body.pos, 0.1f);

        //Vector2 vel = (parent.body.pos - fistNode.body.pos);
        //VMath.NormalizeSafe(ref vel);
        //vel *= 1;
        //fistNode.body.velocity = vel;

        Vector2 vel = (parent.body.pos - fistNode.body.pos);
        //if (vel.Length() < 5)
        //{
        VMath.NormalizeSafe(ref vel);
        vel *= 20;
        //}
        fistNode.body.velocity = vel;
        if (Vector2.DistanceSquared(fistNode.body.pos, parent.body.pos) < 50*50) {
          state = fistmode.ready;
        }
      }
      //if (state != fistmode.ready)
      //    Console.WriteLine(state);
      oldstickpos = newstickpos;
    }

    public override void Draw() {
      Vector2 position = fistNode.body.pos;
      if (position == Vector2.Zero) position = parent.body.pos;
      else {
        //Utils.DrawLine(room, position, parent.body.pos, 2f, parent.body.color, Layers.Under2);
        //Utils.DrawLine(room, target, parent.body.pos, 2f, Color.Red, Layers.Under2);
      }

      room.Camera.Draw(textures.fist, position, Color.White, fistNode.body.scale, fistNode.body.orient, Layers.Over2);
      //layers don't work
    }

    public override void OnRemove(Node other) {
      fistNode.OnDeath(other);
    }

    public void PlayerControlOld(Controller controller) {
      if (controller is FullController) {
        FullController fc = (FullController) controller;
        //fistNode.movement.active = false;
        //fistNode.body.velocity = fistNode.body.effvelocity * nodeKnockback;

        bool atReach = Vector2.Distance(fistNode.body.pos, parent.body.pos) > fistReach;

        if (fc.newGamePadState.ThumbSticks.Right.LengthSquared() > 0.2*0.2) {
          if (!atReach) {
            movingStick = true;
            target = fc.newGamePadState.ThumbSticks.Right*fistReach;
            target.Y *= -1;
            target = parent.body.pos + target;
            Vector2 force = target - fistNode.body.pos;
            fistNode.body.ApplyForce(force/10);
            //fistNode.body.velocity += force;
            //Console.WriteLine(force.X + " : " + force.Y);
          }
        }
        else {
          target = parent.body.pos;
          //movingStick = false;
          //Vector2 restPos = new Vector2(parent.body.radius, 0).Rotate(parent.body.orient) + parent.body.pos;
          //fistNode.body.pos = Vector2.Lerp(fistNode.body.pos, restPos, 0.1f);
          //fistNode.body.orient = Utils.AngleLerp(fistNode.body.orient, parent.body.orient, 0.1f);
        }
        if (atReach) {
          Vector2 direction = fistNode.body.pos - parent.body.pos;
          VMath.NormalizeSafe(ref direction);
          direction *= fistReach;
          fistNode.body.pos = parent.body.pos + direction;
        }
      }
    }
  }
}