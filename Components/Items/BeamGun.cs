using System;
using System.Collections.Generic;
using OrbitVR.Framework;
using OrbitVR.Physics;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Items {
  /// <summary>
  /// The beam gun allows you to shoot out straight beams that can damage or heal targets ahead.
  /// </summary>
  [Info(UserLevel.User, "The beam gun allows you to shoot out straight beams that can damage or heal targets ahead.",
    CompType)]
  public class BeamGun : Component {
    public const mtypes CompType = mtypes.playercontrol | mtypes.item;
    Queue<Node> currentlyColliding = new Queue<Node>();

    private float minLength, currentLength;
    Vector2R stick = Vector2R.Zero;
    private int tempColorVal = 0;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    public Node beamNode { get; set; }
    public bool goThrough { get; set; }
    public float maxLength { get; set; }
    public bool growing { get; set; }
    public float growthRate { get; set; }
    public int beamThickness { get; set; }
    public int maxColorVal { get; set; }
    public int colorValSpeed { get; set; }
    public BeamGun() : this(null) {}

    public BeamGun(Node parent) {
      this.parent = parent;
      goThrough = false;
      growing = true;
      maxLength = 300f;
      beamThickness = 20;
      growthRate = 10f;
      minLength = 15f;
      currentLength = minLength;
      maxColorVal = 5;
      colorValSpeed = 2;
    }

    public override void OnSpawn() {
      beamNode = Node.ContructLineWall(room, parent.body.pos, parent.body.pos, beamThickness, addToWallGroup: false);
      beamNode.body.ExclusionCheck += (a, b) => b.parent == parent;
      beamNode.body.isSolid = false;
      room.Groups.Walls.IncludeEntity(beamNode);
      beamNode.body.OnCollisionStay += OnBeamCollision;
      beamNode.active = false;
    }

    public override void PlayerControl(Input input) {
      stick = input.GetRightStick().toV2R();
      if (input.BtnDown(InputButtons.RightTrigger_Mouse1)) {
        if (!beamNode.active) beamNode.active = true;
        if (currentlyColliding.Count != 0) {
          if (goThrough) {
            int count = currentlyColliding.Count;
            for (int i = 0; i < count; i++) {
              ApplyEffect(currentlyColliding.Dequeue());
            }
            ResizeBeam(maxLength);
          }
          else {
            int count = currentlyColliding.Count;
            float closest = float.MaxValue;
            Node closestNode = null;
            for (int i = 0; i < count; i++) {
              Node n = currentlyColliding.Dequeue();
              float dist = (n.body.pos - parent.body.pos).Length();
              if (dist < closest) {
                closest = dist;
                closestNode = n;
              }
            }
            currentLength = closest - closestNode.body.radius*0.9f; //only support for circles for now
            if (currentLength > maxLength) currentLength = maxLength;
            ResizeBeam(currentLength);
            ApplyEffect(closestNode);
          }
        }
        else {
          if (growing) {
            currentLength = Math.Min(maxLength, currentLength + growthRate);
            ResizeBeam(currentLength);
          }
          else {
            ResizeBeam(maxLength);
          }
        }
      }
      else {
        if (beamNode.active) beamNode.active = false;
        currentLength = minLength;
      }
    }

    public void ApplyEffect(Node other) {
      other.body.velocity += stick;
    }

    public void ResizeBeam(float size) {
      float halfheight = (int) (size/2f);
      float halfwidth = beamThickness/2f;
      float angle = VMath.VectorToAngle(stick);
      Polygon p = (Polygon) beamNode.body.shape;
      p.SetBox(halfwidth, halfheight, false);
      Vector2R endpos = parent.body.pos + (stick.NormalizeSafe()*halfheight);
      beamNode.body.pos = endpos;
      p.SetOrient(angle);

      Vector2R normal = new Vector2R(-stick.Y, stick.X).NormalizeSafe();
      if (normal == Vector2R.Zero) return;
      Color c = Color.Blue;
      int r = Math.Sign(128 - c.R)*10;
      int g = Math.Sign(128 - c.G)*10;
      int b = Math.Sign(128 - c.B)*10;
      for (int i = 0; i < beamThickness; i++) {
        Vector2R offset = normal*(i - beamThickness/2);
        Vector2R start = parent.body.pos + offset;
        Vector2R end = start + (stick.NormalizeSafe()*size);
        int seed = (int) DelegateManager.Triangle((i + tempColorVal), maxColorVal);
        Color newcol = new Color(c.R + r*seed, c.G + g*seed, c.B + g*seed); // *0.5f;
        room.Camera.DrawLine(start, end, 1f, newcol, (int)Layers.Under2);
      }
      tempColorVal += colorValSpeed;
    }

    public void OnBeamCollision(Node beam, Node other) {
      if (other.body.isSolid) {
        currentlyColliding.Enqueue(other);
      }
    }
  }
}