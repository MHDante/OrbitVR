using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using OrbitVR.Components.Drawers;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Graphics;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;

namespace OrbitVR.Framework {

  public class DrawCommand {
    

    public Color color;
    private float layerDepth;
    public float life;
    public float maxlife;
    private Vector2 origin;
    private Color permColor;
    private Vector2 position;
    private float rotation;
    private float scale;
    private Vector2 scalevect;
    public ShaderPack shaderPack;
    private Rectangle? sourceRect;
    private string text;
    private Textures texture;

    private DrawType type;


    public Texture2D texture2d { get; set; }

    public DrawCommand(Textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                       Vector2 origin, float scale, float layerDepth = 0,
                       int maxlife = -1,
                       ShaderPack? shaderPack = null) {
      this.type = DrawType.standard;
      this.texture = texture;
      this.position = position;
      this.color = color;
      this.permColor = color;
      this.scale = scale;
      this.rotation = rotation;
      this.sourceRect = sourceRect;
      this.origin = origin;
      this.layerDepth = layerDepth;
      this.maxlife = maxlife;
      this.life = maxlife;
      this.shaderPack = shaderPack ?? ShaderPack.Default;
    }

    public DrawCommand(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                       Vector2 origin, float scale, float layerDepth = 0,
                       int maxlife = -1,
                       ShaderPack? shaderPack = null) {
      this.type = DrawType.direct;
      this.texture2d = texture;
      this.position = position;
      this.color = color;
      this.permColor = color;
      this.scale = scale;
      this.rotation = rotation;
      this.sourceRect = sourceRect;
      this.origin = origin;
      this.layerDepth = layerDepth;
      this.maxlife = maxlife;
      this.life = maxlife;
      this.shaderPack = shaderPack ?? ShaderPack.Default;
    }

    public DrawCommand(Textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                       Vector2 origin, Vector2 scalevect,
                       float layerDepth = 0,
                       int maxlife = -1, ShaderPack? shaderPack = null) {
      this.type = DrawType.vectScaled;
      this.texture = texture;
      this.position = position;
      this.color = color;
      this.permColor = color;
      this.scalevect = scalevect;
      this.rotation = rotation;
      this.sourceRect = sourceRect;
      this.origin = origin;
      this.layerDepth = layerDepth;
      this.maxlife = maxlife;
      this.life = maxlife;
      this.shaderPack = shaderPack ?? new ShaderPack(color);
    }

    public DrawCommand(string text, Vector2 position, Color color, float scale = 0.5f, int maxlife = -1,
                       ShaderPack? shaderPack = null, float layerDepth = 0) {
      this.type = DrawType.drawString;
      this.position = position;
      this.color = color;
      this.permColor = color;
      this.layerDepth = layerDepth;
      this.scale = scale;
      this.text = text;
      this.maxlife = maxlife;
      this.life = maxlife;
      this.shaderPack = shaderPack ?? ShaderPack.Default;
    }

    public void Draw(SpriteBatch batch) {
      switch (type) {
        case DrawType.standard:
          batch.Draw(Assets.TextureDict[texture], position, sourceRect, color, rotation, origin, scale, effects,
                     layerDepth);
          break;
        case DrawType.direct:
          batch.Draw(texture2d, position, sourceRect, color, rotation, origin, scale, effects, layerDepth);
          break;
        case DrawType.vectScaled:
          batch.Draw(Assets.TextureDict[texture], position, sourceRect, color, rotation, origin, scalevect, effects,
                     layerDepth);
          break;
        case DrawType.drawString:
          batch.DrawString(Assets.Font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
          break;
      }
      if (maxlife > 0) {
        float ratio = (float) Math.Max(life/maxlife, 0.2);
        this.color = this.permColor*ratio;
      }
    }
  }

  public class ThreadedCamera : CameraBase {
    public readonly object _locker = new object();
    Queue<string> _tasks = new Queue<string>();
    Thread _worker;
    Queue<DrawCommand> addPerm = new Queue<DrawCommand>();
    ManualResetEventSlim CameraWaiting = new ManualResetEventSlim(false);
    Queue<DrawCommand> nextFrame = new Queue<DrawCommand>();
    List<DrawCommand> permanents = new List<DrawCommand>();
    Queue<DrawCommand> removePerm = new Queue<DrawCommand>();
    Queue<DrawCommand> thisFrame = new Queue<DrawCommand>();
    public ManualResetEventSlim TomShaneWaiting = new ManualResetEventSlim(true);

    public ThreadedCamera(Room room, float zoom = 0.5f, Vector2? pos = null) : base(room, zoom, pos) {
      _worker = new Thread(Work);
      _worker.Name = "CameraThread";
      _worker.IsBackground = true;
      _worker.Start();
      //Game1.ui.keyManager.addProcessKeyAction("screenshot", KeyCodes.PrintScreen, OnPress: delegate { TakeScreenshot = true; });
    }

    public override void Update() {
      base.Update();
      thisFrame = nextFrame;
      nextFrame = new Queue<DrawCommand>(); //todo: optimize via a/b pooling
      lock (_locker) {
        int count = addPerm.Count;
        for (int i = 0; i < count; i++) {
          permanents.Add(addPerm.Dequeue());
        }

        count = removePerm.Count;
        for (int i = 0; i < count; i++) {
          permanents.Remove(removePerm.Dequeue());
        }
      }

      CameraWaiting.Set();
    }


    public override void AddPermanentDraw(Textures texture, Vector2 position, Color color, float scale, float rotation,
                                          int life) {
      addPerm.Enqueue(new DrawCommand(texture, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, null, color,
                                      rotation, Assets.TextureCenters[texture], scale*zoom, SpriteEffects.None, 0, life));
    }

    public override void AddPermanentDraw(Textures texture, Vector2 position, Color color, Vector2 scalevect,
                                          float rotation,
                                          int life) {
      addPerm.Enqueue(new DrawCommand(texture, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, null, color,
                                      rotation, Assets.TextureCenters[texture], scalevect*zoom, SpriteEffects.None, 0,
                                      life));
    }

    public override void removePermanentDraw(Textures texture, Vector2 position, Color color, float scale) {
      removePerm.Enqueue(new DrawCommand(texture, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, null, color, 0,
                                         Assets.TextureCenters[texture], scale*zoom, SpriteEffects.None, 0));
    }

    private void Work(object obj) {
      while (true) {
        CameraWaiting.Reset();
        lock (_locker) {
          DepthStencilView d = null;
          var oldTargets = batch.GraphicsDevice.GetRenderTargets(out d);
          batch.GraphicsDevice.SetRenderTargets(room.RoomRenderTarget);
          batch.GraphicsDevice.Clear(backgroundColor);
          //After a lot of messing around, found some settings that worked. From: http://robjsoftware.org/2013/02/07/goodbye-xna-helloooo-sharpdx/
          batch.Begin(SpriteSortMode.Deferred, batch.GraphicsDevice.BlendStates.NonPremultiplied);
          for (int i = 0; i < 5; i++) {
            int count = thisFrame.Count;
            for (int j = 0; j < count; j++) {
              DrawCommand gg = thisFrame.Dequeue();

              // ----- Shader Set Parameter Code ---------
              float[] f;
              f = new float[2];
              f[0] = OrbIt.Game.GraphicsDevice.Viewport.Width;
              f[1] = OrbIt.Game.GraphicsDevice.Viewport.Height;

              //Assets.shaderEffect.Parameters["Viewport"].SetValue(f);
              //Assets.shaderEffect.Parameters["colour"].SetValue(gg.shaderPack.colour);
              //Assets.shaderEffect.Parameters["enabled"].SetValue(gg.shaderPack.enabled);

              // ----- End Shader Set Parameter Code ---------

              gg.Draw(batch);
            }
            int permCount = permanents.Count;
            //Console.WriteLine("1: " + permCount);
            for (int j = 0; j < permCount; j++) //todo:proper queue iteration/remove logic
            {
              DrawCommand command = permanents.ElementAt(j);
              if (command.life-- < 0) {
                permanents.Remove(command);
                j--;
                permCount--;
              }
              else {
                command.Draw(batch);
              }
            }
          }
          //Console.WriteLine("2: " + permCount);
          batch.End();
          batch.GraphicsDevice.SetRenderTargets(oldTargets);
        }
        //
        TomShaneWaiting.Set();
        CameraWaiting.Wait(); // No more tasks - wait for a signal
      }
    }

    public override void Draw(Textures texture, Vector2 position, Color color, float scale, Layers Layer,
                              ShaderPack? shaderPack = null, bool center = true) {
      nextFrame.Enqueue(new DrawCommand(texture, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, null, color, 0,
                                        center ? Assets.TextureCenters[texture] : Vector2.Zero, scale*zoom,
                                        SpriteEffects.None, (((float) Layer)/10), -1,
                                        shaderPack));
    }

    public override void Draw(Texture2D texture, Vector2 position, Color color, float scale, Layers Layer,
                              ShaderPack? shaderPack = null, bool center = true) {
      nextFrame.Enqueue(new DrawCommand(texture, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, null, color, 0,
                                        center ? new Vector2(texture.Width/2, texture.Height/2) : Vector2.Zero,
                                        scale*zoom, SpriteEffects.None,
                                        (((float) Layer)/10), -1, shaderPack));
    }

    public override void Draw(Textures texture, Vector2 position, Color color, float scale, float rotation, Layers Layer,
                              ShaderPack? shaderPack = null) {
      nextFrame.Enqueue(new DrawCommand(texture, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, null, color,
                                        rotation, Assets.TextureCenters[texture], scale*zoom, SpriteEffects.None,
                                        (((float) Layer)/10), -1, shaderPack));
    }

    public override void Draw(Texture2D texture, Vector2 position, Color color, float scale, float rotation,
                              Layers Layer,
                              ShaderPack? shaderPack = null) {
      nextFrame.Enqueue(new DrawCommand(texture, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, null, color,
                                        rotation, new Vector2(texture.Width/2, texture.Height/2), scale*zoom,
                                        SpriteEffects.None, (((float) Layer)/10),
                                        -1, shaderPack));
    }

    public override void Draw(Textures texture, Vector2 position, Color color, Vector2 scalevect, float rotation,
                              Layers Layer,
                              ShaderPack? shaderPack = null) {
      nextFrame.Enqueue(new DrawCommand(texture, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, null, color,
                                        rotation, Assets.TextureCenters[texture], scalevect*zoom, SpriteEffects.None,
                                        (((float) Layer)/10), -1,
                                        shaderPack));
    }

    public override void Draw(Textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                              Vector2 origin, float scale, Layers Layer, ShaderPack? shaderPack = null) {
      nextFrame.Enqueue(new DrawCommand(texture, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, sourceRect,
                                        color, rotation, origin, scale*zoom, SpriteEffects.None, (((float) Layer)/10),
                                        -1, shaderPack));
    }

    public override void Draw(Textures texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation,
                              Vector2 origin, Vector2 scalevect, Layers Layer, ShaderPack? shaderPack = null) {
      nextFrame.Enqueue(new DrawCommand(texture, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, sourceRect,
                                        color, rotation, origin, scalevect*zoom, SpriteEffects.None,
                                        (((float) Layer)/10), -1, shaderPack));
    }


    public override void DrawLine(Vector2 start, Vector2 end, float thickness, Color color, Layers Layer) {
      if (thickness*zoom < 1) thickness = 1/zoom;
      Vector2 diff = (end - start); // *mapzoom;
      Vector2 centerpoint = (end + start)/2;
      //centerpoint *= mapzoom;
      float len = diff.Length();
      //thickness *= 2f * mapzoom;
      Vector2 scalevect = new Vector2(len, thickness);
      float angle = (float) (Math.Atan2(diff.Y, diff.X));
      Draw(Textures.Whitepixel, centerpoint, null, color, angle, Assets.TextureCenters[Textures.Whitepixel], scalevect,
           Layer);
    }

    public override void DrawLinePermanent(Vector2 start, Vector2 end, float thickness, Color color, int life)
      //, Layers Layer)
    {
      if (thickness*zoom < 1) thickness = 1/zoom;
      Vector2 diff = (end - start); // *mapzoom;
      Vector2 centerpoint = (end + start)/2;
      //centerpoint *= mapzoom;
      float len = diff.Length();
      //thickness *= 2f * mapzoom;
      Vector2 scalevect = new Vector2(len, thickness);
      float angle = (float) (Math.Atan2(diff.Y, diff.X));
      //Draw(textures.whitepixel, centerpoint, null, color, angle, Assets.textureCenters[textures.whitepixel], scalevect, Layer);
      AddPermanentDraw(Textures.Whitepixel, centerpoint, color, scalevect, angle, life);
    }

    public override void DrawStringWorld(string text, Vector2 position, Color color, Color? color2 = null,
                                         float scale = 0.5f,
                                         bool offset = true, Layers Layer = Layers.Over5) {
      Color c2 = Color.Red;
      if (color2 != null) c2 = (Color) color2;
      Vector2 pos = position*zoom;
      if (offset) pos += CameraOffsetVect;
      nextFrame.Enqueue(new DrawCommand(text, ((position - virtualTopLeft)*zoom) + CameraOffsetVect, c2, scale,
                                        layerDepth: (((float) Layer)/10)));
      nextFrame.Enqueue(new DrawCommand(text, ((position - virtualTopLeft)*zoom) + CameraOffsetVect + new Vector2(1, -1),
                                        color, scale, layerDepth: (((float) Layer)/10)));
    }

    public override void DrawStringScreen(string text, Vector2 position, Color color, Color? color2 = null,
                                          float scale = 0.5f,
                                          bool offset = true, Layers Layer = Layers.Over5) {
      Color c2 = Color.White;
      if (color2 != null) c2 = (Color) color2;
      Vector2 pos = position;
      if (offset) pos += CameraOffsetVect;
      nextFrame.Enqueue(new DrawCommand(text, pos, c2, scale, layerDepth: (((float) Layer)/10)));
      //nextFrame.Enqueue(new DrawCommand(text, pos + new Vector2(1, -1), color, scale, layerDepth: (((float)Layer) / 10)));
    }

    public override void Screenshot() {
      Texture2DBase t2d = room.RoomRenderTarget;
      int i = 0;
      string name;
      string date = DateTime.Now.ToShortDateString().Replace('/', '-');
      do {
        name = "..//..//..//Screenshots//SS_" + date + "_#" + i + ".png";
        i += 1;
      } while (File.Exists(name));
      //Scheduler.fanfare.Play();
      Stream st = new FileStream(name, FileMode.Create);
      t2d.Save(st, ImageFileType.Png);
      st.Close();
      //t2d.Dispose();
    }

    internal void CatchUp() {
      TomShaneWaiting.Wait();
      TomShaneWaiting.Reset();
    }

    internal void AbortThread() {
      try {
        _worker.Abort();
      }
      catch {}
    }
  }
}