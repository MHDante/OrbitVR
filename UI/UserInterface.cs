using System;
using System.Linq;
using OrbitVR.Framework;
using SharpDX;

namespace OrbitVR.UI {
  public class UserInterface {

    private static Vector2R _mousePos;
    public static Vector2R WorldMousePos;
    public static KeyboardState keybState, oldKeyBState;
    public static MouseState mouseState, oldMouseState;
    int _oldMouseScrollValue;
    public Action<int> ScrollAction { get; }
    private float Zoomfactor { get; }
    public bool IsPaused { get; set; }

    public OrbIt game {
      get { return OrbIt.Game; }
    }

    public Room room {
      get { return game.Room; }
    }


    public KeyManager keyManager { get; set; }

    public UserInterface() {
      ScrollAction = null;
      Zoomfactor = 0.9f;
      IsPaused = false;
      keyManager = new KeyManager(this);
    }

    public void Update() {
      ProcessKeyboard();
      ProcessMouse();
      ProcessController();
      keyManager.Update();
    }

    public void ProcessKeyboard() {
      keybState = Keyboard.GetState();
      

      if (keybState.IsKeyDown(Keys.Space) && oldKeyBState.IsKeyUp(Keys.Space)) {
        room.Update();
      }
      

      if (keybState.IsKeyDown(Keys.F) && !oldKeyBState.IsKeyDown(Keys.F)) IsPaused = !IsPaused;

      oldKeyBState = Keyboard.GetState();
    }

    public Node SelectNode(Vector2R pos) {
      Node found = null;
      float shortedDistance = Int32.MaxValue;
      for (int i = room.MasterGroup.fullSet.Count - 1; i >= 0; i--) {
        Node n = (Node) room.MasterGroup.fullSet.ElementAt(i);
        // find node that has been clicked, starting from the most recently placed nodes
        float distsquared = Vector2R.DistanceSquared(n.body.pos, pos);
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

      _mousePos = new Vector2R(mouseState.X, mouseState.Y) - OrbIt.Game.Room.Camera.CameraOffsetVect;
      WorldMousePos = (_mousePos/room.Camera.zoom) + room.Camera.virtualTopLeft;
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

      
        if (true) // || mouseState.LeftButton == ButtonState.Pressed)
        {
          bool found = false;
          for (int i = room.MasterGroup.fullSet.Count - 1; i >= 0; i--) {
            Node n = room.MasterGroup.fullSet.ElementAt(i);
            // find node that has been clicked, starting from the most recently placed nodes
            if (Vector2R.DistanceSquared(n.body.pos, new Vector2R(worldMouseX, worldMouseY)) < n.body.radius*n.body.radius) {
              room.TargetNode = n;
              found = true;
              break;
            }
          }
          if (!found) room.TargetNode = null;
        }

      if (mouseState.ScrollWheelValue < _oldMouseScrollValue) {
        room.Camera.zoom *= Zoomfactor;
      }
      else if (mouseState.ScrollWheelValue > _oldMouseScrollValue) {
        room.Camera.zoom /= Zoomfactor;
      }

      _oldMouseScrollValue = mouseState.ScrollWheelValue;
      oldMouseState = mouseState;
    }
  }
}