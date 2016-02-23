using OrbitVR.Framework;
using OrbitVR.Interface;
using SharpDX;

namespace OrbitVR.Processes {
  public class GridSpawn : Process {
    public GridSpawn() : base() {
      addProcessKeyAction("SpawnNodeGrid", KeyCodes.LeftClick, OnPress: SpawnNodeGrid);
    }

    public void SpawnNodeGrid() {
      ///
      Vector2 MousePos = UserInterface.WorldMousePos;
      int mult = 3;
      int dx = (int) MousePos.X/(room.Level.cellWidth*mult);
      int dy = (int) MousePos.Y/(room.Level.cellHeight*mult);

      int x = (int) (dx*room.Level.cellWidth*mult + room.Level.cellWidth*(mult/2f));
      int y = (int) (dy*room.Level.cellHeight*mult + room.Level.cellHeight*(mult/2f));

      Node n = room.SpawnNode(x, y);
      n.movement.active = false;
      n.movement.pushable = false;
    }
  }
}