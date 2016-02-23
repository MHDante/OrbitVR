using OrbitVR.Framework;

namespace OrbitVR.Processes {
  public class PlayerProcess : Process {
    public PlayerProcess() {}

    protected override void OnActivate() {
      base.OnActivate();
      Player.TryCreatePcPlayer();
    }
  }
}