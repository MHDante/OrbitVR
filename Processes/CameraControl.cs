﻿using System;
using OrbitVR.Framework;
using OrbitVR.UI;

namespace OrbitVR.Processes {
  public class CameraControl : Process {
    public float velocity = 20f;

    public Toggle<Node> targetedNode { get; set; }

    public CameraControl() {
      targetedNode = new Toggle<Node>(null, false);
      addProcessKeyAction("targetNode", KeyCodes.LeftShift, KeyCodes.LeftClick, OnPress: TargetNode);
    }

    public void TargetNode() {
      Node n = room.SelectNodeAt(UserInterface.WorldMousePos);
      if (n == null) {
        targetedNode.enabled = false;
        return;
      }
      targetedNode.enabled = true;
      targetedNode.value = n;
    }

    public override void Update() {
      if (targetedNode.enabled) {
        if (targetedNode.value == null) {
          Console.WriteLine("CAMERACONTROL: We've found it. We've finally found null.");
        }
        else {
          room.Camera.pos = targetedNode.value.body.pos;
          return;
        }
      }

      KeyboardState state = KeyManager.newKeyboardState;
      KeyboardState oldstate = KeyManager.oldKeyboardState;
      bool up = state.IsKeyDown(Keys.Up);
      bool down = state.IsKeyDown(Keys.Down);
      bool left = state.IsKeyDown(Keys.Left);
      bool right = state.IsKeyDown(Keys.Right);

      Stick s = new Stick(up, down, left, right);
      room.Camera.pos += s.v2r*velocity;
    }
  }
}