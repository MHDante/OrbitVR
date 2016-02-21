using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OrbItProcs
{
    /// <summary>
    /// The Obstructor will create walls in the direction of surrounding nodes (Obstructor by default.)
    /// </summary>
    [Info(UserLevel.User, "The Obstructor will create walls in the direction of surrounding nodes (Obstructor by default.)", CompType)]
    public class Obstructor : Component
    {
        public const mtypes CompType = mtypes.affectother | mtypes.affectself;
        public override mtypes compType { get { return CompType; } set { } }

        public override bool active
        {
            get
            {
                return base.active;
            }
            set
            {
                base.active = value;
                if (walls != null)
                {
                    foreach (var n in walls)
                    {
                        n.active = value;
                    }
                }
            }
        }
        /// <summary>
        /// The maximum radius that the node will create walls to other nodes
        /// </summary>
        [Info(UserLevel.User, "The maximum radius that the node will create walls to other nodes")]
        public float radius { get; set; }
        /// <summary>
        /// The amount of walls that the node can possibly create.
        /// </summary>
        [Info(UserLevel.User, "The amount of walls that the node can possibly create.")]
        public int maxWalls { get; set; }
        /// <summary>
        /// If enabled, the node will only create walls with other obstructor nodes.
        /// </summary>
        [Info(UserLevel.User, "If enabled, the node will only create walls with other obstructor nodes.")]
        public bool onlyObstructors { get; set; }
        /// <summary>
        /// The thickness of each wall made.
        /// </summary>
        [Info(UserLevel.User, "The thickness of each wall made.")]
        public int thickness { get; set; }

        public Node[] walls;
        List<KeyValuePair<float, Node>> nodeDistances;
        private bool Available;
        /// <summary>
        /// No Half walls.
        /// </summary>
        [Info(UserLevel.User, "No Half walls.")]
        public bool onlyAvailableObs { get; set; }
        public Obstructor() : this(null) { }
        public Obstructor(Node parent)
        {
            this.parent = parent;
            nodeDistances = new List<KeyValuePair<float, Node>>();
            maxWalls = 5;
            thickness = 10;
            radius = 500;
            onlyObstructors = true;

            walls = new Node[maxWalls];
            for (int i = 0; i < maxWalls; i++)
            {
                walls[i] = CreateBlankWallPoly();
            }
        }

        public Node CreateBlankWallPoly()
        {
            Node wall = Node.ContructLineWall(room, parent.body.pos, parent.body.pos, thickness, addToWallGroup: false);
            wall.active = false;
            wall.body.ExclusionCheck += (a, b) => b.parent == parent || walls.Contains(b.parent);
            return wall;
        }

        public override void OnSpawn()
        {
            if (walls != null)
            {
                foreach(Node n in walls)
                {
                    room.masterGroup.childGroups["Wall Group"].IncludeEntity(n);
                }
            }
        }
        
        public override void AffectOther(Node other)
        {
            if (onlyObstructors && !other.HasActiveComponent<Obstructor>()) return;
            if (onlyAvailableObs && other.HasActiveComponent<Obstructor>() && !other.Comp<Obstructor>().Available) return;

            float dist = Vector2.Distance(other.body.pos, parent.body.pos);
            if (dist < radius)
            {
                if (nodeDistances.Count < maxWalls)
                {
                    nodeDistances.Add(new KeyValuePair<float, Node>(dist, other));
                    nodeDistances.Sort((a,b)=>a.Key.CompareTo(b.Key));
                }
                else if (nodeDistances.Count>0 && dist < nodeDistances.Last().Key)
                {
                    nodeDistances.Add(new KeyValuePair<float, Node>(dist, other));
                    nodeDistances.Sort((a, b) => a.Key.CompareTo(b.Key));
                    nodeDistances.RemoveAt(nodeDistances.Count-1);
                }
            }
        }

        public override void AffectSelf()
        {
            for (int i = 0; i < walls.Length; i++)
            {
                Node wall = walls[i];
                Available = false;
                if (nodeDistances.Count <= i)
                {
                    Available = true;
                    wall.active = false;
                    //wall.nodeState = state.off;
                }
                else
                {
                    KeyValuePair<float, Node> kvp = nodeDistances[i];
                    wall.active = true;
                    UpdateWall(wall, kvp.Key, kvp.Value);
                    //wall.nodeState = state.on;
                }
            }
            //int count = 0;
            //foreach (var dist in nodeDistances)
            //{
            //    Node wall = walls[count++];
            //    if (count > maxWalls)
            //    {
            //        wall.active = false;
            //    }
            //
            //    wall.active = true;
            //    UpdateWall(wall, dist.Key, dist.Value);
            //}
            nodeDistances = new List<KeyValuePair<float, Node>>();

            if (maxWalls != walls.Length && maxWalls > 0)
            {
                Node[] newWalls = new Node[maxWalls];
                int i;
                for (i = 0; i < newWalls.Length; i++)
                {
                    if (i >= walls.Length)
                    {
                        newWalls[i] = CreateBlankWallPoly();
                    }
                    else
                    {
                        newWalls[i] = walls[i];
                    }
                }
                walls = newWalls;
                
            }
        }

        public void UpdateWall(Node wall, float dist, Node other)
        {
            //float dist = Vector2.Distance(start, end);
            
            int halfheight = (int)(dist / 4);
            int halfwidth = thickness / 2;
            float angle = VMath.VectorToAngle(parent.body.pos - other.body.pos);

            //Node n = new Node(room, props, ShapeType.ePolygon);
            Polygon p = (Polygon)wall.body.shape;
            p.SetBox(halfwidth, halfheight, false); //flipped

            Vector2 endpos = parent.body.pos + (other.body.pos - parent.body.pos) * 0.25f;
            wall.body.pos = endpos;// (parent.body.pos + other.body.pos) / 2;
            p.SetOrient(angle);

            //n.body.SetStatic();
            //room.masterGroup.childGroups["Walls"].entities.Add(n);
            //return n;
        }
        public override void OnRemove(Node other)
        {
            foreach (var n in walls) n.OnDeath(other);
        }

    }
}
