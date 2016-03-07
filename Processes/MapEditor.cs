using System;
using System.Collections.Generic;
using OrbitVR.Components.Essential;
using OrbitVR.Framework;
using OrbitVR.Physics;
using OrbitVR.UI;
using SharpDX;
using SharpDX.Direct3D11;

namespace OrbitVR.Processes {
  public class MapEditor : Process {
    public List<Vector2> verts;

    public MapEditor() : base() {
      verts = new List<Vector2>();
      addProcessKeyAction("placevertice", KeyCodes.LeftClick, OnPress: PlaceVertice);
      addProcessKeyAction("FinishWall", KeyCodes.Enter, OnPress: FinishWall);
      addProcessKeyAction("FinishWall", KeyCodes.RightClick, OnPress: FinishWall);
      addProcessKeyAction("ClearWalls", KeyCodes.MiddleClick, OnPress: ClearWalls);
    }

    public void PlaceVertice() {
      int vertX = 0, vertY = 0;
      MouseToGrid(ref vertX, ref vertY);
      Vector2 vert = new Vector2(vertX, vertY);
      if (verts.Contains(vert)) {
        if (verts.IndexOf(vert) == 0) {
          FinishWall();
        }
        else {
          verts.Remove(vert);
        }
      }
      else {
        verts.Add(vert);
      }
    }

    public void FinishWall() {
      if (verts.Count < 3) return;
      Vector2[] vertices = verts.ToArray();
      Node newNode = new Node(room, ShapeType.Polygon);
      Polygon poly = (Polygon) newNode.body.shape;
      poly.SetCenterOfMass(vertices);
      newNode.body.SetStatic();
      newNode.body.orient = 0;
      newNode.movement.mode = movemode.free;
      newNode.body.restitution = 1f;
      newNode.meta.maxHealth.enabled = false;
      room.SpawnNode(newNode, g: room.MasterGroup.childGroups["Wall Group"]);
      verts = new List<Vector2>();
    }

    public void ClearWalls() {
      room.MasterGroup.childGroups["Wall Group"].EmptyGroup();
    }

    public override void Draw() {
      int vertX = 0, vertY = 0;
      MouseToGrid(ref vertX, ref vertY);
      Vector2 vert = new Vector2(vertX, vertY);

      Texture2D tx = Textures.Whitecircle.GetTexture2D();
      Vector2 cen = new Vector2(tx.Description.Width/2f, tx.Description.Height / 2f); //store this in another textureDict to avoid recalculating

      //Todo: why did we use sourceRects before?
      room.Camera.Draw(Textures.Whitecircle, vert, Color.White, 0f, 0.3f, (int)Layers.Over5);
      foreach (Vector2 v in verts) {
        room.Camera.Draw(Textures.Whitecircle, v, Color.Red, 0f, 0.3f, (int)Layers.Over5);
      }
    }


    public void MouseToGrid(ref int x, ref int y) {
      Vector2 MousePos = UserInterface.WorldMousePos;

      //Console.WriteLine(room.worldWidth + " : " + room.worldHeight + "  :::  " + MousePos.X + " : " + MousePos.Y);
      double dx = MousePos.X/(double) room.Level.cellWidth;
      double dy = MousePos.Y/(double) room.Level.cellHeight;

      x = (int) Math.Floor(dx + 0.5)*room.Level.cellWidth;
      y = (int) Math.Floor(dy + 0.5)*room.Level.cellHeight;
    }
  }
}