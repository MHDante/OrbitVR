using System;
using SharpDX;

namespace OrbitVR.Framework {
  public enum InputButtons {
    A_1,
    X_2,
    B_3,
    Y_4,
    Dpad_UpArrow,
    Dpad_DownArrow,
    Dpad_RightArrow,
    Dpad_LeftArrow,
    Select_TAB,
    Start_ESC,
    LeftBumper_Q,
    RightBumper_E,
    LeftTrigger_Mouse2,
    RightTrigger_Mouse1,
  }

  public struct InputState {
    public readonly Stick LeftStick_WASD, RightStick_Mouse;
    public readonly ButtonState A_1, X_2, B_3, Y_4;
    public readonly ButtonState Dpad_UpArrow, Dpad_DownArrow, Dpad_RightArrow, Dpad_LeftArrow;
    public readonly ButtonState Select_TAB, Start_ESC;
    public readonly ButtonState LeftBumper_Q, RightBumper_E;
    public readonly ButtonState LeftTrigger_Mouse2, RightTrigger_Mouse1;
    public readonly float LeftTriggerAnalog, RightTriggerAnalog;

    /*public InputState(Stick LeftStick_WASD, Stick RightStick_Mouse,
                          ButtonState A_1, ButtonState X_2, ButtonState B_3, ButtonState Y_4,
                          ButtonState Dpad_UpArrow, ButtonState Dpad_DownArrow, ButtonState Dpad_RightArrow, ButtonState Dpad_LeftArrow,
                          ButtonState Select_TAB, ButtonState Start_ESC,
                          ButtonState LeftBumper_Q, ButtonState RightBumper_E,
                          ButtonState LeftTrigger_Mouse2, ButtonState RightTrigger_Mouse1,
                          float LeftTriggerAnalog = 0, float RightTriggerAnalog = 0)
        {
            this.LeftStick_WASD = LeftStick_WASD;
            this.RightStick_Mouse = RightStick_Mouse;
            this.A_1 = A_1;
            this.X_2 = X_2;
            this.B_3 = B_3;
            this.Y_4 = Y_4;
            this.Dpad_UpArrow = Dpad_UpArrow;
            this.Dpad_DownArrow = Dpad_DownArrow;
            this.Dpad_RightArrow = Dpad_RightArrow;
            this.Dpad_LeftArrow = Dpad_LeftArrow;
            this.Select_TAB = Select_TAB;
            this.Start_ESC = Start_ESC;
            this.LeftBumper_Q = LeftBumper_Q;
            this.RightBumper_E = RightBumper_E;
            this.LeftTrigger_Mouse2 = LeftTrigger_Mouse2;
            this.RightTrigger_Mouse1 = RightTrigger_Mouse1;
            this.LeftTriggerAnalog = LeftTriggerAnalog;
            this.RightTriggerAnalog = RightTriggerAnalog;
        }*/
    //keyboard/mouse
    public InputState(Stick LeftStick_WASD, Stick RightStick_Mouse,
                      ButtonState LeftTrigger_Mouse2, ButtonState RightTrigger_Mouse1,
                      bool A_1, bool X_2, bool B_3, bool Y_4,
                      bool Dpad_UpArrow, bool Dpad_DownArrow, bool Dpad_RightArrow, bool Dpad_LeftArrow,
                      bool Select_TAB, bool Start_ESC,
                      bool LeftBumper_Q, bool RightBumper_E,
                      float LeftTriggerAnalog = 0, float RightTriggerAnalog = 0) {
      this.LeftStick_WASD = LeftStick_WASD;
      this.RightStick_Mouse = RightStick_Mouse;
      this.LeftTrigger_Mouse2 = LeftTrigger_Mouse2;
      this.RightTrigger_Mouse1 = RightTrigger_Mouse1;
      this.A_1 = A_1 ? ButtonState.Pressed : ButtonState.Released;
      this.X_2 = X_2 ? ButtonState.Pressed : ButtonState.Released;
      this.B_3 = B_3 ? ButtonState.Pressed : ButtonState.Released;
      this.Y_4 = Y_4 ? ButtonState.Pressed : ButtonState.Released;
      this.Dpad_UpArrow = Dpad_UpArrow ? ButtonState.Pressed : ButtonState.Released;
      this.Dpad_DownArrow = Dpad_DownArrow ? ButtonState.Pressed : ButtonState.Released;
      this.Dpad_RightArrow = Dpad_RightArrow ? ButtonState.Pressed : ButtonState.Released;
      this.Dpad_LeftArrow = Dpad_LeftArrow ? ButtonState.Pressed : ButtonState.Released;
      this.Select_TAB = Select_TAB ? ButtonState.Pressed : ButtonState.Released;
      this.Start_ESC = Start_ESC ? ButtonState.Pressed : ButtonState.Released;
      this.LeftBumper_Q = LeftBumper_Q ? ButtonState.Pressed : ButtonState.Released;
      this.RightBumper_E = RightBumper_E ? ButtonState.Pressed : ButtonState.Released;
      this.LeftTriggerAnalog = LeftTriggerAnalog;
      this.RightTriggerAnalog = RightTriggerAnalog;
    }

    //controller
    public InputState(ref GamePadState state, float triggerDeadZone) {
      this.LeftStick_WASD = new Stick(state.ThumbSticks.Left);
      this.LeftStick_WASD.v2.Y *= -1; //todo: fix directional buttonstates?
      this.RightStick_Mouse = new Stick(state.ThumbSticks.Right);
      this.RightStick_Mouse.v2.Y *= -1;
      this.LeftTrigger_Mouse2 = state.Triggers.Left > triggerDeadZone ? ButtonState.Pressed : ButtonState.Released;
      this.RightTrigger_Mouse1 = state.Triggers.Right > triggerDeadZone ? ButtonState.Pressed : ButtonState.Released;
      this.A_1 = state.Buttons.A;
      this.X_2 = state.Buttons.X;
      this.B_3 = state.Buttons.B;
      this.Y_4 = state.Buttons.Y;
      this.Dpad_UpArrow = state.DPad.Up;
      this.Dpad_DownArrow = state.DPad.Down;
      this.Dpad_RightArrow = state.DPad.Right;
      this.Dpad_LeftArrow = state.DPad.Left;
      this.Select_TAB = state.Buttons.Back;
      this.Start_ESC = state.Buttons.Start;
      this.LeftBumper_Q = state.Buttons.LeftShoulder;
      this.RightBumper_E = state.Buttons.RightShoulder;
      this.LeftTriggerAnalog = state.Triggers.Left;
      this.RightTriggerAnalog = state.Triggers.Right;
    }

    public bool IsButtonDown(InputButtons button) {
      switch (button) {
        case InputButtons.A_1:
          return A_1 == ButtonState.Pressed;
        case InputButtons.X_2:
          return X_2 == ButtonState.Pressed;
        case InputButtons.B_3:
          return B_3 == ButtonState.Pressed;
        case InputButtons.Y_4:
          return Y_4 == ButtonState.Pressed;
        case InputButtons.Dpad_UpArrow:
          return Dpad_UpArrow == ButtonState.Pressed;
        case InputButtons.Dpad_DownArrow:
          return Dpad_DownArrow == ButtonState.Pressed;
        case InputButtons.Dpad_RightArrow:
          return Dpad_RightArrow == ButtonState.Pressed;
        case InputButtons.Dpad_LeftArrow:
          return Dpad_LeftArrow == ButtonState.Pressed;
        case InputButtons.Select_TAB:
          return Select_TAB == ButtonState.Pressed;
        case InputButtons.Start_ESC:
          return Start_ESC == ButtonState.Pressed;
        case InputButtons.LeftBumper_Q:
          return LeftBumper_Q == ButtonState.Pressed;
        case InputButtons.RightBumper_E:
          return RightBumper_E == ButtonState.Pressed;
        case InputButtons.LeftTrigger_Mouse2:
          return LeftTrigger_Mouse2 == ButtonState.Pressed;
        case InputButtons.RightTrigger_Mouse1:
          return RightTrigger_Mouse1 == ButtonState.Pressed;
        default:
          return false;
      }
    }
  }


  public abstract class Input {
    public InputState newInputState, oldInputState;
    public Player player;
    public abstract InputState GetState();

    public virtual Vector2 GetLeftStick() {
      return newInputState.LeftStick_WASD.v2;
    }

    public virtual Vector2 GetRightStick() {
      return newInputState.RightStick_Mouse.v2;
    }

    /// <summary> Returns a non-unit vector up to the radius specified.</summary>
    public virtual Vector2 GetRightStick(float range, bool drawRing = false) {
      return newInputState.RightStick_Mouse.v2;
    }

    public virtual void SetNewState() {
      newInputState = GetState();
    }

    public virtual void SetOldState() {
      oldInputState = newInputState;
    }

    public bool BtnDown(InputButtons button) {
      return newInputState.IsButtonDown(button);
    }

    public bool BtnUp(InputButtons button) {
      return !newInputState.IsButtonDown(button);
    }

    public bool BtnClicked(InputButtons button) {
      return newInputState.IsButtonDown(button) && !oldInputState.IsButtonDown(button);
    }

    public bool BtnReleased(InputButtons button) {
      return !newInputState.IsButtonDown(button) && oldInputState.IsButtonDown(button);
    }
  }

  public class PcFullInput : Input {
    public float mouseStickRadius;
    public KeyboardState oldKeyState, newKeyState;
    public MouseState oldMouseState, newMouseState;

    public PcFullInput(Player player) //, float mouseStickRadius)
    {
      this.player = player;
      this.mouseStickRadius = 50f; //mouseStickRadius;
    }

    public override InputState GetState() {
      newKeyState = Keyboard.GetState();
      newMouseState = Mouse.GetState();
      Stick LeftStick_WASD = new Stick(newKeyState.IsKeyDown(Keys.W), newKeyState.IsKeyDown(Keys.S),
                                       newKeyState.IsKeyDown(Keys.A), newKeyState.IsKeyDown(Keys.D));
      Stick RightStick_Mouse = new Stick(GetRightStick(mouseStickRadius));
      newInputState = new InputState(LeftStick_WASD, RightStick_Mouse, newMouseState.RightButton,
                                     newMouseState.LeftButton,
                                     newKeyState.IsKeyDown(Keys.D1), newKeyState.IsKeyDown(Keys.D2),
                                     newKeyState.IsKeyDown(Keys.D3),
                                     newKeyState.IsKeyDown(Keys.D4),
                                     newKeyState.IsKeyDown(Keys.Up), newKeyState.IsKeyDown(Keys.Down),
                                     newKeyState.IsKeyDown(Keys.Right),
                                     newKeyState.IsKeyDown(Keys.Left),
                                     newKeyState.IsKeyDown(Keys.Tab), newKeyState.IsKeyDown(Keys.Escape),
                                     newKeyState.IsKeyDown(Keys.Q),
                                     newKeyState.IsKeyDown(Keys.E));
      return newInputState;
    }

    public override void SetOldState() {
      base.SetOldState();
      oldKeyState = newKeyState;
      oldMouseState = newMouseState;
    }

    /// <summary> Returns a non-unit vector up to the radius specified.</summary>
    public override Vector2 GetRightStick(float radius, bool drawRing = false) {
      Vector2 mousePos = new Vector2(newMouseState.X, newMouseState.Y);
      Vector2 playerPos = (player.node.body.pos - player.room.Camera.virtualTopLeft)*player.room.Camera.zoom +
                          player.room.Camera.CameraOffsetVect;
      Vector2 dir = mousePos - playerPos;
      float lensqr = dir.LengthSquared();
      if (lensqr > radius*radius) {
        VMath.NormalizeSafe(ref dir);
        //dir = dir.NormalizeSafe() * radius;
      }
      else {
        dir /= radius;
      }
      if (drawRing) {
        float scale = (radius*2f)/128;//Todo:Assets.TextureDict[Textures.Ring].Width;
        float alpha = (((float) Math.Sin(OrbIt.Game.Time.TotalGameTime.TotalMilliseconds/300f) + 1f)/4f) + 0.25f;
        player.room.Camera.Draw(Textures.Ring, player.node.body.pos, player.pColor*alpha, scale, (int)Layers.Under2);
      }
      return dir;
    }
  }


  public class ControllerFullInput : Input {
    public GamePadState newGamePadState, oldGamePadState;
    public PlayerIndex playerIndex;
    public float triggerDeadZone;

    public ControllerFullInput(Player player, PlayerIndex playerIndex) //, float triggerDeadZone)
    {
      this.player = player;
      this.playerIndex = playerIndex;
      this.triggerDeadZone = 0.5f;
    }

    public override InputState GetState() {
      newGamePadState = GamePad.GetState(playerIndex, GamePadDeadZone.Circular);

      newInputState = new InputState(ref newGamePadState, triggerDeadZone);
      return newInputState;
    }

    public override void SetOldState() {
      base.SetOldState();
      oldGamePadState = newGamePadState;
    }
  }

  public struct Stick {
    public Vector2 v2;
    public ButtonState up;
    public ButtonState down;
    public ButtonState left;
    public ButtonState right;

    public float AsRadians {
      get { return VMath.VectorToAngle(v2); }
    }

    public int AsDegrees {
      get { return (int) (AsRadians*(180/GMath.PI)); }
    }

    public Stick(Vector2 sourceStick) {
      //v2 = Vector2.Zero;
      up = ButtonState.Released;
      down = ButtonState.Released;
      left = ButtonState.Released;
      right = ButtonState.Released;

      v2 = sourceStick; //multiply by -1?
      if (v2.LengthSquared() < Controller.deadZone*Controller.deadZone) return;

      double angle = Math.Atan2(sourceStick.Y, sourceStick.X);
      int octant = ((int) Math.Round(8*angle/(2*Math.PI) + 9))%8; // TODO: test & clarify

      switch (octant) {
        case 0:
          up = ButtonState.Pressed;
          right = ButtonState.Pressed;
          break;
        case 1:
          up = ButtonState.Pressed;
          break;
        case 2:
          left = ButtonState.Pressed;
          up = ButtonState.Pressed;
          break;
        case 3:
          left = ButtonState.Pressed;
          break;
        case 4:
          down = ButtonState.Pressed;
          left = ButtonState.Pressed;
          break;
        case 5:
          down = ButtonState.Pressed;
          break;
        case 6:
          right = ButtonState.Pressed;
          down = ButtonState.Pressed;
          break;
        case 7:
          right = ButtonState.Pressed;
          break;
      }
    }

    public Stick(bool up, bool down, bool left, bool right) {
      float x = 0, y = 0;
      if (up) {
        y -= 1;
        this.up = ButtonState.Pressed;
      }
      else this.up = ButtonState.Released;
      if (down) {
        y += 1;
        this.down = ButtonState.Pressed;
      }
      else this.down = ButtonState.Released;
      if (left) {
        x -= 1;
        this.left = ButtonState.Pressed;
      }
      else this.left = ButtonState.Released;
      if (right) {
        x += 1;
        this.right = ButtonState.Pressed;
      }
      else this.right = ButtonState.Released;

      Vector2 v = new Vector2(x, y);

      if (x != 0 && y != 0) {
        v *= GMath.invRootOfTwo;
      }
      this.v2 = v;
    }

    public Stick(ButtonState up, ButtonState down, ButtonState left, ButtonState right)
      : this(up == ButtonState.Pressed, down == ButtonState.Pressed,
             left == ButtonState.Pressed, right == ButtonState.Pressed) {}

    public static implicit operator Vector2(Stick s) {
      return s.v2;
    }

    public bool isCentered() //account for v2?
    {
      if (up == ButtonState.Released &&
          down == ButtonState.Released &&
          left == ButtonState.Released &&
          right == ButtonState.Released) return true;
      else return false;
    }
  }
}