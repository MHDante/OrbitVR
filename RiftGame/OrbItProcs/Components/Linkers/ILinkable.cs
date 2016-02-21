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
    public interface ILinkable
    {
        Link link { get; set; }
        Node parent { get; set; }
        bool active { get; set; }
        //void AffectSelf();
        void AffectOther(Node other);
        void Draw();
    }
}
