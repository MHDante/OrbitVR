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
    public class AddComponentWindow
    {
        public bool addToGroup;
        public AddComponentView addCompView;
        public Manager manager;
        public Sidebar sidebar;
        public NeoSidebar neoSidebar;
        public int HeightCounter = 5;
        public int LeftPadding = 5;
        public Label lblComp, lblProperties;
        public Button btnAdd;//, btnCancel;
        public Node node;
        public Link link;
        public DetailedView view;
        public Control under;

        public AddComponentWindow(Sidebar sidebar, Control under, Node n, DetailedView view, bool addToGroup = true)
            : this(sidebar, under, view, addToGroup)
        {
            this.node = n;
            addCompView = new AddComponentView(sidebar, neoSidebar, LeftPadding, 80, sidebar.tbcViews.TabPages[0].Height - 260);
            addCompView.InitNode(n);
        }
        public AddComponentWindow(Sidebar sidebar, Control under, Link link, DetailedView view, bool addToGroup = true)
            : this(sidebar, under, view, addToGroup)
        {
            this.link = link;
            addCompView = new AddComponentView(sidebar, neoSidebar, LeftPadding, 80, sidebar.tbcViews.TabPages[0].Height - 260);
            addCompView.InitLink(link);
        }
        private AddComponentWindow(Sidebar sidebar, Control under, DetailedView view, bool addToGroup = true)
        {
            this.under = under;
            under.Visible = false;
            sidebar.master.Visible = false;
            this.addToGroup = addToGroup;

            Control par = sidebar.tbcViews.TabPages[0];
            UserInterface.GameInputDisabled = true;
            this.view = view;
            this.manager = sidebar.manager;
            this.sidebar = sidebar;
            neoSidebar = new NeoSidebar(manager);
            neoSidebar.Init();
            neoSidebar.Left = sidebar.master.Left;
            neoSidebar.Width = par.Width;
            neoSidebar.Top = 5;
            neoSidebar.Height = par.Height + 15;
            neoSidebar.BevelBorder = BevelBorder.All;
            neoSidebar.BevelColor = Color.Black;
            neoSidebar.Left = LeftPadding;
            neoSidebar.Text = "Add Component";
            neoSidebar.BackColor = new Color(30, 60, 30);
            manager.Add(neoSidebar);

            TitlePanel titlePanelAddComponent = new TitlePanel(sidebar, neoSidebar, "Add Component", true);
            titlePanelAddComponent.btnBack.Click += Close;
            HeightCounter += titlePanelAddComponent.Height;
            NewLabel("Add", 15, false);
            NewLabel("Name", 50, false);
            int left = 145;
            NewLabel("AO", left, false);
            NewLabel("AS", left + 20, false);
            NewLabel("DR", left + 40, false);

            btnAdd = new Button(manager);
            btnAdd.Init();
            btnAdd.Parent = neoSidebar;
            btnAdd.Width = 150;
            btnAdd.Top = neoSidebar.Height - btnAdd.Height * 2;
            btnAdd.Left = neoSidebar.Width / 2 - btnAdd.Width / 2;
            btnAdd.Text = "Add Components";
            btnAdd.Click += AddComponents;
        }
        public void Close(object sender, EventArgs e)
        {
            UserInterface.GameInputDisabled = false;
            manager.Remove(neoSidebar);
            under.Visible = true;
            sidebar.master.Visible = true;
        }
        public void AddComponents(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (node != null)
            {
                AddToNode();
            }
            else if (link != null)
            {
                AddToLink();
            }
            if (view != null)
            {
                view.RefreshRoot();
            }
            Close(null, null);
        }
        public void AddToNode()
        {
            foreach (DetailedItem di in addCompView.viewItems)
            {
                if (!(di.obj is Type)) continue;
                if (!(di.itemControls["checkbox"] as CheckBox).Checked) continue;
                Type t = (Type)di.obj;
                node.addComponent(t, true);
                if (node.group != null && addToGroup) //todo: more checks about whether to add to everyone in group
                {
                    foreach (Node n in node.group.fullSet)
                    {
                        n.addComponent(t, true);
                    }
                }
            }
        }
        public void AddToLink()
        {
            foreach (DetailedItem di in addCompView.viewItems)
            {
                if (!(di.obj is Type)) continue;
                if (!(di.itemControls["checkbox"] as CheckBox).Checked) continue;
                Type t = (Type)di.obj;
                //node.addComponent(c, true);
                Component c = Node.MakeComponent(t, true, null);
                link.AddLinkComponent((ILinkable)c);
            }
        }
        
        public void NewLabel(string s, int left, bool line)
        {
            Label lbl = new Label(manager);
            lbl.Init();
            lbl.Parent = neoSidebar;
            lbl.Top = 60;
            lbl.Text = s;
            lbl.Left = left;
            lbl.Width = 100;
            if (line)
            {
                lbl.Top -= lbl.Height;
                lbl.Height += lbl.Height;
            }
        }

    }
}
