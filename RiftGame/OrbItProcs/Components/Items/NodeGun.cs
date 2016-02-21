using System;
using System.Linq;
using SharpDX;

namespace OrbItProcs {
  /// <summary>
  /// The NodeGun allows players to shoot nodes outwards. The player can use the bumpers the cycle through the custom groups to shoot different types of nodes.
  /// </summary>
  [Info(UserLevel.User,
    "The NodeGun allows players to shoot nodes outwards. The player can use the bumpers the cycle through the custom groups to shoot different types of nodes.",
    CompType)]
  public class NodeGun : Component {
    public enum mode {
      SingleFire,
      AutoFire,
    }

    public const mtypes CompType = mtypes.playercontrol | mtypes.item | mtypes.draw;
    Group currentGroup;
    float deadZone = 0.5f;
    Node lastFired = null;

    private int shootingDelayCount = 0;
    public NodeGun() : this(null) {}

    public NodeGun(Node parent) {
      this.parent = parent;
      this.fireMode = mode.SingleFire;
      this.shootingDelay = 2;
      this.nodeSpeed = 5f;
      this.nodeRadius = 10f;
      this.steerNode = true;
      this.drawCenterNode = true;
    }

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    public mode fireMode { get; set; }
    public float nodeSpeed { get; set; }
    public float nodeRadius { get; set; }
    public int shootingDelay { get; set; }
    public bool steerNode { get; set; }
    public bool drawCenterNode { get; set; }

    public override void OnSpawn() {
      currentGroup = room.groups.general.childGroups.ElementAt(0).Value;
    }

    public override void PlayerControl(Input input) {
      if (fireMode == mode.SingleFire) {
        if (input.BtnDown(InputButtons.RightTrigger_Mouse1)) {
          if (!input.oldInputState.IsButtonDown(InputButtons.RightTrigger_Mouse1)) {
            FireNode(input.GetRightStick());
          }
          else if (steerNode && lastFired != null) {
            lastFired.body.velocity = VMath.VectorRotateLerp(lastFired.body.velocity, input.GetRightStick(), 0.02f);
          }
        }
      }
      else if (fireMode == mode.AutoFire) {
        if (input.BtnDown(InputButtons.RightTrigger_Mouse1)) {
          if (shootingDelayCount++%shootingDelay == 0) {
            FireNode(input.GetRightStick());
          }
        }
      }

      if (input.BtnClicked(InputButtons.RightBumper_E)) {
        if (room.groups.general.childGroups.Values.Count < 2) return;
        bool next = false;
        var tempGroup = room.groups.general.childGroups;
        for (int i = 0; i < tempGroup.Values.Count; i++) {
          Group g = tempGroup.Values.ElementAt(i);
          if (next) {
            currentGroup = g;
            break;
          }
          if (g == currentGroup) {
            if (i == tempGroup.Values.Count - 1) {
              currentGroup = tempGroup.Values.ElementAt(0);
              break;
            }
            next = true;
          }
        }
      }
    }

    public override void Draw() {
      if (!drawCenterNode) return;
      if (currentGroup != null && currentGroup.defaultNode != null) {
        currentGroup.defaultNode.body.pos = parent.body.pos;
        currentGroup.defaultNode.body.radius = parent.body.radius*0.7f;
        Layers templayer = currentGroup.defaultNode.basicdraw.DrawLayer;
        currentGroup.defaultNode.basicdraw.DrawLayer = parent.basicdraw.DrawLayer + 1;
        currentGroup.defaultNode.DrawSlow();
        currentGroup.defaultNode.body.radius /= 0.7f;
        currentGroup.defaultNode.basicdraw.DrawLayer = templayer;
      }
    }

    public void FireNode(Vector2 dir) {
      if (currentGroup == null) return;
      Node n = room.spawnNode(currentGroup);
      if (n == null) return;
      n.body.velocity = dir*nodeSpeed;
      n.body.pos = parent.body.pos + parent.body.velocity*Math.Sign(nodeSpeed);
      n.body.radius = nodeRadius;
      Func<Collider, Collider, bool> excludeParent = (c1, c2) => c2.parent == n;
      parent.body.ExclusionCheckResolution += excludeParent;
      Action<Node, Node> onExit = null;
      onExit = (n1, n2) => {
        parent.body.OnCollisionExit -= onExit;
        parent.body.ExclusionCheckResolution -= excludeParent;
      };
      parent.body.OnCollisionExit += onExit;
      lastFired = n;
    }
  }
}