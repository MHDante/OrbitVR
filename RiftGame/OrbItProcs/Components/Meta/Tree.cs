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
    /// After a short interval, this node spawns smaller nodes that share it's characteristics.
    /// </summary>
    [Info(UserLevel.User, "After a short interval, this node spawns smaller nodes that share it's characteristics.",CompType)]
    public class Tree : Component
    {

        public const mtypes CompType = mtypes.affectself | mtypes.draw | mtypes.tracer;
        public override mtypes compType { get { return CompType; } set { } }
        public int queuecount { get; set; }
        private float r1, g1, b1;
        private Queue<Vector2> positions;
        private Queue<float> scales;

        [Info(UserLevel.Developer)]
        public int depth;
        /// <summary>
        /// The amount of times that branches divide into smaller branches
        /// </summary>
        [Info(UserLevel.User, "The amount of times that branches divide into smaller branches")]
        public int branchStages { get; set; }
        /// <summary>
        /// When a branch divides, the velocity of the resulting nodes will be within this range of the causing node;
        /// </summary>
        [Info(UserLevel.User, "When a branch divides, the velocity of the resulting nodes will be within this range of the causing node;")]
        public float angleRange { get; set; }

        /// <summary>
        /// The relative length of each branch;
        /// </summary>
        [Info(UserLevel.Advanced, "The relative length of each branch;")]
        public int randlife { get; set; }

        private int lifeleft;
        /// <summary>
        /// If enabled, causes the tree to leave a 'trunk' trail behind.
        /// </summary>
        [Info(UserLevel.User, "If enabled, causes the tree to leave a 'trunk' trail behind.")]
        public bool LeaveTrunk { get; set; }

        /// <summary>
        /// The maximum number of children made at every division.
        /// </summary>
        [Info(UserLevel.Advanced, "The maximum number of children made at every division.")]
        public int maxChilds { get { return _maxChilds; } set { _maxChilds = value > 15 ?  15 : value; } }
        private int _maxChilds;

        public Toggle<int> fade { get; set; }
        public Tree() : this(null) { }
        public Tree(Node parent = null)
        {
            if (parent != null) this.parent = parent;
            InitializeLists();
            r1 = Utils.random.Next(255) / 255f;
            g1 = Utils.random.Next(255) / 255f;
            b1 = Utils.random.Next(255) / 255f;
            queuecount = 100;
            fade = new Toggle<int>(queuecount);
            branchStages = 4;
            angleRange = 45;
            randlife = 20;
            maxChilds = 3;
            LeaveTrunk = true;
        }

        public override void InitializeLists()
        {
            positions = new Queue<Vector2>();
            scales = new Queue<float>();
        }
        int deathcount = 0;
        bool original = false;
        public override void AffectSelf()
        {
            if (lifeleft == -1)
            {
                if (deathcount++ > 15)
                {
                    //active = false;
                    return;
                }
                if (!LeaveTrunk) return;
                positions.Enqueue(parent.body.pos);
                scales.Enqueue(parent.body.scale);
                //parent.nodeState = state.drawOnly;
                return;
            }
            if (depth >= branchStages)
            {
                lifeleft = -1;
                parent.body.velocity = new Vector2(0, 0);
                depth = -1;
                original = false;
                //return;
                return;
            }
            original = true;
            //angle = Math.Atan2(parent.transform.velocity.Y, parent.transform.velocity.X) + (Math.PI / 2);
            float scaledown = 1.0f - 0.01f;
            parent.body.radius *= scaledown;
            //if (LeaveTrunk && lifeleft > 0)
            //{
            //    if (positions.Count < queuecount)
            //    {
            //        positions.Enqueue(parent.body.pos);
            //        scales.Enqueue(parent.body.scale);
            //    }
            //    else
            //    {
            //        if (positions.Count > 0)
            //        {
            //            positions.Dequeue();
            //            positions.Enqueue(parent.body.pos);
            //        }
            //        if (scales.Count > 0)
            //        {
            //            scales.Dequeue();
            //            scales.Enqueue(parent.body.scale);
            //        }
            //    }
            //}

            //branchdeath
            if (lifeleft > randlife)
            {
                lifeleft = -1;
                
                int velLength = (int)parent.body.velocity.Length();
                double angle = Math.Atan2(parent.body.velocity.Y, parent.body.velocity.X);

                parent.body.velocity = new Vector2(0, 0);
                //if (parent.comps.ContainsKey(comp.gravity)) parent.comps[comp.gravity].active = false;

                int childcount = Utils.random.Next(maxChilds+1) + 1;
                for(int i = 0; i < childcount; i++)
                {
                    float childscale = parent.body.scale * scaledown;
                    Vector2 childpos = parent.body.pos;
                    float anglechange = Utils.random.Next((int)angleRange) - (angleRange / 2);
                    //
                    anglechange = anglechange * (float)(Math.PI / 180);
                    angle += anglechange; //we might need to do a Math.Max(360,Math.Min(0,x));
                    Vector2 childvel = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * velLength;
                    int randomlife = (Utils.random.Next(randlife - randlife / 10 - 5)) + 5;
                    //int randomlife = randlife;
                    //userP[node.transform.position] = childpos;
                    //userP[node.scale] = childscale;
                    //userP[node.velocity] = childvel;
                    //userP[node.name] = "node" + Node.nodeCounter;

                    Node newNode = new Node(room);
                    Node.cloneNode(parent, newNode);
                    newNode.body.velocity = childvel;
                    newNode.name = "node" + Node.nodeCounter;
                    //newNode.acceptUserProps(userP);
                    Tree newTree = newNode.Comp<Tree>();
                    newTree.depth = depth + 1;
                    if (newTree.depth == branchStages)
                        newTree.original = false;
                    newTree.randlife = randomlife;
                    newTree.lifeleft = 0;
                    newTree.maxChilds = Math.Max(1, maxChilds - (depth % 2));
                    //room.nodesToAdd.Enqueue(newNode);
                    //room.masterGroup.childGroups.Values.ElementAt(1).IncludeEntity(newNode);
                    //Group g = parent.Game1.ui.sidebar.ActiveGroup;
                    if (parent.group != null)
                    {
                        parent.group.IncludeEntity(newNode);
                        newNode.group = parent.group;
                    }
                }
                //parent.nodeState = state.drawOnly;

                HashSet<Node> hs = new HashSet<Node>();
            }
            else
            {
                lifeleft++;
            }
        }

        public override void Draw()
        {
            if (!LeaveTrunk || !original) return;
            room.camera.AddPermanentDraw(parent.texture, parent.body.pos, parent.body.color, parent.body.scale, 0, 200); //trunk doesn't die. (make 500?)
        }

        public void DrawOld()
        {
            if (!LeaveTrunk) return;

            Color col = new Color(0, 0, 0, 0.3f);
            float a, b, c;
            a = b = c = 0;
            int count = 0;
            foreach (Vector2 pos in positions)
            {
                a += r1 / fade;
                b += g1 / fade;
                c += b1 / fade;
                col = new Color(a, b, c, 0.8f);
                if (!fade) col = parent.body.color;

                room.camera.Draw(parent.texture, pos, col, scales.ElementAt(count), 0);
                count++;
            }

            if (!fade) col = parent.body.color;
            room.camera.Draw(parent.texture, parent.body.pos, col, parent.body.scale, 0);

        }

        public void onCollision(Dictionary<dynamic, dynamic> args)
        {
        }

    }
}
