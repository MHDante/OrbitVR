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
using System.Reflection;


using Component = OrbItProcs.Component;
using System.IO;

namespace OrbItProcs {
    public class UserInterface 
    {
        private static UserInterface ui;
        public static bool tomShaneWasClicked = false;
        public static readonly Color TomShanePuke = new Color(75, 187, 0);
        public static readonly Color TomDark = new Color(65, 65, 65);
        public static readonly Color TomLight = new Color(180, 180, 180);
        public enum selection
        {
            placeNode,
            targetSelection,
            groupSelection,
            randomNode,
        }
        public static int SidebarWidth { get; set; }
        public static Vector2 MousePos;
        public static Vector2 WorldMousePos;
        

        #region /// Fields ///

        public OrbIt game{ get { return OrbIt.game; } }
        public Room room { get { return game.room; } }


        public KeyManager keyManager { get; set; }

        
        public static KeyboardState keybState, oldKeyBState;
        public static MouseState mouseState, oldMouseState;
        
        //public string currentSelection = "placeNode";//
        public selection currentSelection = selection.placeNode;
        int oldMouseScrollValue = 0;//
        bool hovertargetting = false;//
        //int rightClickCount = 0;//
        //int rightClickMax = 1;//
        public int sWidth = 1000;////
        public int sHeight = 600;////
        bool isShiftDown = false;
        //bool isTargeting = false;
        public Vector2 spawnPos;
        Vector2 groupSelectionBoxOrigin = new Vector2(0, 0);
        public HashSet<Node> groupSelectSet;
        #endregion

        public float zoomfactor { get; set; }
        public static bool GameInputDisabled { get; set; }

        public static string Checkmark = "\u2714";
        public static string Cross = "\u2718";
        public bool IsPaused { get; set; }

        public Dictionary<dynamic, dynamic> UserProps;

        private bool _SidebarActive = false;
        public bool SidebarActive { get { return _SidebarActive; } 
            set 
            { 
                _SidebarActive = value;
                if (value)
                {
                    OrbIt.game.room.camera.CameraOffset = sidebar.Width;
                }
                else
                {
                    //OrbIt.game.room.camera.CameraOffset = 0;
                }
            } 
        }

        public Node spawnerNode;
        public Sidebar sidebar;

        private UserInterface()
        {
            sidebar = new Sidebar(this);
            
            zoomfactor = 0.9f;
            GameInputDisabled = false;
            IsPaused = false;
            this.keyManager = new KeyManager(this);
            SidebarActive = true;
        }

        public void Initialize()
        {
            sidebar.Initialize();
            ui.sidebar.UpdateGroupComboBoxes();
            ui.sidebar.cbListPicker.ItemIndex = 0;
            ui.sidebar.cbListPicker.ItemIndex = 2;
            ui.sidebar.cbGroupS.ItemIndex = 2;
            ui.sidebar.cbGroupT.ItemIndex = 2;

            ui.sidebar.InitializeGroupsPage();
            ui.sidebar.InitializePlayersPage();
            ui.sidebar.InitializeItemsPage();
            ui.sidebar.InitializeProcessesPage();
            //ui.sidebar.InitializeBulletsPage();
            foreach (var tabpage in ui.sidebar.tbcViews.TabPages)
            {
                string whitespace = "  ";
                tabpage.Text = whitespace + tabpage.Text + whitespace;
            }
        }

        public void SetSidebarActive(bool active)
        {
            if (active)
            {
                sidebar.master.Visible = true;
                sidebar.master.Enabled = true;
            }
            else
            {
                sidebar.master.Visible = false;
                sidebar.master.Enabled = false;
            }
            SidebarActive = active;
        }

        public void SwitchView()
        {
            sidebar.activeTabControl = (sidebar.activeTabControl == sidebar.tbcViews) ? sidebar.tbcMain : sidebar.tbcViews;
            if (sidebar.activeTabControl == sidebar.tbcMain)
            {
                OrbIt.ui.sidebar.UpdateGroupComboBoxes();
            }
        }

        public void ToggleSidebar()
        {
            if (SidebarActive)
            {
                sidebar.master.Visible = false;
                sidebar.master.Enabled = false;
                //foreach(Button b in ToolWindow.buttons.Values)
                //{
                //    b.Visible = false;
                //}

                sidebar.toolWindow.toolBar.Visible = false;

            }
            else
            {
                sidebar.master.Visible = true;
                sidebar.master.Enabled = true;
                foreach (Button b in ToolWindow.buttons.Values)
                {
                    b.Visible = true;
                }
            }
            SidebarActive = !SidebarActive;
        }

        public List<DetailedView> detailedViews = new List<DetailedView>();
        private int refreshCount = 0;

        public void Update(GameTime gameTime)
        {
            ProcessKeyboard();
            
            ProcessMouse();
            
            ProcessController();
            //only update once per second to save performance (heavy reflection)
            if (refreshCount++ % 60 == 0)
            {
                if (sidebar != null)
                {
                    foreach (var view in detailedViews)
                    {
                        view.RefreshLight(true);
                    }
                }
            }

            //game.testing.KeyManagerTest(() => Keybindset.Update());
            keyManager.Update();
            sidebar.Update();

            //randomizerProcess = new Randomizer();
            
        }

        public void ProcessKeyboard()
        {
            keybState = Keyboard.GetState();

            if (GameInputDisabled) return;

            if (keybState.IsKeyDown(Keys.Y))
                hovertargetting = true;
            else
                hovertargetting = false;


            if (keybState.IsKeyDown(Keys.Space) && oldKeyBState.IsKeyUp(Keys.Space))
            {
                room.Update(null);
            }

            if (keybState.IsKeyDown(Keys.LeftShift))
            {
                if (!isShiftDown)
                { 
                    MouseState ms = Mouse.GetState();
                    spawnPos = new Vector2(ms.X, ms.Y) / room.camera.zoom;
                }
                isShiftDown = true;
            }
            else
            {
                isShiftDown = false;
            }

            //if (keybState.IsKeyDown(Keys.F) && !oldKeyBState.IsKeyDown(Keys.F))
            //    IsPaused = !IsPaused;

            oldKeyBState = Keyboard.GetState();
        }

        public Node SelectNode(Vector2 pos)
        {
            Node found = null;
            float shortedDistance = Int32.MaxValue;
            for (int i = room.masterGroup.fullSet.Count - 1; i >= 0; i--)
            {
                Node n = (Node)room.masterGroup.fullSet.ElementAt(i);
                // find node that has been clicked, starting from the most recently placed nodes
                float distsquared = Vector2.DistanceSquared(n.body.pos, pos);
                if (distsquared < n.body.radius * n.body.radius)
                {
                    if (distsquared < shortedDistance)
                    {
                        found = n;
                        shortedDistance = distsquared;
                    }

                }
            }
            return found;
        }

        public void ProcessController()
        {
            //GamePad.SetVibration(PlayerIndex.Two, 0.1f, 0.9f);
            //System.Console.WriteLine(GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X);
            //GraphData.AddFloat(GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X * 10);
        }

        public Action<int> ScrollAction;

        public void SetScrollableControl(Control control, Action<int> action)
        {
            if (control == null || action == null) return;
            control.MouseOver += (s, e) => {
                ScrollAction = action;
            };
            control.MouseOut += (s, e) =>
            {
                ScrollAction = null;
            };
        }

        public void ProcessMouse()
        {
            mouseState = Mouse.GetState();

            if (UserInterface.tomShaneWasClicked)
            {
                mouseState = oldMouseState;
            }
            //if (mouseState.XButton1 == ButtonState.Pressed)
            //    System.Console.WriteLine("X1");
            //
            //if (mouseState.XButton2 == ButtonState.Pressed)
            //    System.Console.WriteLine("X2");

            MousePos = new Vector2(mouseState.X, mouseState.Y) - OrbIt.game.room.camera.CameraOffsetVect;
            WorldMousePos = (MousePos / room.camera.zoom) + room.camera.virtualTopLeft;
            //ignore mouse clicks outside window
            if (!OrbIt.isFullScreen)
            {
                if (mouseState.X >= OrbIt.ScreenWidth || mouseState.X < 0 || mouseState.Y >= OrbIt.ScreenHeight || mouseState.Y < 0)
                    return;
            }

            //if (!keyManager.MouseInGameBox)
            //{
                if (ScrollAction != null)
                {
                    if (mouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue)
                    {
                        ScrollAction(2);
                    }
                    else if (mouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue)
                    {
                        ScrollAction(-2);
                    }
                }
                
                oldMouseState = mouseState;
               // return;
            //}

            if (GameInputDisabled || !keyManager.MouseInGameBox) return;
            //game.processManager.PollMouse(mouseState, oldMouseState);
            int worldMouseX = (int)WorldMousePos.X;
            int worldMouseY = (int)WorldMousePos.Y;

            
            if (hovertargetting)
            {
                if (true)// || mouseState.LeftButton == ButtonState.Pressed)
                {
                    bool found = false;
                    for (int i = room.masterGroup.fullSet.Count - 1; i >= 0; i--)
                    {
                        Node n = (Node)room.masterGroup.fullSet.ElementAt(i);
                        // find node that has been clicked, starting from the most recently placed nodes
                        if (Vector2.DistanceSquared(n.body.pos, new Vector2(worldMouseX, worldMouseY)) < n.body.radius * n.body.radius)
                        {
                            room.targetNode = n;
                            found = true;
                            break;
                        }
                    }
                    if (!found) room.targetNode = null;
                }
            }

            if (mouseState.ScrollWheelValue < oldMouseScrollValue)
            {
                room.camera.zoom *= zoomfactor;
            }
            else if (mouseState.ScrollWheelValue > oldMouseScrollValue)
            {
                room.camera.zoom /= zoomfactor;
            }

            oldMouseScrollValue = mouseState.ScrollWheelValue;
            oldMouseState = mouseState;
        }

        internal static UserInterface Start()
        {
            ui = new UserInterface();
            return ui;
        }
    }
}