using System.Collections.Generic;
using OrbitVR.Framework;
using OrbitVR.Physics;
using SharpDX;

namespace OrbitVR.Components.Meta {
  public class Diode : Component {
    public enum Mode {
      firstBlocked,
      secondBlocked,
      bothBlocked,
      neitherBlocked,
    }

    public const mtypes CompType = mtypes.affectself;
    public Vector2 end;
    HashSet<Node> goingThrough = new HashSet<Node>();

    public Vector2 start;
    private int usedTickets;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    public bool semaphore { get; set; }
    public int maxTickets { get; set; }
    public bool PlayersOnly { get; set; }
    public Mode mode { get; set; }
    public Diode() : this(null) {}

    public Diode(Node parent) {
      this.parent = parent;
      mode = Mode.neitherBlocked;
      maxTickets = 4;
      usedTickets = 0;
      semaphore = false;
      PlayersOnly = true;
    }

    public override void OnSpawn() {
      parent.body.OnCollisionEnter += delegate(Node source, Node dest) {
                                        if (PlayersOnly) {
                                          if (!dest.IsPlayer) return;
                                        }
                                        bool happened = IsOnCorrectSide(parent, dest, true);
                                        if (happened) {
                                          if (semaphore) {
                                            if (usedTickets <= maxTickets) {
                                              goingThrough.Add(dest);
                                              usedTickets++;
                                              return;
                                            }
                                          }
                                          goingThrough.Add(dest);
                                        }
                                      };
      parent.body.OnCollisionExit += delegate(Node source, Node dest) {
                                       if (PlayersOnly) {
                                         if (!dest.IsPlayer) return;
                                       }
                                       bool happened = IsOnCorrectSide(parent, dest, true);
                                       if (semaphore && happened && goingThrough.Contains(dest) &&
                                           usedTickets <= maxTickets) {
                                         usedTickets--;
                                       }
                                       goingThrough.Remove(dest);
                                     };
      parent.body.ExclusionCheckResolution += delegate(Collider c1, Collider c2) {
                                                if (PlayersOnly) {
                                                  if (!c2.parent.IsPlayer) return true;
                                                }
                                                if (semaphore && usedTickets > maxTickets) return false;

                                                return goingThrough.Contains(c2.parent) ||
                                                       IsOnCorrectSide(parent, c2.parent, true);
                                              };
    }

    public override void AffectSelf() {
      if (!semaphore) {
        parent.SetColor(Color.White);
        return;
      }
      if (maxTickets - usedTickets <= 0) {
        parent.SetColor(Color.Red);
      }
      else if (maxTickets - usedTickets == 1) {
        parent.SetColor(Color.Goldenrod);
      }
      else if (maxTickets - usedTickets == 2) {
        parent.SetColor(Color.Yellow);
      }
      else if (maxTickets - usedTickets >= 3) {
        parent.SetColor(Color.Green);
      }
    }

    public bool IsOnCorrectSide(Node wall, Node other, bool belowPi) {
      Vector2 direction = other.body.pos - wall.body.pos;
      float dirAngle = VMath.VectorToAngle(direction);
      float resAngle = (parent.body.orient - dirAngle).between0and2pi();
      if (belowPi) return resAngle < GMath.PI;
      return resAngle >= GMath.PI;
    }
  }
}