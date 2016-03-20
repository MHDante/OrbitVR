using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Processes {
  public class ResizeRoom : Process {
    public ResizeRoom()
      : base() {
      addProcessKeyAction("Resize", KeyCodes.LeftClick, OnPress: resize);
    }

    public void resize() {
      Vector2R pos = UserInterface.WorldMousePos;
      if (pos.X < 0 || pos.Y < 0) return;
      OrbIt.Game.Room.Resize(pos);
    }

    public override void Update() {
      Vector2R pos = UserInterface.WorldMousePos;
      if (pos.X < 0 || pos.Y < 0) return;

      int minX = 0;
      int minY = 0;
      int maxX = (int) pos.X;
      int maxY = (int) pos.Y;

      Color c = Color.Green;
      room.Camera.DrawLine(new Vector2R(minX, minY), new Vector2R(minX, maxY), 2f, c, (int)Layers.Over5);
      room.Camera.DrawLine(new Vector2R(minX, minY), new Vector2R(maxX, minY), 2f, c, (int)Layers.Over5);
      room.Camera.DrawLine(new Vector2R(maxX, maxY), new Vector2R(minX, maxY), 2f, c, (int)Layers.Over5);
      room.Camera.DrawLine(new Vector2R(maxX, maxY), new Vector2R(maxX, minY), 2f, c, (int)Layers.Over5);
    }
  }
}