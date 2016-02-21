using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;


namespace OrbItProcs
{
    public class CollapsePanel
    {
        public event Action ExpandedHeightChanged;

        public GroupPanel panel;
        public Button collapseButton;
        public Control parent;
        public Dictionary<string, Control> panelControls;

        private bool _IsExtended = true;
        public bool IsExtended
        {
            get { return _IsExtended; }
            set { _IsExtended = value; if (value) Extend(); else Collapse(); }
        }

        private int _ExpandedHeight;
        public int ExpandedHeight
        {
            get { return _ExpandedHeight; }
            set 
            {
                _ExpandedHeight = value; 
                if (panel != null) panel.Height = value; 
                if (IsExtended && parent != null) parent.Refresh(); 
                if (ExpandedHeightChanged != null) ExpandedHeightChanged.Invoke(); 
            }
        }

        public int Top
        {
            get { return panel.Top; }
            set { panel.Top = value; collapseButton.Top = value; }
        }
        public int Left
        {
            get { return panel.Left; }
            set { panel.Left = value; collapseButton.Left = value; }
        }
        public int Width
        {
            get { return panel.Width; }
            set { panel.Width = value; }
        }
        public int Height
        {
            get { return panel.Height; }
            set { panel.Height = value; }
        }
        public string Text
        {
            get { return panel.Text; }
            set { panel.Text = "  " + value.Trim(); }
        }

        public CollapsePanel(Manager manager, Control parent, string Name, int expandedHeight = 100, bool extended = true)
        {
            this.panel = new GroupPanel(manager);
            panel.Init();
            panel.Height = expandedHeight;
            panel.Width = parent.Width - 20;
            panel.Resizable = true;
            //panel.Text = "  " + Name.Trim();
            Text = Name;
            this.collapseButton = new Button(manager);
            collapseButton.Init();
            collapseButton.Width = 15;
            collapseButton.Height = 18;
            collapseButton.Text = "^";
            collapseButton.Click += collapseButton_Click;
            this.ExpandedHeight = expandedHeight;

            this.panelControls = new Dictionary<string, Control>();
            this.parent = parent;
            parent.Add(panel);
            parent.Add(collapseButton);
            this.IsExtended = extended;

            parent.Refresh();
        }

        void collapseButton_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Toggle();
        }

        public void Add(string name, Control control)
        {
            panel.Add(control);
            if (panelControls.ContainsValue(control)) return;
            panelControls.Add(name, control);
        }

        public void Collapse()
        {
            _IsExtended = false;
            collapseButton.Text = "v";
            panel.Height = 20;

            parent.Refresh();
        }
        public void Extend()
        {
            _IsExtended = true;
            collapseButton.Text = "^";
            panel.Height = ExpandedHeight;

            parent.Refresh();
        }

        public void Toggle()
        {
            if (collapseButton.Text.Equals("^"))
            {
                Collapse();

            }
            else
            {
                Extend();
            }
            //StackView.Refresh();
        }

    }
}
