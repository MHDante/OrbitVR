using System;
using OrbitVR.PSMove;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpOVR;
using Device = SharpDX.Direct3D11.Device;

namespace OrbitVR.UI {
  public abstract class VRGame : Game {
    private float bodyYaw = 3.141592f;
    private EyeRenderDesc[] eyeRenderDesc = new EyeRenderDesc[2];
    private PoseF[] eyeRenderPose = new PoseF[2];
    private SwapTexture[] eyeTexture = new SwapTexture[2];
    public Vector3 headPos = new Vector3(0f, 0, 0);
    public HMD hmd;
    private Vector3[] hmdToEyeViewOffset = new Vector3[2];
    private LayerEyeFov layerEyeFov;
    private SharpDX.Direct3D11.Texture2D mirrorTexture;
    

    public Matrix projection;
    public PSMoveController PsMoveController;
    public Matrix view;

    public bool UsePsMove { get; } = false;

    protected VRGame(String name) : base(name){
      OVR.Initialize();
      hmd = OVR.HmdCreate(0) ?? OVR.HmdCreateDebug(HMDType.DK2);
      ToDispose(hmd);
      Width = hmd.Resolution.Width;
      Height = hmd.Resolution.Height;
      //Graphics.PreferredFullScreenOutputIndex = 1;
    }


    protected override void Initialize() {
      base.Initialize();

      if (UsePsMove) {
        var manager = new PSMoveManager();
        manager.Initialize();
        ToDispose(manager);
        PsMoveController = new PSMoveController(Vector3.Zero);
        ToDispose(PsMoveController);
      }
      eyeTexture[0] = hmd.CreateSwapTexture(GraphicsDevice, Format.B8G8R8A8_UNorm,
                                            hmd.GetFovTextureSize(EyeType.Left, hmd.DefaultEyeFov[0]), true);
      eyeTexture[1] = hmd.CreateSwapTexture(GraphicsDevice, Format.B8G8R8A8_UNorm,
                                            hmd.GetFovTextureSize(EyeType.Right, hmd.DefaultEyeFov[1]), true);
      ToDispose(eyeTexture[0]);
      ToDispose(eyeTexture[1]);

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
      mirrorTexture = hmd.CreateMirrorTexture(GraphicsDevice, backBuffer.Description);

      // Set presentation interval to immediate as SubmitFrame will be taking care of VSync
      //GraphicsDevice.Presenter.PresentInterval = PresentInterval.Immediate;

      // Configure tracking
      hmd.ConfigureTracking(
                            TrackingCapabilities.Orientation | TrackingCapabilities.Position |
                            TrackingCapabilities.MagYawCorrection,
                            TrackingCapabilities.None);

      // Set enabled capabilities
      hmd.EnabledCaps = HMDCapabilities.LowPersistence | HMDCapabilities.DynamicPrediction;
    }

    protected internal sealed override void Draw() {
      base.Draw();
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
        d3dDeviceContext.OutputMerger.SetRenderTargets(swapTexture.DepthStencilView, swapTexture.CurrentView);
        d3dDeviceContext.Rasterizer.SetViewport(swapTexture.Viewport);
        ((SharpDX.Direct3D11.Device) GraphicsDevice).ImmediateContext.ClearDepthStencilView(
                                                                                            swapTexture.DepthStencilView,
                                                                                            DepthStencilClearFlags.Depth |
                                                                                            DepthStencilClearFlags
                                                                                              .Stencil, 1, 0);
        ((SharpDX.Direct3D11.Device) GraphicsDevice).ImmediateContext.ClearRenderTargetView(swapTexture.CurrentView,
                                                                                            Color.CornflowerBlue);

        //if (UsePsMove) PsMoveController.Draw(GraphicsDevice, view, projection);Todo: model
        DrawScene();
      }
    }

    protected abstract void DrawScene();

    public override void EndDraw() {
      // Cancel original EndDraw() as the Present call is made through hmd.SubmitFrame().
      hmd.SubmitFrame(0, ref layerEyeFov.Header);

      // Copy the mirror texture to the back buffer and present it
      GraphicsDevice.ImmediateContext.CopyResource(mirrorTexture, backBuffer);
      swapChain.Present(1, PresentFlags.None);
    }

    public override void Update() {
      base.Update();
      if (UsePsMove) {
        PSMoveManager.GetManagerInstance().Update();
        PsMoveController.Update();
      }
    }

    protected override void Dispose(bool disposeManagedResources) {
      base.Dispose(disposeManagedResources);
      if (disposeManagedResources) {
        OVR.Shutdown();
      }
    }
  }
}