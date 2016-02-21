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
    /// This node has a nifty sword the node can swing to attack enemies. 
    /// </summary>
    [Info(UserLevel.User, "This node has a nifty sword the node can swing to attack enemies. ", CompType)]
    public class Sword : Component
    {
        public override bool active
        {
            get
            {
                return base.active;
            }
            set
            {
                base.active = value;
                if (swordNode != null)
                {
                    swordNode.active = value;
                }
            }
        }
        /// <summary>
        /// The sword node that will be held and swung.
        /// </summary>
        [Info(UserLevel.User, "The sword node that will be held and swung.")]
        [CopyNodeProperty]
        public Node swordNode { get; set; }
        public enum swordState
        {
            sheathed,
            stabbing,
            swinging,
            cooldown
        }

        public const mtypes CompType = mtypes.playercontrol | mtypes.draw | mtypes.item;// | mtypes.affectself;
        public override mtypes compType { get { return CompType; } set { } }
        /// <summary>
        /// The distance from the player the sword will swing at.
        /// </summary>
        [Info(UserLevel.User, "The distance from the player the sword will swing at.")]
        public float distance { get; set; }
        /// <summary>
        /// The length of the sword that is used when the sword is initialized.
        /// </summary>
        [Info(UserLevel.Advanced, "The length of the sword that is used when the sword is initialized.")]
        public float swordLength { get; set; }
        /// <summary>
        /// The width of the sword that is used when the sword is initialized.
        /// </summary>
        [Info(UserLevel.Advanced, "The width of the sword that is used when the sword is initialized.")]
        public float swordWidth { get; set; }
        /// <summary>
        /// The strength of the sword, which affects the amount of damage it does.
        /// </summary>
        [Info(UserLevel.User, "The strength of the sword, which affects the amount of damage it does.")]
        public float damageMultiplier { get; set; }
        /// <summary>
        /// The force at which to push the other node back when clashing swords.
        /// </summary>
        [Info(UserLevel.User, "The force at which to push the other node back when clashing swords.")]
        public float parryKnockback { get; set; }
        /// <summary>
        /// The force at which to push the other node back after a direct hit to the other node.
        /// </summary>
        [Info(UserLevel.User, "The force at which to push the other node back after a direct hit to the other node.")]
        public float nodeKnockback { get; set; }
        //public int swingRate { get; set; }
        //int swingRateCount = 0;
        //bool enabled; 
        //public float length{get;set;}
        //public int speed { get; set; }
        private bool movingStick = false;

        Vector2 target;
        public Sword() : this(null) { }
        public Sword(Node parent)
        {
            this.parent = parent;
            distance = 60;
            swordLength = 40;
            swordWidth = 5;
            //swingRate = 5;
            //speed = 3;
            damageMultiplier = 10f;
            nodeKnockback = 500f;
            parryKnockback = 20f;

            swordNode = new Node(room);
            swordNode.name = "sword";
        }

        public override void AfterCloning()
        {
            if (swordNode == null) return;
            swordNode = swordNode.CreateClone(room);
            //sword = new Node(room, props);
        }

        public override void OnSpawn()
        {
            //Node.cloneNode(parent.Game1.ui.sidebar.ActiveDefaultNode, sword);
            //parent.body.texture = textures.orientedcircle;
            swordNode.dataStore["swordnodeparent"] = parent;
            Polygon poly = new Polygon();
            poly.body = swordNode.body;
            poly.SetBox(swordWidth, swordLength);
            
            swordNode.body.shape = poly;
            swordNode.body.pos = parent.body.pos;
            swordNode.body.DrawPolygonCenter = false;
            swordNode.basicdraw.active = false;
            ///room.spawnNode(sword);

            room.groups.items.IncludeEntity(swordNode);
            swordNode.OnSpawn();
            swordNode.body.AddExclusionCheck(parent.body);
            swordNode.body.ExclusionCheck += delegate(Collider p, Collider o) { return !movingStick; };
            swordNode.body.OnCollisionEnter += (p, o) =>
            {
                if (o.dataStore.ContainsKey("swordnodeparent"))
                {
                    Node otherparent = o.dataStore["swordnodeparent"];
                    Vector2 f = otherparent.body.pos - parent.body.pos;
                    VMath.NormalizeSafe(ref f);
                    f *= parryKnockback;
                    otherparent.body.ApplyForce(f);
                }
                else if (o.IsPlayer)
                {
                    o.player.node.meta.CalculateDamage(parent, damageMultiplier);
                }
            };
            //sword.body.exclusionList.Add(parent.body);
            //
            //parent.body.exclusionList.Add(sword.body);
        }
        public override void AffectSelf()
        {
        }
        public override void PlayerControl(Input input)
        {
            swordNode.movement.active = false;
            //sword.body.velocity = Utils.AngleToVector(sword.body.orient + (float)Math.PI/2) * 100;
            swordNode.body.velocity = swordNode.body.effvelocity * nodeKnockback;
            Vector2 rightstick = input.GetRightStick();
            if (rightstick.LengthSquared() > 0.9 * 0.9)
            {
                movingStick = true;
                target = rightstick;
                //enabled = true;
                target.Normalize();
                target *= distance;
                target += parent.body.pos;
                swordNode.body.pos = Vector2.Lerp(swordNode.body.pos, target, 0.1f);
                //sword.body.pos = target + parent.body.pos;
                Vector2 result = swordNode.body.pos - parent.body.pos;
                swordNode.body.SetOrientV2(result);
                    
            }
            else
            {
                movingStick = false;
                //enabled = false;
                Vector2 restPos = new Vector2(parent.body.radius, 0).Rotate(parent.body.orient) + parent.body.pos;
                swordNode.body.pos = Vector2.Lerp(swordNode.body.pos, restPos, 0.1f);
                swordNode.body.orient = GMath.AngleLerp(swordNode.body.orient, parent.body.orient, 0.1f);
            }
            //sword.body.pos = position;
        }

        public override void Draw()
        {
            Vector2 position = swordNode.body.pos;
            if (position == Vector2.Zero) position = parent.body.pos;
            room.camera.Draw(textures.sword, position, parent.body.color, swordNode.body.scale * 2, swordNode.body.orient, Layers.Over3);
        }
        public override void OnRemove(Node other)
        {
            swordNode.OnDeath(other);
        }
    }
}
