using System;
using System.Collections.Generic;
using System.Linq;
using OrbitVR.Components.Movement;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Processes {
  class FloodFill : Process {
    static Dictionary<Vector2, List<Node>> spawnedNodes = new Dictionary<Vector2, List<Node>>();
    static Dictionary<Vector2, int[]> spawnPoints = new Dictionary<Vector2, int[]>();
    public static Action afterFilling;

    static Action<Node, Node> add = delegate(Node Source, Node Dest) {
                                      if (Source.dataStore.Keys.Contains("Filling") &&
                                          Dest.dataStore.Keys.Contains("Filling"))
                                        spawnPoints[Source.CheckData<Vector2>("Filling")][1]++;
                                    };

    static Action<Node, Node> rem = delegate(Node Source, Node Dest) {
                                      if (Source.dataStore.Keys.Contains("Filling") &&
                                          Dest.dataStore.Keys.Contains("Filling"))
                                        spawnPoints[Source.CheckData<Vector2>("Filling")][1]--;
                                    };

    public FloodFill()
      : base() {
      addProcessKeyAction("Fill", KeyCodes.LeftClick, OnPress: mouseFill);
    }

    static public void mouseFill() {
      startFill(UserInterface.WorldMousePos);
    }

    static public void startFill(Vector2 pos) {
      if (!spawnPoints.Keys.Contains(pos)) spawnPoints[pos] = new int[2];
    }

    static public void boulderFountain() {
      List<Vector2> removeList = new List<Vector2>();
      foreach (var v in spawnPoints.Keys) {
        int Spawned = spawnPoints[v][0];
        int cols = spawnPoints[v][1]/3;
        Console.WriteLine(Spawned + " <> " + cols);
        if (Spawned >= cols - 10) {
          float ratio = (float) Utils.random.NextDouble() + .5f;

          Dictionary<dynamic, dynamic> standardDictionary = new Dictionary<dynamic, dynamic>() {
            {nodeE.position, v},
            {typeof (Friction), true},
            {nodeE.radius, 25f*(ratio)},
            //{ typeof(gravity, true },
          };
          spawnPoints[v][0]++;
          Node n = OrbIt.Game.Room.SpawnNode(standardDictionary);
          spawnedNodes.GetOrAdd(v).Add(n);
          n.SetData("Filling", v);
          n.body.OnCollisionEnter += add;
          n.body.OnCollisionExit += rem;
        }
        else {
          removeList.Add(v);
        }
      }
      foreach (var v in removeList) {
        finish(v);
      }
    }

    static private void finish(Vector2 v) {
      spawnPoints.Remove(v);
      foreach (var n in spawnedNodes[v]) {
        n.clearData("Filling");
        n.body.OnCollisionEnter -= add;
        n.body.OnCollisionExit -= rem;
      }
      if (spawnPoints.Count == 0 && afterFilling != null) {
        afterFilling();
        afterFilling = null;
      }
    }
  }
}