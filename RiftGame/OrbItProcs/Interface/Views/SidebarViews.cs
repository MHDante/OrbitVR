using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Component = OrbItProcs.Component;
using Console = System.Console;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;
using Microsoft.Xna.Framework;

namespace OrbItProcs
{
    //views pages
    public partial class Sidebar
    {
        public DetailedView detailedView { get; set; }
        public InspectorView inspectorView { get; set; }
        public GroupsView groupsView { get; set; }
        public GroupsView presetsView { get; set; }

        public TabControl tbcViews;

        private TabControl _activeTabControl;

        public Button btnOptions;

        public TabControl activeTabControl
        {
            get { return _activeTabControl; }
            set
            {
                if (value != null)
                {
                    if (_activeTabControl != null && value != _activeTabControl)
                    {
                        _activeTabControl.Visible = false;
                    }
                }
                _activeTabControl = value;
                _activeTabControl.Visible = true;
                _activeTabControl.Refresh();
            }
        }

        public ToolWindow toolWindow;
        public TabControl tbcGroups;
        public void InitializeGroupsPage()
        {
            tbcMain.Visible = false;

            tbcViews = new TabControl(manager);
            tbcViews.Init();
            master.Add(tbcViews);
            tbcViews.Left = 0;
            tbcViews.Top = 0;
            tbcViews.Width = master.Width - 5;
            tbcViews.Height = master.Height - 60;
            tbcViews.Anchor = Anchors.All;
            tbcViews.Color = UserInterface.TomLight;

            tbcViews.AddPage();

            TabPage groupsTab = tbcViews.TabPages[0];
            //tbcViews.Color = Color.Transparent;
            groupsTab.Text = "Groups";
            tbcViews.SelectedIndex = 0;
            activeTabControl = tbcViews;

            TitlePanel titlePanelGroups = new TitlePanel(this, groupsTab, "Groups", false);

            tbcGroups = new TabControl(manager);
            tbcGroups.Init();
            tbcGroups.Parent = groupsTab;
            tbcGroups.Top = titlePanelGroups.Height * 2;
            tbcGroups.Height = 460;
            tbcGroups.Width = groupsTab.Width;

            tbcGroups.AddPage("Custom");
            TabPage customPage = tbcGroups.TabPages[0];
            groupsView = new GroupsView(this, customPage, 0, -20, room.groups.general);
            groupsView.btnCreateGroup.Text = "     Create \nCustom  Group";
            groupsView.lblGroupLabel.Text = "Custom Groups";
            groupsView.UpdateGroups();

            
            tbcGroups.AddPage("Presets");
            tbcGroups.SelectedIndex = 1;
            TabPage presetsPage = tbcGroups.TabPages[1];
            presetsView = new GroupsView(this, presetsPage, 0, -20, room.groups.preset);
            presetsView.btnCreateGroup.Text = "     Create \nPreset  Group";
            presetsView.lblGroupLabel.Text = "Preset Groups";
            presetsView.UpdateGroups();
            tbcGroups.SelectedIndex = 0;

            tbcViews.SelectedIndex = 0;

            toolWindow = new ToolWindow(this);
            gamemodeWindow = new GamemodeWindow(this);
            gamemodeWindow.window.Visible = false;

            Button btnGameMode = new Button(manager);
            btnGameMode.Init();
            btnGameMode.Top = tbcViews.Top + tbcViews.Height;
            btnGameMode.Left = 15;
            btnGameMode.Text = "Mode";
            btnGameMode.Width = (int)manager.Skin.Fonts[0].Resource.MeasureString(btnGameMode.Text).X+10;
            btnGameMode.ClientMargins = new Margins(0, btnGameMode.ClientMargins.Top, 0, btnGameMode.ClientMargins.Bottom);
            btnGameMode.Anchor = Anchors.Bottom;
            master.Add(btnGameMode);
            btnGameMode.Click += (s, e) =>
            {
                gamemodeWindow.window.Visible = !gamemodeWindow.window.Visible;
            };

            btnOptions = new Button(manager);
            btnOptions.Init();
            master.Add(btnOptions);
            btnOptions.Left = btnGameMode.Left+btnGameMode.Width;
            btnOptions.Top = tbcViews.Top + tbcViews.Height;
            btnOptions.Text = "Options";
            btnOptions.Width = (int)manager.Skin.Fonts[0].Resource.MeasureString(btnOptions.Text).X + 10;
            btnOptions.ClientMargins = new Margins(0, btnOptions.ClientMargins.Top, 0, btnOptions.ClientMargins.Bottom);
            btnOptions.Anchor = Anchors.Bottom;

            btnOptions.Click += (s, e) =>
            {
                new OptionsWindow(this);
            };

            btnFullScreen = new Button(manager);
            btnFullScreen.Init();
            master.Add(btnFullScreen);
            btnFullScreen.Left = btnOptions.Left + btnOptions.Width;
            btnFullScreen.Top = tbcViews.Top + tbcViews.Height;
            btnFullScreen.Text = "FullScreen";
            btnFullScreen.Width = (int)manager.Skin.Fonts[0].Resource.MeasureString(btnFullScreen.Text).X + 10;
            btnFullScreen.ClientMargins = new Margins(0, btnFullScreen.ClientMargins.Top, 0, btnFullScreen.ClientMargins.Bottom);
            btnFullScreen.Anchor = Anchors.Bottom;

            btnFullScreen.Click += (s, e) =>
            {
                if (btnFullScreen.Text == "FullScreen")
                {
                    btnFullScreen.Text = "Windowed";
                    game.setResolution(game.prefFullScreenResolution ?? resolutions.AutoFullScreen, true);
                }
                else
                {
                    game.setResolution(game.prefFullScreenResolution ?? resolutions.WSXGA_1680x1050, false);
                    btnFullScreen.Text = "FullScreen";
                }
            };

            btnPause = new Button(manager);
            btnPause.Init();
            master.Add(btnPause);
            btnPause.Left = btnFullScreen.Left + btnFullScreen.Width;
            btnPause.Top = tbcViews.Top + tbcViews.Height;
            btnPause.Text = "Pause";
            btnPause.Width = (int)manager.Skin.Fonts[0].Resource.MeasureString("Resume").X + 10;
            btnPause.ClientMargins = new Margins(0, btnPause.ClientMargins.Top, 0, btnPause.ClientMargins.Bottom);
            btnPause.Anchor = Anchors.Bottom;

            btnPause.Click += (s, e) =>
            {
                ui.IsPaused = !ui.IsPaused;
                btnPause.Text = ui.IsPaused ? "Resume" : "Pause";
            };

            btnLoadLevel = new Button(manager);
            btnLoadLevel.Init();
            master.Add(btnLoadLevel);
            btnLoadLevel.Left = btnGameMode.Left;
            btnLoadLevel.Top = btnGameMode.Top + btnGameMode.Height;
            btnLoadLevel.Text = "Load Level";
            btnLoadLevel.Width = (int)manager.Skin.Fonts[0].Resource.MeasureString(btnLoadLevel.Text).X + 10;
            btnLoadLevel.ClientMargins = new Margins(0, btnLoadLevel.ClientMargins.Top, 0, btnLoadLevel.ClientMargins.Bottom);
            btnLoadLevel.Anchor = Anchors.Bottom;

            btnLoadLevel.Click += btnLoadLevel_Click;

            btnSaveLevel = new Button(manager);
            btnSaveLevel.Init();
            master.Add(btnSaveLevel);
            btnSaveLevel.Left = btnLoadLevel.Left + btnLoadLevel.Width;
            btnSaveLevel.Top = btnGameMode.Top + btnGameMode.Height;
            btnSaveLevel.Text = "Save Level";
            btnSaveLevel.Width = (int)manager.Skin.Fonts[0].Resource.MeasureString(btnSaveLevel.Text).X + 10;
            btnSaveLevel.ClientMargins = new Margins(0, btnSaveLevel.ClientMargins.Top, 0, btnSaveLevel.ClientMargins.Bottom);
            btnSaveLevel.Anchor = Anchors.Bottom;

            btnSaveLevel.Click += btnSaveLevel_Click;


        }

        void btnSaveLevel_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Group g = room.groups.walls;

            if (g.fullSet.Count == 0)
                PopUp.Toast("Unable to save: there are no walls.");
            else
                PopUp.Text("Level Name:", "Name Level", delegate(bool c, object input)
                {
                    if (c) LevelSave.SaveLevel(g, room.worldWidth, room.worldHeight, (string)input);//Assets.saveNode(inspectorArea.editNode, (string)input);
                    return true;
                });


            //string filename = Assets.levelsFilepath + "/" + "name" + ".xml";//"Presets//Rooms//room1.bin";
            //Orbit.game.serializer = new Polenter.Serialization.SharpSerializer(true);

            //Orbit.game.serializer.Serialize(sidebar.room, filename);
        }

        void btnLoadLevel_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            LoadLevelWindow loadLevelWindow = new LoadLevelWindow(this);
        }

        public GamemodeWindow gamemodeWindow;
        public PlayerView playerView;
        public void InitializePlayersPage()
        {
            tbcViews.AddPage();
            TabPage playersTab = tbcViews.TabPages[1];
            playersTab.Text = "Players";
            tbcViews.SelectedIndex = 1;
            activeTabControl = tbcViews;

            TitlePanel titlePanelPlayers = new TitlePanel(this, playersTab, "Players", false);

            playerView = new PlayerView(this, playersTab, LeftPadding, titlePanelPlayers.Height);



        }
        GroupsView itemsView;
        private Button btnFullScreen, btnPause, btnLoadLevel, btnSaveLevel;
        public void InitializeItemsPage()
        {
            tbcViews.AddPage();
            TabPage itemsTab = tbcViews.TabPages[2];
            itemsTab.Text = "Items";
            tbcViews.SelectedIndex = 2;
            activeTabControl = tbcViews;

            TitlePanel titlePanelItems = new TitlePanel(this, itemsTab, "Items", false);

            itemsView = new GroupsView(this, itemsTab, 0, titlePanelItems.Height, room.groups.items);
            
            itemsView.UpdateGroups();

            tbcViews.SelectedIndex = 0;
        }
        NormalView processesView;
        public void InitializeProcessesPage()
        {
            tbcViews.AddPage();
            TabPage processTab = tbcViews.TabPages[3];
            processTab.Text = "Processes";
            tbcViews.SelectedIndex = 3;
            activeTabControl = tbcViews;

            TitlePanel titlePanelProcesses = new TitlePanel(this, processTab, "Processes", false);

            processesView = new NormalView(this, processTab, 0, titlePanelProcesses.Height, Height: 150);

            processInsView = new InspectorView(this, processTab, 0, processesView.Top + processesView.Height + 20, Height: 150);

            processesView.OnSelectionChanged += processesView_OnSelectionChanged;
            tbcViews.SelectedIndex = 0;
        }
        InspectorView processInsView;
        void processesView_OnSelectionChanged(ViewItem viewItem)
        {
            if (viewItem.obj is Process)
            {
                Process p = (Process)viewItem.obj;
                processInsView.SetRootObject(p);
            }
        }

        public void UpdateProcessView()
        {
            processesView.ClearView();
            processInsView.ClearView();
            foreach (Process p in room.processManager.activeProcesses)
            {
                processesView.AddObject(p);
            }
        }
        /*
        public void InitializeBulletsPage()
        {
            tbcViews.AddPage();
            TabPage bulletsTab = tbcViews.TabPages[3];
            bulletsTab.Text = "Bullets";
            tbcViews.SelectedIndex = 3;
            activeTabControl = tbcViews;

            TitlePanel titlePanelBullets = new TitlePanel(this, bulletsTab, "Bullets", false);

            //itemsView = new GroupsView(this, testingTab, 0, 0, room.groups.itemGroup);
            //itemsView.lblGroupLabel.Text = "Testing";
            //itemsView.UpdateGroups();


            tbcViews.SelectedIndex = 0;
        }
        */
    }
}
