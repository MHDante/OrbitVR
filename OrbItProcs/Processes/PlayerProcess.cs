namespace OrbItProcs {
  public class PlayerProcess : Process {
    public PlayerProcess() {}

    protected override void OnActivate() {
      base.OnActivate();
      Player.TryCreatePcPlayer();
    }
  }
}