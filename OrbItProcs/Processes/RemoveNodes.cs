using System;
using System.Linq;
using SharpDX;

namespace OrbItProcs {
  public class RemoveNodes : Process {
    public RemoveNodes()
      : base() {
      //LeftClick += LeftC;
      //RightClick += RightC;
      //MiddleClick += MiddleC;
      addProcessKeyAction("SingleDelete", KeyCodes.LeftClick, OnPress: SingleDel);
      addProcessKeyAction("DragDelete", KeyCodes.RightClick, OnHold: SingleDel);
      addProcessKeyAction("RemoveAll", KeyCodes.MiddleClick, OnPress: RemoveAll);
    }

    public Node FindNode(Vector2 pos) {
      Node found = null;
      float shortedDistance = Int32.MaxValue;
      for (int i = room.masterGroup.fullSet.Count - 1; i >= 0; i--) {
        Node n = (Node) room.masterGroup.fullSet.ElementAt(i);
        // find node that has been clicked, starting from the most recently placed nodes
        float distsquared = Vector2.DistanceSquared(n.body.pos, pos);
        if (distsquared < n.body.radius*n.body.radius) {
          if (distsquared < shortedDistance) {
            found = n;
            shortedDistance = distsquared;
          }
        }
      }
      return found;
    }

    public void SingleDel() {
      //if (buttonState == ButtonState.Released) return;
      Node found = FindNode(UserInterface.WorldMousePos);
      if (found != null && !found.IsPlayer && found.dataStore.Count == 0) {
        found.OnDeath(null);
      }
    }

    public void RemoveAll() {
      //TODO: AHH
      //OrbIt.ui.sidebar.btnRemoveAllNodes_Click(null, null);
    }
  }
}