﻿using System;
using System.Linq;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Processes {
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

    public Node FindNode(Vector2R pos) {
      Node found = null;
      float shortedDistance = Int32.MaxValue;
      for (int i = room.MasterGroup.fullSet.Count - 1; i >= 0; i--) {
        Node n = (Node) room.MasterGroup.fullSet.ElementAt(i);
        // find node that has been clicked, starting from the most recently placed nodes
        float distsquared = Vector2R.DistanceSquared(n.body.pos, pos);
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