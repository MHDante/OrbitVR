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
    public class ListView<T> where T : ViewItem
    {
        public OrbIt game;
        public Room room { get { return sidebar.room; } }
        public UserInterface ui;
        public Sidebar sidebar;
        public InspectorInfo ActiveInspectorParent;
        public Node rootNode;

        public int Left;
        public int Top;
        private int _Width;
        public virtual int Width { get { return _Width; } set { _Width = value; if (backPanel != null) backPanel.Width = value; } }
        private int _Height = 150;
        public virtual int Height { get { return _Height; } set { _Height = value; if (backPanel != null) backPanel.Height = value; } }
        //public int Height { get { return (propertyEditPanel.grouppanel.Top + propertyEditPanel.grouppanel.Height); } }
        public int HeightCounter;
        public int LeftPadding = 5;
        public int VertPadding = 7;
        public int ScrollPosition = 0;

        public Manager manager;
        public Control parent;

        public Panel backPanel;
        public List<T> viewItems { get; protected set; }
        public T selectedItem { get; set; }

        public Color backColor, textColor;

        protected bool isVisible = false;
        public ListView(Sidebar sidebar, Control parent, int Left, int Top, bool Init = true, int? Height = null)
        {
            this.game = sidebar.game;
            this.ui = sidebar.ui;
            this.sidebar = sidebar;
            this.manager = sidebar.manager;
            this.parent = parent;

            this.Left = Left;
            this.Top = Top;
            this.Height = Height ?? OrbIt.ScreenHeight / 3;

            HeightCounter = Top;
            Width = parent.Width - LeftPadding * 2;
            backColor = UserInterface.TomDark;
            textColor = Color.Black;

            if (Init)
            {
                Initialize();
            }
        }
        
        public void Initialize()
        {
            #region /// Components List (back panel) ///
            backPanel = new Panel(manager);
            
            backPanel.Init();
            backPanel.Left = Left + LeftPadding;
            backPanel.Top = HeightCounter;
            backPanel.Width = Width;
            //backPanel.Parent = parent;
            backPanel.Height = Height;
            backPanel.Text = "";
            backPanel.AutoScroll = true;
            backPanel.Color = backColor;//new Color(0, 0, 0, 50);
            backPanel.BevelBorder = BevelBorder.All;
            backPanel.BevelStyle = BevelStyle.Etched;
            backPanel.ClientArea.Height = 200;
            parent.Add(backPanel);
            HeightCounter += backPanel.Height + VertPadding;
            backPanel.ShowVertScrollBar = true;
            #endregion
        }

        public virtual void SelectItem(T item)
        {
            if (selectedItem != null)
            {
                selectedItem.isSelected = false;
            }
            selectedItem = item;
            item.isSelected = true;
        }

        public void SetupScroll(T item)
        {
            sidebar.ui.SetScrollableControl(item.panel, List_ChangeScrollPosition);
        }

        public void List_ChangeScrollPosition(int change)
        {
            backPanel.ScrollTo(backPanel.ScrollBarValue.Horizontal, ScrollPosition + change * 8);
            ScrollPosition = backPanel.ScrollBarValue.Vertical;
        }

        public void SetScrollPosition(int value)
        {
            //int previous = compsBackPanel.ScrollBarValue.Vertical;
            backPanel.ScrollTo(backPanel.ScrollBarValue.Horizontal, value);
            ScrollPosition = backPanel.ScrollBarValue.Vertical;
        }
    }
    public class ViewItem
    {
        protected bool _isSelected = false;
        public virtual bool isSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RefreshColor();
            }
        }

        public void RefreshColor()
        {
            if (isSelected)
            {
                if (panel != null) panel.Color = textColor;
                if (label != null) label.TextColor = backColor;
            }
            else
            {
                if (panel != null) panel.Color = backColor;
                if (label != null) label.TextColor = textColor;
            }
        }

        //public ListView listView;
        public object obj;
        public Label label;
        public Panel panel;

        public int itemHeight = 23;
        public int buttonHeight = 13;
        public int buttonWidth = 15;

        public Color textColor;
        public Color backColor;

        public Action OnSelect;


        public ViewItem(Manager manager, object obj, Control parent, int Top, int Left)
        {
            //this.listView = listView;
            this.obj = obj;
            panel = new Panel(manager);
            panel.Init();
            //panel.Parent = parent;
            parent.Add(panel);
            panel.Top = Top;
            panel.Height = itemHeight;
            panel.Click += textPanel_Click;
            //textPanel.BackColor = Color.Transparent;
            //textPanel.Color = new Color(0,0,0,120);
            panel.BevelColor = new Color(0, 0, 0);
            panel.BevelBorder = BevelBorder.All;
            panel.BevelStyle = BevelStyle.Raised;
            panel.Width = parent.Width - 25;

            //listView.sidebar.ui.SetScrollableControl(textPanel, listView.ComponentsList_ChangeScrollPosition);

            label = new Label(manager);
            label.Init();
            label.Parent = panel;
            label.Left = Left;
            label.Width = panel.Width - 10;
            label.Top = 1;

            if (obj != null)
            {
                if (obj is Node)
                {
                    label.Text = "Root";
                }
                else
                {
                    label.Text = obj.ToString().LastWord('.');
                }
            }
            else
            {
                label.Text = "";
            }
            //isSelected = false;
            RefreshColor();
        }

        public void textPanel_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButton.Left)
            {
                if (!isSelected) 
                {
                    if (OnSelect != null)
                    {
                        OnSelect();
                    }
                }
            }
        }
    }
}
