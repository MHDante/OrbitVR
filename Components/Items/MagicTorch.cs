using System;
using System.Collections.Generic;
using OrbitVR.Components.AffectOthers;
using OrbitVR.Components.Essential;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;
using Collision = OrbitVR.Components.Essential.Collision;

namespace OrbitVR.Components.Items {
  /// <summary>
  /// The magic torch lets you hold a torch that can have any affect others component attached to it.
  /// </summary>
  [Info(UserLevel.User,
    "The magic torch lets you hold a torch that can have any affect others component attached to it.", CompType)]
  public class MagicTorch : Component {
    public enum MagicType {
      gravity,
      displace,
      spring,
      transfer,
    }

    public const mtypes CompType = mtypes.playercontrol | mtypes.draw | mtypes.item; // | mtypes.affectself;

    private static Dictionary<MagicType, Type> MagicToType = new Dictionary<MagicType, Type>() {
      {MagicType.gravity, typeof (Gravity)},
      {MagicType.displace, typeof (Displace)},
      {MagicType.spring, typeof (Spring)},
      {MagicType.transfer, typeof (Transfer)},
    };

    private static Dictionary<MagicType, Textures> MagicToRune = new Dictionary<MagicType, Textures>() {
      {MagicType.gravity, Textures.Rune12},
      {MagicType.displace, Textures.Rune13},
      {MagicType.spring, Textures.Rune6},
      {MagicType.transfer, Textures.Rune10},
    };

    private MagicType _magicType = MagicType.gravity;
    float total;

    public override bool active {
      get { return base.active; }
      set {
        base.active = value;
        if (torchNode != null) {
          torchNode.active = value;
        }
      }
    }

    /// <summary>
    /// The torch node that will be held and swung.
    /// </summary>
    [Info(UserLevel.User, "The torch node that will be held and swung.")]
    [CopyNodeProperty]
    public Node torchNode { get; set; }

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// The radius of the torch.
    /// </summary>
    [Info(UserLevel.User, "The radius of the torch.")]
    public float torchRadius { get; set; }

    /// <summary>
    /// The multiplier affects the strength of the torch, which will make to more gravitating, or more repulsive.
    /// </summary>
    [Info(UserLevel.User,
      "The multiplier affects the strength of the torch, which will make to more gravitating, or more repulsive.")]
    public float torchMultiplier { get; set; }

    /// <summary>
    /// Represents the max distance the torch can reach from the player.
    /// </summary>
    [Info(UserLevel.User, "Represents the max distance the torch can reach from the player.")]
    public float torchReach { get; set; }

    /// <summary>
    /// The magic type is the component that is active on the torch. Change it!
    /// </summary>
    [Info(UserLevel.User, "The magic type is the component that is active on the torch. Change it!")]
    public MagicType magicType {
      get { return _magicType; }
      set {
        _magicType = value;
        foreach (MagicType m in Enum.GetValues(typeof (MagicType))) {
          Type t = MagicToType[m];
          IMultipliable comp = (IMultipliable) torchNode.comps[t];
          bool act = m == _magicType;
          comp.active = act;
          if (act) activeComp = comp;
        }
      }
    }

    private IMultipliable activeComp { get; set; }

    public MagicTorch() : this(null) {}

    public MagicTorch(Node parent) {
      this.parent = parent;
      torchRadius = 15;
      torchReach = 200;
      torchMultiplier = 50;


      Dictionary<dynamic, dynamic> props = new Dictionary<dynamic, dynamic>() {
        {typeof (BasicDraw), false},
        {typeof (Collision), false},
        {typeof (Gravity), false},
        {typeof (Displace), false},
        {typeof (Spring), false},
        {typeof (Transfer), false},
      };

      torchNode = new Node(room, props);
      torchNode.name = "magictorch";
      torchNode.body.radius = torchRadius;


      magicType = MagicType.gravity;
    }

    public override void AfterCloning() {
      if (torchNode == null) return;
      torchNode = torchNode.CreateClone(room);
      //sword = new Node(room, props);
      magicType = magicType;
    }

    public override void OnSpawn() {
      //Node.cloneNode(parent.Game1.ui.sidebar.ActiveDefaultNode, sword);
      //parent.body.texture = textures.orientedcircle;
      torchNode.dataStore["magictorchnodeparent"] = parent;
      torchNode.body.pos = parent.body.pos;

      torchNode.AffectExclusionCheck += (node) => node == parent;

      room.Groups.Items.IncludeEntity(torchNode);
      torchNode.OnSpawn();
      torchNode.body.AddExclusionCheck(parent.body);
      torchNode.body.OnCollisionEnter += (p, o) => {
                                           Node otherparent = null;
                                           if (o.dataStore.ContainsKey("swordnodeparent")) {
                                             otherparent = o.dataStore["swordnodeparent"];
                                           }
                                           else if (o.dataStore.ContainsKey("fistnodeparent")) {
                                             otherparent = o.dataStore["fistnodeparent"];
                                           }
                                           else if (o.dataStore.ContainsKey("magictorchnodeparent")) {
                                             otherparent = o.dataStore["magictorchnodeparent"];
                                           }
                                           if (otherparent != null) {
                                             Vector2 f = otherparent.body.pos - parent.body.pos;
                                             VMath.NormalizeSafe(ref f);
                                             f *= 10;
                                             otherparent.body.ApplyForce(f);
                                           }
                                           //if (o.player != null)
                                           //{
                                           //    //o.player.node.meta.CalculateDamage(parent, damageMultiplier);
                                           //}
                                         };
      //sword.body.exclusionList.Add(parent.body);
      //
      //parent.body.exclusionList.Add(sword.body);
    }

    public override void PlayerControl(Input input) {
      Vector2 newstickpos = input.GetRightStick(torchReach, true);

      Vector2 pos = newstickpos*torchReach;
      torchNode.body.pos = parent.body.pos + pos;
      float positive = 0f;
      float negative = 0f;
      if (input is PcFullInput) {
        positive = (input.BtnDown(InputButtons.RightTrigger_Mouse1) ? 1f : 0f)*torchMultiplier;
        negative = (input.BtnDown(InputButtons.LeftTrigger_Mouse2) ? 1f : 0f)*torchMultiplier;
      }
      else {
        positive = input.newInputState.RightTriggerAnalog*torchMultiplier;
        negative = input.newInputState.LeftTriggerAnalog*torchMultiplier;
      }
      total = positive - negative;

      //torchNode.Comp<Gravity>().multiplier = total;
      if (torchNode != activeComp.parent) {
        System.Diagnostics.Debugger.Break();
      }
      activeComp.multiplier = total;
    }

    public override void Draw() {
      Color col = Color.White;
      if (total > 0) {
        col = Color.Lerp(Color.White, Color.Green, total/torchMultiplier);
      }
      else {
        col = Color.Lerp(Color.White, Color.Red, total/torchMultiplier*-1f);
      }

      Vector2 position = torchNode.body.pos;
      if (position == Vector2.Zero) position = parent.body.pos;
      else {
        room.Camera.DrawLine(position, parent.body.pos, 2f, col, (int)Layers.Over3);
      }

      torchNode.body.color = col;
      room.Camera.Draw(Textures.Ring, position, Color.Black, torchNode.body.scale, torchNode.body.orient, (int)Layers.Over4);
      room.Camera.Draw(MagicToRune[magicType], position, col, torchNode.body.scale,
                       VMath.VectorToAngle(position - parent.body.pos) + GMath.PIbyTwo, (int)Layers.Over3);
    }


    public override void OnRemove(Node other) {
      torchNode.OnDeath(other);
    }
  }
}