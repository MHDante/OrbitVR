using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;
using NeoSidebar = TomShane.Neoforce.Controls.SideBar;

namespace OrbItProcs
{
    public class EditNodeWindow
    {
        //public Game1 game;
        public Manager manager;
        public Sidebar sidebar;
        public NeoSidebar neoSidebar;
        public ComponentView componentView { get; set; }
        public int HeightCounter = 5;
        public int LeftPadding = 5;
        TextBox txtName;
        Label lblName;
        TitlePanel titlePanelEditNode;
        public EditNodeWindow(Sidebar sidebar, Group group) 
            : this(sidebar, ViewType.Group)
        {
            titlePanelEditNode.lblTitle.Text = "Edit Group";
            lblName.Text = "Group Name:";
            txtName.Text = group.Name;
        }
        public EditNodeWindow(Sidebar sidebar, Node node)
            : this(sidebar, ViewType.Node)
        {
            titlePanelEditNode.lblTitle.Text = "Edit Node";
            lblName.Text = "Node Name:";
            txtName.Text = node.name;
        }
        public EditNodeWindow(Sidebar sidebar, string Type, string Name, ViewType viewType)
            : this(sidebar, viewType)
        {
            titlePanelEditNode.lblTitle.Text = "Edit " + Type;
            lblName.Text = Type + " Name:";
            txtName.Text = Name;
        }
        private EditNodeWindow(Sidebar sidebar, ViewType viewType)
        {
            this.sidebar = sidebar;
            this.manager = sidebar.manager;
            neoSidebar = new NeoSidebar(manager);
            neoSidebar.Init();
            int tomtom = 5;
            neoSidebar.ClientArea.BackColor = UserInterface.TomDark;
            neoSidebar.BackColor = Color.Black;
            neoSidebar.BevelBorder = BevelBorder.All;
            Margins tomtomtomtom = new Margins(tomtom, tomtom, tomtom, tomtom);
            neoSidebar.ClientMargins = tomtomtomtom;

            neoSidebar.Left = sidebar.master.Left;
            neoSidebar.Width = sidebar.Width;
            neoSidebar.Top = 0;
            neoSidebar.Resizable = false;
            neoSidebar.Movable = false;
            neoSidebar.Height = OrbIt.ScreenHeight;
            neoSidebar.Text = "Edit";
            manager.Add(neoSidebar);

            int width = 120;
            int offset = neoSidebar.Width - width - 20;

            titlePanelEditNode = new TitlePanel(sidebar, neoSidebar, "Edit", true);
            titlePanelEditNode.btnBack.Click += (s, e) =>
            {
                sidebar.groupsView.UpdateGroups();
                manager.Remove(neoSidebar);
            };

            HeightCounter += titlePanelEditNode.Height;

            lblName = new Label(manager);
            lblName.Init();
            lblName.Parent = neoSidebar;
            lblName.Left = LeftPadding;
            lblName.Top = HeightCounter;
            lblName.Width = width;

            txtName = new TextBox(manager);
            txtName.Init();
            txtName.Parent = neoSidebar;
            txtName.Top = HeightCounter;
            txtName.Width = width;
            txtName.Left = offset;
            HeightCounter += txtName.Height + LeftPadding;
            txtName.TextColor = Color.Black;
            txtName.Enabled = false;

            componentView = new ComponentView(sidebar, neoSidebar, 0, HeightCounter, viewType);
            componentView.Width = neoSidebar.Width - 20;
            componentView.insView.Height += componentView.insView.Height / 2;

            neoSidebar.Width += 100;
            neoSidebar.Width -= 100;
        }
    }
}
