using System;
using System.Collections.Generic;
using System.Linq;
using OrbitVR.Framework;
using OrbitVR.Interface;
using SharpDX;

namespace OrbitVR.Processes {
  public class GroupSelect : Process {
    private Vector2 groupSelectionBoxOrigin;
    public HashSet<Node> groupSelectSet;

    public GroupSelect() : base() {
      //LeftHold += LeftH;
      //LeftClick += LeftC;
      addProcessKeyAction("grouph", KeyCodes.LeftClick, OnHold: UpdateBox, OnPress: SetOrigin, OnRelease: SelectGroup);
    }

    public void UpdateBox() {
      Vector2 mousePos = UserInterface.WorldMousePos;

      float lowerx = Math.Min(mousePos.X, groupSelectionBoxOrigin.X);
      float upperx = Math.Max(mousePos.X, groupSelectionBoxOrigin.X);
      float lowery = Math.Min(mousePos.Y, groupSelectionBoxOrigin.Y);
      float uppery = Math.Max(mousePos.Y, groupSelectionBoxOrigin.Y);

      room.addRectangleLines(lowerx, lowery, upperx, uppery);
      //Console.WriteLine(mousePos.X + " " + glob.X);
    }

    public void SetOrigin() {
      groupSelectionBoxOrigin = UserInterface.WorldMousePos;
    }

    public override void Draw() {
      if (Manager.processDict.ContainsKey(typeof(GroupSelect)))
      {
        HashSet<Node> groupset = Manager.GetProcess<GroupSelect>().groupSelectSet;
        if (groupset != null)
        {
          room.TargetNodeGraphic.body.color = Color.LimeGreen;
          foreach (Node n in groupset.ToList())
          {
            room.TargetNodeGraphic.body.pos = n.body.pos;
            room.TargetNodeGraphic.body.radius = n.body.radius * 1.5f;
            room.TargetNodeGraphic.Draw();
          }
        }
      }
    }

    public void SelectGroup() {
      bool ctrlDown = UserInterface.oldKeyBState.IsKeyDown(Keys.LeftControl);
      bool altDown = UserInterface.oldKeyBState.IsKeyDown(Keys.LeftAlt);
      if (altDown) ctrlDown = false;

      Vector2 mousePos = UserInterface.WorldMousePos;

      float lowerx = Math.Min(mousePos.X, groupSelectionBoxOrigin.X);
      float upperx = Math.Max(mousePos.X, groupSelectionBoxOrigin.X);
      float lowery = Math.Min(mousePos.Y, groupSelectionBoxOrigin.Y);
      float uppery = Math.Max(mousePos.Y, groupSelectionBoxOrigin.Y);

      if (!ctrlDown && !altDown) groupSelectSet = new HashSet<Node>();


      foreach (Node n in room.masterGroup.fullSet.ToList()) {
        float xx = n.body.pos.X;
        float yy = n.body.pos.Y;

        if (xx >= lowerx && xx <= upperx
            && yy >= lowery && yy <= uppery) {
          if (altDown) {
            if (groupSelectSet.Contains(n)) groupSelectSet.Remove(n);
            else groupSelectSet.Add(n);
          }
          else {
            groupSelectSet.Add(n);
          }
        }
      }
      //System.Console.WriteLine(groupSelectSet.Count);

      room.addRectangleLines(lowerx, lowery, upperx, uppery);
    }
  }
}