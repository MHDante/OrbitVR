using System;
using System.Linq;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace OrbItProcs.ScrapBox {
  public static class Spider {
    public enum state {
      waiting,
      protecting,
      stabbing,
      walking,
    }

    const string fp = "Textures//SpiderFrames/";
    public static Texture2D Wait = OrbIt.game.Content.Load<Texture2D>("Textures//SpiderFrames/SpiderAni0001");
    public static Texture2D[] Protect = new Texture2D[23];
    public static Texture2D[] Stab = new Texture2D[25];
    public static Texture2D[] Walk = new Texture2D[28];

    static int protectCount = 0, stabCount = 0, walkCount = 0, masterCount = 0, masterMod = 3;
    public static state State = state.waiting;
    public static float spiderPos = -100f;
    static Texture2D currentTexture = null;
    static int freezeCount = 0, freezeMax = 10;
    static bool frozen = false;
    static public float scale = .6f;
    public static int spiderAttackDamage = 5;
    public static Vector2 finalpos, spiderHead;

    public static bool WinSpiderGame = false;

    static Spider() {
      int count = 0;
      //var fullList = Directory.GetFiles(fp);
      for (int i = 2; i <= 24; i++) {
        string filename = "Textures//SpiderFrames/SpiderAni00";
        if (i < 10) filename += "0";
        filename += i;
        Protect[i - 2] = OrbIt.game.Content.Load<Texture2D>(filename);
      }

      for (int i = 26; i <= 50; i++) {
        string filename = "Textures//SpiderFrames/SpiderAni00";
        if (i < 10) filename += "0";
        filename += i;
        Stab[i - 26] = OrbIt.game.Content.Load<Texture2D>(filename);
      }

      for (int i = 52; i <= 79; i++) {
        string filename = "Textures//SpiderFrames/SpiderAni00";
        if (i < 10) filename += "0";
        filename += i;
        Walk[i - 52] = OrbIt.game.Content.Load<Texture2D>(filename);
      }
      currentTexture = Wait;
    }

    public static void UpdateSpider(Room room) {
      if (false /*room.loading*/) return;
      finalpos = room.gridsystemAffect.position +
                 new Vector2((room.worldWidth - Wait.Width*scale)/2,
                   room.gridsystemAffect.gridHeight - (Wait.Height/2) - spiderPos + 400);

      room.camera.Draw(currentTexture, finalpos, Color.White, scale, Layers.Over4, center: false);

      spiderHead = new Vector2(room.gridsystemAffect.gridWidth/2,
        room.gridsystemAffect.position.Y + room.gridsystemAffect.gridHeight - spiderPos);
      float radiusReach = 300f;

      if (frozen) room.camera.Draw(textures.whitecircle, spiderHead, Color.Red*0.4f, radiusReach/64f, 0, Layers.Under5);

      if (masterCount++%masterMod != 0) return;
      if (State == state.waiting) {
        currentTexture = Wait;
        //room.camera.Draw(Wait, finalpos, Color.White, .5f, Layers.Over4, center: false);
        int r = Utils.random.Next(3);
        if (r == 0) {
          State = state.stabbing;
        }
        else if (r == 1) {
          State = state.protecting;
        }
        else {
          State = state.walking;
        }
      }
      else if (State == state.walking) {
        currentTexture = Walk[walkCount];
        //Texture2D t = Walk[walkCount];
        //room.camera.Draw(t, finalpos, Color.Blue, .5f, Layers.Over4, center: false);
        walkCount++;
        spiderPos += 1.5f;
        if (walkCount >= Walk.Length) {
          walkCount = 0;
          State = state.waiting;
        }
      }
      else if (State == state.protecting) {
        currentTexture = Protect[protectCount];
        if (protectCount == 15) {
          frozen = true;
        }
        if (protectCount == 19) {
          frozen = false;
        }
        if (protectCount == 16 || protectCount == 18) {
          foreach (Node n in room.masterGroup.fullSet.ToList()) {
            if (!n.IsPlayer) {
              float dist = Vector2.Distance(n.body.pos, spiderHead);
              if (dist < 250 && n.body.texture == textures.boulder1) {
                n.body.texture = textures.boulderShine;
                n.collision.active = false;
              }

              continue;
            }
            if (Vector2.Distance(n.body.pos, spiderHead) > radiusReach) continue;
            n.meta.CalculateDamage(null, spiderAttackDamage);
            //n.body.velocity = new Vector2(0, -2);

            n.movement.maxVelocity.value = 30f;
            n.body.velocity = new Vector2(0, -30);
            Action<Node, Node> callback = null;
            callback = delegate(Node n1, Node n2) {
              n.movement.maxVelocity.value = 2;
              n.body.OnCollisionEnter -= callback;
            };
            n.body.OnCollisionEnter += callback;
          }
        }
        if (protectCount == 17) {
          freezeCount++;
          if (freezeCount < freezeMax) return;
          freezeCount = 0;
        }
        //Texture2D t = Protect[protectCount];
        //room.camera.Draw(t, finalpos, Color.Green, .5f, Layers.Over4, center: false);
        protectCount++;
        spiderPos -= 0;
        if (protectCount >= Protect.Length) {
          protectCount = 0;
          State = state.waiting;
        }
      }
      else if (State == state.stabbing) {
        currentTexture = Stab[stabCount];
        //Texture2D t = Stab[stabCount];
        //room.camera.Draw(t, finalpos, Color.Red, .5f, Layers.Over4, center: false);
        stabCount++;
        if (stabCount >= Stab.Length) {
          stabCount = 0;
          State = state.waiting;
        }
      }
    }

    private static void SpiderBounce() {
      //this is meant to be integrated into movement
      Node parent = null;
      Room room = null;
      if (parent.body.pos.Y < parent.body.radius) {
        parent.body.pos.Y = DelegateManager.Triangle(parent.body.pos.Y - parent.body.radius, room.worldHeight) +
                            parent.body.radius;
        parent.body.velocity.Y *= -1;
        parent.body.InvokeOnCollisionStay(null);
        if (parent.IsPlayer) {
          try {
            //LoadLevelWindow.StaticLevel("NameofLevel");
          }
          catch (Exception e) {
            WinSpiderGame = true;
          }
        }
        else {
          //if (parent.texture == textures.boulder1)
          //{
          //    parent.texture = textures.boulderShine;
          //    parent.collision.active = false;
          //}
        }
      }

      int levelLeft = (int) room.gridsystemAffect.position.X, levelTop = (int) room.gridsystemAffect.position.Y;
      int levelwidth = room.gridsystemAffect.gridWidth;
      int levelheight = room.gridsystemAffect.gridHeight;

      if (parent.body.pos.X >= (levelLeft + levelwidth - parent.body.radius)) {
        //float off = parent.body.pos.X - (levelwidth - parent.body.radius);
        //parent.body.pos.X = (levelwidth - parent.body.radius - off) % room.worldWidth;
        parent.body.pos.X = DelegateManager.Triangle(parent.body.pos.X, levelwidth - (int) parent.body.radius) +
                            levelLeft;
        parent.body.velocity.X *= -1;
        parent.body.InvokeOnCollisionStay(null); //todo: find out why we needed null, fix this
      }
      else if (parent.body.pos.X < levelLeft + parent.body.radius) {
        //float off = parent.body.radius - parent.body.pos.X;
        //parent.body.pos.X = (parent.body.radius + off) % room.worldWidth;
        parent.body.pos.X = DelegateManager.Triangle(parent.body.pos.X - parent.body.radius, levelwidth) +
                            parent.body.radius + levelLeft;
        parent.body.velocity.X *= -1;
        parent.body.InvokeOnCollisionStay(null);
      }
      else if (parent.body.pos.Y < levelTop + parent.body.radius) {
        if (parent.body.pos.Y > levelTop + parent.body.radius - 100) {
          parent.body.velocity.Y *= -1;
        }
        else return;
        //parent.body.pos.Y = DelegateManager.Triangle(parent.body.pos.Y - levelTop - parent.body.radius, levelheight) + parent.body.radius + levelTop;
        //parent.body.velocity.Y *= -1;
        //parent.body.InvokeOnCollisionStay(null);
        //parent.body.pos.Y += 5;
      }
      else if (!parent.IsPlayer && parent.body.texture == textures.boulder1) {
        float y = finalpos.Y;
        float distFromCenter = parent.body.pos.X - room.gridsystemAffect.gridWidth/2;
        distFromCenter = (float) Math.Abs(distFromCenter);
        float maxDistFromCenter = 120;

        float distFromSpiderhead = Vector2.Distance(parent.body.pos, spiderHead);
        if (distFromSpiderhead < 120 && !false /*room.loading*/) {
          spiderPos -= 10;
          parent.texture = textures.boulderShine;
          parent.collision.active = false;
        }

        if (!false /*room.loading*/&& parent.body.pos.Y >= y + 200 && distFromCenter > maxDistFromCenter) {
          parent.texture = textures.boulderShine;
          parent.collision.active = false;
        }

        if (parent.body.pos.Y >= (levelTop + levelheight - parent.body.radius)) {
          parent.OnDeath(null);
        }
      }

      if (parent.IsPlayer) {
        float y = finalpos.Y;
        float distFromCenter = parent.body.pos.X - room.gridsystemAffect.gridWidth/2;
        int sign = -1;
        if (distFromCenter < 0) sign = 1;
        distFromCenter = (float) Math.Abs(distFromCenter);
        float maxDistFromCenter = 120;
        if (!false /*room.loading*/&& parent.body.pos.Y >= y + 200 && distFromCenter > maxDistFromCenter) {
          parent.movement.maxVelocity.value = 30f;
          parent.body.velocity = new Vector2(20*sign, 0);
          Action<Node, Node> callback = null;
          callback = delegate(Node n1, Node n2) {
            parent.movement.maxVelocity.value = 2;
            parent.body.OnCollisionEnter -= callback;
          };
          parent.body.OnCollisionEnter += callback;
        }
      }
    }
  }
}