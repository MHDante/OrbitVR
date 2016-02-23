using System;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace OrbitVR.PSMove {
  public class PSMoveController : IDisposable {
    // Used to send and receive controller data from the PSMoveWorker thread
    private PSMoveDataContext dataContext;
    public float lerpRate;
    private Vector3 lerpTarget;

    // PSMove controller ID - 0-based
    public int PSMoveID = 0;
    public Transform transform;
    public bool useInterpolation;

    // Pressed This Frame
    public event EventHandler OnButtonTrianglePressed;
    public event EventHandler OnButtonCirclePressed;
    public event EventHandler OnButtonCrossPressed;
    public event EventHandler OnButtonSquarePressed;
    public event EventHandler OnButtonSelectPressed;
    public event EventHandler OnButtonStartPressed;
    public event EventHandler OnButtonPSPressed;
    public event EventHandler OnButtonMovePressed;

    // Released This Frame
    public event EventHandler OnButtonTriangleReleased;
    public event EventHandler OnButtonCircleReleased;
    public event EventHandler OnButtonCrossReleased;
    public event EventHandler OnButtonSquareReleased;
    public event EventHandler OnButtonSelectReleased;
    public event EventHandler OnButtonStartReleased;
    public event EventHandler OnButtonPSReleased;
    public event EventHandler OnButtonMoveReleased;

    #region Controller Properties

    public bool IsConnected {
      get { return dataContext.GetIsConnected(); }
    }

    public bool IsEnabled {
      get { return dataContext.GetIsTrackingEnabled(); }
    }

    public bool IsTracking {
      get { return dataContext.GetIsSeenByTracker(); }
    }

    public float TriggerValue {
      get { return (float) dataContext.GetTriggerValue()/255.0f; }
    }

    public bool IsTriangleButtonDown {
      get { return dataContext.GetButtonTriangle(); }
    }

    public bool IsCircleButtonDown {
      get { return dataContext.GetButtonCircle(); }
    }

    public bool IsCrossButtonDown {
      get { return dataContext.GetButtonCross(); }
    }

    public bool IsSquareButtonDown {
      get { return dataContext.GetButtonSquare(); }
    }

    public bool IsSelectButtonDown {
      get { return dataContext.GetButtonSelect(); }
    }

    public bool IsStartButtonDown {
      get { return dataContext.GetButtonStart(); }
    }

    public bool IsPSButtonDown {
      get { return dataContext.GetButtonPS(); }
    }

    public bool IsMoveButtonDown {
      get { return dataContext.GetButtonMove(); }
    }

    // Debug
    public bool ShowTrackingDebug;
    public bool ShowHMDFrustumDebug;
    private Model model;

    #endregion


    /// <summary>
    /// NOTE! This function does NOT pair the controller by Bluetooth.
    /// If the controller is not already paired, it can only be connected by USB.
    /// See README for more information.
    /// </summary>
    public PSMoveController(Vector3 Position, Model m = null)
    {
      model = m ??  OrbIt.Game.Content.Load<Model>("PSMove");
      transform = new Transform();
      transform.parent = new Transform() { position = Position };
      if (PSMoveManager.GetManagerInstance() != null)
      {
        dataContext = PSMoveManager.GetManagerInstance().AcquirePSMove(this.PSMoveID);
      }
      model = m;
    }

    public void Dispose()
    {
      if (PSMoveManager.GetManagerInstance() != null)
      {
        PSMoveManager.GetManagerInstance().ReleasePSMove(dataContext);
      }
    }


    #region Controller Actions

    /// <summary>
    /// Converts the current orientation to the identity orientation
    /// </summary
    public void ResetYaw() {
      if (dataContext != null) {
        dataContext.ResetYaw();
      }
    }

    public void CycleTrackingColor() {
      if (dataContext != null) {
        dataContext.PostCycleColourRequest();
      }
    }

    /// <summary>
    /// Sets the amount of rumble
    /// </summary>
    /// <param name="rumble">the rumble amount (0-1)</param>
    public void SetRumble(float rumble) {
      if (dataContext != null) {
        // Clamp value between 0 and 1:
        dataContext.PostRumbleRequest((byte) (Math.Min(Math.Max(rumble, 0f), 1f)*255));
      }
    }

    #endregion

    #region Unity Callbacks

    public void Draw(GraphicsDevice device, Matrix v, Matrix p) {
      model.Draw(device, transform.getMatrix(), v, p);
    }

    public void Update() {
      // Get the latest state from the 
      dataContext.ComponentRead(transform.parent);

      // Button Pressed Handlers
      if (OnButtonTrianglePressed != null && dataContext.GetButtonTrianglePressed())
        OnButtonTrianglePressed(this, EventArgs.Empty);
      if (OnButtonCirclePressed != null && dataContext.GetButtonCirclePressed())
        OnButtonCirclePressed(this, EventArgs.Empty);
      if (OnButtonCrossPressed != null && dataContext.GetButtonCrossPressed())
        OnButtonCrossPressed(this, EventArgs.Empty);
      if (OnButtonSquarePressed != null && dataContext.GetButtonSquarePressed())
        OnButtonSquarePressed(this, EventArgs.Empty);
      if (OnButtonSelectPressed != null && dataContext.GetButtonSelectPressed())
        OnButtonSelectPressed(this, EventArgs.Empty);
      if (OnButtonStartPressed != null && dataContext.GetButtonStartPressed())
        OnButtonStartPressed(this, EventArgs.Empty);
      if (OnButtonPSPressed != null && dataContext.GetButtonPSPressed())
        OnButtonPSPressed(this, EventArgs.Empty);
      if (OnButtonMovePressed != null && dataContext.GetButtonMovePressed())
        OnButtonMovePressed(this, EventArgs.Empty);

      // Button Released Handlers
      if (OnButtonTriangleReleased != null && dataContext.GetButtonTriangleReleased())
        OnButtonTriangleReleased(this, EventArgs.Empty);
      if (OnButtonCircleReleased != null && dataContext.GetButtonCircleReleased())
        OnButtonCircleReleased(this, EventArgs.Empty);
      if (OnButtonCrossReleased != null && dataContext.GetButtonCrossReleased())
        OnButtonCrossReleased(this, EventArgs.Empty);
      if (OnButtonSquareReleased != null && dataContext.GetButtonSquareReleased())
        OnButtonSquareReleased(this, EventArgs.Empty);
      if (OnButtonSelectReleased != null && dataContext.GetButtonSelectReleased())
        OnButtonSelectReleased(this, EventArgs.Empty);
      if (OnButtonStartReleased != null && dataContext.GetButtonStartReleased())
        OnButtonStartReleased(this, EventArgs.Empty);
      if (OnButtonPSReleased != null && dataContext.GetButtonPSReleased())
        OnButtonPSReleased(this, EventArgs.Empty);
      if (OnButtonMoveReleased != null && dataContext.GetButtonMoveReleased())
        OnButtonMoveReleased(this, EventArgs.Empty);

      bool seen = dataContext.GetIsSeenByTracker();
      // Update the transform of this game object based on the new pose

      if (useInterpolation) {
        if (seen) {
          lerpTarget = dataContext.Pose.WorldPosition;
        }
        transform.position = Vector3.Lerp(transform.position, lerpTarget, lerpRate);
      }
      else if (seen) {
        transform.position = dataContext.Pose.WorldPosition;
      }
      transform.rotation = dataContext.Pose.WorldOrientation;
    }

    #endregion
  }
}