using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrbItProcs
{
    public class PolygonSpawner : Process
    {
        //int clickCount = 0;
        List<Vector2> verts = new List<Vector2>();

        private static int Orientation(Vector2 p1, Vector2 p2, Vector2 p)
        {
            // Determinant
            float Orin = (p2.X - p1.X) * (p.Y - p1.Y) - (p.X - p1.X) * (p2.Y - p1.Y);
            if (Orin > 0)
                return -1; //          (* Orientaion is to the left-hand side  *)
            if (Orin < 0)
                return 1; // (* Orientaion is to the right-hand side *)
            return 0; //  (* Orientaion is neutral aka collinear  *)
        }

        public PolygonSpawner()
        {
            this.addProcessKeyAction("createVert", KeyCodes.LeftClick, OnPress: CreateVertice);
            this.addProcessKeyAction("createVert", KeyCodes.Enter, OnPress: CreatePolygon);
            this.addProcessKeyAction("createBox", KeyCodes.RightClick, OnPress: CreateBox);
            this.addProcessKeyAction("createRandPoly", KeyCodes.MiddleClick, OnPress: CreateRandomPolygon);
        }

        public void CreateVertice()
        {
            Vector2 mp = UserInterface.WorldMousePos;
            verts.Add(mp);
        }

        public void CreatePolygon()
        {
            
            if (verts.Count < 3) return;
            
            Vector2[] vertices = verts.ToArray();
            Node newNode = new Node(room, ShapeType.Polygon);
            //Node.cloneNode(Game1.ui.sidebar.ActiveDefaultNode, newNode);
            //Polygon poly = new Polygon();
            //poly.body = newNode.body;
            ////poly.FindCentroid(vertices);
            Polygon poly = (Polygon)newNode.body.shape;
            poly.SetCenterOfMass(vertices);

            //newNode.body.shape = poly;
            room.spawnNode(newNode);
            verts = new List<Vector2>();
        }

        public void CreateRandomPolygon()
        {
            Vector2 mp = UserInterface.WorldMousePos;
            List<Vector2> randVerts = new List<Vector2>();
            int count = Utils.random.Next(7) + 3;
            for (int i = 0; i < count; i++)
            {
                Vector2 v = new Vector2(Utils.random.Next(200) - 100, Utils.random.Next(200) - 100);
                randVerts.Add(v);
            }

            Vector2[] vertices = randVerts.ToArray();
            //Node newNode = new Node(ShapeType.ePolygon);
            Node newNode = OrbIt.ui.sidebar.ActiveDefaultNode.CreateClone();
            //Node.cloneNode(OrbIt.ui.sidebar.ActiveDefaultNode, newNode);
            Polygon poly = new Polygon();
            poly.body = newNode.body;
            poly.SetCenterOfMass(vertices);
            //poly.Set(vertices, vertices.Length);
            newNode.body.shape = poly;
            newNode.body.pos = mp;
            room.spawnNode(newNode);

            verts = new List<Vector2>();
        }

        public void CreateBox()
        {
            Vector2 mp = UserInterface.WorldMousePos;

            //if (verts.Count < 3) return;
            //Vector2[] vertices = verts.ToArray();
            //Node newNode = new Node(ShapeType.ePolygon);
            Node newNode = OrbIt.ui.sidebar.ActiveDefaultNode.CreateClone();
            //Node.cloneNode(OrbIt.ui.sidebar.ActiveDefaultNode, newNode);
            Polygon poly = new Polygon();
            poly.body = newNode.body;
            poly.SetBox(Utils.random.Next(100),Utils.random.Next(100));
            newNode.body.shape = poly;
            newNode.body.pos = mp;
            room.spawnNode(newNode);

            //verts = new List<Vector2>();
        }
    }
}
