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
using EventArgs = TomShane.Neoforce.Controls.EventArgs;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;
using NeoSidebar = TomShane.Neoforce.Controls.SideBar;

namespace OrbItProcs
{
    public class EditLinkWindow
    {
        public Manager manager;
        public Sidebar sidebar;
        public NeoSidebar neoSidebar;
        public ComponentView componentView { get; set; }
        public int HeightCounter = 5;
        public int LeftPadding = 5;
        TitlePanel titlePanelEditNode;
        public EditLinkWindow(Sidebar sidebar, Link link, string FieldName)
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

            titlePanelEditNode = new TitlePanel(sidebar, neoSidebar, "Edit" + FieldName, true);
            titlePanelEditNode.btnBack.Click += (s, e) =>
            {
                sidebar.groupsView.UpdateGroups();
                manager.Remove(neoSidebar);
            };

            HeightCounter += titlePanelEditNode.Height;

            componentView = new ComponentView(sidebar, neoSidebar, 0, HeightCounter, ViewType.Link);
            componentView.Width = neoSidebar.Width - 20;
            componentView.insView.Height += componentView.insView.Height / 2;

            neoSidebar.Width += 100;
            neoSidebar.Width -= 100;

            componentView.SwitchLink(link);
        }
    }
}
