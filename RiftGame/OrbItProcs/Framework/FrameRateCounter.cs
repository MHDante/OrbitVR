using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace OrbItProcs {
    public class FrameRateCounter {
        OrbIt game;
        int frameRate = 0;
        int frameCounter = 0;
        int updateRate = 0;
        public int updateCounter = 0;
        public TimeSpan elapsedTime = TimeSpan.Zero;


        public FrameRateCounter(OrbIt game)
        {
            //content = new ContentManager(game.Services);
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            updateCounter++;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
                updateRate = updateCounter;
                updateCounter = 0;

            }
            Draw(Assets.font);
        }

        public void UpdateElapsed(TimeSpan elapsed)
        {
            elapsedTime += elapsed;
            updateCounter++;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
                updateRate = updateCounter;
                updateCounter = 0;
                
            }
        }


        public void Draw(SpriteFont spriteFont)
        {
            frameCounter++;
            int y1 = 70;

            string fps = string.Format("fps: {0}", frameRate);
            string ups = string.Format("ups: {0}", updateRate);
            string process = "";
            //string fpsups = string.Format("fps:{0} ups:{1}", frameRate, updateRate);
            Room room = OrbIt.game.room;
            bool hasProcess = room != null && OrbIt.ui.keyManager.TemporaryProcess != null;
            if (hasProcess)
            {
                y1 += 30;
                process = OrbIt.ui.keyManager.TemporaryProcess.GetType().ToString().LastWord('.');
            }

            room.camera.DrawStringScreen(fps, new Vector2(0, OrbIt.ScreenHeight - y1), Color.Black);
            y1 -= 30;
            room.camera.DrawStringScreen(ups, new Vector2(0, OrbIt.ScreenHeight - y1), Color.Black);
            y1 -= 30;
            if (hasProcess) room.camera.DrawStringScreen(process, new Vector2(0, OrbIt.ScreenHeight - y1), Color.Black);

            if (room.masterGroup != null)
            {
                string count = room.groups.general.fullSet.Count.ToString();
                int x = OrbIt.ScreenWidth - (count.Length * 7) - 20;
                room.camera.DrawStringScreen(count, new Vector2(x, OrbIt.ScreenHeight - y1), Color.Black, offset: false, Layer: Layers.Over5);
            }
        }
    }
}
