using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace OrbItProcs
{
    public class GridSpawn : Process
    {
        public GridSpawn() : base()
        {
            addProcessKeyAction("SpawnNodeGrid", KeyCodes.LeftClick, OnPress: SpawnNodeGrid);
        }
        public void SpawnNodeGrid()
        {
            ///
            Vector2 MousePos = UserInterface.WorldMousePos;
            int mult = 3;
            int dx = (int)MousePos.X / (room.level.cellWidth * mult);
            int dy = (int)MousePos.Y / (room.level.cellHeight * mult);

            int x = (int)(dx * room.level.cellWidth * mult + room.level.cellWidth * (mult / 2f));
            int y = (int)(dy * room.level.cellHeight * mult + room.level.cellHeight * (mult / 2f));

            Node n = room.spawnNode(x, y);
            n.movement.active = false;
            n.movement.pushable = false;
        }

    }
}
