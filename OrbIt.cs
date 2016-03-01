using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX.Direct3D11;

namespace OrbitVR {
  public class OrbIt : VRGame {
    public static OrbIt Game;
    public static UserInterface UI;
    private FrameRateCounter _frameRateCounter;
    
    public static int ScreenWidth => Game.Width;
    public static int ScreenHeight => Game.Height;
    public static GlobalGameMode GlobalGameMode { get; private set; }
    public Room Room { get; private set; }
    public ProcessManager ProcessManager { get; private set; }

    protected OrbIt():base("Orbit") {
      Game = this;
      DeviceCreationFlags |= DeviceCreationFlags.BgraSupport;
    }

    protected override void Initialize() {
      base.Initialize();
      Assets.LoadAssets();
      Window.AllowUserResizing = false; //Todo: make true, fix crash.
      Room = new Room(ScreenWidth*2, ScreenHeight);
      GlobalGameMode = new GlobalGameMode(this);
      _frameRateCounter = new FrameRateCounter(this);
      Player.CreatePlayers(Room);
      UI = new UserInterface();
      ProcessManager = new ProcessManager();
      ProcessManager.SetProcessKeybinds();
      Room.ActiveGroupName = "Group1";

      GlobalKeyBinds(UI);
    }

    public override void Update() {
      base.Update();
      _frameRateCounter.Update();
      UI.Update();
      
      if (!UI.IsPaused) {
        Room.Update();
        Room.Draw();
        ProcessManager.Draw();
      }
    }

    protected override void DrawScene() {
      Room.Draw3D();
    }

    private void GlobalKeyBinds(UserInterface ui) {
      ui.keyManager.addGlobalKeyAction("exitgame", KeyCodes.Escape, OnPress: Window.Close);
      ui.keyManager.addGlobalKeyAction("removeall", KeyCodes.Delete, OnPress: Room.EmptyCurrentGroup);
    }
  }
}