using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpOVR;

namespace OrbitVR {
  // Use these namespaces here to override SharpDX.Direct3D11

  /// <summary>
  /// Simple RiftGame game using SharpDX.Toolkit.
  /// </summary>
  public class RiftGame : Game {
    public float bodyYaw = 3.141592f;
    public EyeRenderDesc[] eyeRenderDesc = new EyeRenderDesc[2];
    public PoseF[] eyeRenderPose = new PoseF[2];
    public SwapTexture[] eyeTexture = new SwapTexture[2];
    public GraphicsDeviceManager graphicsDeviceManager;

    public Vector3 headPos = new Vector3(0f, 0f, -5f);

    public HMD hmd;
    public Vector3[] hmdToEyeViewOffset = new Vector3[2];

    public LayerEyeFov layerEyeFov;
    public SharpDX.Direct3D11.Texture2D mirrorTexture;

    public Model model;
    public Matrix projection;

    public Matrix view;

    protected SharpDX.Direct3D11.Device Device {
      get { return (SharpDX.Direct3D11.Device) GraphicsDevice; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiftGame" /> class.
    /// </summary>
    public RiftGame() {
      // Creates a graphics manager. This is mandatory.
      graphicsDeviceManager = new GraphicsDeviceManager(this);

      // Setup the relative directory to the executable directory 
      // for loading contents with the ContentManager
      Content.RootDirectory = "Content";

      // Initialize OVR Library
      OVR.Initialize();

      // Create our HMD
      hmd = OVR.HmdCreate(0) ?? OVR.HmdCreateDebug(HMDType.DK2);

      // Match back buffer size with HMD resolution
      graphicsDeviceManager.PreferredBackBufferWidth = hmd.Resolution.Width;
      graphicsDeviceManager.PreferredBackBufferHeight = hmd.Resolution.Height;
      graphicsDeviceManager.PreferredFullScreenOutputIndex = 1;
    }

    protected override void Initialize() {
      // Modify the title of the window
      Window.Title = "RiftGame";

      // Create our render target
      eyeTexture[0] = hmd.CreateSwapTexture(GraphicsDevice, Format.R8G8B8A8_UNorm,
                                            hmd.GetFovTextureSize(EyeType.Left, hmd.DefaultEyeFov[0]), true);
      eyeTexture[1] = hmd.CreateSwapTexture(GraphicsDevice, Format.R8G8B8A8_UNorm,
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
      hmd.EnabledCaps = HMDCapabilities.LowPersistence | HMDCapabilities.DynamicPrediction;

      base.Initialize();
    }

    protected override void LoadContent() {
      // Load a 3D model
      // The [Ship.fbx] file is defined with the build action [ToolkitModel] in the project
      model = Content.Load<Model>("Ship");

      // Enable default lighting on model.
      BasicEffect.EnableDefaultLighting(model, true);

      base.LoadContent();
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
        Device.ImmediateContext.ClearDepthStencilView(swapTexture.DepthStencilView,
                                                      DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1,
                                                      0);
        Device.ImmediateContext.ClearRenderTargetView(swapTexture.CurrentView, Color.CornflowerBlue);

        // Perform the actual drawing
        InternalDraw(gameTime);
      }
    }

    protected override void EndDraw() {
      // Cancel original EndDraw() as the Present call is made through hmd.SubmitFrame().
      hmd.SubmitFrame(0, ref layerEyeFov.Header);

      // Copy the mirror texture to the back buffer and present it
      GraphicsDevice.Copy(mirrorTexture, GraphicsDevice.BackBuffer);
      GraphicsDevice.Present();
    }

    protected virtual void InternalDraw(GameTime gameTime) {
      // Use time in seconds directly
      var time = (float) gameTime.TotalGameTime.TotalSeconds;

      // ------------------------------------------------------------------------
      // Draw the 3d model
      // ------------------------------------------------------------------------
      var world = Matrix.Scaling(0.003f)*
                  Matrix.RotationY(time)*
                  Matrix.Translation(0, -1.5f, 2.0f);
      model.Draw(GraphicsDevice, world, view, projection);
      base.Draw(gameTime);
    }

    protected override void Dispose(bool disposeManagedResources) {
      base.Dispose(disposeManagedResources);
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
  }
}