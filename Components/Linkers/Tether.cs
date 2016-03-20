using System.Collections.Generic;
using System.Linq;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Linkers {
  /// <summary>
  /// The position of linked nodes is controlled to stay at a particular distance from the source node.
  /// </summary>
  [Info(UserLevel.User,
    "The position of linked nodes is controlled to stay at a particular distance from the source node.", CompType)]
  public class Tether : Component, ILinkable {
    public const mtypes CompType = mtypes.draw | mtypes.affectself | mtypes.exclusiveLinker;
    private bool _activated = false;
    private HashSet<Node> _incoming = new HashSet<Node>();

    private bool _lockedAngle = false;

    private bool _lockedDistance = false;

    public int _maxdist = 300;
    public int _mindist = 100;
    private HashSet<Node> _outgoing = new HashSet<Node>();
    private Dictionary<Node, Vector2R> confiningVects;

    private Dictionary<Node, int> lockedVals;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// Determines the state of the node. If activated, it actively applies tether's effect. Middle click in select mode to toggle.
    /// </summary>
    [Info(UserLevel.Advanced,
      "Determines the state of the node. If activated, it actively applies tether's effect. Middle click in select mode to toggle."
      )]
    public bool activated {
      get { return _activated; }
      set { _activated = value; }
    }

    public HashSet<Node> outgoing {
      get { return _outgoing; }
      set { _outgoing = value; }
    }

    public HashSet<Node> incoming {
      get { return _incoming; }
      set { _incoming = value; }
    }

    /// <summary>
    /// If set, tethered nodes will not change their angle relative to the source node.
    /// </summary>
    [Info(UserLevel.User, "If set, tethered nodes will not change their angle relative to the source node.")]
    public bool lockedAngle {
      get { return _lockedAngle; }
      set {
        if (!_lockedAngle && value) Confine();
        _lockedAngle = value;
      }
    }

    /// <summary>
    /// If set, tethered nodes will not change their distance relative to the source node.
    /// </summary>
    [Info(UserLevel.User, "If set, tethered nodes will not change their distance relative to the source node.")]
    public bool lockedDistance {
      get { return _lockedDistance; }
      set {
        if (!_lockedDistance && value) Lock();
        _lockedDistance = value;
      }
    }

    /// <summary>
    /// Keeps tethered nodes within this distance from the source node.
    /// </summary>
    [Info(UserLevel.User, "Keeps tethered nodes within this distance from the source node.")]
    public int maxdist {
      get { return _maxdist; }
      set {
        _maxdist = value;
        if (_maxdist < _mindist) _maxdist = _mindist;
      }
    }

    /// <summary>
    /// Keeps tethered nodes at least this distance away from the source node.
    /// </summary>
    [Info(UserLevel.User, "Keeps tethered nodes at least this distance away from the source node.")]
    public int mindist {
      get { return _mindist; }
      set {
        _mindist = value;
        if (_mindist > _maxdist) _mindist = _maxdist;
      }
    }

    public Tether() : this(null) {}

    public Tether(Node parent = null) {
      activated = true;
      if (parent != null) {
        this.parent = parent;
      }
    }

    [Info(UserLevel.Developer)]
    public Link link { get; set; }

    public override bool active {
      get { return _active; }
      set {
        _active = value;
        if (!_active) {
          foreach (Node other in outgoing.ToList()) {
            outgoing.Remove(other);
            other.Comp<Tether>().incoming.Remove(parent);
          }
          foreach (Node other in incoming.ToList()) {
            other.Comp<Tether>().outgoing.Remove(parent);
            incoming.Remove(other);
          }
        }
      }
    }

    public override void AffectOther(Node other) // called when used as a link
    {
      if (!active || !activated) {
        return;
      }
      Vector2R diff = other.body.pos - parent.body.pos;
      float len;
      if (lockedDistance) {
        len = lockedVals[other];
      }
      else {
        len = diff.Length();
      }

      if (len > maxdist) {
        if (lockedAngle) {
          other.body.pos = parent.body.pos + confiningVects[other]*maxdist;
        }
        else {
          float percent = maxdist/len;
          diff *= percent;
          other.body.pos = parent.body.pos + diff;
        }
      }
      else if (len < mindist) {
        if (lockedAngle) {
          other.body.pos = parent.body.pos + confiningVects[other]*mindist;
        }
        else {
          float percent = mindist/len;
          diff *= percent;
          other.body.pos = parent.body.pos + diff;
        }
      }
      else {
        if (lockedAngle) {
          other.body.pos = parent.body.pos + confiningVects[other]*len;
          //Console.WriteLine("{0}, {1}, {2}", confiningVects[other], other.transform.position, len);
        }
      }
      //diff = other.transform.position - parent.transform.position;
      //Console.WriteLine(diff.Length());
    }

    public override void Draw() {
      Color col;
      if (activated)
        col = Color.Blue;
      else
        col = Color.White;
      room.Camera.Draw(parent.body.texture, parent.body.pos, col, parent.body.scale*1.2f, (int)Layers.Under2);

      foreach (Node receiver in outgoing) {
        Vector2R diff = receiver.body.pos - parent.body.pos;
        Vector2R perp = new Vector2R(diff.Y, -diff.X);
        VMath.NormalizeSafe(ref perp);
        perp *= 2;

        room.Camera.DrawLine(parent.body.pos, receiver.body.pos, 2f, col, (int)Layers.Under3);

        room.Camera.DrawLine(parent.body.pos + perp, receiver.body.pos + perp, 2f, Color.Red, (int)Layers.Under3);
        room.Camera.DrawLine(parent.body.pos - perp, receiver.body.pos - perp, 2f, Color.Green, (int)Layers.Under3);

        perp *= 20;

        Vector2R center = (receiver.body.pos + parent.body.pos)/2;


        Vector2R point = receiver.body.pos - (diff/5);
        room.Camera.DrawLine(point + perp, receiver.body.pos, 2f, col, (int)Layers.Under3);
        room.Camera.DrawLine(point - perp, receiver.body.pos, 2f, col, (int)Layers.Under3);
      }
    }

    public override void InitializeLists() {
      outgoing = new HashSet<Node>();
      incoming = new HashSet<Node>();
    }

    public override void AffectSelf() {
      if (active && activated) {
        foreach (Node other in outgoing) {
          Vector2R diff = other.body.pos - parent.body.pos;
          float len;
          if (lockedDistance) {
            len = lockedVals[other];
          }
          else {
            len = diff.Length();
          }

          if (len > maxdist) {
            if (lockedAngle) {
              other.body.pos = parent.body.pos + confiningVects[other]*maxdist;
            }
            else {
              float percent;
              if (lockedDistance) percent = len/diff.Length();
              else percent = maxdist/len;
              diff *= percent;
              other.body.pos = parent.body.pos + diff;
            }
          }
          else if (len < mindist) {
            if (lockedAngle) {
              other.body.pos = parent.body.pos + confiningVects[other]*mindist;
            }
            else {
              float percent;
              if (lockedDistance) percent = len/diff.Length();
              else percent = mindist/len;
              diff *= percent;
              other.body.pos = parent.body.pos + diff;
            }
          }
          else {
            if (lockedAngle) {
              other.body.pos = parent.body.pos + confiningVects[other]*len;
              //Console.WriteLine("{0}, {1}, {2}", confiningVects[other], other.transform.position, len);
            }
            if (lockedDistance) {
              float percent = len/diff.Length();
              //else percent = mindist / len;
              diff *= percent;
              other.body.pos = parent.body.pos + diff;
            }
          }


          //diff = other.transform.position - parent.transform.position;
          //Console.WriteLine(diff.Length());
        }
      }
    }

    public void Confine() {
      if (parent == null) return;
      confiningVects = new Dictionary<Node, Vector2R>();
      foreach (Node other in outgoing.ToList()) {
        Vector2R len = other.body.pos - parent.body.pos;
        VMath.NormalizeSafe(ref len);
        confiningVects[other] = len;
      }
    }

    public void Lock() {
      if (parent == null) return;
      lockedVals = new Dictionary<Node, int>();
      foreach (Node other in outgoing.ToList()) {
        Vector2R len = other.body.pos - parent.body.pos;
        lockedVals[other] = (int) len.Length();
      }
    }

    public void AddToOutgoing(Node node) {
      if (node != parent && node.HasComp<Tether>()) {
        if (outgoing.Contains(node)) {
          outgoing.Remove(node);
          node.Comp<Tether>().incoming.Remove(parent);
          if (lockedAngle && confiningVects.ContainsKey(node)) confiningVects.Remove(node);
          if (lockedDistance && lockedVals.ContainsKey(node)) lockedVals.Remove(node);
        }
        else {
          outgoing.Add(node);
          node.Comp<Tether>().incoming.Add(parent);
          if (lockedAngle && !confiningVects.ContainsKey(node)) {
            Vector2R v = (node.body.pos - parent.body.pos);
            VMath.NormalizeSafe(ref v);
            confiningVects.Add(node, v);
          }
          if (lockedDistance && !lockedVals.ContainsKey(node))
            lockedVals.Add(node, (int) (node.body.pos - parent.body.pos).Length());
        }
      }
    }
  }
}