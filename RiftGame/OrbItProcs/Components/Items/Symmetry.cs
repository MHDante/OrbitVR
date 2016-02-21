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
    /// The symmetry component allows you to spawn a set of nodes than move in a pattern, based on the symmetry to their starting conditions... and a link.
    /// </summary>
    [Info(UserLevel.User, "The symmetry component allows you to spawn a set of nodes than move in a pattern, based on the symmetry to their starting conditions... and a link.", CompType)]
    public class Symmetry : Component
    {
        public const mtypes CompType = mtypes.playercontrol | mtypes.item | mtypes.affectself;
        public override mtypes compType { get { return CompType; } set { } }

        public Symmetry() : this(null) { }
        public Symmetry(Node parent)
        {
            this.parent = parent;
        }
        public override void PlayerControl(Input input)
        {
            if (input.BtnClicked(InputButtons.RightTrigger_Mouse1))
            {
                RandomizeSymmetry();
            }
            else if (input.BtnClicked(InputButtons.RightBumper_E))
            {
                if (links.Count != 0)
                {
                    DestroyLink(links.Dequeue());
                }
            }
            else if (input.BtnClicked(InputButtons.LeftBumper_Q))
            {
                int count = links.Count;
                for (int i = 0; i < count; i++)
                {
                    DestroyLink(links.Dequeue());
                }
            }
        }
        private void DestroyLink(Link link)
        {
            link.active = false;
            //link.sourceNode.group.DiscludeEntity(link.sourceNode);
            foreach (Node n in link.targets)
            {
                if (n.group != null)
                    n.group.DiscludeEntity(n);
            }
        }

        Queue<Link> links = new Queue<Link>();
        public void RandomizeSymmetry()
        {
            Group group = room.groups.general.childGroups.Values.ElementAt(0);
            Color color = Utils.randomColor();
            Vector2 center = parent.body.pos;
            float dist = (float)Utils.random.NextDouble() * 100f + 20f;
            float angle = (float)Utils.random.NextDouble() * GMath.TwoPI;
            float speed = (float)Utils.random.NextDouble() * 5f + 1f;
            int numberOfNodes = Utils.random.Next(10) + 2;

            float angleIncrement = GMath.TwoPI / numberOfNodes;

            Node centerNode = room.defaultNode.CreateClone(room);
            centerNode.body.color = color;
            centerNode.collision.active = false;
            centerNode.body.pos = center;
            room.spawnNode(centerNode, lifetime: -1, g: group);
            HashSet<Node> outerNodes = new HashSet<Node>();
            for(int i = 0; i < numberOfNodes; i++)
            {
                Node n = centerNode.CreateClone(room);
                float angleFromCenter = angleIncrement * i;
                Vector2 spawnPosition = (VMath.AngleToVector(angleFromCenter) * dist) + center;
                Vector2 spawnVelocity = VMath.AngleToVector(angleFromCenter + angle) * speed;
                room.spawnNode(n, lifetime: -1, g: group);
                n.body.pos = spawnPosition;
                n.body.velocity = spawnVelocity;
                n.body.radius = 5f;
                n.body.mass = 10f;
                n.body.color = color * 0.5f;
                n.movement.mode = movemode.free;
                outerNodes.Add(n);

                //n.addComponent<PhaseOrb>(true);
                //n.Comp<PhaseOrb>().phaserLength = 200;

                //n.addComponent<Laser>(true);
                //n.Comp<Laser>().laserLength = 200;

                n.addComponent<Waver>(true);
                n.Comp<Waver>().Length = 200;
                n.Comp<Waver>().reflective = true;

                n.addComponent<ColorChanger>(true);
            }
            centerNode.movement.active = false;
            centerNode.basicdraw.active = false;

            Gravity grav = new Gravity();
            grav.multiplier = 20f;
            grav.radius = float.MaxValue;

            Spring spring = new Spring();
            spring.restdist = 100;
            spring.radius = float.MaxValue;

            Follow follow = new Follow();

            RelativeMotion rel = new RelativeMotion();

            Link link = new Link(parent, outerNodes, grav);
            //Link link = new Link(outerNodes, parent, follow);
            //link.AddLinkComponent(rel, true);
            //link.AddLinkComponent(follow, true);
            link.active = true;
            link.DrawLinkLines = false;
            links.Enqueue(link);
        }
    }
}
