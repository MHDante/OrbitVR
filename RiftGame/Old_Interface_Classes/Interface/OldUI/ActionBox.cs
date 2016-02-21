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
    public class ActionBox
    {
        public enum aboxstate
        {
            Ready,
            OnCooldown,
            Refreshing,
            Unavailable,
        }
    }
}
