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

namespace OrbItProcs
{
    /// <summary>
    /// Adds a friction force to the node, slowing it down.
    /// </summary>
    [Info(UserLevel.User, "Adds a friction force to the node, slowing it down.")]
    public class Friction : Component
    {
        public enum material
        {
            ice,
            wax,
            wetfloor,
            grass,
            soil,
            dirt,
            asphalt,
            rubber,
        }
        public static Dictionary<material, float> coefficients = new Dictionary<material, float>()
        {
            {material.ice, 1},
            {material.wax, 3},
            {material.wetfloor, 5},
            {material.grass, 7},
            {material.soil, 12},
            {material.dirt, 20},
            {material.asphalt, 35},
            {material.rubber, 50},
        };
        public const mtypes CompType = mtypes.affectself;
        public override mtypes compType { get { return CompType; } set { } }

        public float force { get; set; }
        public Friction() : this(null) { }
        public Friction(Node parent)
        {
            this.parent = parent;
            force = 0.01f;
        }

        public override void AffectSelf()
        {
            parent.body.velocity *= 1 - force * parent.body.mass;
        }
    }
}
