﻿using OrbitVR.UI;

namespace OrbitVR.Components.Meta {
  /// <summary>
  /// This node will only exist for a short time.
  /// </summary>
  [Info(UserLevel.User, "This node will only exist for a short time.", CompType)]
  public class Lifetime : Component {
    public const mtypes CompType = mtypes.affectself;
    private Toggle<int> _timeUntilDeath;
    //amount of 'dying time' contributed to the timeUntilDead, before the node dies. (if timeUntildead is not enabled, this stays at 0.)
    private int lifeLeft = 0;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// If enabled, this node will be deleted when it's lifetime has reached or surpassed this number.
    /// </summary>
    [Info(UserLevel.User,
      "If enabled, this node will be deleted when it's lifetime has reached or surpassed this number.")]
    public Toggle<int> timeUntilDeath {
      get { return _timeUntilDeath; }
      set {
        _timeUntilDeath = value;
        if (value.enabled) lifeLeft = 0;
      }
    }

    /// <summary>
    /// How many milliseconds this node has been alive
    /// </summary>
    [Info(UserLevel.Developer, "How many milliseconds have passed since this node was spawned")]
    public int lifetime { get; set; }

    public Lifetime() : this(null) {}

    public Lifetime(Node parent = null) {
      if (parent != null) this.parent = parent;
      timeUntilDeath = new Toggle<int>(5000, false);
      lifetime = 0;
    }

    public override void OnSpawn() {
      lifetime = 0;
      lifeLeft = 0;
    }

    public override void AffectSelf() {
      int mill = OrbIt.Game.Time.ElapsedGameTime.Milliseconds;
      lifetime += mill;
      if (timeUntilDeath.enabled) {
        lifeLeft += mill;
        if (lifeLeft > timeUntilDeath)
          parent.OnDeath(null);
      }
    }
  }
}