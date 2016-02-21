﻿using System;
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
    /// Nodes will leave behind a trail consisting of fading images of themselves.
    /// </summary>
    [Info(UserLevel.User, "Nodes will leave behind a trail consisting of fading images of themselves.", CompType)]
    public class PhaseOrb : Component
    {
        public const mtypes CompType = mtypes.draw | mtypes.tracer;
        public override mtypes compType { get { return CompType; } set { } }

        private int _phaserLength = 10;
        /// <summary>
        /// Sets the length of the phaser.
        /// </summary>
        [Info(UserLevel.User, "Sets the length of the phaser. ")]
        public int phaserLength
        {
            get
            {
                return _phaserLength;
            }
            set
            {
                _phaserLength = value;
            }
        }
        public Toggle<int> fade { get; set; }
        public PhaseOrb() : this(null) { }
        public PhaseOrb(Node parent = null)
        {
            if (parent != null)
            {
                this.parent = parent;
            }
            InitializeLists(); 
            fade = new Toggle<int>(phaserLength);
        }

        public override void AfterCloning()
        {
            //if (!parent.HasComp<Queuer>()) parent.addComponent(comp.queuer, true);
            //parent.Comp<Queuer>().qs = parent.Comp<Queuer>().qs | queues.scale | queues.position;
        }
        public override void Draw()
        {
            room.camera.AddPermanentDraw(parent.texture, parent.body.pos, parent.body.color, parent.body.scale, 0, phaserLength);
        }
    }
}
