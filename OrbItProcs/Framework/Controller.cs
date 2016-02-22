using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace OrbItProcs {
  public enum ControlSide {
    left,
    right
  }

  public enum FullPadMode {
    mirrorMode,
    spellMode
  }

  public struct HalfPadState {
    /// <summary>
    /// HalfPadLeft: LStick. HalfPadRight: RStick. SpellMode: LStick.
    /// </summary>
    public Stick stick1;

    /// <summary>
    /// HalfPadLeft: Dpad. HalfPadRight: ABXY. SpellMode: Dpad and RStick.
    /// </summary>
    public Stick stick2;

    /// <summary>
    /// HalfPadLeft: LBumper. HalfPadRight: RBumper. SpellMode: LBumper, RBumper and A.
    /// </summary>
    public ButtonState Btn1;

    /// <summary>
    /// HalfPadLeft: LTrigger. HalfPadRight: RTrigger. SpellMode: B, LTrigger and RTrigger 
    /// For Pressure Sensitive analog triggers, use Btn2AsTrigger.
    /// </summary>
    public ButtonState Btn2;

    /// <summary>
    /// HalfPadLeft: LTrigger. HalfPadRight: RTrigger. SpellMode: LTrigger and RTrigger.
    /// </summary>
    public float Btn2AsTrigger;

    /// <summary>
    /// HalfPadLeft: LStick press. HalfPadRight: RStick press. SpellMode: LStickPress, RStickPress and X.
    /// </summary>
    public ButtonState Btn3;

    /// <summary>
    /// HalfPadLeft: Select. HalfPadRight: Start. SpellMode: Select and Start
    /// </summary>
    public ButtonState BtnStart;

    public static HalfPadState NullPadState = new HalfPadState();

    public HalfPadState(ControlSide side, PlayerIndex controllerIndex) {
      //SharpDX.XInput.Controller c;
      //var s = c.GetState();
      //var gp = s.Gamepad;
      //gp.

      GamePadState gamePadState = GamePad.GetState(controllerIndex);
      if (side == ControlSide.left) {
        stick1 = new Stick(gamePadState.ThumbSticks.Left);
        stick2 = new Stick(gamePadState.DPad.Up,
                           gamePadState.DPad.Down,
                           gamePadState.DPad.Left,
                           gamePadState.DPad.Right);

        Btn1 = gamePadState.Buttons.LeftShoulder;
        Btn2 = (gamePadState.Triggers.Left < Controller.deadZone) ? ButtonState.Released : ButtonState.Pressed;
        //TODO: test
        Btn2AsTrigger = gamePadState.Triggers.Left;
        Btn3 = gamePadState.Buttons.LeftStick;

        BtnStart = gamePadState.Buttons.Back;
      }
      else //if (side == ControlSide.right) 
      {
        stick1 = new Stick(gamePadState.ThumbSticks.Right);
        stick2 = new Stick(gamePadState.Buttons.Y,
                           gamePadState.Buttons.A,
                           gamePadState.Buttons.X,
                           gamePadState.Buttons.B);

        Btn1 = gamePadState.Buttons.RightShoulder;
        Btn2 = (gamePadState.Triggers.Right < Controller.deadZone) ? ButtonState.Released : ButtonState.Pressed;
        //TODO: test
        Btn2AsTrigger = gamePadState.Triggers.Right;
        Btn3 = gamePadState.Buttons.RightStick;

        BtnStart = gamePadState.Buttons.Start;
      }
    }

    public HalfPadState(FullPadMode mode, PlayerIndex controllerIndex) {
      GamePadState gamePadState = GamePad.GetState(controllerIndex, GamePadDeadZone.Circular);
      if (mode == FullPadMode.mirrorMode) {
        stick1 = new Stick(gamePadState.Buttons.Y,
                           gamePadState.Buttons.A,
                           gamePadState.Buttons.X,
                           gamePadState.Buttons.B);
        if (stick1.isCentered()) {
          stick1 = new Stick(gamePadState.ThumbSticks.Left);
        }

        stick2 = new Stick(gamePadState.ThumbSticks.Right);
        if (stick2.isCentered()) {
          stick2 = new Stick(gamePadState.DPad.Up,
                             gamePadState.DPad.Down,
                             gamePadState.DPad.Left,
                             gamePadState.DPad.Right);
        }

        Btn1 = gamePadState.Buttons.RightShoulder;
        if (Btn1 == ButtonState.Released) Btn1 = gamePadState.Buttons.LeftShoulder;

        Btn2 = (gamePadState.Triggers.Right < Controller.deadZone) ? ButtonState.Released : ButtonState.Pressed;
        //TODO: test
        if (Btn2 == ButtonState.Released)
          Btn2 = (gamePadState.Triggers.Left < Controller.deadZone) ? ButtonState.Released : ButtonState.Pressed;

        Btn2AsTrigger = Math.Max(gamePadState.Triggers.Right, gamePadState.Triggers.Left);

        Btn3 = gamePadState.Buttons.RightStick;
        if (Btn3 == ButtonState.Released) Btn3 = gamePadState.Buttons.LeftStick;

        BtnStart = gamePadState.Buttons.Start;
        if (BtnStart == ButtonState.Released) BtnStart = gamePadState.Buttons.Back;
      }
      else {
        stick1 = new Stick(gamePadState.ThumbSticks.Left);
        stick2 = new Stick(gamePadState.ThumbSticks.Right);
        if (stick2.isCentered()) {
          stick2 = new Stick(gamePadState.DPad.Up,
                             gamePadState.DPad.Down,
                             gamePadState.DPad.Left,
                             gamePadState.DPad.Right);
        }

        Btn1 = gamePadState.Buttons.RightShoulder;
        if (Btn1 == ButtonState.Released) Btn1 = gamePadState.Buttons.LeftShoulder;
        if (Btn1 == ButtonState.Released) Btn1 = gamePadState.Buttons.A;

        Btn2 = (gamePadState.Triggers.Right < Controller.deadZone) ? ButtonState.Released : ButtonState.Pressed;
        //TODO: test
        if (Btn2 == ButtonState.Released)
          Btn2 = (gamePadState.Triggers.Left < Controller.deadZone) ? ButtonState.Released : ButtonState.Pressed;
        if (Btn2 == ButtonState.Released) Btn2 = gamePadState.Buttons.B;
        if (Btn2 == ButtonState.Released) Btn2 = gamePadState.Buttons.Y;

        Btn2AsTrigger = Math.Max(gamePadState.Triggers.Right, gamePadState.Triggers.Left);
        if (gamePadState.Buttons.B == ButtonState.Pressed ||
            gamePadState.Buttons.Y == ButtonState.Pressed) Btn2AsTrigger = 1.0f;

        Btn3 = gamePadState.Buttons.RightStick;
        if (Btn3 == ButtonState.Released) Btn3 = gamePadState.Buttons.LeftStick;
        if (Btn3 == ButtonState.Released) Btn3 = gamePadState.Buttons.X;

        BtnStart = gamePadState.Buttons.Start;
        if (BtnStart == ButtonState.Released) BtnStart = gamePadState.Buttons.Back;
      }
    }
  }

  static class ControllerExtensions {
    public static bool isAvailable(this Controller.ControllerCodes code) {
      return (Controller.availableControllers & code) != 0;
    }

    public static bool isAvailable(this int code) {
      return ((int) Controller.availableControllers & code) != 0;
    }
  }

  public abstract class Controller {
    [Flags]
    public enum ControllerCodes {
      None = 0, //  00000000
      FirstLeft = 1, //  00000001
      FirstRight = 1 << 1, //  00000010
      SecondLeft = 1 << 2, //  00000100
      SecondRight = 1 << 3, //  00001000
      ThirdLeft = 1 << 4, //  00010000
      ThirdRight = 1 << 5, //  00100000
      FourthLeft = 1 << 6, //  01000000
      FourthRight = 1 << 7, //  10000000
      First = FirstLeft | FirstRight, //  00000011
      Second = SecondLeft | SecondRight, //  00001100
      Third = ThirdLeft | ThirdRight, //  00110000
      Fourth = FourthLeft | FourthRight, //  11000000
      All = First | Second | Third | Fourth //  11111111
    }

    public const float deadZone = 0.2f;
    public const int maxControllers = 4;
    public static List<HalfController> halfControllers = new List<HalfController>();
    public static List<FullController> fullControllers = new List<FullController>();
    public static ControllerCodes availableControllers = ControllerCodes.All;

    public static Dictionary<int, PlayerIndex> intToPlayerIndex =
      new Dictionary<int, PlayerIndex>() {
        {1, PlayerIndex.One},
        {2, PlayerIndex.Two},
        {3, PlayerIndex.Three},
        {4, PlayerIndex.Four}
      };

    protected ControllerCodes controllerCode;
    protected PlayerIndex controllerIndex;
    public bool enabled = true;

    public int playerNum;

    public static void ResetControllers() {
      availableControllers = ControllerCodes.All;
      halfControllers = new List<HalfController>();
      fullControllers = new List<FullController>();
    }

    public abstract Vector2 GetLeftStick();
    public abstract Vector2 GetRightStick();

    public static int connectedControllers() {
      for (int i = 1; i <= 4; i++)
        if (!GamePad.GetState(intToPlayerIndex[i]).IsConnected)
          return i - 1;
      return 4;
    }

    protected void assign(ControllerCodes controller) {
      controllerCode = controller;
      availableControllers ^= controller;
    }

    public virtual void unassign() {
      availableControllers = availableControllers | controllerCode;
      controllerCode = ControllerCodes.None;
    }

    public virtual void UpdateNewState() {}
    public virtual void UpdateOldState() {}
  }

  public class FullController : Controller {
    public GamePadState newGamePadState;
    public GamePadState oldGamePadState;

    private FullController(int player, ref bool success) {
      fullControllers.Add(this);

      this.playerNum = player;
      int c = connectedControllers();
      if (player > c) {
        success = false;
        return;
      }

      if (ControllerCodes.First.isAvailable()) {
        controllerIndex = PlayerIndex.One;
        assign(ControllerCodes.First);
      }
      else if (ControllerCodes.Second.isAvailable()) {
        controllerIndex = PlayerIndex.Two;
        assign(ControllerCodes.Second);
      }
      else if (ControllerCodes.Third.isAvailable()) {
        controllerIndex = PlayerIndex.Three;
        assign(ControllerCodes.Third);
      }
      else if (ControllerCodes.Fourth.isAvailable()) {
        controllerIndex = PlayerIndex.Four;
        assign(ControllerCodes.Fourth);
      }
      else {
        Console.WriteLine("Insufficient controllers! Player " + player + " will not work!");
        //PopUp.Toast("Insufficient controllers! Player "+ player +" will not work!");
        enabled = false;
        success = false;
        return;
      }
      success = true;
    }

    public static FullController GetNew(int player) {
      bool win = false;
      FullController f = new FullController(player, ref win);
      return win ? f : null;
    }

    public GamePadState getState() {
      if (enabled == false) return new GamePadState();
      return GamePad.GetState(controllerIndex, GamePadDeadZone.Circular);
    }

    public override Vector2 GetRightStick() {
      return newGamePadState.ThumbSticks.Right*new Vector2(1, -1);
    }

    public override Vector2 GetLeftStick() {
      return newGamePadState.ThumbSticks.Left*new Vector2(1, -1);
    }

    public override void UpdateNewState() {
      newGamePadState = getState();
    }

    public override void UpdateOldState() {
      oldGamePadState = newGamePadState;
    }
  }

  public class HalfController : Controller {
    bool fullControllerAvailable;
    public FullPadMode fullPadMode;
    public HalfPadState newHalfPadState;
    public HalfPadState oldHalfPadState;
    public ControlSide side;

    private HalfController(int player, ref bool assign, FullPadMode mode = FullPadMode.spellMode) {
      halfControllers.Add(this);
      this.fullPadMode = mode;
      this.playerNum = player;
      assign = reassign();
    }

    public static HalfController GetNew(int player, FullPadMode mode = FullPadMode.spellMode) {
      bool win = false;
      HalfController h = new HalfController(player, ref win, mode);
      return win ? h : null;
    }

    public override Vector2 GetRightStick() {
      return newHalfPadState.stick1*new Vector2(1, -1);
    }

    public override Vector2 GetLeftStick() {
      return newHalfPadState.stick2*new Vector2(1, -1);
    }

    public HalfPadState getState() {
      if (enabled == false) return HalfPadState.NullPadState;
      if (fullControllerAvailable == true) {
        return new HalfPadState(fullPadMode, controllerIndex);
      }
      return new HalfPadState(side, controllerIndex);
    }

    public override void UpdateNewState() {
      newHalfPadState = getState();
    }

    public override void UpdateOldState() {
      oldHalfPadState = newHalfPadState;
    }

    public override void unassign() {
      if (side == ControlSide.right)
        halfControllers.First(x => x.controllerCode == (ControllerCodes) ((int) controllerCode << 1))
                       .fullControllerAvailable = true;
      base.unassign();
    }

    public bool reassign() {
      for (int j = 0; j < maxControllers*2; j++) {
        Console.WriteLine("j = " + j);
        int i = (j*2)%((maxControllers*2 - 1) + j/(maxControllers*2 - 1)); //magic
        if (i >= connectedControllers()*2) {
          if (j < maxControllers) {
            j = maxControllers - 1;
            continue;
          }
          else {
            Console.WriteLine("Insufficient controllers! Player " + playerNum + " will not work!");
            enabled = false;

            return false;
          }
        }

        if ((1 << i).isAvailable()) {
          controllerIndex = intToPlayerIndex[(i/2) + 1];
          if ((i%2) == 0) {
            side = ControlSide.left;
          }
          else {
            side = ControlSide.right;
            fullControllerAvailable = false;
            halfControllers.First(x => x.controllerCode == (ControllerCodes) ((int) controllerCode >> 1))
                           .fullControllerAvailable = false;
          }
          assign((ControllerCodes) (1 << i));
          return true;
        }
      }
      if (!GamePad.GetState(controllerIndex).IsConnected)
        Console.WriteLine("Warning: Player " + playerNum + " is disconnected.");
      return false;
    }
  }
}