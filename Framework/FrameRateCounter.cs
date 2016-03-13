using System;
using SharpDX;

namespace OrbitVR.Framework {
  public class FrameRateCounter {
    public TimeSpan elapsedTime = TimeSpan.Zero;
    int frameCounter = 0;
    int frameRate = 0;
    OrbIt game;
    public int updateCounter = 0;
    int updateRate = 0;


    public FrameRateCounter(OrbIt game) {
      //content = new ContentManager(game.Services);
      this.game = game;
    }

    public void Update() {
      elapsedTime += OrbIt.Game.Time.ElapsedGameTime;
      updateCounter++;

      if (elapsedTime > TimeSpan.FromSeconds(1)) {
        elapsedTime -= TimeSpan.FromSeconds(1);
        frameRate = frameCounter;
        frameCounter = 0;
        updateRate = updateCounter;
        updateCounter = 0;
      }
      Draw();
    }

    public void UpdateElapsed(TimeSpan elapsed) {
      elapsedTime += elapsed;
      updateCounter++;
      if (elapsedTime > TimeSpan.FromSeconds(1)) {
        elapsedTime -= TimeSpan.FromSeconds(1);
        frameRate = frameCounter;
        frameCounter = 0;
        updateRate = updateCounter;
        updateCounter = 0;
      }
    }


    public void Draw() {
      frameCounter++;
      int y1 = 70;

      string fps = string.Format("fps: {0}", frameRate);
      string ups = string.Format("ups: {0}", updateRate);
      string process = "";
      //string fpsups = string.Format("fps:{0} ups:{1}", frameRate, updateRate);
      Room room = OrbIt.Game.Room;
      bool hasProcess = room != null && OrbIt.UI.keyManager.TemporaryProcess != null;
      if (hasProcess) {
        y1 += 30;
        process = OrbIt.UI.keyManager.TemporaryProcess.GetType().ToString().LastWord('.');
      }

      room.Camera.DrawStringScreen(fps, new Vector2(0, OrbIt.ScreenHeight - y1), Color.Black);
      y1 -= 30;
      room.Camera.DrawStringScreen(ups, new Vector2(0, OrbIt.ScreenHeight - y1), Color.Black);
      y1 -= 30;
      if (hasProcess) room.Camera.DrawStringScreen(process, new Vector2(0, OrbIt.ScreenHeight - y1), Color.Black);

      if (room.MasterGroup != null) {
        string count = room.Groups.General.fullSet.Count.ToString();
        int x = OrbIt.ScreenWidth - (count.Length*7) - 20;
        room.Camera.DrawStringScreen(count, new Vector2(x, OrbIt.ScreenHeight - y1), Color.Black, offset: false,
                                     layer: Layers.Over5);
      }
    }
  }
}