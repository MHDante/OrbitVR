
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
namespace OrbitVR
{
  public class OrbIt : VRGame
  {
    public static OrbIt Game;
    public static UserInterface UI;
    private FrameRateCounter _frameRateCounter;

    public GameTime Time => gameTime;
    public static int ScreenWidth => Game.Graphics.PreferredBackBufferWidth;
    public static int ScreenHeight => Game.Graphics.PreferredBackBufferHeight;
    public static GlobalGameMode GlobalGameMode { get; private set; }
    public Room Room { get; private set; }
    public ProcessManager ProcessManager { get; private set; }

    private Model model;
    protected OrbIt()
    {
      Game = this;
      Graphics.DeviceCreationFlags |= DeviceCreationFlags.BgraSupport;
      Content.RootDirectory = "Content";

    }

    protected override void Initialize()
    {
      base.Initialize();
      Assets.LoadAssets();
      Window.AllowUserResizing = false; //Todo: make true, fix crash.
      Room = new Room(ScreenWidth, ScreenHeight);
      GlobalGameMode = new GlobalGameMode(this);
      _frameRateCounter = new FrameRateCounter(this);
      Player.CreatePlayers(Room);
      UI = new UserInterface();
      ProcessManager = new ProcessManager();
      ProcessManager.SetProcessKeybinds();
      Room.ActiveGroupName = "Group1";

      GlobalKeyBinds(UI);
      model = Content.Load<Model>("Ship");
      BasicEffect.EnableDefaultLighting(model, true);


    }

    protected override void Update(GameTime gt)
    {
      base.Update(gameTime);
      _frameRateCounter.Update();
      if (IsActive) UI.Update();
      
      if (!UI.IsPaused)
      {
        Room.Update();
        Room.Draw();
        ProcessManager.Draw();
      }
    }

    protected override void DrawScene(GameTime gt)
    {

      // Use time in seconds directly
      //var time = (float)gameTime.TotalGameTime.TotalSeconds;
      var world = Matrix.Scaling(1f) *
            Matrix.RotationY(MathHelper.Pi) *
            Matrix.Translation(0, 0, 200);
      //model.Draw(GraphicsDevice, world, view, projection);
      Room.Draw3D(world);
    }

    private void GlobalKeyBinds(UserInterface ui)
    {
      ui.keyManager.addGlobalKeyAction("exitgame", KeyCodes.Escape, OnPress: Exit);
      ui.keyManager.addGlobalKeyAction("removeall", KeyCodes.Delete, OnPress: Room.EmptyCurrentGroup);
    }
  }
}