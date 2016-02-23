using System.Collections.Generic;
using OrbitVR.Components.Linkers;
using OrbitVR.Framework;
using OrbitVR.Interface;
using SharpDX;

namespace OrbitVR.Components.AffectOthers {
  /// <summary>
  /// The follow component causes this node to follow other nodes. If it is following two nodes, it will go in the average direction of the two.
  /// </summary>
  [Info(UserLevel.User,
    "The follow component causes this node to follow other nodes. If it is following two nodes, it will go in the average direction of the two.",
    CompType)]
  public class Follow : Component, ILinkable {
    public enum followMode {
      FollowNone,
      FollowAll,
      FollowNearest,
      //FollowTarget,
    }

    public enum leadMode {
      LeadNone,
      LeadAll,
      LeadNearest,
    }

    public const mtypes CompType = mtypes.affectother | mtypes.affectself;
    List<Vector2> directions = new List<Vector2>();
    float nearestDistSqrd = float.MaxValue;
    Node nearestNode = null;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// The radius is the reach of the follow component, deciding how far a node can be to be followable.
    /// </summary>
    [Info(UserLevel.User,
      "The radius is the reach of the follow component, deciding how far a node can be to be followable.")]
    public float radius { get; set; }

    /// <summary>
    /// If enabled, the node will flee from others rather than follow.
    /// </summary>
    [Info(UserLevel.User, "If enabled, the node will flee from others rather than follow.")]
    public bool flee { get; set; }

    public float LerpPercent { get; set; }
    public followMode FollowMode { get; set; }
    public leadMode LeadMode { get; set; }
    public Follow() : this(null) {}

    public Follow(Node parent) {
      this.parent = parent;
      radius = 600;
      flee = false;
      LerpPercent = 10f;
      FollowMode = followMode.FollowNearest;
      LeadMode = leadMode.LeadNone;
    }

    public Link link { get; set; }

    public override void AffectOther(Node other) {
      Vector2 dir = other.body.pos - parent.body.pos;
      if (link != null) {
        if (FollowMode != followMode.FollowNone) {
          TurnTowardsDirection(parent, dir, flee, LerpPercent);
        }
        if (LeadMode != leadMode.LeadNone) {
          TurnTowardsDirection(other, dir, !flee, LerpPercent);
        }
        return;
      }

      float distSquared = dir.LengthSquared();
      if (distSquared > radius*radius) return;
      if (FollowMode == followMode.FollowNearest) {
        if (distSquared < nearestDistSqrd) {
          nearestDistSqrd = distSquared;
          nearestNode = other;
        }
      }
      else if (FollowMode == followMode.FollowAll) {
        directions.Add(dir.NormalizeSafe());
      }
      if (LeadMode == leadMode.LeadAll) {
        TurnTowardsDirection(other, dir, !flee, LerpPercent);
      }
      else if (LeadMode == leadMode.LeadNearest) {
        if (distSquared < nearestDistSqrd) {
          nearestDistSqrd = distSquared;
          nearestNode = other;
        }
      }
    }

    public override void AffectSelf() {
      if (FollowMode == followMode.FollowNearest) {
        if (nearestDistSqrd == float.MaxValue || nearestNode == null) return;
        TurnTowardsDirection(parent, nearestNode.body.pos - parent.body.pos, flee, LerpPercent);
        if (LeadMode != leadMode.LeadNearest) {
          nearestDistSqrd = float.MaxValue;
          nearestNode = null;
        }
      }
      else if (FollowMode == followMode.FollowAll) {
        if (directions.Count == 0) return;
        Vector2 result = new Vector2();
        foreach (Vector2 dir in directions) {
          result += dir;
        }
        TurnTowardsDirection(parent, result, flee, LerpPercent);
        directions = new List<Vector2>();
      }
      if (LeadMode == leadMode.LeadNearest) {
        if (nearestDistSqrd == float.MaxValue || nearestNode == null) return;
        TurnTowardsDirection(nearestNode, nearestNode.body.pos - parent.body.pos, !flee, LerpPercent);
        nearestDistSqrd = float.MaxValue;
        nearestNode = null;
      }
    }

    public static void TurnTowardsDirection(Node node, Vector2 direction, bool flip, float lerpPercent) {
      if (direction == Vector2.Zero) return;
      if (flip) direction *= new Vector2(-1, -1);
      float oldAngle = VMath.VectorToAngle(node.body.velocity);
      float newAngle = VMath.VectorToAngle(direction);
      float lerpedAngle = GMath.AngleLerp(oldAngle, newAngle, lerpPercent/100f);
      Vector2 finalDir = VMath.AngleToVector(lerpedAngle);
      node.body.velocity = VMath.Redirect(node.body.velocity, finalDir);
    }
  }
}