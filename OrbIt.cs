
using System;
using System.Threading;
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
    public static int ScreenWidth => 360;
    public static int ScreenHeight => 360;
    public static GlobalGameMode GlobalGameMode { get; private set; }
    public Room Room { get; private set; }
    public ProcessManager ProcessManager { get; private set; }

    private Model model;
    Model landscape;
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
      landscape = Content.Load<Model>("landscape");

      BasicEffect.EnableDefaultLighting(model, true);

     
    }
    

    protected override void UpdateAsync()
    {
      _frameRateCounter.Update();
      if (IsActive) UI.Update();
      
      if (!UI.IsPaused)
      {
        Room.Update();
        Room.Draw();
        ProcessManager.Draw();
      }
      Room.Camera.EndDrawing();

    }


    protected override void DrawScene(GameTime gt)
    {



      // Use time in seconds directly
      var time = (float)Time.TotalGameTime.TotalSeconds;

      var world =
        //Matrix.Translation(-ScreenWidth/2, - ScreenHeight/2, -5) * 
        Matrix.Scaling(-1f) *
        Matrix.RotationY(MathHelper.Pi)*
        Matrix.Identity;
      //model.Draw(GraphicsDevice, world, view, projection);
      DrawLandscape(gt);
      Room.Draw3D(world);
    }
    public void DrawLandscape(GameTime gameTime)
    {

      Vector3 lightDirection = Vector3.Normalize(new Vector3(3, -1, 1));
      Vector3 lightColor = new Vector3(0.3f, 0.4f, 0.2f);

      // First we draw the ground geometry using BasicEffect.
      foreach (ModelMesh mesh in landscape.Meshes)
      {
        if (mesh.Name != "Billboards")
        {
          foreach (BasicEffect effect in mesh.Effects)
          {
            effect.World = Matrix.Translation(0,-40,0);
            effect.View = view;
            effect.Projection = projection;

            effect.LightingEnabled = true;

            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.Direction = lightDirection;
            effect.DirectionalLight0.DiffuseColor = lightColor;

            effect.AmbientLightColor = new Vector3(0.1f, 0.2f, 0.1f);
          }
          GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.Opaque);
          GraphicsDevice.SetDepthStencilState(GraphicsDevice.DepthStencilStates.Default);
          GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullBack);

          mesh.Draw(GraphicsDevice,null);
        }
      }
    }
    private void GlobalKeyBinds(UserInterface ui)
    {
      ui.keyManager.addGlobalKeyAction("exitgame", KeyCodes.Escape, OnPress: Exit);
      ui.keyManager.addGlobalKeyAction("removeall", KeyCodes.Delete, OnPress: Room.EmptyCurrentGroup);
    }
  }
}