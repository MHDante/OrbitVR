using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrbItProcs;

using Color = SharpDX.Color;
using SharpDX;
using SharpOVR;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.DXGI;
using SharpDX.Toolkit.Content;

namespace OrbItProcs.ScrapBox
{
    class Misc
    {
        

        public void AssignColor(Group activegroup, Node newNode)
        {
            if (Group.IntToColor.ContainsKey(activegroup.GroupId))
            {
                newNode.body.color = Group.IntToColor[activegroup.GroupId];
            }
            else
            {
                int Enumsize = Enum.GetValues(typeof(System.Drawing.KnownColor)).Length;

                //int rand = Utils.random.Next(size - 1);
                int index = 0;
                foreach (char c in activegroup.Name.ToCharArray().ToList())
                {
                    index += (int)c;
                }
                index = index % (Enumsize - 1);

                System.Drawing.Color syscolor = System.Drawing.Color.FromKnownColor((System.Drawing.KnownColor)index);
                Color xnacol = new Color(syscolor.R, syscolor.G, syscolor.B, syscolor.A);
                newNode.body.color = xnacol;
            }
        }
    }
}
