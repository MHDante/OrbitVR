using System;
using System.Collections.Generic;
using System.Linq;
using OrbitVR.Components.Meta;
using OrbitVR.Components.Tracers;
using OrbitVR.Physics;
using SharpDX;

namespace OrbitVR.Framework.Gametypes {
  public class BigTonyData : PlayerData {
    public bool switchAvailable = true;
  }

  //todo:make the players use maxvel 20
  public class BigTony : Gametype {
    public static float maxScore = 50000;
    public static int colorChange = 120;
    public static float smallmass = 2;
    public static float bigmass = 5;
    public static float tonymass = 10;
    public static Node bigtony = null;

    public Vector2 accel;
    Action<Node> after;
    Action<Node> before;
    public Action<Node, Node> onCollisionEnter;
    public Action<Node, Node> onCollisionExit;
    public Action<Node, Node> onCollisionStay;

    Dictionary<dynamic, dynamic> playerProps = new Dictionary<dynamic, dynamic>() {
      {nodeE.texture, textures.blackorb},
      {typeof (PhaseOrb), true},
    };

    Dictionary<dynamic, dynamic> userP = new Dictionary<dynamic, dynamic>() {
      {nodeE.position, new Vector2(0, 0)},
      {nodeE.texture, textures.blueorb},
    };

    public BigTony() : base() {
      //OrbIt.ui.SetSidebarActive(false);
      onCollisionEnter = delegate(Node s, Node t) {
                           if (t != null && !Player.players.Select(p => p.node).Contains(t)) {
                             t.body.color = Add(s.body.color, new Color(colorChange, colorChange, colorChange));
                               //changed node to s
                           }
                         };
      onCollisionExit = delegate(Node s, Node t) {
                          if (t != null && !Player.players.Select(p => p.node).Contains(t)) {
                            t.body.color = Color.White;
                          }
                        };
      accel = new Vector2(0, 0);
      absaccel = 0.2f;
      friction = 0.01f;
      before = (n) => { if (n != bigtony) n.body.mass = smallmass; };
      after = (n) => { if (n != bigtony) n.body.mass = bigmass; };

      InitializePlayers();
    }

    public float absaccel { get; set; }
    public float friction { get; set; }

    public void InitializePlayers() {
      for (int i = 0; i < 8; i++) {
        //Player p = Player.GetNew(i);
        Player p = new Player(i);
        if (p == null) break;
        Vector2 spawnPos = Vector2.Zero;
        double angle = Utils.random.NextDouble()*Math.PI*2;
        angle -= Math.PI;
        float dist = 200;
        float x = dist*(float) Math.Cos(angle);
        float y = dist*(float) Math.Sin(angle);
        spawnPos = new Vector2(room.WorldWidth/2, room.WorldHeight/2) - new Vector2(x, y);

        //add //{ nodeE.position, spawnPos },
        p.node = room.spawnNode(playerProps);
        p.node.name = "player" + p.playerIndex;
        

        Collider collider = new Collider(new Circle(25));
        p.node.collision.AddCollider("trigger", collider);
        collider.OnCollisionEnter += onCollisionEnter;
        collider.OnCollisionExit += onCollisionExit;

        p.node.addComponent<Swap>(true);
        p.node.Comp<Swap>().OnSwapBefore += before;
        p.node.Comp<Swap>().OnSwapAfter += after;
      }
    }

    public void MakeBigTony(Room room) {
      //if (bigtony != null)
      //{
      //    return;
      //}
      Dictionary<dynamic, dynamic> tonyProps = new Dictionary<dynamic, dynamic>() {
        {nodeE.position, new Vector2(room.WorldWidth/2, room.WorldHeight/2)},
        {nodeE.texture, textures.blackorb},
        {typeof (PhaseOrb), true},
      };
      Node tony = new Node(room, tonyProps);
      room.Scheduler.doEveryXMilliseconds(delegate {
                                            //if (OrbIt.soundEnabled) Scheduler.end.Play(0.3f, -0.5f, 0f);
                                            int rad = 100;
                                            for (int i = 0; i < 10; i++) {
                                              int rx = Utils.random.Next(rad*2) - rad;
                                              int ry = Utils.random.Next(rad*2) - rad;
                                              //room.spawnNode(room.worldWidth / 2 + rx, room.worldHeight / 2 + ry);
                                            }
                                          }, 2000);
      //tony.body.pos = new Vector2(room.worldWidth / 2, room.worldHeight / 2);
      tony.body.radius = 64;
      tony.body.mass = tonymass;
      tony.body.velocity *= 100;
      tony.name = "bigTony";
      tony.body.texture = textures.blackorb;

      bigtony = tony;

      EventHandler updateScores = null;
      updateScores = (ooo, eee) => {
                       foreach (var p in Player.players) {
                         if (p.node == bigtony) {
                           p.node.meta.score += OrbIt.Game.Time.ElapsedGameTime.Milliseconds;
                           if (p.node.meta.score >= maxScore) {
                             p.node.body.radius += 500;
                             p.node.body.mass += 100;
                             foreach (var pp in Player.players) {
                               //pp.node.body.ClearHandlers();
                               //pp.nodeCollision.body.ClearHandlers();
                               pp.node.collision.AllHandlersEnabled = false;
                             }
                             //if (OrbIt.soundEnabled) Scheduler.fanfare.Play();
                             bigtony.OnAffectOthers -= updateScores;
                           }
                         }
                       }
                     };
      bigtony.OnAffectOthers += updateScores;

      room.MasterGroup.fullSet.Add(bigtony); //#bigtony
    }

    public static Color Add(Color c, Color b) {
      c = new Color(c.R + b.R, c.G + b.G, c.B + b.B, c.A);
      return c;
    }

    public static Color Subtract(Color c, Color b) {
      c = new Color(c.R - b.R, c.G - b.G, c.B - b.B, c.A);
      return c;
    }
  }
}