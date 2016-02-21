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
    public class CameraControl : Process
    {
        public float velocity = 20f;
        public Toggle<Node> targetedNode { get; set; }
        public CameraControl() 
        { 
            targetedNode = new Toggle<Node>(null, false);
            addProcessKeyAction("targetNode", KeyCodes.LeftShift, KeyCodes.LeftClick, OnPress: TargetNode);
        }
        public void TargetNode()
        {
            Node n = room.SelectNodeAt(UserInterface.WorldMousePos);
            if (n == null)
            {
                targetedNode.enabled = false;
                return;
            }
            targetedNode.enabled = true;
            targetedNode.value = n;
        }
        public override void Update()
        {
            if (targetedNode.enabled)
            {
                if (targetedNode.value == null)
                {
                    Console.WriteLine("CAMERACONTROL: We've found it. We've finally found null.");
                }
                else
                {
                    room.camera.pos = targetedNode.value.body.pos;
                    return;
                }
            }

            KeyboardState state = KeyManager.newKeyboardState;
            KeyboardState oldstate = KeyManager.oldKeyboardState;
            bool up = state.IsKeyDown(Keys.Up);
            bool down = state.IsKeyDown(Keys.Down);
            bool left = state.IsKeyDown(Keys.Left);
            bool right = state.IsKeyDown(Keys.Right);

            Stick s = new Stick(up, down, left, right);
            room.camera.pos += s.v2 * velocity;
        }

    }
}
