using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OrbitVR.Components.AffectOthers;
using OrbitVR.Components.Linkers;
using OrbitVR.Components.Meta;
using OrbitVR.Framework;
using OrbitVR.Physics;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Items {
  /// <summary>
  /// The shovel allows you to pick up nodes, hold them, and throw them away.
  /// </summary>
  [Info(UserLevel.User, "The shovel allows you to pick up nodes, hold them, and throw them away.", CompType)]
  public class Shovel : Component {
    public enum ModePlayers {
      GrabOtherPlayers,
      GrabSelf,
      GrabBoth,
      GrabNone,
    }

    public enum ModeShovelPosition {
      AbsoluteStickPos,
      PhysicsBased,
    }

    public const mtypes CompType = mtypes.playercontrol | mtypes.draw | mtypes.item; // | mtypes.affectself;
    public static int counter = 0;
    //Func<Collider, Collider, bool> exclusionDel;
    float compoundedMass = 0f;
    Link rangeLink;
    bool shovelling = false;
    Link shovelLink;

    public override bool active {
      get { return base.active; }
      set {
        base.active = value;
        if (shovelNode != null) {
          shovelNode.active = value;
        }
      }
    }

    /// <summary>
    /// The shovel node that will be held and swung.
    /// </summary>
    [Info(UserLevel.User, "The shovel node that will be held and swung.")]
    public Node shovelNode { get; set; }

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// The radius of the fist.
    /// </summary>
    [Info(UserLevel.User, "The radius of the shovel.")]
    public float shovelRadius { get; set; }

    ///// <summary>
    ///// The multiplier affects the strength of the torch, which will make to more gravitating, or more repulsive.
    ///// </summary>
    //[Info(UserLevel.User, "The multiplier affects the strength of the torch, which will make to more gravitating, or more repulsive.")]
    //public float shovelMultiplier { get; set; }
    /// <summary>
    /// Represents the max distance the torch can reach from the player.
    /// </summary>
    [Info(UserLevel.User, "Represents the max distance the shovel can reach from the player.")]
    public float shovelReach { get; set; }

    /// <summary>
    /// Sets the maximum amount of nodes that can be captured by the shovel at any time.
    /// </summary>
    [Info(UserLevel.User, "Sets the maximum amount of nodes that can be captured by the shovel at any time.")]
    public int maxShovelCapacity { get; set; }

    /// <summary>
    /// The maximum reach the shovel can have in order to pick up nodes. (The size of the shovel head's reach)
    /// </summary>
    [Info(UserLevel.User,
      "The maximum reach the shovel can have in order to pick up nodes. (The size of the shovel head's reach)")]
    public float scoopReach { get; set; }

    /// <summary>
    /// Controls how the shovel will behave in terms of player control. Absoulte make it the exact stick position. Physics based deals with forces.
    /// </summary>
    [Info(UserLevel.User,
      "Controls how the shovel will behave in terms of player control. Absoulte make it the exact stick position. Physics based deals with forces."
      )]
    public ModeShovelPosition modeShovelPosition { get; set; }

    /// <summary>
    /// The modePlayers allows you to specific which players the shovel can pick up. You can shovel yourself, other players, both, or none.
    /// </summary>
    [Info(UserLevel.User,
      "The modePlayers allows you to specific which players the shovel can pick up. You can shovel yourself, other players, both, or none."
      )]
    public ModePlayers modePlayers { get; set; }

    public bool physicsThrow { get; set; }
    public float throwSpeed { get; set; }
    public float physicsDivisor { get; set; }

    public Shovel() : this(null) {}

    public Shovel(Node parent) {
      this.parent = parent;
      shovelRadius = 15;
      shovelReach = 100;
      scoopReach = 60;
      maxShovelCapacity = 5;
      modePlayers = ModePlayers.GrabNone;
      modeShovelPosition = ModeShovelPosition.AbsoluteStickPos;
      physicsDivisor = 8;
      physicsThrow = false;
      throwSpeed = 12;

      shovelNode = new Node(room);
      shovelNode.name = "shovel" + counter++;
      shovelNode.body.radius = shovelRadius;
      shovelNode.body.ExclusionCheck += (c1, c2) => c2 == parent.body;
      shovelNode.body.mass = 0.001f;
      shovelNode.body.texture = Textures.Shoveltip;
    }

    //public static List<Shovel> shovels = new List<Shovel>();

    public override void AfterCloning() {
      //shovels.Add(this);
      //if (shovelNode == null) return;
      //shovelNode = shovelNode.CreateClone(room);
    }

    public override void OnSpawn() {
      //Node.cloneNode(parent.Game1.ui.sidebar.ActiveDefaultNode, sword);
      //parent.body.texture = textures.orientedcircle;
      shovelNode.dataStore["shovelnodeparent"] = parent;
      shovelNode.body.pos = parent.body.pos;

      shovelNode.AffectExclusionCheck += (node) => node == parent;

      room.Groups.Items.IncludeEntity(shovelNode);
      Debug.WriteLine(room.Groups.Items.entities.Count);
      shovelNode.OnSpawn();
      shovelNode.body.AddExclusionCheck(parent.body);
      Spring spring = new Spring();
      spring.restdist = 0;
      spring.springMode = Spring.mode.PullOnly;
      spring.active = true;
      spring.multiplier = 1000;

      Tether tether = new Tether();
      tether.mindist = 0;
      tether.maxdist = 20;
      tether.active = true;

      shovelLink = new Link(shovelNode, new HashSet<Node>(), spring);
      shovelLink.AddLinkComponent(tether);

      //to keep the shovel in reach for physics based control
      Tether rangeTether = new Tether();
      rangeTether.mindist = 0;
      rangeTether.maxdist = (int) shovelReach;
      rangeTether.active = true;
      rangeLink = new Link(parent, shovelNode, rangeTether);
      if (modeShovelPosition == ModeShovelPosition.PhysicsBased) {
        rangeLink.active = true;
      }

      //exclusionDel = delegate(Collider c1, Collider c2)
      //{
      //    return shovelLink.active && shovelLink.targets.Contains(c2.parent);
      //};
      //shovelNode.body.ExclusionCheck += exclusionDel;
      //parent.body.ExclusionCheck += exclusionDel;
    }

    public override void PlayerControl(Input input) {
      Vector2 newstickpos = input.GetRightStick(shovelReach, true); //input.GetRightStick();
      Vector2 pos = newstickpos*shovelReach;
      Vector2 worldStickPos = parent.body.pos + pos;
      Vector2 diff = worldStickPos - shovelNode.body.pos;
      //float angle = Utils.VectorToAngle(shovelNode.body.pos - parent.body.pos) + VMath.PIbyTwo % VMath.twoPI;
      Vector2 shovelDir = shovelNode.body.pos - parent.body.pos;
      shovelDir = new Vector2(shovelDir.Y, -shovelDir.X);
      shovelNode.body.SetOrientV2(shovelDir);

      if (modeShovelPosition == ModeShovelPosition.AbsoluteStickPos) {
        shovelNode.body.pos = worldStickPos;
      }
      else if (modeShovelPosition == ModeShovelPosition.PhysicsBased) {
        float len = diff.Length();
        if (len < 1) {
          shovelNode.body.velocity = Vector2.Zero;
        }
        else {
          float velLen = shovelNode.body.velocity.Length();

          Vector2 diffcopy = diff;
          VMath.NormalizeSafe(ref diffcopy);

          Vector2 normalizedVel = shovelNode.body.velocity;
          VMath.NormalizeSafe(ref normalizedVel);

          float result = 0;
          Vector2.Dot(ref diffcopy, ref normalizedVel, out result);

          diffcopy *= result;
          Vector2 force = (diff/physicsDivisor);
          if (shovelling && compoundedMass >= 1) force /= compoundedMass*1;
          shovelNode.body.velocity = diffcopy + force;
          //shovelNode.body.ApplyForce(force);
        }
      }

      if (shovelling) {
        //if (fc.newGamePadState.Triggers.Right < deadzone && fc.oldGamePadState.Triggers.Right > deadzone)
        if (input.BtnReleased(InputButtons.RightTrigger_Mouse1)) {
          shovelling = false;
          foreach (Node n in shovelLink.targets.ToList()) {
            if (physicsThrow) {
              n.body.velocity = n.body.effvelocity;
            }
            else {
              Vector2 stickdirection = newstickpos;
              VMath.NormalizeSafe(ref stickdirection);

              n.body.velocity = stickdirection*throwSpeed;
            }
            n.collision.active = true;
            shovelLink.targets.Remove(n);
            n.body.ClearExclusionChecks();
            n.body.color = n.body.permaColor;
          }
          shovelLink.formation.UpdateFormation();
          shovelLink.active = false;
          shovelNode.room.AllActiveLinks.Remove(shovelLink);
          compoundedMass = 0f;
        }
      }
      else {
        if (input.BtnClicked(InputButtons.RightTrigger_Mouse1)) {
          shovelling = true;
          ObservableHashSet<Node> capturedNodes = new ObservableHashSet<Node>();
          int count = 0;
          Action<Collider, Collider> del = delegate(Collider c1, Collider c2) {
                                             if (count >= maxShovelCapacity) return;
                                             if (c2.parent.dataStore.ContainsKey("shovelnodeparent")) return;
                                             if (c2.parent.HasComp<Diode>()) return;
                                             if (modePlayers != ModePlayers.GrabBoth && c2.parent.IsPlayer) {
                                               if (modePlayers == ModePlayers.GrabNone) return;
                                               if (modePlayers == ModePlayers.GrabSelf && c2.parent != parent) return;
                                               if (modePlayers == ModePlayers.GrabOtherPlayers && c2.parent == parent)
                                                 return;
                                             }
                                             float dist = Vector2.Distance(c1.pos, c2.pos);
                                             if (dist <= scoopReach) {
                                               count++;
                                               capturedNodes.Add(c2.parent);
                                               c2.parent.body.color = parent.body.color;
                                             }
                                           };
          shovelNode.room.GridsystemAffect.retrieveOffsetArraysAffect(shovelNode.body, del, scoopReach*2);
          shovelLink.targets = capturedNodes;
          shovelLink.formation.UpdateFormation();
          shovelLink.active = true;
          shovelNode.room.AllActiveLinks.Add(shovelLink);
          compoundedMass = 0f;
          foreach (Node n in capturedNodes) {
            n.collision.active = false;
            compoundedMass += n.body.mass;
          }
        }
      }
    }

    public override void Draw() {
      Color col = Color.White;

      Vector2 position = shovelNode.body.pos;
      if (position == Vector2.Zero) position = parent.body.pos;
      else {
        room.Camera.DrawLine(position, parent.body.pos, 2f, col, (int)Layers.Over3);
      }
    }

    public override void OnRemove(Node other) {
      shovelLink.DeleteLink();
      rangeLink.DeleteLink();
      shovelNode.OnDeath(other);
      //Debug.WriteLine("Deleting shovel links");
    }
  }
}