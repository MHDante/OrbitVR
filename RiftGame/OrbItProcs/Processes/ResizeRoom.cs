using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;
using SharpDX;
using SharpOVR;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.DXGI;
using SharpDX.Toolkit.Content;

namespace OrbItProcs
{
    public class ResizeRoom : Process
    {
        public ResizeRoom()
            : base()
        {
            addProcessKeyAction("Resize", KeyCodes.LeftClick, OnPress: resize);
        }
        public void resize()
        {
            Vector2 pos = UserInterface.WorldMousePos;
            if (pos.X < 0 || pos.Y < 0) return;
            OrbIt.game.room.resize(pos);
        }
        public override void Update()
        {
            Vector2 pos = UserInterface.WorldMousePos;
            if (pos.X < 0 || pos.Y < 0) return;

            int minX = 0;
            int minY = 0;
            int maxX = (int)pos.X;
            int maxY = (int)pos.Y;

            Color c = Color.Green;
            room.camera.DrawLine(new Vector2(minX, minY), new Vector2(minX, maxY), 2f, c, Layers.Over5);
            room.camera.DrawLine(new Vector2(minX, minY), new Vector2(maxX, minY), 2f, c, Layers.Over5);
            room.camera.DrawLine(new Vector2(maxX, maxY), new Vector2(minX, maxY), 2f, c, Layers.Over5);
            room.camera.DrawLine(new Vector2(maxX, maxY), new Vector2(maxX, minY), 2f, c, Layers.Over5);
        }
    }
}
