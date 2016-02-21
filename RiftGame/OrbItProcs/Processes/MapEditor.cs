using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OrbItProcs
{
    public class MapEditor : Process
    {
        public List<Vector2> verts;
        public MapEditor() : base()
        {
            verts = new List<Vector2>();
            addProcessKeyAction("placevertice", KeyCodes.LeftClick, OnPress: PlaceVertice);
            addProcessKeyAction("FinishWall", KeyCodes.Enter, OnPress: FinishWall);
            addProcessKeyAction("FinishWall", KeyCodes.RightClick, OnPress: FinishWall);
            addProcessKeyAction("ClearWalls", KeyCodes.MiddleClick, OnPress: ClearWalls);
        }

        public void PlaceVertice()
        {
            int vertX = 0, vertY = 0;
            MouseToGrid(ref vertX, ref vertY);
            Vector2 vert = new Vector2(vertX, vertY);
            if (verts.Contains(vert))
            {
                if (verts.IndexOf(vert) == 0)
                {
                    FinishWall();
                }
                else
                {
                    verts.Remove(vert);
                }
            }
            else
            {
                verts.Add(vert);
            }
        }

        public void FinishWall()
        {
            if (verts.Count < 3) return;
            Vector2[] vertices = verts.ToArray();
            Node newNode = new Node(room, ShapeType.Polygon);
            Polygon poly = (Polygon)newNode.body.shape;
            poly.SetCenterOfMass(vertices);
            newNode.body.SetStatic();
            newNode.body.orient = 0;
            newNode.movement.mode = movemode.free;
            newNode.body.restitution = 1f;
            newNode.meta.maxHealth.enabled = false;
            room.spawnNode(newNode, g: room.masterGroup.childGroups["Wall Group"]);
            verts = new List<Vector2>();
        }

        public void ClearWalls()
        {
            room.masterGroup.childGroups["Wall Group"].EmptyGroup();
        }

        public override void Draw()
        {
            int vertX = 0, vertY = 0;
            MouseToGrid(ref vertX, ref vertY);
            Vector2 vert = new Vector2(vertX, vertY);

            Texture2D tx = Assets.textureDict[textures.whitecircle];
            Vector2 cen = new Vector2(tx.Width / 2f, tx.Height / 2f);//store this in another textureDict to avoid recalculating

            room.camera.Draw(textures.whitecircle, vert, null, Color.White, 0f, cen, 0.3f, Layers.Over5);

            foreach(Vector2 v in verts)
            {
                room.camera.Draw(textures.whitecircle, v, null, Color.Red, 0f, cen, 0.3f, Layers.Over5);
            }
        }



        public void MouseToGrid(ref int x, ref int y)
        {
            Vector2 MousePos = UserInterface.WorldMousePos;

            //Console.WriteLine(room.worldWidth + " : " + room.worldHeight + "  :::  " + MousePos.X + " : " + MousePos.Y);
            double dx = MousePos.X / (double)room.level.cellWidth;
            double dy = MousePos.Y / (double)room.level.cellHeight;

            x = (int)Math.Floor(dx + 0.5) * room.level.cellWidth;
            y = (int)Math.Floor(dy + 0.5) * room.level.cellHeight;
        }
    }
}
