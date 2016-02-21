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
    public class DiodeSpawner : Process
    {
        public const int diodeThickness = 50;
        public DiodeSpawner()
        {
            this.addProcessKeyAction("PlaceDiode", KeyCodes.LeftClick, OnPress: PlaceDiode);
            this.addProcessKeyAction("ChangeDiodeMode", KeyCodes.MiddleClick, OnPress: ChangeDiodeMode);
            this.addProcessKeyAction("AddTickets", KeyCodes.RightClick, OnPress: AddTickets);
        }
        bool firstclick = true;
        Vector2 firstPos = Vector2.Zero;
        Node lastSpawnedDiode = null;
        public void PlaceDiode()
        {
            Vector2 mousePos = UserInterface.WorldMousePos;
            if (firstclick)
            {
                firstclick = false;
                firstPos = mousePos;
            }
            else
            {
                firstclick = true;
                var dict = new Dictionary<dynamic, dynamic>() { { typeof(Diode), true },{nodeE.texture, textures.gradient1} };
                lastSpawnedDiode = Node.ContructLineWall(room, firstPos, mousePos, diodeThickness, dict, false);

                room.masterGroup.IncludeEntity(lastSpawnedDiode);
                lastSpawnedDiode.OnSpawn();

                lastSpawnedDiode.SetColor(new Color(255, 255, 255, 255));
                lastSpawnedDiode.Comp<Diode>().start = firstPos;
                lastSpawnedDiode.Comp<Diode>().end = mousePos;
                Console.WriteLine(lastSpawnedDiode.body.orient);
            }
        }

        public void ChangeDiodeMode()
        {
            Vector2 pos = UserInterface.WorldMousePos;
            Node found = null;
            float shortedDistance = Int32.MaxValue;
            for (int i = room.masterGroup.fullSet.Count - 1; i >= 0; i--)
            {
                Node n = (Node)room.masterGroup.fullSet.ElementAt(i);
                if (!n.HasComp<Diode>()) continue;
                // find node that has been clicked, starting from the most recently placed nodes
                float distsquared = Vector2.DistanceSquared(n.body.pos, pos);
                //if (distsquared < n.body.radius * n.body.radius)
                //{
                if (distsquared < shortedDistance)
                {
                    found = n;
                    shortedDistance = distsquared;
                }
                //}
            }
            if (found != null)
            {
                Diode d = found.Comp<Diode>();
                //int mode = (int)d.mode;
                //int countModes = Enum.GetValues(typeof(Diode.Mode)).Length;
                //d.mode = (Diode.Mode)((mode + 1) % countModes);
                found.body.orient += GMath.PI;
            }
        }
        public void AddTickets()
        {
            Vector2 pos = UserInterface.WorldMousePos;
            Node found = null;
            float shortedDistance = Int32.MaxValue;
            for (int i = room.masterGroup.fullSet.Count - 1; i >= 0; i--)
            {
                Node n = (Node)room.masterGroup.fullSet.ElementAt(i);
                if (!n.HasComp<Diode>()) continue;
                // find node that has been clicked, starting from the most recently placed nodes
                float distsquared = Vector2.DistanceSquared(n.body.pos, pos);
                //if (distsquared < n.body.radius * n.body.radius)
                //{
                if (distsquared < shortedDistance)
                {
                    found = n;
                    shortedDistance = distsquared;
                }
                //}
            }
            if (found != null)
            {
                Diode d = found.Comp<Diode>();

                if (!d.semaphore)
                {
                    d.semaphore = true;
                    d.maxTickets = 1;
                }
                else
                {
                    d.maxTickets += 1;
                    d.maxTickets %= 4;
                }
            }
        }
        
    }
}
