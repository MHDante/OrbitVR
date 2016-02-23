namespace OrbitVR.Components.AffectOthers {
  public interface IMultipliable {
    Node parent { get; set; }
    bool active { get; set; }
    float multiplier { get; set; }
  }
}