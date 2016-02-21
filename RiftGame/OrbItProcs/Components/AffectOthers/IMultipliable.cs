namespace OrbItProcs {
  public interface IMultipliable {
    Node parent { get; set; }
    bool active { get; set; }
    float multiplier { get; set; }
  }
}