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
using TomShane.Neoforce.Controls;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;
using Microsoft.Xna.Framework.Graphics;

namespace OrbItProcs
{

    public class ToolWindow
    {
        public Manager manager;
        public Sidebar sidebar;
        public SideBar toolBar;
        public int HeightCounter = 5;
        public int LeftPadding = 5;
        public Dictionary<string, Texture2D> buttonTextures = new Dictionary<string,Texture2D>();
  //      Texture2D[,] textures;
        public static Dictionary<string, Button> buttons = new Dictionary<string, Button>();
        
        public ToolWindow(Sidebar sidebar)
        {
            this.sidebar = sidebar;
            this.manager = sidebar.manager;
            toolBar = new SideBar(manager);
            OrbIt.game.MainWindow.Add(toolBar);
            toolBar.MouseOver += delegate { UserInterface.tomShaneWasClicked = true; };
            toolBar.MouseOut += delegate { UserInterface.tomShaneWasClicked = false; };
            toolBar.Anchor = Anchors.Top | Anchors.Right | Anchors.Bottom;
            toolBar.Init();
            toolBar.Height = OrbIt.game.MainWindow.ClientArea.Height;
            toolBar.Width = 70;
            toolBar.Left = OrbIt.game.MainWindow.ClientArea.Width - toolBar.Width;

            buttonTextures["select"] = Assets.btnTextures[0, 0];
            buttonTextures["random"] = Assets.btnTextures[1, 0];
            buttonTextures["spawn"] = Assets.btnTextures[0, 1];
            buttonTextures["level"] = Assets.btnTextures[1, 1];
            buttonTextures["forceSpawn"] = Assets.btnTextures[0, 2];
            buttonTextures["forcePush"] = Assets.btnTextures[1, 2];
            buttonTextures["control"] = Assets.btnTextures[0, 3];
            buttonTextures["static"] = Assets.btnTextures[1, 3];
            buttonTextures["remove"] = Assets.btnTextures[0, 4];
            buttonTextures["resize"] = Assets.btnTextures[1, 4];

        }
        public void reset()
        {
            foreach (Button b in buttons.Values) b.Enabled = true;
        }

        public void AddButton(string s, Action action, string tooltip = null)
        {
            Button button = new Button(manager);

            button.Init();
            button.Parent = toolBar;
            button.Text = "";
            button.Width = toolBar.Width - (30 / 2);
            button.Left = 5;
            button.Top = HeightCounter;
            button.Height = button.Width;
            button.MouseOver += delegate { UserInterface.tomShaneWasClicked = true; };
            button.MouseOut += delegate { UserInterface.tomShaneWasClicked = false; };
                HeightCounter += button.Height + 5;

            
            button.Click += (se, e) =>
            {
                foreach(Button bb in buttons.Values)
                {
                    reset();
                    button.Enabled = false;
                    bb.TextColor = Color.White;
                    bb.Refresh();
                }
                button.TextColor = UserInterface.TomShanePuke;
                if (action != null) action();
            };
            OrbIt.game.Graphics.DeviceReset += (se, e) =>
            {
                button.Refresh();
            };

            if (!String.IsNullOrWhiteSpace(tooltip)) button.ToolTip.Text = tooltip;
            else button.ToolTip.Text = s;
            Texture2D tt = buttonTextures[s];

            button.Draw += (se, e) =>
            {
                if (!(se as Button).Enabled) e.Renderer.Draw(Assets.textureDict[textures.whitepixel], e.Rectangle.contract(5), Color.Goldenrod);
                e.Renderer.Draw(tt, e.Rectangle, Color.White);
            };

            buttons.Add(s, button);
        }
    }
}
