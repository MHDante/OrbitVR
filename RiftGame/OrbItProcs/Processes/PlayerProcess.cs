using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OrbItProcs
{
    public class PlayerProcess : Process
    {
        public PlayerProcess() { }
        protected override void OnActivate()
        {
            base.OnActivate();
            Player.TryCreatePcPlayer();
        }

    }
}
