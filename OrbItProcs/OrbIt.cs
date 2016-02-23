using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpOVR;
using Rectangle = System.Drawing.Rectangle;

namespace OrbItProcs {
  public class OrbIt : VRGame {
    public static OrbIt Game;
    public static UserInterface UI;
    private FrameRateCounter frameRateCounter;
    public GameTime Time => this.gameTime;
    public static int ScreenWidth => Game.Graphics.PreferredBackBufferWidth;
    public static int ScreenHeight => Game.Graphics.PreferredBackBufferHeight;
    public static GlobalGameMode GlobalGameMode { get; set; }
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
      Window.AllowUserResizing = false;//Todo: make true, fix crash.
      Room = new Room(this, ScreenWidth, ScreenHeight);
      GlobalGameMode = new GlobalGameMode(this);
      frameRateCounter = new FrameRateCounter(this);
      Player.CreatePlayers(Room);
      UI = UserInterface.Start();
      UI.Initialize();
      Room.attatchToSidebar(UI);
      GlobalKeyBinds(UI);
    }

    protected override void Update(GameTime gt) {
      base.Update(gameTime);
      frameRateCounter.Update(gameTime);
      if (IsActive) UI.Update(gameTime);
      if (!UI.IsPaused) Room.Update(gameTime);
    }
    
    protected override void DrawScene(GameTime gt) {
      Room.Draw3D();
    }

    protected override void Dispose(bool disposeManagedResources) {
      base.Dispose(disposeManagedResources);
      if (disposeManagedResources) {
        OVR.Shutdown();
      }
    }
    
    private void GlobalKeyBinds(UserInterface ui) {
      ui.keyManager.addGlobalKeyAction("exitgame", KeyCodes.Escape, OnPress: () => Exit());
      //ui.keyManager.addGlobalKeyAction("togglesidebar", KeyCodes.OemTilde, OnPress: ui.ToggleSidebar);
      //ui.keyManager.addGlobalKeyAction("switchview", KeyCodes.PageDown, OnPress: ui.SwitchView);
      //TODO: ui.keyManager.addGlobalKeyAction("removeall", KeyCodes.Delete, OnPress: () => ui.sidebar.btnRemoveAllNodes_Click(null, null));
    }
  }
}