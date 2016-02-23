using System;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Essential {
  public enum AIMode {
    Agro,
    Friendly,
    Neutral,
    Player,
    None,
  }

  [Flags]
  public enum ItemSlots {
    None = 0,
    A_Green = 1,
    B_Red = 2,
    X_Blue = 4,
    Y_Yellow = 8,
    All = 15,
  }

  /// <summary>
  /// The Meta component hold information about the node such as health, shields, and player/ai modes.
  /// </summary>
  [Info(UserLevel.User,
    "The Meta component hold information about the node such as health, shields, and player/ai modes.", CompType)]
  public class Meta : Component {
    public enum DamageMode {
      OnlyPlayers,
      OnlyNonPlayers,
      Everything,
      Nothing,
    }

    public enum HealthBarMode {
      Fade,
      Bar,
      none,
    }

    public const mtypes CompType = mtypes.essential | mtypes.draw | mtypes.playercontrol;
    private float lightRotation = 0f;

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// The score of the player. If enabled and the player reaches the max score, the game ends.
    /// </summary>
    [Info(UserLevel.User, "The score of the player. If enabled and the player reaches the max score, the game ends.")]
    public Toggle<float> score { get; set; }

    /// <summary>
    /// The maximum health the node. When it reaches 0, the node dies.
    /// </summary>
    [Info(UserLevel.User, "The maximum health the node. When it reaches 0, the node dies.")]
    public Toggle<float> maxHealth { get; set; }

    /// <summary>
    /// The current health of the node.
    /// </summary>
    [Info(UserLevel.User, "The current health of the node.")]
    public float currentHealth { get; set; }

    /// <summary>
    /// The worth of the node represents how likely it is that the node will drop valuable loot upon death.
    /// </summary>
    [Info(UserLevel.User,
      "The worth of the node represents how likely it is that the node will drop valuable loot upon death.")]
    public Toggle<float> worth { get; set; }

    /// <summary>
    /// Represents the amount of damage mitigation upon losing health.
    /// </summary>
    [Info(UserLevel.User, "Represents the amount of damage mitigation upon losing health.")]
    public Toggle<float> armour { get; set; }

    /// <summary>
    /// The damageMode describes who this node can hurt.
    /// </summary>
    [Info(UserLevel.User, "The damageMode describes who this node can hurt.")]
    public DamageMode damageMode { get; set; }

    /// <summary>
    /// Select how to indicate health
    /// </summary>
    [Info(UserLevel.User, "Select how to indicate health")]
    public HealthBarMode healthBar { get; set; }

    //[Info(UserLevel.Developer)]
    //public ItemSlots itemSlots { get; set; }
    /// <summary>
    /// If enabled, the health bar will rotate.
    /// </summary>
    [Info(UserLevel.Advanced, "If enabled, the health bar will rotate.")]
    public bool rotateHealthBar { get; set; }

    /// <summary>
    /// If this node is controlled by an AI, this will determine it's behaviour.
    /// </summary>
    [Info(UserLevel.User, "If this node is controlled by an AI, this will determine it's behaviour.")]
    public AIMode AImode { get; set; }

    /// <summary>
    /// If the node is deadly, this node will kill other nodes on contact.
    /// </summary>
    [Info(UserLevel.User, "If the node is deadly, this node will kill other nodes on contact.")]
    public bool deadly { get; set; }

    /// <summary>
    /// This affects the amount of damage this node will do when attacking other nodes.
    /// </summary>
    [Info(UserLevel.User, "This affects the amount of damage this node will do when attacking other nodes.")]
    public float damageMultiplier { get; set; }

    /// <summary>
    /// If enabled, the node will ignore the grid system and iterate over every node in the room.
    /// </summary>
    [Info(UserLevel.User, "If enabled, the node will ignore the grid system and iterate over every node in the room.")]
    public bool IgnoreAffectGrid { get; set; }

    /// <summary>
    /// The radius of the node.
    /// </summary>
    [Info(UserLevel.User, "The radius of the node.")]
    public float radius {
      get { return parent != null ? parent.body.radius : 25; }
      set { if (parent != null) parent.body.radius = value; }
    }

    public Action<Node, Node> OnDeath { get; set; }
    public Action<Node, Node, int> OnTakeDamage { get; set; }
    public bool itemSwitching { get; set; }
    public Meta() : this(null) {}

    public Meta(Node parent) {
      this.parent = parent;
      score = new Toggle<float>(0, false);
      maxHealth = new Toggle<float>(100, true);
      currentHealth = maxHealth.value;
      worth = new Toggle<float>(1, false);
      armour = new Toggle<float>(1, false);
      damageMode = DamageMode.OnlyPlayers;
      AImode = AIMode.None;
      deadly = false;
      active = true;
      IgnoreAffectGrid = false;
      damageMultiplier = 1f;
      itemSwitching = true;
    }

    public override void PlayerControl(Input input) {
      if (itemSwitching) {
        if (input.BtnClicked(InputButtons.A_1)) parent.player.currentItem = ItemSlots.A_Green;
        if (input.BtnClicked(InputButtons.B_3)) parent.player.currentItem = ItemSlots.B_Red;
        if (input.BtnClicked(InputButtons.X_2)) parent.player.currentItem = ItemSlots.X_Blue;
        if (input.BtnClicked(InputButtons.Y_4)) parent.player.currentItem = ItemSlots.Y_Yellow;
      }
    }

    public void CalculateDamage(Node other, float damage) {
      if (maxHealth.enabled) {
        float resultingDamage = OrbIt.GlobalGameMode.DetermineDamage(other, parent, damage)*damageMultiplier;
        currentHealth = (float) Math.Max(currentHealth - resultingDamage, 0);
        currentHealth = (float) Math.Min(currentHealth, maxHealth.value);
        float percent = 0.65f;
        if (healthBar == HealthBarMode.Fade)
          parent.body.color = parent.body.permaColor*((currentHealth/maxHealth.value)*percent + (1f - percent));
        if (currentHealth <= 0) {
          //Die(other);
          parent.OnDeath(other);
        }
      }
    }

    public override void Draw() {
      if (healthBar == HealthBarMode.Bar) {
        float healthRatio = (float) currentHealth/(float) maxHealth;
        drawBar(parent, 1, healthRatio, rotateHealthBar, Color.Lime, Color.YellowGreen, Color.Orange, Color.Red);
      }
      if (parent.IsPlayer) {
        Color Q;
        switch (parent.player.currentItem) {
          case ItemSlots.A_Green:
            Q = Color.ForestGreen;
            break;
          case ItemSlots.B_Red:
            Q = Color.Crimson;
            break;
          case ItemSlots.X_Blue:
            Q = Color.CornflowerBlue;
            break;
          case ItemSlots.Y_Yellow:
            Q = Color.Gold;
            break;
          default:
            Q = Color.Black;
            break;
        }

        room.Camera.Draw(Textures.Pointer, parent.body.pos, Q, parent.body.scale, parent.body.orient, Layers.Over4);
        ItemSlots itemSlots = parent.player.occupiedSlots;
        Textures A, B, X, Y;
        A = (itemSlots & ItemSlots.A_Green) == ItemSlots.A_Green ? Textures.ItemLight : Textures.ItemWhisper;
        B = (itemSlots & ItemSlots.B_Red) == ItemSlots.B_Red ? Textures.ItemLight : Textures.ItemWhisper;
        X = (itemSlots & ItemSlots.X_Blue) == ItemSlots.X_Blue ? Textures.ItemLight : Textures.ItemWhisper;
        Y = (itemSlots & ItemSlots.Y_Yellow) == ItemSlots.Y_Yellow ? Textures.ItemLight : Textures.ItemWhisper;

        room.Camera.Draw(A, parent.body.pos, Color.ForestGreen, parent.body.scale*1.7f, lightRotation, Layers.Under2);
        room.Camera.Draw(B, parent.body.pos, Color.Crimson, parent.body.scale*1.7f, lightRotation + GMath.PIbyTwo,
                         Layers.Under2);
        room.Camera.Draw(X, parent.body.pos, Color.CornflowerBlue, parent.body.scale*1.7f, lightRotation + GMath.PI,
                         Layers.Under2);
        room.Camera.Draw(Y, parent.body.pos, Color.Gold, parent.body.scale*1.7f,
                         lightRotation + GMath.PI + GMath.PIbyTwo, Layers.Under2);

        lightRotation += 0.1f;
      }
    }

    public static void drawBar(Node node, float scale, float Ratio, bool Rotate, Color full, Color? threeQuarters = null,
                               Color? half = null, Color? oneQuarter = null) {
      float baseRotation = Rotate ? node.body.orient : 0f;
      float rotation = baseRotation + (1f - Ratio)*GMath.TwoPI;
      float rotation2 = baseRotation;
      Color c;
      Layers hideLayer = Layers.Over1;
      if (Ratio > 0.75f)
        c = full;
      else if (Ratio > 0.5f)
        c = threeQuarters ?? full;
      else if (Ratio > 0.25f) {
        c = half ?? threeQuarters ?? full;
        hideLayer = Layers.Over3;
        rotation2 = rotation - GMath.PI;
      }
      else {
        c = oneQuarter ?? half ?? threeQuarters ?? full;
        hideLayer = Layers.Over3;
        rotation2 = rotation - GMath.PI;
      }
      node.room.Camera.Draw(Textures.OuterL, node.body.pos, Color.Black, node.body.scale*scale, baseRotation, hideLayer);
      node.room.Camera.Draw(Textures.OuterR, node.body.pos, Color.Black, node.body.scale*scale, baseRotation,
                            Layers.Over1);
      node.room.Camera.Draw(Textures.InnerL, node.body.pos, c, node.body.scale*scale, rotation, Layers.Over2);
      node.room.Camera.Draw(Textures.InnerR, node.body.pos, c, node.body.scale*scale, rotation2, Layers.Over2);
    }

    public override void OnRemove(Node other) {
      if (OnDeath != null) OnDeath(parent, other);
    }
  }
}