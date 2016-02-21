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
    public class SingleSelect : Process
    {
        public SingleSelect() : base()
        {
            addProcessKeyAction("SingleSel", KeyCodes.LeftClick, OnPress: SingleSel);
            //addProcessKeyAction("MakeLink", KeyCodes.RightClick, OnPress: MakeLink);
            //addProcessKeyAction("StartLink", KeyCodes.MiddleClick, OnPress: StartLink);

            addProcessKeyAction("PickupNode", KeyCodes.RightClick, OnPress: PickupNode, OnHold: DragNode, OnRelease: ReleaseNode);
        }
        public void SingleSel()
        {
            //if (buttonState == ButtonState.Released) return;
            Node found = room.SelectNodeAt(UserInterface.WorldMousePos);
            OrbIt.ui.sidebar.SetTargetNode(found);

        }
        Node nodeInHand = null;
        public void PickupNode()
        {
            nodeInHand = room.SelectNodeAt(UserInterface.WorldMousePos);
            if (nodeInHand != null)
            {
                nodeInHand.movement.effvelocityMode = true;
                OrbIt.ui.sidebar.SetTargetNode(nodeInHand);
            }
        }
        public void DragNode()
        {
            if (nodeInHand != null)
            {
                //nodeInHand.body.velocity = UserInterface.WorldMousePos - nodeInHand.body.pos;
                //nodeInHand.body.velocity = nodeInHand.body.effvelocity;
                nodeInHand.body.pos = UserInterface.WorldMousePos;
            }
        }
        public void ReleaseNode()
        {
            if (nodeInHand != null)
            {
                nodeInHand.movement.effvelocityMode = false;
                nodeInHand = null;
                OrbIt.ui.sidebar.SetTargetNode(null);
            }
        }

        public void MakeLink()
        {
            //if (buttonState == ButtonState.Released) return;
            Node found = room.SelectNodeAt(UserInterface.WorldMousePos);
            if (found != null)
            {
                if (room.targetNode != null && room.targetNode.HasComp<Flow>())
                {
                    room.targetNode.Comp<Flow>().AddToOutgoing(found);
                }
                if (room.targetNode != null && room.targetNode.HasComp<Tether>())
                {
                    room.targetNode.Comp<Tether>().AddToOutgoing(found);
                }
            }
        }
        public void StartLink()
        {
            //if (buttonState == ButtonState.Released) return;
            Node found = room.SelectNodeAt(UserInterface.WorldMousePos);
            if (found != null)
            {
                if (found.HasComp<Flow>())
                {
                    found.Comp<Flow>().activated = !found.Comp<Flow>().activated;
                }
                if (found.HasComp<Tether>())
                {
                    found.Comp<Tether>().activated = !found.Comp<Tether>().activated;
                }
            }
        }
    }
}
