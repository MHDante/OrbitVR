using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

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
