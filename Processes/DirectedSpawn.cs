﻿using System;
using System.Collections.Generic;
using OrbitVR.Components.Meta;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Processes {
  public class DirectedSpawn : Process {
    int rightClickCount = 0; //
    int rightClickMax = 1; //
    private Vector2R spawnPos;

    public DirectedSpawn() : base() {
      addProcessKeyAction("DirectedLaunch", KeyCodes.LeftClick, OnHold: DirectedLaunch);
      addProcessKeyAction("SetSpawnPosition", KeyCodes.RightClick, OnPress: SetSpawnPosition,
                          OnRelease: UnsetSpawnPosition);
    }

    public void SetSpawnPosition() {
      spawnPos = UserInterface.WorldMousePos;
    }

    public void UnsetSpawnPosition() {
      spawnPos = Vector2R.Zero;
    }

    public void DirectedLaunch() {
      if (spawnPos == Vector2R.Zero) return;
      rightClickCount++;
      if (rightClickCount%rightClickMax == 0) {
        Vector2R positionToSpawn = spawnPos;
        Vector2R diff = UserInterface.WorldMousePos;
        diff = diff - positionToSpawn;
        Dictionary<dynamic, dynamic> userP = new Dictionary<dynamic, dynamic>() {
          {typeof (Lifetime), true},
          {nodeE.position, positionToSpawn},
          {nodeE.velocity, diff},
        };
        Action<Node> after = delegate(Node n) { n.body.velocity = diff; };
        room.SpawnNode(userP, after);
        rightClickCount = 0;
      }
    }
  }
}