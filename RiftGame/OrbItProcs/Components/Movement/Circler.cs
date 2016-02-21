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
using System.Runtime.Serialization;
namespace OrbItProcs
{
    /// <summary>
    /// This node will now move in circles or spirals.
    /// </summary>
    [Info(UserLevel.User, "This node will now move in circles or spirals.", CompType)]
    public class Circler : Component {

        public const mtypes CompType = mtypes.affectself;
        public override mtypes compType { get { return CompType; } set { } }

        /// <summary>
        /// The change in angle every frame, if set to non-zero, the node will spiral.
        /// </summary>
        [Info(UserLevel.User, "The change in angle every frame, if set to non-zero, the node will spiral.")]
        public float angleVelocity { get; set; }
        /// <summary>
        /// The rate of change of the rate of change of spiralization 
        /// </summary>
        [Info(UserLevel.Advanced, "The rate of change of the rate of change of spiralization ")]
        public float angleAcceleration { get; set; }
        /// <summary>
        /// Tied to angleVelocity, if it surpasses this value, the angleAcceleration is inverted
        /// </summary>
        [Info(UserLevel.Advanced, "Tied to angleVelocity, if it surpasses this value, the angleAcceleration is inverted")]
        public float maxVel { get; set; }
        /// <summary>
        ///  Tied to angleVelocity, if it goes below this value, the angleAcceleration is inverted
        /// </summary>
        [Info(UserLevel.Advanced, " Tied to angleVelocity, if it goes below this value, the angleAcceleration is inverted")]
        public float minVel { get; set; }
        /// <summary>
        /// The angle at which the node is travelling. In essence: The change of direction applied to the node every frame.
        /// </summary>
        [Info(UserLevel.User, "The angle at which the node is travelling. In essence: The change of direction applied to the node every frame.")]
        public float angle { get; set; }
        /// <summary>
        /// If the angle surpasses this value, the angleVelocity will invert, giving the illusion of unraveling.
        /// </summary>
        [Info(UserLevel.User, "If the angle surpasses this value, the angleVelocity will invert, giving the illusion of unraveling.")]
        public float maxAngle { get; set; }
        /// <summary>
        /// If the angle goes below this value, the angleVelocity will invert, giving the illusion of ravelling.
        /// </summary>
        [Info(UserLevel.User, "If the angle goes below this value, the angleVelocity will invert, giving the illusion of ravelling.")]
        public float minAngle { get; set; }
        /// <summary>
        /// if enabled, minAngle and maxAngle will no longer invert the angleChange, but will now reset the angle to produce a trippy effect.
        /// </summary>
        [Info(UserLevel.User, "if enabled, minAngle and maxAngle will no longer invert the angleChange, but will now reset the angle to produce a trippy effect.")]
        public bool loop { get ; set ; }

        public Circler() : this(null) { }
        public Circler(Node parent = null)
        {
            if (parent != null) this.parent = parent;
            angleVelocity = 0.01f;
            angleAcceleration = 0.001f;
            maxVel = 0.3f;
            minVel = 0f;
            angle = 0f;
            maxAngle = 3.14f;
            minAngle = -3.14f;
            loop = true;
        }

        public override void OnSpawn()
        {
            //base.OnSpawn();
            if (parent != null && parent.body.velocity.Length() > 0)
            {
                angle = (float)Math.Atan2(parent.body.velocity.Y, parent.body.velocity.X);
            }
        }



        public override void AffectSelf()
        {
            angle += angleVelocity;
            if (angle < minAngle)
            {
                angleVelocity *= -1;
                angle = minAngle;
            }
            else if (angle > maxAngle)
            {
                if (loop)
                {
                    angle = minAngle;
                }
                else
                {
                    angleVelocity *= -1;
                    angle = maxAngle;
                }
            }
            angleVelocity += angleAcceleration;
            if (angleVelocity < minVel)
            {
                angleAcceleration *= -1;
                angleVelocity = minVel;
            }
            else if (angleVelocity > maxVel)
            {
                angleAcceleration *= -1;
                angleVelocity = maxVel;
            }

            float length = parent.body.velocity.Length();
            float x = length * (float)Math.Sin(angle);
            float y = length * (float)Math.Cos(angle);
            parent.body.velocity = new Vector2(x, y);

        }

        public override void Draw()
        {

        }
    }
}
