using System;
using System.Collections.Generic;
using System.Linq;
using OrbitVR.Framework;
using SharpDX;
using SharpDX.Toolkit;

namespace OrbitVR.UI {
  public class UserInterface {
    public enum Selection {
      PlaceNode,
      TargetSelection,
      GroupSelection,
      RandomNode,
    }
    
    public static Vector2 MousePos;
    public static Vector2 WorldMousePos;
    public Action<int> ScrollAction;
    public float zoomfactor { get; set; }
    public bool IsPaused { get; set; }

    public UserInterface() {

      zoomfactor = 0.9f;
      IsPaused = false;
      keyManager = new KeyManager(this);
    }

    public void Update(GameTime gameTime) {
      ProcessKeyboard();
      ProcessMouse();
      ProcessController();
      keyManager.Update();
    }

    public void ProcessKeyboard() {
      keybState = Keyboard.GetState();

      if (keybState.IsKeyDown(Keys.Y))
        hovertargetting = true;
      else
        hovertargetting = false;


      if (keybState.IsKeyDown(Keys.Space) && oldKeyBState.IsKeyUp(Keys.Space)) {
        room.Update(null);
      }

      if (keybState.IsKeyDown(Keys.LeftShift)) {
        if (!isShiftDown) {
          MouseState ms = Mouse.GetState();
          spawnPos = new Vector2(ms.X, ms.Y)/room.Camera.zoom;
        }
        isShiftDown = true;
      }
      else {
        isShiftDown = false;
      }

      //if (keybState.IsKeyDown(Keys.F) && !oldKeyBState.IsKeyDown(Keys.F))
      //    IsPaused = !IsPaused;

      oldKeyBState = Keyboard.GetState();
    }

    public Node SelectNode(Vector2 pos) {
      Node found = null;
      float shortedDistance = Int32.MaxValue;
      for (int i = room.MasterGroup.fullSet.Count - 1; i >= 0; i--) {
        Node n = (Node) room.MasterGroup.fullSet.ElementAt(i);
        // find node that has been clicked, starting from the most recently placed nodes
        float distsquared = Vector2.DistanceSquared(n.body.pos, pos);
        if (distsquared < n.body.radius*n.body.radius) {
          if (distsquared < shortedDistance) {
            found = n;
            shortedDistance = distsquared;
          }
        }
      }
      return found;
    }

    public void ProcessController() {
      //GamePad.SetVibration(PlayerIndex.Two, 0.1f, 0.9f);
      //System.Console.WriteLine(GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X);
      //GraphData.AddFloat(GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X * 10);
    }

    public void ProcessMouse() {
      mouseState = Mouse.GetState();
      
      //if (mouseState.XButton1 == ButtonState.Pressed)
      //    System.Console.WriteLine("X1");
      //
      //if (mouseState.XButton2 == ButtonState.Pressed)
      //    System.Console.WriteLine("X2");

      MousePos = new Vector2(mouseState.X, mouseState.Y) - OrbIt.Game.Room.Camera.CameraOffsetVect;
      WorldMousePos = (MousePos/room.Camera.zoom) + room.Camera.virtualTopLeft;
      //ignore mouse clicks outside window
      if (!IsPaused) {
        if (mouseState.X >= OrbIt.ScreenWidth || mouseState.X < 0 || mouseState.Y >= OrbIt.ScreenHeight ||
            mouseState.Y < 0)
          return;
      }

      //if (!keyManager.MouseInGameBox)
      //{
      if (ScrollAction != null) {
        if (mouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue) {
          ScrollAction(2);
        }
        else if (mouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue) {
          ScrollAction(-2);
        }
      }

      oldMouseState = mouseState;
      // return;
      //}

      if (!keyManager.MouseInGameBox) return;
      //game.processManager.PollMouse(mouseState, oldMouseState);
      int worldMouseX = (int) WorldMousePos.X;
      int worldMouseY = (int) WorldMousePos.Y;


      if (hovertargetting) {
        if (true) // || mouseState.LeftButton == ButtonState.Pressed)
        {
          bool found = false;
          for (int i = room.MasterGroup.fullSet.Count - 1; i >= 0; i--) {
            Node n = (Node) room.MasterGroup.fullSet.ElementAt(i);
            // find node that has been clicked, starting from the most recently placed nodes
            if (Vector2.DistanceSquared(n.body.pos, new Vector2(worldMouseX, worldMouseY)) < n.body.radius*n.body.radius) {
              room.TargetNode = n;
              found = true;
              break;
            }
          }
          if (!found) room.TargetNode = null;
        }
      }

      if (mouseState.ScrollWheelValue < oldMouseScrollValue) {
        room.Camera.zoom *= zoomfactor;
      }
      else if (mouseState.ScrollWheelValue > oldMouseScrollValue) {
        room.Camera.zoom /= zoomfactor;
      }

      oldMouseScrollValue = mouseState.ScrollWheelValue;
      oldMouseState = mouseState;
    }
    

    #region /// Fields ///

    public OrbIt game {
      get { return OrbIt.Game; }
    }

    public Room room {
      get { return game.Room; }
    }


    public KeyManager keyManager { get; set; }


    public static KeyboardState keybState, oldKeyBState;
    public static MouseState mouseState, oldMouseState;

    //public string currentSelection = "placeNode";//
    public Selection currentSelection = Selection.PlaceNode;
    int oldMouseScrollValue = 0; //
    bool hovertargetting = false; //
    //int rightClickCount = 0;//
    //int rightClickMax = 1;//
    public int sWidth = 1000; ////
    public int sHeight = 600; ////
    bool isShiftDown = false;
    //bool isTargeting = false;
    public Vector2 spawnPos;
    Vector2 groupSelectionBoxOrigin = new Vector2(0, 0);
    public HashSet<Node> groupSelectSet;

    #endregion
  }
}