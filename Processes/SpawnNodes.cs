using System;
using System.Collections.Generic;
using SharpDX;

namespace OrbItProcs {
  public class SpawnNodes : Process {
    int rightClickCount = 0; //
    int rightClickMax = 1; //
    private Vector2 spawnPos;

    public SpawnNodes() : base() {
      //             List<int> a = new List<int>();
      //             List<int> b = new List<int>();
      //             a.

      batchSpawnNum = 2;
      radiusRange = new Toggle<float>(10f, true);
      radiusCenter = 15f;

      addProcessKeyAction("SpawnNode", KeyCodes.LeftClick, OnPress: SpawnNode);
      //addProcessKeyAction("SetSpawnPosition", KeyCodes.LeftShift, OnPress: SetSpawnPosition);
      addProcessKeyAction("BatchSpawn", KeyCodes.RightClick, OnHold: BatchSpawn);
      //addProcessKeyAction("DirectionalLaunch", KeyCodes.LeftShift, KeyCodes.RightClick, OnHold: DirectionalLaunch);
      
    }

    public int batchSpawnNum { get; set; }

    public Toggle<float> radiusRange { get; set; }
    public float radiusCenter { get; set; }

    public void SetRadius(Node n) {
      n.body.radius = radiusCenter;
      if (radiusRange.enabled) {
        n.body.radius = (float) Utils.random.NextDouble()*radiusRange.value - (radiusRange.value/2) + radiusCenter;
      }
    }

    public void SpawnNode() {
      SetRadius(room.spawnNode((int) UserInterface.WorldMousePos.X, (int) UserInterface.WorldMousePos.Y));
    }
    

    public void SetSpawnPosition() {
      spawnPos = UserInterface.WorldMousePos;
    }

    public void BatchSpawn() {
      int rad = 100;
      for (int i = 0; i < batchSpawnNum; i++) {
        int rx = Utils.random.Next(rad*2) - rad;
        int ry = Utils.random.Next(rad*2) - rad;
        SetRadius(room.spawnNode((int) UserInterface.WorldMousePos.X + rx, (int) UserInterface.WorldMousePos.Y + ry));
      }
    }

    public void DirectionalLaunch() {
      rightClickCount++;
      if (rightClickCount%rightClickMax == 0) {
        //Vector2 positionToSpawn = new Vector2(Game1.sWidth, Game1.sHeight);
        Vector2 positionToSpawn = spawnPos;
        //positionToSpawn /= (game.room.mapzoom * 2);
        //positionToSpawn /= (2);
        Vector2 diff = UserInterface.WorldMousePos;
        //diff *= room.zoom;
        diff = diff - positionToSpawn;
        //diff.Normalize();

        //new node(s)
        Dictionary<dynamic, dynamic> userP = new Dictionary<dynamic, dynamic>() {
          {typeof (Lifetime), true},
          {nodeE.position, positionToSpawn},
          {nodeE.velocity, diff},
        };

        if (UserInterface.oldKeyBState.IsKeyDown(Keys.LeftControl)) {
          Action<Node> after = delegate(Node n) {
                                 n.body.velocity = diff;
                                 if (n.body.velocity.IsFucked()) System.Diagnostics.Debugger.Break();
                               };
          SetRadius(room.spawnNode(userP, after));
        }
        else {
          SetRadius(room.spawnNode(userP));
        }
        rightClickCount = 0;
      }
    }
  }
}