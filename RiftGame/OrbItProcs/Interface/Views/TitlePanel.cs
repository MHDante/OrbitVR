using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;

namespace OrbItProcs
{
    public class TitlePanel
    {
        public Sidebar sidebar;
        public Panel topPanel;
        public Label lblTitle;
        public Button btnBack;
        public string Title { get { return lblTitle.Text; } set { lblTitle.Text = value; } }
        private int Padding;
        public int Height { get { return topPanel.Height + Padding; } }
        public TitlePanel(Sidebar sidebar, Control parent, string Title, bool BackButton, int Top = 0, int Padding = 5)
        {

            this.Padding = Padding;
            topPanel = new Panel(sidebar.manager);
            topPanel.Init();
            topPanel.Parent = parent;
            topPanel.Left = Padding;
            topPanel.Top = Top + Padding;
            topPanel.Width = parent.Width - Padding * 3;
            if (parent is SideBar) topPanel.Width -= Padding * 1;
            int col = 30;
            topPanel.Color = new Color(col, col, col);
            topPanel.BevelBorder = BevelBorder.All;
            topPanel.BevelStyle = BevelStyle.Flat;
            topPanel.BevelColor = Color.Black;


            lblTitle = new Label(sidebar.manager);
            lblTitle.Init();
            lblTitle.Parent = topPanel;
            lblTitle.Top = Padding * 2;
            lblTitle.Width = 120;
            lblTitle.Left = parent.Width / 2 - lblTitle.Width / 4;
            //lblTitle.Text = Title;
            this.Title = Title;
            topPanel.Height = 24 + Padding * 3;

            if (!BackButton) return;
            btnBack = new Button(sidebar.manager);
            btnBack.Init();
            btnBack.Parent = topPanel;
            btnBack.Top = Padding;
            btnBack.Text = "Back";
            btnBack.Width = 40;
            btnBack.Left = Padding;
            //topPanel.Height = btnBack.Height + Padding * 3;
        }

    }
}
