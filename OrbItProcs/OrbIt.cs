using System;
using Polenter.Serialization;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpOVR;
using Rectangle = System.Drawing.Rectangle;
using SColor = System.Drawing.Color;
using sc = System.Console;

namespace OrbItProcs {
  public enum resolutions {
    AutoFullScreen,

    VGA_640x480,
    SVGA_800x600,
    XGA_1024x768,
    HD_1366x768,
    WXGA_1280x800,
    SXGA_1280x1024,
    WSXGA_1680x1050,
    FHD_1920x1080,
  }

  public abstract class VRGame : Game {
    public bool UsePsMove { get; } = false;
  }

  public class OrbIt : VRGame {
    public static OrbIt game;
    public static UserInterface ui;
    public static GameTime gametime;
    public static bool isFullScreen = false;
    private static bool GraphicsReset = false;
    private static bool redrawWhenPaused = false;
    private static GlobalGameMode _globalGameMode = null;
    public static Action OnUpdate;
    public BlendStateCollection BlendStates;

    /////////////////VR///////////////////
    private float bodyYaw = 3.141592f;
    private EyeRenderDesc[] eyeRenderDesc = new EyeRenderDesc[2];
    private PoseF[] eyeRenderPose = new PoseF[2];
    private SwapTexture[] eyeTexture = new SwapTexture[2];
    private FrameRateCounter frameRateCounter;
    private GeometricPrimitive gameScreenQuad;
    public GraphicsDeviceManager Graphics;
    private Vector3 headPos = new Vector3(0f, 50f, -5f);
    public HMD hmd;
    private Vector3[] hmdToEyeViewOffset = new Vector3[2];
    Model landscape;
    private LayerEyeFov layerEyeFov;
    private SharpDX.Direct3D11.Texture2D mirrorTexture;
    private Model model;

    public SharpDX.Direct2D1.PixelFormat pixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.R8G8B8A8_UNorm,
                                                                                         AlphaMode.Premultiplied);

    private Matrix projection;

    public PSMoveController PsMoveController;

    public SharpSerializer serializer = new SharpSerializer();
    private Model ship;
    private Matrix view;
    private Model wand;
    private Vector3 worldOrigin = new Vector3(0, 0, 2);
    private float worldRotationY = 0f; //MathHelper.Pi;

    private OrbIt() {
      // Creates a graphics manager. This is mandatory.
      Graphics = new GraphicsDeviceManager(this);
      Graphics.DeviceCreationFlags |= DeviceCreationFlags.BgraSupport;
      // Setup the relative directory to the executable directory 
      // for loading contents with the ContentManager
      Content.RootDirectory = "Content";

      // Initialize OVR Library
      OVR.Initialize();

      // Create our HMD
      hmd = OVR.HmdCreate(0) ?? OVR.HmdCreateDebug(HMDType.DK2);

      // Match back buffer size with HMD resolution
      Graphics.PreferredBackBufferWidth = hmd.Resolution.Width;
      Graphics.PreferredBackBufferHeight = hmd.Resolution.Height;
      Graphics.PreferredFullScreenOutputIndex = 1;
    }


    public static int ScreenWidth {
      get { return game.Graphics.PreferredBackBufferWidth; }
      set { game.Graphics.PreferredBackBufferWidth = value; }
    }

    public static int ScreenHeight {
      get { return game.Graphics.PreferredBackBufferHeight; }
      set { game.Graphics.PreferredBackBufferHeight = value; }
    }

    public static int GameAreaWidth {
      get { return ScreenWidth; }
    }

    public static int GameAreaHeight {
      get { return ScreenHeight; }
    }

    public resolutions? prefFullScreenResolution { get; set; }
    public resolutions prefWindowedResolution { get; set; }

    public static GlobalGameMode globalGameMode {
      get { return OrbIt._globalGameMode; }
      set {
        OrbIt._globalGameMode = value;
        //if (ui != null && ui.sidebar != null)
        //{
        //    ui.sidebar.gamemodeWindow = new GamemodeWindow(ui.sidebar);
        //}
      }
    }

    public Room room { get; set; }

    protected override void LoadContent() {
      base.LoadContent();

      landscape = Content.Load<Model>("landscape");
      if(UsePsMove)wand = Content.Load<Model>("PSMove");
      ship = Content.Load<Model>("Ship");

      if (UsePsMove) PsMoveController.Initialize(Vector3.Zero, wand);
    }

    protected override void Initialize() {
      base.Initialize();
      InitializeVR();
      Assets.LoadAssets(Content);
      Window.AllowUserResizing = false;
      room = new Room(this, ScreenWidth, ScreenHeight);
      globalGameMode = new GlobalGameMode(this);
      frameRateCounter = new FrameRateCounter(this);

      gameScreenQuad = GeometricPrimitive.Plane.New(GraphicsDevice, 2, 2, 32, true);

      //gameScreenQuad = GeometricPrimitive.Sphere.New(GraphicsDevice, 5, 32, true);
      //gameScreenQuad = GeometricPrimitive.Cylinder.New(GraphicsDevice,2,2,32,true);

      Player.CreatePlayers(room);
      ui = UserInterface.Start();
      ui.Initialize();
      room.attatchToSidebar(ui);
      GlobalKeyBinds(ui);
      if (UsePsMove) new PSMoveManager().Initialize();
      if (UsePsMove) PsMoveController = new PSMoveController();
    }

    private void InitializeVR() {
      eyeTexture[0] = hmd.CreateSwapTexture(GraphicsDevice, Format.B8G8R8A8_UNorm,
                                            hmd.GetFovTextureSize(EyeType.Left, hmd.DefaultEyeFov[0]), true);
      eyeTexture[1] = hmd.CreateSwapTexture(GraphicsDevice, Format.B8G8R8A8_UNorm,
                                            hmd.GetFovTextureSize(EyeType.Right, hmd.DefaultEyeFov[1]), true);

      // Create our layer
      layerEyeFov = new LayerEyeFov {
        Header = new LayerHeader(LayerType.EyeFov, LayerFlags.None),
        ColorTextureLeft = eyeTexture[0].TextureSet,
        ColorTextureRight = eyeTexture[1].TextureSet,
        ViewportLeft = new Rect(0, 0, eyeTexture[0].Size.Width, eyeTexture[0].Size.Height),
        ViewportRight = new Rect(0, 0, eyeTexture[1].Size.Width, eyeTexture[1].Size.Height),
        FovLeft = hmd.DefaultEyeFov[0],
        FovRight = hmd.DefaultEyeFov[1],
      };

      // Keep eye view offsets
      eyeRenderDesc[0] = hmd.GetRenderDesc(EyeType.Left, hmd.DefaultEyeFov[0]);
      eyeRenderDesc[1] = hmd.GetRenderDesc(EyeType.Right, hmd.DefaultEyeFov[1]);
      hmdToEyeViewOffset[0] = eyeRenderDesc[0].HmdToEyeViewOffset;
      hmdToEyeViewOffset[1] = eyeRenderDesc[1].HmdToEyeViewOffset;

      // Create a mirror texture
      mirrorTexture = hmd.CreateMirrorTexture(GraphicsDevice, GraphicsDevice.BackBuffer.Description);

      // Set presentation interval to immediate as SubmitFrame will be taking care of VSync
      GraphicsDevice.Presenter.PresentInterval = PresentInterval.Immediate;

      // Configure tracking
      hmd.ConfigureTracking(
                            TrackingCapabilities.Orientation | TrackingCapabilities.Position |
                            TrackingCapabilities.MagYawCorrection,
                            TrackingCapabilities.None);

      // Set enabled capabilities
      //hmd.EnabledCaps = HMDCapabilities.LowPersistence | HMDCapabilities.DynamicPrediction;
    }

    protected override void Update(GameTime gameTime) {
      base.Update(gameTime);
      OrbIt.gametime = gameTime;
      frameRateCounter.Update(gameTime);
      if (IsActive) ui.Update(gameTime);

      if (!ui.IsPaused) room.Update(gameTime);
      else if (redrawWhenPaused) room.drawOnly();


      if (GraphicsReset) {
        Graphics.ApplyChanges();
        room.roomRenderTarget = RenderTarget2D.New(GraphicsDevice,
                                                   ScreenWidth,
                                                   ScreenHeight,
                                                   pixelFormat.Format);

        GraphicsReset = false;
      }
      if (OnUpdate != null) OnUpdate.Invoke();

      if (UsePsMove) PSMoveManager.GetManagerInstance().Update();
      if (UsePsMove) PsMoveController.Update();
    }

    protected override void Draw(GameTime gameTime) {
      var rollPitchYaw = Matrix.RotationY(bodyYaw);

      // Get eye poses
      hmd.GetEyePoses(0, hmdToEyeViewOffset, eyeRenderPose);
      layerEyeFov.RenderPoseLeft = eyeRenderPose[0];
      layerEyeFov.RenderPoseRight = eyeRenderPose[1];

      for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++) {
        var eye = hmd.EyeRenderOrder[eyeIndex];
        var renderDesc = eyeRenderDesc[(int) eye];
        var renderPose = eyeRenderPose[(int) eye];

        // Calculate view matrix  
        Matrix finalRollPitchYaw = rollPitchYaw*renderPose.Orientation.GetMatrix();
        var finalUp = finalRollPitchYaw.Transform(Vector3.UnitY);
        var finalForward = finalRollPitchYaw.Transform(-Vector3.UnitZ);
        var shiftedEyePos = headPos + rollPitchYaw.Transform(renderPose.Position);
        view = Matrix.LookAtRH(shiftedEyePos, shiftedEyePos + finalForward, finalUp);

        // Calculate projection matrix
        projection = OVR.MatrixProjection(renderDesc.Fov, 0.01f, 10000.0f, Projection.RightHanded);
        projection.Transpose();

        // Get render target
        var swapTexture = eyeTexture[(int) eye];
        swapTexture.AdvanceToNextView();

        // Clear the screen
        GraphicsDevice.SetRenderTargets(swapTexture.DepthStencilView, swapTexture.CurrentView);
        GraphicsDevice.SetViewport(swapTexture.Viewport);
        ((SharpDX.Direct3D11.Device) GraphicsDevice).ImmediateContext.ClearDepthStencilView(
                                                                                            swapTexture.DepthStencilView,
                                                                                            DepthStencilClearFlags.Depth |
                                                                                            DepthStencilClearFlags
                                                                                              .Stencil, 1, 0);
        ((SharpDX.Direct3D11.Device) GraphicsDevice).ImmediateContext.ClearRenderTargetView(swapTexture.CurrentView,
                                                                                            Color.CornflowerBlue);

        // Perform the actual drawing
        DrawScene(gameTime);
      }
    }

    protected void DrawScene(GameTime gameTime) {
      Rectangle frame = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
      var quadEffect = new BasicEffect(GraphicsDevice) {
        World = Matrix.RotationY(worldRotationY)*Matrix.Translation(headPos + worldOrigin),
        View = view,
        Projection = projection,
        TextureEnabled = true,
        Texture = room.roomRenderTarget
      };
      foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes) {
        pass.Apply();
      
        gameScreenQuad.Draw();
      }
      DrawLandscape(gameTime);
      if (UsePsMove) PsMoveController.Draw(GraphicsDevice, view, projection);


      if (room.camera.TakeScreenshot) {
        room.camera.Screenshot();
        room.camera.TakeScreenshot = false;
      }
    }

    protected override void EndDraw() {
      // Cancel original EndDraw() as the Present call is made through hmd.SubmitFrame().
      hmd.SubmitFrame(0, ref layerEyeFov.Header);

      // Copy the mirror texture to the back buffer and present it
      GraphicsDevice.Copy(mirrorTexture, GraphicsDevice.BackBuffer);
      GraphicsDevice.Present();
    }

    protected override void OnExiting(object sender, EventArgs args) {
      base.OnExiting(sender, args);
      Dispose(true);
    }

    protected override void Dispose(bool disposeManagedResources) {
      base.Dispose(disposeManagedResources);
      if (UsePsMove) PsMoveController.OnDestroy();
      if (UsePsMove) PSMoveManager.GetManagerInstance().OnApplicationQuit();

      if (disposeManagedResources) {
        // Release the eye textures
        eyeTexture[0].Dispose();
        eyeTexture[1].Dispose();

        // Release the HMD
        hmd.Dispose();

        // Shutdown the OVR Library
        OVR.Shutdown();
      }
    }

    public static void Start() {
      if (game != null) throw new SystemException("Game was already Started");
      game = new OrbIt();
      game.Run(); //XNA LOGIC HAPPENS. IT HAPPENS.
    }

    private void GlobalKeyBinds(UserInterface ui) {
      ui.keyManager.addGlobalKeyAction("exitgame", KeyCodes.Escape, OnPress: () => Exit());
      //ui.keyManager.addGlobalKeyAction("togglesidebar", KeyCodes.OemTilde, OnPress: ui.ToggleSidebar);
      //ui.keyManager.addGlobalKeyAction("switchview", KeyCodes.PageDown, OnPress: ui.SwitchView);
      //TODO: ui.keyManager.addGlobalKeyAction("removeall", KeyCodes.Delete, OnPress: () => ui.sidebar.btnRemoveAllNodes_Click(null, null));
    }

    public void DrawLandscape(GameTime gameTime) {
      Vector3 lightDirection = Vector3.Normalize(new Vector3(3, -1, 1));
      Vector3 lightColor = new Vector3(0.3f, 0.4f, 0.2f);

      // First we draw the ground geometry using BasicEffect.
      foreach (ModelMesh mesh in landscape.Meshes) {
        if (mesh.Name != "Billboards") {
          foreach (BasicEffect effect in mesh.Effects) {
            effect.View = game.view;
            effect.Projection = game.projection;

            effect.LightingEnabled = true;

            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.Direction = lightDirection;
            effect.DirectionalLight0.DiffuseColor = lightColor;

            effect.AmbientLightColor = new Vector3(0.1f, 0.2f, 0.1f);
          }
          GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.Opaque);
          GraphicsDevice.SetDepthStencilState(GraphicsDevice.DepthStencilStates.Default);
          GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.CullBack);

          mesh.Draw(GraphicsDevice, new Matrix[0]);
        }
      }
    }
  }
}