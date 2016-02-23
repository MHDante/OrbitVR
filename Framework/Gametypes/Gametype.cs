using System.Collections.Generic;
using SharpDX.Toolkit;

namespace OrbItProcs {
  public class Gametype {
    public Room room;

    public Gametype() {
      room = OrbIt.Game.Room;
      players = new HashSet<Player>();
    }

    public HashSet<Player> players { get; set; }

    public virtual void Update(GameTime gameTime) {
      //foreach(Player p in players)
      //{
      //    p.Update(gameTime);
      //}
    }

    public virtual void Draw() {
      //foreach (Player p in players)
      //{
      //    p.Draw();
      //}
    }
  }
}