using System.Collections.Generic;
using SharpDX;

namespace OrbItProcs {
  /// <summary>
  /// The shovel allows you to pick up nodes, hold them, and throw them away.
  /// </summary>
  [Info(UserLevel.User, "The shovel allows you to pick up nodes, hold them, and throw them away.", CompType)]
  public class LinkGun : Component {
    public enum LinkMode {
      TargetsToSelf,
      TargetsToTargets,
      TargetsChained,
    }

    public const mtypes CompType = mtypes.playercontrol | mtypes.draw | mtypes.item; // | mtypes.affectself;
    private LinkMode _linkMode;
    Queue<Node> attachedNodesQueue;
    Gravity grav;
    private Link shootLink;
    Spring spring;
    GunState state = GunState.inactive;
    public LinkGun() : this(null) {}

    public LinkGun(Node parent) {
      this.parent = parent;
      //this.com = comp.shovel;
      shootNodeRadius = 25; //fill in property later
      linkToPlayers = true;
      shootNodeSpeed = 5f;
      linkMode = LinkMode.TargetsChained;

      shootNode = new Node(room);
      shootNode.name = "linknode";
      shootNode.body.radius = shootNodeRadius;
      shootNode.body.ExclusionCheck += (c1, c2) => c2 == parent.body;
      shootNode.body.mass = 2f;
      shootNode.body.texture = textures.cage;
    }

    public override bool active {
      get { return base.active; }
      set {
        base.active = value;
        if (shootNode != null) {
          shootNode.active = value;
        }
      }
    }

    /// <summary>
    /// The shovel node that will be held and swung.
    /// </summary>
    [Info(UserLevel.User, "The shovel node that will be held and swung.")]
    [CopyNodeProperty]
    public Node shootNode { get; set; }

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// The radius of the fist.
    /// </summary>
    [Info(UserLevel.User, "The radius of the shovel.")]
    public float shootNodeRadius { get; set; }

    public float shootNodeSpeed { get; set; }
    public bool linkToPlayers { get; set; }
    public Link attachLink { get; set; }

    public LinkMode linkMode {
      get { return _linkMode; }
      set {
        if (value != _linkMode) UpdateAttachLink();
        _linkMode = value;
      }
    }

    private void UpdateAttachLink() {
      //todo: switch between different linkModes here
    }

    public override void AfterCloning() {
      if (shootNode == null) return;
      shootNode = shootNode.CreateClone(room);
    }

    public override void OnSpawn() {
      shootNode.dataStore["linknodeparent"] = parent;
      shootNode.body.pos = parent.body.pos;
      shootNode.addComponent<ColorChanger>(true);
      shootNode.AffectExclusionCheck += (node) => node == parent;
      room.groups.items.IncludeEntity(shootNode);
      shootNode.OnSpawn();
      shootNode.body.AddExclusionCheck(parent.body);
      shootNode.active = false;
      shootNode.movement.maxVelocity.value = 50f;

      spring = new Spring();
      spring.restdist = 0;
      spring.springMode = Spring.mode.PushOnly;
      spring.multiplier = 400;

      //Tether tether = new Tether();
      //tether.mindist = 0;
      //tether.maxdist = 20;
      //tether.active = true;

      //shootNodeLink = new Link(parent, shootNode, spring);
      //shootNodeLink.IsEntangled = true;
      ////shovelLink.components.Add(tether);

      grav = new Gravity();
      grav.mode = Gravity.Mode.ConstantForce;
      //grav.Repulsive = true;
      grav.multiplier = 100;

      shootLink = new Link(parent, shootNode, grav);
      shootLink.AddLinkComponent(spring, true);


      //
      Gravity attachGrav = new Gravity();
      attachGrav.multiplier = 100;
      attachGrav.mode = Gravity.Mode.ConstantForce;

      Tether aTether = new Tether();
      aTether.maxdist = 100;
      aTether.mindist = 0;
      aTether.activated = true;

      Spring aSpring = new Spring();
      aSpring.springMode = Spring.mode.PushAndPull;
      aSpring.multiplier = 100;
      aSpring.restdist = 0;

      attachedNodesQueue = new Queue<Node>();
      //attachLink = new Link(parent, new HashSet<Node>(), attachGrav); //targetsToSelf

      attachLink = new Link(new HashSet<Node>(), new HashSet<Node>(), aTether); //targetsChained
      attachLink.FormationType = formationtype.Chain;
      //attachLink.AddLinkComponent(aTether, true);
      //attachLink.AddLinkComponent(attachGrav, true);


      shootNode.body.OnCollisionEnter += (n, other) => {
        if (other == parent) return;
        if (linkMode == LinkMode.TargetsToSelf) {
          if (!attachLink.targets.Contains(other)) {
            attachLink.targets.Add(other);
            attachedNodesQueue.Enqueue(other);
            attachLink.active = true;
          }
        }
        else if (linkMode == LinkMode.TargetsToTargets) {
          if (!attachLink.sources.Contains(other)) {
            attachLink.sources.Add(other);
            attachLink.targets.Add(other);
            attachedNodesQueue.Enqueue(other);
            attachLink.active = true;
          }
        }
        else if (linkMode == LinkMode.TargetsChained) {
          if (!attachLink.sources.Contains(other)) {
            attachedNodesQueue.Enqueue(other);
            attachLink.formation.AddChainNode(other);
            attachLink.active = true;
          }
        }
      };

      parent.body.ExclusionCheck += (c1, c2) =>
        attachLink.targets.Contains(c2.parent);
    }

    public override void PlayerControl(Input input) {
      if (state == GunState.inactive) {
        if (input.BtnClicked(InputButtons.RightTrigger_Mouse1)) {
          state = GunState.extending;
          Vector2 dir = input.GetRightStick().NormalizeSafe()*shootNodeSpeed + parent.body.velocity;
          shootNode.body.pos = parent.body.pos + dir*5;
          shootNode.body.velocity = dir;
          shootNode.active = true;

          grav.active = false;
          spring.active = true;
          shootLink.active = true;
        }
      }
      else if (state == GunState.extending) {
        if (input.BtnReleased(InputButtons.RightTrigger_Mouse1)) {
          state = GunState.retracting;
          grav.active = true;
          spring.active = false;
        }
      }
      else if (state == GunState.retracting) {
        shootNode.body.velocity = VMath.Redirect(shootNode.body.velocity, parent.body.pos - shootNode.body.pos);
        float catchZone = 20f; //1f for bipedal action
        if ((parent.body.pos - shootNode.body.pos).Length() < catchZone) {
          state = GunState.inactive;
          shootNode.active = false;
          shootLink.active = false;
        }
      }

      if (input.BtnClicked(InputButtons.RightBumper_E)) {
        if (attachedNodesQueue.Count > 0) {
          Node n = attachedNodesQueue.Dequeue();
          if (attachLink.targets.Contains(n)) {
            attachLink.targets.Remove(n);
          }
          if (attachLink.sources.Contains(n)) {
            attachLink.sources.Remove(n);
          }
          if (attachLink.formation.chainList.Contains(n)) {
            attachLink.formation.chainList.Remove(n);
          }
          if (attachedNodesQueue.Count == 0) {
            attachLink.active = false;
          }
          attachLink.formation.UpdateFormation();
        }
      }

      if (input.BtnClicked(InputButtons.LeftBumper_Q)) {
        if (attachedNodesQueue.Count > 0) {
          if (attachedNodesQueue.Count != 0)
            attachedNodesQueue = new Queue<Node>();
          if (attachLink.targets.Count != 0)
            attachLink.targets = new ObservableHashSet<Node>();

          if (!attachLink.sources.Contains(parent)) {
            attachLink.formation.ClearChain();
            if (attachLink.sources.Count != 0)
              attachLink.sources = new ObservableHashSet<Node>();
          }

          attachLink.active = false;
        }
      }

      if (attachLink.active) {
        float amountPushed = 1f - input.newInputState.LeftTriggerAnalog;
        amountPushed = amountPushed*100 + 50;
        if (attachLink.HasComp<Tether>()) {
          attachLink.Comp<Tether>().maxdist = (int) amountPushed;
          attachLink.Comp<Tether>().mindist = (int) amountPushed;
        }
        if (attachLink.HasComp<Spring>()) {
          attachLink.Comp<Spring>().restdist = (int) amountPushed;
        }
      }
    }

    public override void Draw() {
      //Color col = Color.White;
      //
      //Vector2 position = shootNode.body.pos;
      //if (position == Vector2.Zero)
      //{
      //    position = parent.body.pos;
      //    return;
      //}
      //if (deployed)
      //{
      //    //room.camera.DrawLine(position, parent.body.pos, 2f, col, Layers.Under2);
      //}
    }

    public override void OnRemove(Node other) {
      shootNode.OnDeath(other);
    }

    private enum GunState {
      inactive,
      extending,
      retracting,
    }
  }
}