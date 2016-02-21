using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OrbItProcs
{
    /// <summary>
    /// Modifies the color of affected nodes by using a distance equation. This can be based on their spacial position or an "Imaginary color position"
    /// </summary>
    [Info(UserLevel.User, "Modifies the color of affected nodes by using a distance equation. This can be based on their spacial position or an 'Imaginary color position'",CompType)]
    public class ColorGravity : Component, ILinkable
    {
        public const mtypes CompType = mtypes.affectself | mtypes.affectother;
        public override mtypes compType { get { return CompType; } set { } }
        [Info(UserLevel.Developer)]
        public Link link { get; set; }

        public enum DistanceMod
        {
            color,
            spatial,
        }
        /// <summary>
        /// Chooses what to base gravity calcutations on. Spatial: color gravity increases the closer nodes are to each other. Color: Color gravity increases when the nodes have similar colors.
        /// </summary>
        [Info(UserLevel.User, "Chooses what to base gravity calcutations on. Spatial: color gravity increases the closer nodes are to each other. Color: Color gravity increases when the nodes have similar colors.")]
        public DistanceMod distancemod { get; set; }
        
        
        public enum Mode
        {
            hue,
            rgb
        }
        /// <summary>
        /// Changes what data the color gravity is based on
        /// </summary>
        [Info(UserLevel.User, "Changes what data the color gravity is based on")]
        public Mode mode { get; set; }

        [Info(UserLevel.Developer)]
        public override bool active
        {
            get
            {
                return base.active;
            }
            set
            {
                if (value)
                {
                    r = parent.body.color.R / 255f;
                    g = parent.body.color.G / 255f;
                    b = parent.body.color.B / 255f;
                }
                base.active = value;
            }
        }
        /// <summary>
        /// The rate of change of each of the components of node's color.
        /// </summary>
        [Info(UserLevel.Advanced, "The rate of change of each of the components of node's color.")]
        public Vector3 colvelocity;
        /// <summary>
        /// The strength of the color gravity;
        /// </summary>
        [Info(UserLevel.Advanced, "The strength of the color gravity;")]
        public float multiplier { get; set; }

        private float r;
        private float g;
        private float b;
        private float hue;
        /// <summary>
        /// The rate of change of each of the components of node's color.
        /// </summary>
        [Info(UserLevel.Advanced, "The rate of change of each of the components of node's color.")]
        public float huevelocity { get; set; }
        /// <summary>
        /// Linear Dampening of color velocity
        /// </summary>
        [Info(UserLevel.Advanced, "Linear Dampening of color velocity")]
        public float friction { get; set; }
        /// <summary>
        /// The rate of change of each of the components of node's color.
        /// </summary>
        [Info(UserLevel.Advanced, "The rate of change of each of the components of node's color.")]
        public float divisor { get; set; }
        /// <summary>
        /// Maximum Color velocity
        /// </summary>
        [Info(UserLevel.Advanced, "Maximum Color velocity")]
        public float maxhuevel { get; set; }


        //public bool inverse

        public ColorGravity() : this(null){ }
        public ColorGravity(Node parent = null)
        {
            if (parent != null) this.parent = parent;
            colvelocity = new Vector3(0f, 0f, 0f);
            multiplier = 1f;
            distancemod = DistanceMod.color;
            mode = Mode.rgb;
            huevelocity = 0f;
            hue = 0f;
            divisor = 1000f;
            maxhuevel = 10f;
            friction = 0.9f;
        }

        public override void OnSpawn()
        {
            r = parent.body.color.R / 255f;
            g = parent.body.color.G / 255f;
            b = parent.body.color.B / 255f;
            hue = HueFromColor(parent.body.color);
        }

        public override void AffectOther(Node other)
        {
            if (mode == Mode.hue)
            {
                if (!other.HasComp<ColorGravity>()) return;

                float dist = 1f;
                if (distancemod == DistanceMod.color)
                {
                    dist = hue - other.Comp<ColorGravity>().hue;
                    if (dist == 0) return;
                    float force = multiplier * other.body.mass * parent.body.mass / (dist * dist);
                    if (dist < 0) force *= -1;
                    float diff = hue - other.Comp<ColorGravity>().hue;
                    if (Math.Abs(diff) > 180) force *= -1;
                    if (force > maxhuevel) force = maxhuevel;
                    else if (force < -maxhuevel) force = -maxhuevel;
                    other.Comp<ColorGravity>().huevelocity += force;
                    huevelocity += force;


                    //float otherhue = other.Comp<ColorGravity>().hue;
                    //dist = Utils.CircularDistance(hue, otherhue);
                    //if (dist < 1) return;
                    //float force = multiplier / (dist * dist) / divisor;
                    //huevelocity += force;

                    //Console.WriteLine("dist: {0} force: {1}", dist, force);

                }
                else if (distancemod == DistanceMod.spatial)
                {
                    dist = Vector2.Distance(parent.body.pos, other.body.pos) / divisor;
                    if (dist == 0) return;
                    float force = multiplier * other.body.mass * parent.body.mass / (dist * dist);
                    float diff = hue - other.Comp<ColorGravity>().hue;
                    //int wrap = Math.Abs(diff) > 180 ? -1 : 1;
                    if (Math.Abs(diff) > 180) force *= -1;
                    if (diff < 0) force *= -1;
                    if (force > maxhuevel) force = maxhuevel;
                    else if (force < -maxhuevel) force = -maxhuevel;

                    other.Comp<ColorGravity>().huevelocity += force;
                    huevelocity -= force;

                    
                }
            }
            else if (mode == Mode.rgb)
            {
                Vector3 parentCol = parent.body.color.ToVector3();
                Vector3 otherCol = other.body.color.ToVector3();
                float dist = 1f;
                if (distancemod == DistanceMod.color)
                {
                    dist = Vector3.Distance(parentCol, otherCol) / 100f;
                }
                else if (distancemod == DistanceMod.spatial)
                {
                    dist = Vector2.Distance(parent.body.pos, other.body.pos) / divisor / divisor;
                }
                Vector3 direction = otherCol - parentCol;
                if (dist < 1) dist = 1;
                if (direction != Vector3.Zero) direction.Normalize();
                float mag = multiplier * parent.body.mass * other.body.mass / (dist * dist);
                Vector3 impulse = mag * direction;
                impulse /= 10000f;
                if (other.HasActiveComponent<ColorGravity>())
                    other.Comp<ColorGravity>().colvelocity += impulse;
                colvelocity -= impulse;
            }
        }
        public override void AffectSelf()
        {
            if (mode == Mode.hue)
            {
                if (huevelocity > maxhuevel) huevelocity = maxhuevel;
                else if (huevelocity < -maxhuevel) huevelocity = -maxhuevel;
                huevelocity *= friction;
                hue += huevelocity;
                //Console.WriteLine("1) {0} : HUE: {1}   HUEVEL: {2}", parent.name, hue, huevelocity);
                //if (hue < 0) { hue = 0; huevelocity *= -1; }
                //if (hue > 360) { hue = 360; huevelocity *= -1; }
                hue = DelegateManager.Triangle(hue, 360f);
                parent.body.color = ColorChanger.getColorFromHSV(hue);
                //Console.WriteLine("2) {0} : HUE: {1}   HUEVEL: {2}", parent.name, hue, huevelocity);
            }
            else if (mode == Mode.rgb)
            {
                r += colvelocity.X / friction;
                g += colvelocity.Y / friction;
                b += colvelocity.Z / friction;
                if (r > 1f || r < 0f)
                {
                    r = DelegateManager.Triangle(r, 1f);
                    colvelocity.X *= -1;
                }
                if (g > 1f || g < 0f)
                {
                    g = DelegateManager.Triangle(g, 1f);
                    colvelocity.Y *= -1;
                }
                if (b > 1f || b < 0f)
                {
                    b = DelegateManager.Triangle(b, 1f);
                    colvelocity.Z *= -1;
                }
                parent.body.color = new Color(r, g, b);
            }
        }

        public static float HueFromColor(Color c)
        {
            //180/pi*atan2( sqrt(3)*(G-B) , 2*R-G-B )
            return (float)(180 / Math.PI * Math.Atan2(Math.Sqrt(3) * (c.G - c.B), 2 * c.R - c.G - c.B));
        }
        public static float HueFromColor(int r, int g, int b)
        {
            return (float)(180 / Math.PI * Math.Atan2(Math.Sqrt(3) * (g - b), 2 * r - g - b));
        }

        public override void Draw()
        {
        }

    }
}
