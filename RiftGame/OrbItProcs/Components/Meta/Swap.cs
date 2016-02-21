using System;
using SharpDX;

namespace OrbItProcs {
  public class Swap : Component {
    public Action<Node> OnSwapAfter;
    public Action<Node> OnSwapBefore;
    public Swap() : this(null) {}

    public Swap(Node parent = null) {
      if (parent != null) this.parent = parent;
    }

    public void SwitchPlayerNode(Node n1, Node n2) {
      if (!n1.IsPlayer && !n2.IsPlayer) {
        return;
      }
      else if (n1.IsPlayer && n2.IsPlayer) {
        BigTonyData data1 = n1.player.Data<BigTonyData>();
        BigTonyData data2 = n2.player.Data<BigTonyData>();
        if (data1 == null || data2 == null) return;
        if (!data1.switchAvailable || !data2.switchAvailable) return;
        n1.player.node.body.color = n2.player.node.body.color;
        Node temp = n1.player.node;
        n1.player.node.collision.SwapCollider(n2.player.node, "trigger");

        n1.player.node = n2.player.node;
        n1.player.node.body.color = n1.player.pColor;
        n2.player.node = temp;

        data1.switchAvailable = false;
        data2.switchAvailable = false;
        n1.room.scheduler.doAfterXMilliseconds(nn => data1.switchAvailable = true, 1000);
        n1.room.scheduler.doAfterXMilliseconds(nn => data2.switchAvailable = true, 1000);
      }
      else {
        Player p1;
        Node n;
        if (n1.IsPlayer && !n2.IsPlayer) {
          p1 = n1.player;
          n = n2;
        }
        else if (!n1.IsPlayer && n2.IsPlayer) {
          p1 = n2.player;
          n = n1;
        }
        else {
          p1 = null;
          n = null;
        }
        p1.node.body.color = Color.White;
        p1.node.body.texture = textures.whitecircle;
        if (OnSwapBefore != null) {
          OnSwapBefore(p1.node);
        }
        //if (p1.node != bigtony) p1.node.body.mass = smallmass;
        p1.node.collision.SwapCollider(n, "trigger");

        Collider col = p1.node.collision.GetCollider("trigger");
        if (col != null) col.HandlersEnabled = false;

        Collider col2 = n.collision.GetCollider("trigger");
        if (col2 != null) col2.HandlersEnabled = true;

        p1.node = n;
        if (OnSwapAfter != null) {
          OnSwapAfter(p1.node);
        }
        //if (p1.node != bigtony) p1.node.body.mass = bigmass;

        p1.node.body.texture = textures.blackorb;
        p1.node.body.color = p1.pColor;
        p1.node.room.scheduler.doAfterXMilliseconds(nn => p1.Data<BigTonyData>().switchAvailable = true, 1000);
      }
      //don't switch if both are nodes
    }
  }
}