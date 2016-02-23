using System.Collections.Generic;
using OrbitVR.UI;

namespace OrbitVR.Components.Movement {
  /// <summary>
  /// Adds a friction force to the node, slowing it down.
  /// </summary>
  [Info(UserLevel.User, "Adds a friction force to the node, slowing it down.")]
  public class Friction : Component {
    public enum material {
      ice,
      wax,
      wetfloor,
      grass,
      soil,
      dirt,
      asphalt,
      rubber,
    }

    public const mtypes CompType = mtypes.affectself;

    public static Dictionary<material, float> coefficients = new Dictionary<material, float>() {
      {material.ice, 1},
      {material.wax, 3},
      {material.wetfloor, 5},
      {material.grass, 7},
      {material.soil, 12},
      {material.dirt, 20},
      {material.asphalt, 35},
      {material.rubber, 50},
    };

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    public float force { get; set; }

    public Friction() : this(null) {}

    public Friction(Node parent) {
      this.parent = parent;
      force = 0.01f;
    }

    public override void AffectSelf() {
      parent.body.velocity *= 1 - force*parent.body.mass;
    }
  }
}