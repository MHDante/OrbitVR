using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.CodeDom;
using System.Runtime.Serialization;
using System.Drawing;
using SColor = System.Drawing.Color;
using Component = OrbItProcs.Component;
using Console = System.Console;
using sc = System.Console;

using System.IO;
using System.Collections.ObjectModel;
using System.Threading;
using Polenter.Serialization;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

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
        #region ///////////////////// FIELDS ///////////////////
        public static OrbIt game;
        public static UserInterface ui;
        public static GameTime gametime;
        public static bool soundEnabled = false;
        public static bool isFullScreen = false;
        private static bool GraphicsReset = false;
        private static bool redrawWhenPaused = false;
        public SharpSerializer serializer = new SharpSerializer();
        private FrameRateCounter frameRateCounter;
        #endregion

        #region ///////////////////// PROPERTIES ///////////////////
        public static int ScreenWidth { get { return game.Graphics.PreferredBackBufferWidth; } set { game.Graphics.PreferredBackBufferWidth = value; } }
        public static int ScreenHeight { get { return game.Graphics.PreferredBackBufferHeight; } set { game.Graphics.PreferredBackBufferHeight = value; } }
        public static int GameAreaWidth { get { return ScreenWidth - ui.sidebar.Width - ui.sidebar.toolWindow.toolBar.Width; } }
        public static int GameAreaHeight { get { return ScreenHeight - 40; } }
        public resolutions? prefFullScreenResolution{ get; set; }
        public resolutions prefWindowedResolution { get; set; }
        private static GlobalGameMode _globalGameMode = null;

        public static GlobalGameMode globalGameMode
        {
            get { return OrbIt._globalGameMode; }
            set { OrbIt._globalGameMode = value; 
                if (ui != null && ui.sidebar != null)
                {
                    ui.sidebar.gamemodeWindow = new GamemodeWindow(ui.sidebar);
                }
            }
        }
        public Room room { get; set; }
        #endregion

        #region ///////////////////// EVENTS ///////////////////
        public static Action OnUpdate;
        #endregion

        #region ///////////////////// INITIALIZATION ///////////////////
        private OrbIt() : base(true)
        {
            game = this;
            Content.RootDirectory = "Content";
            Graphics.SynchronizeWithVerticalRetrace = true;
            ExitConfirmation = false;
            Manager.Input.InputMethods = InputMethods.Mouse | InputMethods.Keyboard;
            prefWindowedResolution = resolutions.HD_1366x768;
            Manager.AutoCreateRenderTarget = false;
            Graphics.PreferMultiSampling = false;
            SystemBorder = false;
        }

        protected override void Initialize()
        {
            Assets.LoadAssets(Content);
            base.Initialize();
            base.MainWindow.TransparentClientArea = true;
            room = new Room(this, ScreenWidth, ScreenHeight-40);
            setResolution(prefWindowedResolution, false);

            globalGameMode = new GlobalGameMode(this);
            frameRateCounter = new FrameRateCounter(this);
            
            Player.CreatePlayers(room);
            ui = UserInterface.Start();
            ui.Initialize();
            room.attatchToSidebar(ui);
            GlobalKeyBinds(ui);
        }
        #endregion



        #region ///////////////////// GAME LOOP ///////////////////
        protected override void Update(GameTime gameTime)
        {
            
            OrbIt.gametime = gameTime;
            base.Update(gameTime);
            frameRateCounter.Update(gameTime);
            if (IsActive) ui.Update(gameTime);

            if (!ui.IsPaused) room.Update(gameTime);
            else if(redrawWhenPaused) room.drawOnly();

            base.Draw(gameTime);
            if (GraphicsReset)
            {
                Manager.Graphics.ApplyChanges();
                room.roomRenderTarget = new RenderTarget2D(GraphicsDevice, ScreenWidth, ScreenHeight);
                GraphicsReset = false;
            }
            if (OnUpdate!= null) OnUpdate.Invoke();
        }

        //called by tom-shame
        protected override void DrawScene(GameTime gameTime)
        {
            Manager.Renderer.Begin(BlendingMode.Default);
            Microsoft.Xna.Framework.Rectangle frame = new Microsoft.Xna.Framework.Rectangle(0, 0, ScreenWidth, ScreenHeight);

            Manager.Renderer.Draw(room.roomRenderTarget, new Microsoft.Xna.Framework.Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);

            if (room.camera.TakeScreenshot)
            {
                room.camera.Screenshot();
                room.camera.TakeScreenshot = false;
            }
            
            Manager.Renderer.End();

            
        }
        protected override void Draw(GameTime gameTime)
        {
            //fuck tom shane
        }
        #endregion

        #region ///////////////////// HELPER METHODS ///////////////////
        public static void Start()
        {
            if (game != null) throw new SystemException("Game was already Started");
            game = new OrbIt();
            game.Run(); ///XNA LOGIC HAPPENS. IT HAPPENS.
        }

        private void GlobalKeyBinds(UserInterface ui)
        {
            ui.keyManager.addGlobalKeyAction("exitgame", KeyCodes.Escape, OnPress: () => Exit());
            ui.keyManager.addGlobalKeyAction("togglesidebar", KeyCodes.OemTilde, OnPress: ui.ToggleSidebar);
            ui.keyManager.addGlobalKeyAction("switchview", KeyCodes.PageDown, OnPress: ui.SwitchView);
            ui.keyManager.addGlobalKeyAction("removeall", KeyCodes.Delete, OnPress: () => ui.sidebar.btnRemoveAllNodes_Click(null, null));
        }
        public void setResolution(resolutions r, bool fullScreen, bool resizeRoom = false)
        {
            switch (r)
            {
                case resolutions.AutoFullScreen:
                    ScreenWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                    ScreenHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                    break;
                case resolutions.FHD_1920x1080:
                    ScreenWidth = 1920; ScreenHeight = 1080; break;
                case resolutions.HD_1366x768:
                    ScreenWidth = 1366; ScreenHeight = 768; break;
                case resolutions.SVGA_800x600:
                    ScreenWidth = 800; ScreenHeight = 600; break;
                case resolutions.SXGA_1280x1024:
                    ScreenWidth = 1280; ScreenHeight = 1024; break;
                case resolutions.VGA_640x480:
                    ScreenWidth = 640; ScreenHeight = 480; break;
                case resolutions.WSXGA_1680x1050:
                    ScreenWidth = 1680; ScreenHeight = 1050; break;
                case resolutions.WXGA_1280x800:
                    ScreenWidth = 1280; ScreenHeight = 800; break;
                case resolutions.XGA_1024x768:
                    ScreenWidth = 1024; ScreenHeight = 768; break;
            }
            Manager.Graphics.IsFullScreen = fullScreen;
            GraphicsReset = true;
        }
        #endregion
    }           
}
