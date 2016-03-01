using System.Collections.Generic;

namespace OrbitVR.Framework.Gametypes {
  public class Gametype {
    public Room room;

    public HashSet<Player> players { get; set; }

    public Gametype() {
      room = OrbIt.Game.Room;
      players = new HashSet<Player>();
    }

    public virtual void Update() {
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