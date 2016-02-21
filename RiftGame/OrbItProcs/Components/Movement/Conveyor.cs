using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OrbItProcs
{
    /// <summary>
    /// The conveyor will speed up / slow down nodes that are on top of this node. (or push them in an absolute direction)
    /// </summary>
    [Info(UserLevel.User, "The conveyor will speed up / slow down nodes that are on top of this node. (or push them in an absolute direction)")]
    public class Conveyor : Component
    {
        public enum DirectionMode
        {
            Relative,
            Absolute,
        }
        public const mtypes CompType = mtypes.none;
        public override mtypes compType { get { return CompType; } set { } }
        /// <summary>
        /// Sets the directmode, deciding how the conveyor will function. Relative takes into account the affected node's current velocity, whereas absolute pushes them in the direction of X and Y
        /// </summary>
        [Info(UserLevel.User, "Sets the directmode, deciding how the conveyor will function. Relative takes into account the affected node's current velocity, whereas absolute pushes them in the direction of X and Y")]
        public DirectionMode directionMode { get; set; }
        
        
        /// <summary>
        /// The strength of the force in which the conveyor will push affected nodes. (percentage)
        /// </summary>
        [Info(UserLevel.User, "The strength of the force in which the conveyor will push affected nodes. (percentage)")]
        public float multiplier { get; set; }
        /// <summary>
        /// If enabled, the affected node will slow down rather than speeding up.
        /// </summary>
        [Info(UserLevel.User, "If enabled, the affected node will slow down rather than speeding up.")]
        public bool slowdown { get; set; }

        public float relativeAngle { get; set; }
        public float absoluteX { get; set; }
        public float absoulteY { get; set; }
        public Conveyor() : this(null) { }
        public Conveyor(Node parent)
        {
            this.parent = parent;
            directionMode = DirectionMode.Relative;
            relativeAngle = 0;
            absoluteX = 0;
            absoulteY = 1;
            multiplier = 5f;
            slowdown = false;
        }
        public override void OnSpawn()
        {
            parent.collision.isSolid = false;
            parent.basicdraw.DrawLayer = Layers.Under4;
            parent.body.OnCollisionStay += (c1, c2) =>
                {
                    if (!active || c2 == null) return;

                    if (directionMode == DirectionMode.Relative)
                    {
                        int sign = 1;
                        if (slowdown) sign = -1;

                        //float tempAngle = (float)Math.Atan2(c2.body.velocity.Y, c2.body.velocity.X); //todo:implement
                        

                        c2.body.velocity *= 1f + (multiplier / 100f * sign);
                    }
                    else if (directionMode == DirectionMode.Absolute)
                    {
                        Vector2 force = new Vector2(absoluteX, absoulteY) * (multiplier / 100f);
                        c2.body.velocity += force;
                    }
                };
        }

    }
}
