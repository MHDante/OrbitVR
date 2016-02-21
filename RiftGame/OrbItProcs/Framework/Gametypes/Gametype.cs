using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace OrbItProcs
{
    public class Gametype
    {
        public Room room;
        public HashSet<Player> players { get; set; }
        public Gametype()
        {
            room = OrbIt.game.room;
            players = new HashSet<Player>();
        }
        public virtual void Update(GameTime gameTime)
        {
            //foreach(Player p in players)
            //{
            //    p.Update(gameTime);
            //}
        }
        public virtual void Draw()
        {
            //foreach (Player p in players)
            //{
            //    p.Draw();
            //}
        }
    }
}
