using OrbitVR.Components.Essential;
using OrbitVR.Framework;
using OrbitVR.UI;

namespace OrbitVR.Components.Linkers {
  /// <summary>
  /// Affected nodes are not only driven by their own velocity, but are also subject to this nodes' velocity.
  /// </summary>
  [Info(UserLevel.Advanced,
    "Affected nodes are not only driven by their own velocity, but are also subject to this nodes' velocity.", CompType)
  ]
  public class RelativeMotion : Component, ILinkable {
    public const mtypes CompType = mtypes.affectself;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    //public int _maxdist = 300;
    //public int maxdist { get { return _maxdist; } set { _maxdist = value; if (_maxdist < _mindist) _maxdist = _mindist; } }
    //public int _mindist = 100;
    //public int mindist { get { return _mindist; } set { _mindist = value; if (_mindist > _maxdist) _mindist = _maxdist; } }

    public RelativeMotion() : this(null) {}

    public RelativeMotion(Node parent = null) {
      if (parent != null) {
        this.parent = parent;
      }
    }

    public Link link { get; set; }

    public override void AffectOther(Node other) // called when used as a link
    {
      //other.transform.position += parent.transform.velocity;
      other.body.pos += parent.body.effvelocity;

      other.movement.mode = movemode.free;
    }
  }
}