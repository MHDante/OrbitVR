using System;
using System.Collections.Generic;
using OrbitVR.Components.Drawers;
using OrbitVR.Components.Essential;
using OrbitVR.Components.Meta;
using OrbitVR.Components.Tracers;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;
using Collision = OrbitVR.Components.Essential.Collision;

namespace OrbitVR.Components.Items {
  public enum ShootMode {
    Rapid,
    Single,
    Burst,
    Auto,
  }

  /// <summary>
  /// Shoots out damaging lasers that are automatic, single fire or rapid firing.
  /// </summary>
  [Info(UserLevel.User, "Shoots out damaging lasers that are automatic, single fire or rapid firing.", CompType)]
  public class Shooter : Component {
    public const mtypes CompType = mtypes.playercontrol | mtypes.draw | mtypes.item | mtypes.aicontrol;
    // | mtypes.affectself;

    public static Node bulletNode;
    private bool _isTurret = false;
    private Toggle<int> _maxAmmo;
    private int ammo;
    private int shootingRateCount = 0;
    private float tempTurretTimer = 0;

    public override bool active {
      get { return base.active; }
      set {
        base.active = value;
        if (bulletNode != null) {
          bulletNode.active = value;
        }
      }
    }

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    /// <summary>
    /// The mode in which to fire nodes. This will change the way input is handled.
    /// </summary>
    [Info(UserLevel.User, "The mode in which to fire nodes. This will change the way input is handled.")]
    public ShootMode shootMode { get; set; }

    /// <summary>
    /// The amount of time the bullet will live before disappearing.
    /// </summary>
    [Info(UserLevel.User, "The amount of time the bullet will live before disappearing.")]
    public int bulletLife { get; set; }

    /// <summary>
    /// The higher the delay, the slower you will shoot.
    /// </summary>
    [Info(UserLevel.User, "The higher the delay, the slower you will shoot.")]
    public int shootingDelay { get; set; }

    /// <summary>
    /// The speed at which bullets will travel.
    /// </summary>
    [Info(UserLevel.User, "The speed at which bullets will travel.")]
    public float speed { get; set; }

    /// <summary>
    /// The amount of damage each bullet will inflict on the node it collides with.
    /// </summary>
    [Info(UserLevel.User, "The amount of damage each bullet will inflict on the node it collides with.")]
    public float damage { get; set; }

    /// <summary>
    /// If enabled, the bullet's velocity will be determined by the controller stick's distance from the center of the stick.
    /// </summary>
    [Info(UserLevel.User,
      "If enabled, the bullet's velocity will be determined by the controller stick's distance from the center of the stick."
      )]
    public bool useStickVelocity { get; set; }

    /// <summary>
    /// Max ammo
    /// </summary>
    [Info(UserLevel.User, "Max ammo")]
    public Toggle<int> maxAmmo {
      get { return _maxAmmo; }
      set {
        _maxAmmo = value;
        ammo = value;
      }
    }

    /// <summary>
    /// If the shooter is a turret, it will fire at players.
    /// </summary>
    [Info(UserLevel.User, "If the shooter is a turret, it will fire at players.")]
    public bool isTurret {
      get { return _isTurret; }
      set {
        _isTurret = value;
        if (value) {
          parent.IsAI = true;
        }
        else {
          parent.IsAI = false;
        }
      }
    }

    /// <summary>
    /// The time interval between which the AI will shoot a bullet from the turret.
    /// </summary>
    [Info(UserLevel.User, "The time interval between which the AI will shoot a bullet from the turret.")]
    public float TurretTimerSeconds { get; set; }

    public Shooter() : this(null) {}

    public Shooter(Node parent) {
      this.parent = parent;
      bulletLife = 700;
      shootingDelay = 50;
      speed = 15f;
      damage = 10f;
      shootMode = ShootMode.Single;
      useStickVelocity = false;
      maxAmmo = new Toggle<int>(50, false);
      TurretTimerSeconds = 1;
    }

    public override void OnSpawn() {
      Action<Node> ap = (n) => { if (maxAmmo.enabled && ammo < maxAmmo) ammo++; };
      parent.scheduler.AddAppointment(new Appointment(ap, 5000, infinite: true));
    }

    public static void MakeBullet(Room room) {
      Dictionary<dynamic, dynamic> props = new Dictionary<dynamic, dynamic>() {
        {nodeE.radius, 5f},
        {typeof (Laser), true},
        {typeof (ColorChanger), true},
        {typeof (Lifetime), true},
      };
      bulletNode = new Node(room, props);
      bulletNode.Comp<Collision>().isSolid = false;
      bulletNode.body.isSolid = true;
      bulletNode.body.restitution = 1f;
      bulletNode.Comp<ColorChanger>().colormode = ColorChanger.ColorMode.hueShifter;
      bulletNode.Comp<Lifetime>().timeUntilDeath.enabled = true;
      bulletNode.Comp<Laser>().thickness = 5f;
      bulletNode.Comp<Laser>().laserLength = 20;
      bulletNode.Comp<Essential.Movement>().randInitialVel.enabled = false;
      bulletNode.group = room.Groups.Bullets;
    }

    public override void PlayerControl(Input input) {
      if (shootMode == ShootMode.Rapid) {
        if (input.BtnDown(InputButtons.RightTrigger_Mouse1)) {
          if (shootingRateCount++%shootingDelay == 0) {
            FireNode(input.GetRightStick().toV2R());
          }
        }
        else {
          shootingRateCount = 0;
        }
      }
      else if (shootMode == ShootMode.Single) {
        //if (fc.newGamePadState.IsButtonDown(Buttons.RightTrigger) && fc.oldGamePadState.IsButtonUp(Buttons.RightTrigger))
        if (input.BtnClicked(InputButtons.RightTrigger_Mouse1)) {
          if (shootingRateCount++%shootingDelay == 0) {
            FireNode(input.GetRightStick().toV2R());
          }
        }
        else {
          shootingRateCount = 0;
        }
      }
      else if (shootMode == ShootMode.Auto) {
        Vector2R rightstick = input.GetRightStick().toV2R();
        if (rightstick != Vector2R.Zero) {
          if (shootingRateCount++%shootingDelay == 0) {
            FireNode(rightstick);
          }
        }
        else {
          shootingRateCount = 0;
        }
      }
    }

    public override void AIControl(AIMode aiMode) {
      //if (aiMode == AIMode.)
      tempTurretTimer += OrbIt.Game.Time.ElapsedGameTime.Milliseconds;
      if (tempTurretTimer > TurretTimerSeconds*1000) {
        float nearest = float.MaxValue;
        Node nearNode = null;
        foreach (Node n in room.Groups.Player.entities) {
          float dist = Vector2R.Distance(parent.body.pos, n.body.pos);
          if (dist < nearest) {
            nearNode = n;
            nearest = dist;
          }
        }
        if (nearNode != null) {
          Vector2R dir = nearNode.body.pos - parent.body.pos;
          VMath.NormalizeSafe(ref dir);
          dir.Y *= -1;
          FireNode(dir);
          tempTurretTimer = 0;
        }
      }
    }

    public override void Draw() {
      Essential.Meta.drawBar(parent, 0.5f, (float) ammo/(float) maxAmmo, true, Color.Goldenrod);
    }

    public void FireNode(Vector2R dir) {
      if (maxAmmo.enabled) ammo--;
      if (!useStickVelocity) VMath.NormalizeSafe(ref dir);
      Node n = bulletNode.CreateClone(room);
      n.Comp<Lifetime>().timeUntilDeath.value = bulletLife;
      n.body.velocity = dir*speed;
      n.body.pos = parent.body.pos;
      n.body.AddExclusionCheck(parent.body);
      //n.body.AddExclusion(parent.body);
      if (parent.HasComp<Sword>()) {
        n.body.AddExclusionCheck(parent.Comp<Sword>().swordNode.body);
        //n.body.AddExclusion(parent.Comp<Sword>().sword.body);
      }
      if (parent.IsPlayer) {
        n.Comp<ColorChanger>().colormode = ColorChanger.ColorMode.none;
        n.SetColor(parent.player.pColor);
      }
      room.SpawnNode(n, g: room.Groups.Bullets);
      Action<Node, Node> bulletHit = (n1, n2) => {
                                       Node bullet, them;
                                       if (n1 == n) {
                                         bullet = n1;
                                         them = n2;
                                       }
                                       else if (n2 == n) {
                                         bullet = n2;
                                         them = n1;
                                       }
                                       else {
                                         return;
                                       }
                                       if (parent.meta.damageMode == Essential.Meta.DamageMode.OnlyPlayers) {
                                         if (!them.IsPlayer) return;
                                       }
                                       else if (parent.meta.damageMode == Essential.Meta.DamageMode.OnlyNonPlayers) {
                                         if (them.IsPlayer) return;
                                       }
                                       else if (parent.meta.damageMode == Essential.Meta.DamageMode.Nothing) {
                                         return;
                                       }
                                       them.meta.CalculateDamage(parent, damage);
                                       bullet.OnDeath(null);
                                     };
      n.body.OnCollisionEnter += bulletHit;
      //n.body.isSolid = false;
      if (maxAmmo.enabled && ammo <= 0) {
        parent.RemoveComponent(typeof (Shooter));
      }
    }
  }
}