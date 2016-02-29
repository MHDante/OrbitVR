using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX.Direct3D11;
using SharpDX.Toolkit;

namespace OrbitVR {
  public class OrbIt : VRGame {
    public static OrbIt Game;
    public static UserInterface UI;
    private FrameRateCounter _frameRateCounter;

    public GameTime Time => gameTime;
    public static int ScreenWidth => Game.Graphics.PreferredBackBufferWidth;
    public static int ScreenHeight => Game.Graphics.PreferredBackBufferHeight;
    public static GlobalGameMode GlobalGameMode { get; private set; }
    public Room Room { get; private set; }
    public ProcessManager ProcessManager { get; private set; }

    protected OrbIt() {
      Game = this;
      Graphics.DeviceCreationFlags |= DeviceCreationFlags.BgraSupport;
      Content.RootDirectory = "Content";
    }

    protected override void Initialize() {
      base.Initialize();
      Assets.LoadAssets(Content);
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

    protected override void Update(GameTime gt) {
      base.Update(gameTime);
      _frameRateCounter.Update(gameTime);
      if (IsActive) UI.Update(gameTime);

      GameSystems.Add(new EffectCompilerSystem(this));
      if (!UI.IsPaused) {
        Room.Update();
        Room.Draw();
        ProcessManager.Draw();
      }
    }

    protected override void DrawScene(GameTime gt) {
      Room.Draw3D();
    }

    private void GlobalKeyBinds(UserInterface ui) {
      ui.keyManager.addGlobalKeyAction("exitgame", KeyCodes.Escape, OnPress: Exit);
      ui.keyManager.addGlobalKeyAction("removeall", KeyCodes.Delete, OnPress: Room.EmptyCurrentGroup);
    }
  }
}