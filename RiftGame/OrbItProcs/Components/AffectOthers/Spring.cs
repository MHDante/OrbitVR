using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;
using SharpDX;
using SharpOVR;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.DXGI;
using SharpDX.Toolkit.Content;
using System.Reflection;


namespace OrbItProcs
{
    /// <summary>
    /// Pushes away nodes that are within the radius, can also pull in nodes that beyond the radius. 
    /// </summary>
    [Info(UserLevel.User, "Pushes away nodes that are within the radius, can also pull in nodes that beyond the radius. ", CompType)]
    public class Spring : Component, ILinkable, IMultipliable//, IRadius
    {
        public enum mode
        {
            PushAndPull,
            PushOnly,
            PullOnly,
        }
        public const mtypes CompType = mtypes.affectself | mtypes.affectother;
        public override mtypes compType { get { return CompType; } set { } }
        [Info(UserLevel.Developer)]
        public Link link { get; set; }
        /// <summary>
        /// The strength of the spring's force
        /// </summary>
        [Info(UserLevel.User, "The strength of the spring's force")]
        public float multiplier { get; set; }
        /// <summary>
        /// The maximum reach of the spring, after which it will have no effect.
        /// </summary>
        [Info(UserLevel.User, "The maximum reach of the spring, after which it will have no effect.")]
        public float radius { get; set; }
        /// <summary>
        /// Sets the mode of the spring. Pull means with will pull nodes beyond the restdist. Push means it will push nodes within the restdist.
        /// </summary>
        [Info(UserLevel.User, "Sets the mode of the spring. Pull means with will pull nodes beyond the restdist. Push means it will push nodes within the restdist.")]
        public mode springMode { get; set; }

        private int _restdist;
        /// <summary>
        /// The distance at which no force is applied.
        /// </summary>
        [Info(UserLevel.User, "The distance at which no force is applied.")]
        public int restdist { get { return _restdist; } set { _restdist = value; if (_restdist < _deadzone.value) _restdist = _deadzone.value; } }

        private Toggle<int> _deadzone;
        /// <summary>
        /// Represents minimum distance taken into account when calculating push away.
        /// </summary>
        [Info(UserLevel.User, "Represents minimum distance taken into account when calculating push away.")]
        public Toggle<int> deadzone { get { return _deadzone; } set { _deadzone = value; if (_deadzone.value > _restdist) _deadzone.value = _restdist; } }

        public Spring() : this(null) { }
        public Spring(Node parent = null)
        {
            if (parent != null)
            {
                this.parent = parent;
            }
            multiplier = 100f;
            radius = 800;
            _restdist = 300;
            _deadzone = new Toggle<int>(100, false);
        }
        public override void AffectOther(Node other)
        {
            if (!active) return;

            float dist = Vector2.Distance(parent.body.pos, other.body.pos);
            //if (dist > radius) return;
            if (springMode == mode.PullOnly && dist < restdist) return;
            if (springMode == mode.PushOnly && dist > restdist) return;
            //if (dist > restdist * 2) return;
            if (deadzone.enabled && dist < deadzone.value) dist = deadzone.value;

            float stretch = dist - restdist;
            float strength = -stretch * multiplier / 10000f;
            Vector2 force = other.body.pos - parent.body.pos;
            VMath.NormalizeSafe(ref force);
            force *= strength;
            other.body.ApplyForce(force);
        }

    }
}
