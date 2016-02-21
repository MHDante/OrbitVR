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
    public class DetailedView : ListView<DetailedItem>
    {
        public Action<DetailedItem, object> ItemCreator;
        public Action<Control, DetailedItem, EventArgs> OnItemEvent;
        public bool ColorChangeOnSelect = true;
        public Action<UserLevel> OnUserLevelChanged;

        public DetailedView(Sidebar sidebar, Control parent, int Left, int Top, bool Init = true, int? Height = null)
            : base(sidebar, parent, Left, Top, Init, Height)
        {
            viewItems = new List<DetailedItem>();
            sidebar.ui.detailedViews.Add(this);
        }
        public void Setup(Action<DetailedItem, object> ItemCreator, Action<Control, DetailedItem, EventArgs> OnItemEvent)
        {
            this.ItemCreator += ItemCreator;
            this.OnItemEvent += OnItemEvent;
        }

        public void InvokeOnItemEvent(Control control, DetailedItem item, EventArgs eventArgs)
        {
            if (OnItemEvent != null)
            {
                OnItemEvent(control, item, eventArgs);
            }
        }
        
        public virtual void ClearView()
        {
            selectedItem = null;
            foreach(DetailedItem i in viewItems.ToList())
            {
                backPanel.Remove(i.panel);
                viewItems.Remove(i);
            }
        }
        public virtual void RefreshLight(bool notFocused)
        {
            if (viewItems != null)
            {
                foreach (DetailedItem item in viewItems)
                {
                    if (item.obj == null) continue;
                    if (item.obj is InspectorInfo)
                    {
                        object o = (item.obj as InspectorInfo).obj;
                        if (o != null && o is Component || o is Node || o is Body)
                        {
                            continue;
                        }
                    }
                    item.label.Text = item.obj.ToString().LastWord('.');
                }
            }
        }
        public virtual void RefreshRoot()
        {
            throw new SystemException("Why are you calling this, you don't even know what the root is");
        }

        public void AdjustWidth()
        {
            if (viewItems != null)
            {
                int width = backPanel.Width - 4; //#magic number
                if (viewItems.Count >= 10)
                    width -= 18;
                foreach(var item in viewItems)
                {
                    item.panel.Width = width;
                }
            }
        }

        public void SetButtonBool(Button button, bool b)
        {
            if (b)
            {
                button.Text = "On";
                button.TextColor = UserInterface.TomShanePuke;
            }
            else
            {
                button.Text = "Off";
                button.TextColor = Color.Red;
            }
        }
        public bool GetButtonBool(Button button, bool toggle = true)
        {
            if (button.Text.Equals("On"))
            {
                if (toggle) { SetButtonBool(button, false); return false; }
                return true;
            }
            else
            {
                if (toggle) { SetButtonBool(button, true); return true; }
                return false;
            }
        }
        public virtual void CreateItem(DetailedItem item)
        {
            viewItems.Add(item);
            SetupScroll(item);
            item.OnSelect = delegate
            {
                if (this != null)
                {
                    SelectItem(item);
                }
            };
        }
    }

    public class DetailedItem : ViewItem
    {
        public DetailedView detailedView;
        public Dictionary<string, Control> itemControls;
        private string _toolTip = "";
        public string toolTip
        {
            get { return _toolTip; }
            set
            {
                _toolTip = value;
                if (panel != null) panel.ToolTip.Text = value;
                foreach(Control c in itemControls.Values)
                {
                    if (!c.Text.Equals("-")) c.ToolTip.Text = value;
                }
            }
        }

        public override bool isSelected
        {
            get
            {
                return base.isSelected;
            }
            set
            {
                _isSelected = value;
                detailedView.selectedItem = this;
                if (detailedView != null && detailedView.ColorChangeOnSelect)
                    RefreshColor();
            }
        }

        public DetailedItem(Manager manager, DetailedView detailedView, object obj, Control parent, int Top, int Left)
            : base(manager, obj, parent, Top, Left)
        {
            this.detailedView = detailedView;
            //panel.Width = Width - 4;
            textColor = detailedView.textColor;
            backColor = detailedView.backColor;
            RefreshColor();

            itemControls = new Dictionary<string, Control>();
            if (detailedView.ItemCreator != null)
            {
                detailedView.ItemCreator(this, obj);
            }
        }

        public void AddControl(Control control)
        {
            if (detailedView == null || control == null || control.Name.Equals("")) return;

            if (itemControls.ContainsKey(control.Name))
            {
                RemoveControl(control.Name);
            }
            itemControls[control.Name] = control;
            if (control.Text.Equals("-")) control.ToolTip.Text = "Remove";
            else control.ToolTip.Text = toolTip;
            control.ToolTip.Left = 100;

            detailedView.sidebar.ui.SetScrollableControl(control, detailedView.List_ChangeScrollPosition);

            if (control is ComboBox)
            {
                (control as ComboBox).ItemIndexChanged += (s, e) =>
                {
                    detailedView.InvokeOnItemEvent(control, this, e);
                };
                return;
            }
            control.Click += (s, e) =>
            {
                detailedView.InvokeOnItemEvent(control, this, e);
            };
            control.KeyUp += (s, e) =>
            {
                detailedView.InvokeOnItemEvent(control, this, e);
            };
            if (control is Button)
            {
                control.Click += (s, e) =>
                {
                    control.Focused = false;
                };
            }
            
            //todo: add more handlers as necessary
        }
        public void RemoveControl(string name)
        {

        }
    }
}
