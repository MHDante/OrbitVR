using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.CodeDom;
using System.Runtime.Serialization;
using SColor = System.Drawing.Color;
using Component = OrbItProcs.Component;
using Console = System.Console;
using sc = System.Console;

using System.IO;
using System.Collections.ObjectModel;
using System.Threading;
using Polenter.Serialization;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpOVR;
using Rectangle = System.Drawing.Rectangle;

namespace OrbItProcs
{

    public enum resolutions
    {
        AutoFullScreen,

        VGA_640x480,
        SVGA_800x600,
        XGA_1024x768,
        HD_1366x768,
        WXGA_1280x800,
        SXGA_1280x1024,
        WSXGA_1680x1050,
        FHD_1920x1080,
    }

    public class OrbIt : Game
    {
    
        public GraphicsDeviceManager Graphics;
    public SharpDX.Direct3D11.Device Device
    {
      get { return (SharpDX.Direct3D11.Device)GraphicsDevice; }
    }

    private HMD hmd;
    public static OrbIt game;
        public static UserInterface ui;
        public static GameTime gametime;
        public static bool soundEnabled = false;
        public static bool isFullScreen = false;
        private static bool GraphicsReset = false;
        private static bool redrawWhenPaused = false;
        public SharpSerializer serializer = new SharpSerializer();
        private FrameRateCounter frameRateCounter;
        public static int ScreenWidth { get { return game.Graphics.PreferredBackBufferWidth; } set { game.Graphics.PreferredBackBufferWidth = value; } }
        public static int ScreenHeight { get { return game.Graphics.PreferredBackBufferHeight; } set { game.Graphics.PreferredBackBufferHeight = value; } }
        public static int GameAreaWidth { get { return ScreenWidth - ui.sidebar.Width; } }
        public static int GameAreaHeight { get { return ScreenHeight - 40; } }
        public resolutions? prefFullScreenResolution{ get; set; }
        public resolutions prefWindowedResolution { get; set; }
        private static GlobalGameMode _globalGameMode = null;

        public static GlobalGameMode globalGameMode
        {
            get { return OrbIt._globalGameMode; }
            set { OrbIt._globalGameMode = value; 
                //if (ui != null && ui.sidebar != null)
                //{
                //    ui.sidebar.gamemodeWindow = new GamemodeWindow(ui.sidebar);
                //}
            }
        }

        public Room room { get; set; }
        public static Action OnUpdate;
      private SpriteBatch spriteBatch;

      private OrbIt()
        {
          // Creates a graphics manager. This is mandatory.
          Graphics = new GraphicsDeviceManager(this);

          // Setup the relative directory to the executable directory 
          // for loading contents with the ContentManager
          Content.RootDirectory = "Content";

          // Initialize OVR Library
          //OVR.Initialize();

          // Create our HMD
          //hmd = OVR.HmdCreate(0) ?? OVR.HmdCreateDebug(HMDType.DK2);

          // Match back buffer size with HMD resolution
          Graphics.PreferredBackBufferWidth = 640;
          Graphics.PreferredBackBufferHeight = 480;
          //Graphics.PreferredFullScreenOutputIndex = 1;




          game = this;
            prefWindowedResolution = resolutions.HD_1366x768;
        }

      protected override void LoadContent()
      {
        base.LoadContent();
        spriteBatch = new SpriteBatch(GraphicsDevice);
      }

      protected override void Initialize()
        {
            Assets.LoadAssets(Content);
            base.Initialize();
            room = new Room(this, ScreenWidth, ScreenHeight-40);
            globalGameMode = new GlobalGameMode(this);
            frameRateCounter = new FrameRateCounter(this);

            Player.CreatePlayers(room);
            ui = UserInterface.Start();
            ui.Initialize();
            room.attatchToSidebar(ui);
            GlobalKeyBinds(ui);
        }

        protected override void Update(GameTime gameTime)
        {
            
            OrbIt.gametime = gameTime;
            base.Update(gameTime);
            frameRateCounter.Update(gameTime);
            if (IsActive) ui.Update(gameTime);

            if (!ui.IsPaused) room.Update(gameTime);
            else if(redrawWhenPaused) room.drawOnly();

            
            if (GraphicsReset)
            {
                Graphics.ApplyChanges();
                room.roomRenderTarget = RenderTarget2D.New(GraphicsDevice, new Texture2DDescription() {Width = ScreenWidth, Height = ScreenHeight});
        
                GraphicsReset = false;
            }
            if (OnUpdate!= null) OnUpdate.Invoke();

        }

        //called by tom-shame
        protected override void Draw(GameTime gameTime)
        {
      base.Draw(gameTime);
      spriteBatch.Begin();
            Rectangle frame = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

            ShaderResourceView s = new ShaderResourceView(Graphics.GraphicsDevice, room.roomRenderTarget);
          spriteBatch.Draw(s,new RectangleF(0,0, ScreenWidth, ScreenHeight), Color.White);
      
            if (room.camera.TakeScreenshot)
            {
                room.camera.Screenshot();
                room.camera.TakeScreenshot = false;
            }

          spriteBatch.End();

            
        }

        public static void Start()
        {
            if (game != null) throw new SystemException("Game was already Started");
            game = new OrbIt();
            game.Run(); //XNA LOGIC HAPPENS. IT HAPPENS.
        }

        private void GlobalKeyBinds(UserInterface ui)
        {
            ui.keyManager.addGlobalKeyAction("exitgame", KeyCodes.Escape, OnPress: () => Exit());
            //ui.keyManager.addGlobalKeyAction("togglesidebar", KeyCodes.OemTilde, OnPress: ui.ToggleSidebar);
            //ui.keyManager.addGlobalKeyAction("switchview", KeyCodes.PageDown, OnPress: ui.SwitchView);
            //TODO: ui.keyManager.addGlobalKeyAction("removeall", KeyCodes.Delete, OnPress: () => ui.sidebar.btnRemoveAllNodes_Click(null, null));
        }

    }           
}
