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
using System.Collections.ObjectModel;

namespace OrbItProcs
{
    public class PlayerView : InspectorView
    {
        public Label lblPlayers;
        public InspectorView insView;
        public Group playerGroup;
        public Button btnEditAllPlayers;
        public override int Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
                RefreshPlayers();
                if (insView == null) return;
                insView.Width = value;
                insView.AdjustWidth();
            }
        }
        public EditNodeWindow editGroupWindow;

        public PlayerView(Sidebar sidebar, Control parent, int Left, int Top)
            : base(sidebar, parent, Left, Top, false)
        {
            playerGroup = sidebar.room.groups.player;

            btnEditAllPlayers = new Button(manager);
            btnEditAllPlayers.Init();
            btnEditAllPlayers.Parent = parent;
            btnEditAllPlayers.Text = "Edit All Players";
            btnEditAllPlayers.Width = 150;
            btnEditAllPlayers.Left = parent.Width / 2 - btnEditAllPlayers.Width / 2;
            btnEditAllPlayers.Top = Top + VertPadding * 2;
            HeightCounter +=  btnEditAllPlayers.Height * 2;
            btnEditAllPlayers.Click += (s, e) =>
            {
                editGroupWindow = new EditNodeWindow(sidebar, "All Players", room.groups.player.Name, ViewType.Group);
                editGroupWindow.componentView.SwitchGroup(room.groups.player);
                //editGroupWindow.componentView.SwitchNode(n, false);

            };

            lblPlayers = new Label(manager);
            lblPlayers.Init();
            lblPlayers.Parent = parent;
            lblPlayers.Text = "Players";
            lblPlayers.Width = 150;
            lblPlayers.Left = LeftPadding;
            lblPlayers.TextColor = Color.Black;
            lblPlayers.Top = HeightCounter;
            HeightCounter += lblPlayers.Height + VertPadding;
            Width = parent.Width - LeftPadding * 4;

            base.Initialize();

            insView = new InspectorView(sidebar, parent, Left, HeightCounter);
            insView.GroupSync = true;
            insView.Height = 120;
            insView.Width = Width;
            this.ItemCreator += ItemCreatorDelegate;

            InitializePlayers();
        }
        private void ItemCreatorDelegate(DetailedItem item, object obj)
        {
            if (item.obj is InspectorInfo)
            {
                InspectorInfo ii = (InspectorInfo)item.obj;
                if (!(ii.obj is Node)) return;
                Node n = (Node)ii.obj;
                item.label.Text = n.name;
                Button btnEdit = new Button(manager);
                btnEdit.Init();
                btnEdit.Parent = item.panel;
                btnEdit.Width = 30;
                btnEdit.Left = item.panel.Width - btnEdit.Width - 10;
                btnEdit.Top = 2;
                btnEdit.Height = item.buttonHeight;

                btnEdit.Text = "Edit";
                btnEdit.ToolTip.Text = "Edit";
                btnEdit.Name = "Player Edit";
                btnEdit.TextColor = UserInterface.TomShanePuke;

                EventHandler editplayer = (s, e) =>
                {
                    item.isSelected = true;
                    //editGroupWindow = new EditGroupWindow(sidebar);
                    //editGroupWindow.componentView.SwitchGroup(g);
                    editGroupWindow = new EditNodeWindow(sidebar, "Player", n.name, ViewType.Node);
                    //editGroupWindow.componentView.SwitchGroup(g);
                    editGroupWindow.componentView.SwitchNode(n, false);
                };

                btnEdit.Click += editplayer;
                item.panel.DoubleClick += editplayer;

                Button btnEnabled = new Button(manager);
                btnEnabled.Init();
                btnEnabled.Parent = item.panel;
                btnEnabled.Width = 30;
                btnEnabled.Left = btnEdit.Left - btnEnabled.Width - 5;
                btnEnabled.Top = 2;
                btnEnabled.Height = item.buttonHeight;
                //btnEnabled.Draw += btnEnabled_Draw;

                //btnEnabled.Text = "On";
                SetButtonBool(btnEnabled, n.active);
                btnEnabled.ToolTip.Text = "Player Enabled";
                btnEnabled.TextColor = UserInterface.TomShanePuke;
                btnEnabled.Click += (s, e) =>
                {
                    n.active = GetButtonBool(btnEnabled);
                    SetButtonBool(btnEnabled, n.active);
                };
            }
        }

        public void SetVisible(bool visible)
        {
            //if (isVisible == visible) return;
            isVisible = visible;


            if (visible && selectedItem != null)
            {
                SetPlayer(null);
                SetPlayer(selectedItem.obj);
            }
            else
            {
                SetPlayer(null);
            }

            //insArea.InsBox.Visible = visible;
            //insArea.propertyEditPanel.grouppanel.Visible = visible;
            //
            //insArea.InsBox.Refresh();
            //insArea.propertyEditPanel.grouppanel.Refresh();
        }
        public void SetPlayer(object obj)
        {
            insView.backPanel.Visible = true;
            insView.backPanel.Refresh();
            if (obj == null)
            {
                insView.SetRootObject(null);
            }
            else if (obj is Node)
            {
                insView.SetRootObject((obj as Node).meta);
            }
        }

        public override void SelectItem(DetailedItem item)
        {
            InspectorInfo ii;
            if (item.obj is InspectorInfo && (ii = (InspectorInfo)item.obj).obj is Node)
            {
                Node n = (Node)ii.obj;
                InspectorInfo metaitem = new InspectorInfo(null, n.meta, sidebar);
                insView.SetRootInspectorItem(metaitem);
                base.SelectItem(item);
            }
        }

        public void RefreshPlayers()
        {
            ///
        }
        public void InitializePlayers()
        {
            ClearView();

            int heightCount = 0;
            if (viewItems != null)
            {
                foreach (DetailedItem item in viewItems)
                {
                    backPanel.Remove(item.panel);
                }
            }
            
            viewItems = new List<DetailedItem>();
            int itemCount = playerGroup.entities.Count;
            int width = backPanel.Width - 4; //#magic number
            if (itemCount >= 10)
                width -= 18;
            foreach (Node p in playerGroup.entities)
            {
                InspectorInfo cItem = new InspectorInfo(null, p, sidebar);
                DetailedItem di = new DetailedItem(manager, this, cItem, backPanel, heightCount, LeftPadding);
                CreateItem(di);
                di.label.Text = p.name;
                heightCount += (viewItems[0].itemHeight - 2);
            }
            //heightCount += compItems[0].label.Height;
            //compItems.Add(new ComponentItem(manager, this, null, compsBackPanel, heightCount, LeftPadding));
            ScrollPosition = 0;
            backPanel.ScrollTo(backPanel.ScrollBarValue.Horizontal, 0);
            SetVisible(true);
            backPanel.Refresh();
        }
    }
}
