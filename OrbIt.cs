using OrbitVR.Framework;
using OrbitVR.Interface;
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
    public OrbIt()
    {
      Game = this;
      Graphics.DeviceCreationFlags |= DeviceCreationFlags.BgraSupport;
      Content.RootDirectory = "Content";
    }
    protected override void Initialize() {
      base.Initialize();
      Assets.LoadAssets(Content);
      Window.AllowUserResizing = false; //Todo: make true, fix crash.
      Room = new Room(ScreenWidth, ScreenHeight);
      GlobalGameMode = new GlobalGameMode(this);
      _frameRateCounter = new FrameRateCounter(this);
      Player.CreatePlayers(Room);
      UI = UserInterface.Start();
      UI.Initialize();
      Room.attatchToSidebar(UI);
      GlobalKeyBinds(UI);
    }
    protected override void Update(GameTime gt) {
      base.Update(gameTime);
      _frameRateCounter.Update(gameTime);
      if (IsActive) UI.Update(gameTime);
      if (!UI.IsPaused) Room.Update(gameTime);
    }
    protected override void DrawScene(GameTime gt) {
      Room.Draw3D();
    }
    private void GlobalKeyBinds(UserInterface ui) {
      ui.keyManager.addGlobalKeyAction("exitgame", KeyCodes.Escape, OnPress: Exit);
      //ui.keyManager.addGlobalKeyAction("togglesidebar", KeyCodes.OemTilde, OnPress: ui.ToggleSidebar);
      //ui.keyManager.addGlobalKeyAction("switchview", KeyCodes.PageDown, OnPress: ui.SwitchView);
      //ui.keyManager.addGlobalKeyAction("removeall", KeyCodes.Delete, OnPress: () => ui.sidebar.btnRemoveAllNodes_Click(null, null));
    }
  }
}