using System.Collections.Generic;
using OrbitVR.Framework;
using OrbitVR.Physics;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Processes {
  public class PolygonSpawner : Process {
    //int clickCount = 0;
    List<Vector2R> verts = new List<Vector2R>();

    public PolygonSpawner() {
      this.addProcessKeyAction("createVert", KeyCodes.LeftClick, OnPress: CreateVertice);
      this.addProcessKeyAction("createVert", KeyCodes.Enter, OnPress: CreatePolygon);
      this.addProcessKeyAction("createBox", KeyCodes.RightClick, OnPress: CreateBox);
      this.addProcessKeyAction("createRandPoly", KeyCodes.MiddleClick, OnPress: CreateRandomPolygon);
    }

    private static int Orientation(Vector2R p1, Vector2R p2, Vector2R p) {
      // Determinant
      float Orin = (p2.X - p1.X)*(p.Y - p1.Y) - (p.X - p1.X)*(p2.Y - p1.Y);
      if (Orin > 0)
        return -1; //          (* Orientaion is to the left-hand side  *)
      if (Orin < 0)
        return 1; // (* Orientaion is to the right-hand side *)
      return 0; //  (* Orientaion is neutral aka collinear  *)
    }

    public void CreateVertice() {
      Vector2R mp = UserInterface.WorldMousePos;
      verts.Add(mp);
    }

    public void CreatePolygon() {
      if (verts.Count < 3) return;

      Vector2R[] vertices = verts.ToArray();
      Node newNode = new Node(room, ShapeType.Polygon);
      //Node.cloneNode(Game1.ui.sidebar.ActiveDefaultNode, newNode);
      //Polygon poly = new Polygon();
      //poly.body = newNode.body;
      ////poly.FindCentroid(vertices);
      Polygon poly = (Polygon) newNode.body.shape;
      poly.SetCenterOfMass(vertices);

      //newNode.body.shape = poly;
      room.SpawnNode(newNode);
      verts = new List<Vector2R>();
    }

    public void CreateRandomPolygon() {
      Vector2R mp = UserInterface.WorldMousePos;
      List<Vector2R> randVerts = new List<Vector2R>();
      int count = Utils.random.Next(7) + 3;
      for (int i = 0; i < count; i++) {
        Vector2R v = new Vector2R(Utils.random.Next(200) - 100, Utils.random.Next(200) - 100);
        randVerts.Add(v);
      }

      Vector2R[] vertices = randVerts.ToArray();
      //Node newNode = new Node(ShapeType.ePolygon);
      Node newNode = room.ActiveDefaultNode.CreateClone();
      //Node.cloneNode(OrbIt.ui.sidebar.ActiveDefaultNode, newNode);
      Polygon poly = new Polygon();
      poly.body = newNode.body;
      poly.SetCenterOfMass(vertices);
      //poly.Set(vertices, vertices.Length);
      newNode.body.shape = poly;
      newNode.body.pos = mp;
      room.SpawnNode(newNode);

      verts = new List<Vector2R>();
    }

    public void CreateBox() {
      Vector2R mp = UserInterface.WorldMousePos;

      //if (verts.Count < 3) return;
      //Vector2[] vertices = verts.ToArray();
      //Node newNode = new Node(ShapeType.ePolygon);
      Node newNode = room.ActiveDefaultNode.CreateClone();
      //Node.cloneNode(OrbIt.ui.sidebar.ActiveDefaultNode, newNode);
      Polygon poly = new Polygon();
      poly.body = newNode.body;
      poly.SetBox(Utils.random.Next(100), Utils.random.Next(100));
      newNode.body.shape = poly;
      newNode.body.pos = mp;
      room.SpawnNode(newNode);

      //verts = new List<Vector2>();
    }
  }
}