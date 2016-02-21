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
    public class DirectedSpawn : Process
    {
        private Vector2 spawnPos;
        int rightClickCount = 0;//
        int rightClickMax = 1;//
        public DirectedSpawn() : base()
        {
            addProcessKeyAction("DirectedLaunch", KeyCodes.LeftClick, OnHold: DirectedLaunch);
            addProcessKeyAction("SetSpawnPosition", KeyCodes.RightClick, OnPress: SetSpawnPosition, OnRelease:UnsetSpawnPosition);
        }

        public void SetSpawnPosition()
        {
            spawnPos = UserInterface.WorldMousePos;
        }
        public void UnsetSpawnPosition()
        {
            spawnPos = Vector2.Zero;
        }
        public void DirectedLaunch()
        {
            if (spawnPos == Vector2.Zero) return;
            rightClickCount++;
            if (rightClickCount % rightClickMax == 0)
            {
                Vector2 positionToSpawn = spawnPos;
                Vector2 diff = UserInterface.WorldMousePos;
                diff = diff - positionToSpawn;
                Dictionary<dynamic, dynamic> userP = new Dictionary<dynamic, dynamic>() {
                            { typeof(Lifetime), true },
                            { nodeE.position, positionToSpawn },
                            { nodeE.velocity, diff },
                        };
                Action<Node> after = delegate(Node n)
                {
                    n.body.velocity = diff;
                };
                room.spawnNode(userP, after);
                rightClickCount = 0;
            }
        }
    }
}
