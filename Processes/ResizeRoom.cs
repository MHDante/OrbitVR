using OrbitVR.Framework;
using OrbitVR.Interface;
using SharpDX;

namespace OrbitVR.Processes {
  public class ResizeRoom : Process {
    public ResizeRoom()
      : base() {
      addProcessKeyAction("Resize", KeyCodes.LeftClick, OnPress: resize);
    }

    public void resize() {
      Vector2 pos = UserInterface.WorldMousePos;
      if (pos.X < 0 || pos.Y < 0) return;
      OrbIt.Game.Room.Resize(pos);
    }

    public override void Update() {
      Vector2 pos = UserInterface.WorldMousePos;
      if (pos.X < 0 || pos.Y < 0) return;

      int minX = 0;
      int minY = 0;
      int maxX = (int) pos.X;
      int maxY = (int) pos.Y;

      Color c = Color.Green;
      room.Camera.DrawLine(new Vector2(minX, minY), new Vector2(minX, maxY), 2f, c, Layers.Over5);
      room.Camera.DrawLine(new Vector2(minX, minY), new Vector2(maxX, minY), 2f, c, Layers.Over5);
      room.Camera.DrawLine(new Vector2(maxX, maxY), new Vector2(minX, maxY), 2f, c, Layers.Over5);
      room.Camera.DrawLine(new Vector2(maxX, maxY), new Vector2(maxX, minY), 2f, c, Layers.Over5);
    }
  }
}