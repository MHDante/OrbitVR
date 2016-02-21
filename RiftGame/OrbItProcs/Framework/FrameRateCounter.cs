using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace OrbItProcs {
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

    public void Update(GameTime gameTime) {
      elapsedTime += gameTime.ElapsedGameTime;
      updateCounter++;

      if (elapsedTime > TimeSpan.FromSeconds(1)) {
        elapsedTime -= TimeSpan.FromSeconds(1);
        frameRate = frameCounter;
        frameCounter = 0;
        updateRate = updateCounter;
        updateCounter = 0;
      }
      Draw(Assets.font);
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


    public void Draw(SpriteFont spriteFont) {
      frameCounter++;
      int y1 = 70;

      string fps = string.Format("fps: {0}", frameRate);
      string ups = string.Format("ups: {0}", updateRate);
      string process = "";
      //string fpsups = string.Format("fps:{0} ups:{1}", frameRate, updateRate);
      Room room = OrbIt.game.room;
      bool hasProcess = room != null && OrbIt.ui.keyManager.TemporaryProcess != null;
      if (hasProcess) {
        y1 += 30;
        process = OrbIt.ui.keyManager.TemporaryProcess.GetType().ToString().LastWord('.');
      }

      room.camera.DrawStringScreen(fps, new Vector2(0, OrbIt.ScreenHeight - y1), Color.Black);
      y1 -= 30;
      room.camera.DrawStringScreen(ups, new Vector2(0, OrbIt.ScreenHeight - y1), Color.Black);
      y1 -= 30;
      if (hasProcess) room.camera.DrawStringScreen(process, new Vector2(0, OrbIt.ScreenHeight - y1), Color.Black);

      if (room.masterGroup != null) {
        string count = room.groups.general.fullSet.Count.ToString();
        int x = OrbIt.ScreenWidth - (count.Length*7) - 20;
        room.camera.DrawStringScreen(count, new Vector2(x, OrbIt.ScreenHeight - y1), Color.Black, offset: false,
          Layer: Layers.Over5);
      }
    }
  }
}