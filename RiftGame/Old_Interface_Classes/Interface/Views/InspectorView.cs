using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Component = OrbItProcs.Component;
using Console = System.Console;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;
using Color = SharpDX.Color;
using SharpDX;
using SharpOVR;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.DXGI;
using SharpDX.Toolkit.Content;
using System.Collections.ObjectModel;

namespace OrbItProcs
{
    public enum NumBoxMode
    {
        byOne,
        quadratic,
        byTen

    }
    public class InspectorView : DetailedView
    {
        public InspectorInfo rootItem;
        private Group _activeGroup;
        public Group activeGroup
        {
            get { return _activeGroup; }
            set
            {
                _GroupSync = value != null;
                _activeGroup = value;
            }
        }
        private bool _GroupSync = false;
        public bool GroupSync { get { return _GroupSync && activeGroup != null; } set { _GroupSync = value; } }

        public InspectorView(Sidebar sidebar, Control parent, int Left, int Top, bool Init = true, int? Height = null)
            : base(sidebar, parent, Left, Top, Init, Height)
        {
            //backPanel.Height = 120;
            Setup(ItemCreatorDelegate, OnEvent);
        }
        public void SetRootObject(object obj)
        {
            ClearView();
            if (obj == null)
            {
                rootItem = null;
                return;
            }
            if (obj is InspectorInfo)
            {
                SetRootInspectorItem((InspectorInfo)obj);
                return;
            }
            InspectorInfo insItem = new InspectorInfo(null, obj, sidebar);
            SetRootInspectorItem(insItem);
        }
        public void SetRootInspectorItem(InspectorInfo insItem)
        {
            rootItem = insItem;
            ClearView();
            if (insItem == null) return;
            insItem.GenerateChildren();
            foreach (InspectorInfo i in insItem.children)
            {
                CreateNewItem(i);
            }
        }
        public override void RefreshRoot()
        {
            if (rootItem != null)
            {
                SetRootInspectorItem(rootItem);
            }
        }
        public void CreateNewItem(InspectorInfo item)
        {
            int top = 0;
            if (viewItems.Count > 0)
            {
                top = (viewItems[0].itemHeight - 4) * viewItems.Count;
            }
            DetailedItem detailedItem = new DetailedItem(manager, this, item, backPanel, top, LeftPadding);
            if (item.ToolTip.Length > 0) detailedItem.panel.ToolTip.Text = item.ToolTip;
            viewItems.Add(detailedItem);
            SetupScroll(detailedItem);
        }

        public void OnEvent(Control control, DetailedItem item, EventArgs e)
        {
           if (item == null || control == null || item.obj == null) return;
            if (!(item.obj is InspectorInfo)) return;
            InspectorInfo ins = (InspectorInfo)item.obj;
            if (e is KeyEventArgs && control.GetType() == typeof(TextBox))
            {
                KeyEventArgs ke = (KeyEventArgs)e;
                if (ke.Key != Microsoft.Xna.Framework.Input.Keys.Enter) return;
                TextBox textbox = (TextBox)control;
                object san = ins.TrySanitize(textbox.Text);
                if (san != null)
                {
                    ins.SetValue(san);
                    //Console.WriteLine(ins.parentItem.obj.GetType() + " >> " + ins + " >> " + san.GetType() + " >> " + san);
                    if (GroupSync)
                    {
                        ins.ApplyToAllNodes(activeGroup);
                    }
                }
                marginalize(textbox);
            }
            else if (control is ComboBox)
            {
                ins.SetValue(control.Text);
                if (GroupSync)
                {
                    ins.ApplyToAllNodes(activeGroup);
                }
            }
            else if (control is Button)
            {
                if (control.Name.Equals("bool_button_enabled"))
                {
                    ins.SetValue(GetButtonBool((Button)control));
                    if (GroupSync)
                    {
                        ins.ApplyToAllNodes(activeGroup);
                    }
                }
                else if (control.Name.Equals("toggle_button_enabled"))
                {
                    ins.SetValue(GetButtonBool((Button)control));
                    if (GroupSync)
                    {
                        ins.ApplyToAllNodes(activeGroup);
                    }
                }
                else if (control.Name.Equals("component_button_enabled"))
                {
                    Component component = (Component)ins.obj;
                    component.active = GetButtonBool((Button)control);
                    if (this is ComponentView)
                    {
                        ComponentView cv = (ComponentView)this;
                        if (cv.lblCurrentComp.Text.Equals(component.GetType().ToString().LastWord('.')))
                            cv.lblCurrentComp.TextColor = control.TextColor;
                    }
                    //ins.SetValue(checkbox.Checked);
                    if (GroupSync)
                    {
                        foreach (Node n in activeGroup.fullSet)
                        {
                            if (n.HasComp(component.GetType()))
                                n.comps[component.GetType()].active = component.active;
                        }
                    }
                }
                else if (control.Name.Equals("component_button_remove"))
                {
                    if (ins.obj is Component)
                    {
                        Component component = (Component)ins.obj;
                        if (this is ComponentView && (this as ComponentView).viewType == ViewType.Link)
                        {
                            ComponentView cv = (ComponentView)this;
                            cv.rootLink.components.Remove(component.GetType());
                        }
                        else
                        {
                            component.parent.RemoveComponent(component.GetType());
                            if (GroupSync)
                            {
                                foreach (Node n in activeGroup.fullSet)
                                {
                                    n.RemoveComponent(component.GetType());
                                }
                            }
                        }
                    }
                    if (this is ComponentView) (this as ComponentView).RefreshComponents();
                }
                else if (control.Name.Equals("method_button_invoke"))
                {
                    if (ins.methodInfo != null)
                    {
                        ins.methodInfo.Invoke(ins.parentobj, null);
                        if (GroupSync)
                        {
                            ins.ApplyToAllNodes(activeGroup);
                        }
                    }
                }
            }
        }

        private void ItemCreatorDelegate(DetailedItem item, object obj)
        {
            //editGroupWindow = new EditNodeWindow(sidebar, "All Players", room.groups.playerGroup.Name);
            //editGroupWindow.componentView.SwitchGroup(room.groups.playerGroup);
            //editGroupWindow.componentView.SwitchNode(n, false);
            if (obj == null) return;
            if (obj is InspectorInfo)
            {
                InspectorInfo inspectorItem = (InspectorInfo)obj;

                if (inspectorItem.methodInfo != null)
                {
                    Button btnInvoke = new Button(manager);
                    btnInvoke.Init();
                    btnInvoke.Parent = item.panel;
                    btnInvoke.TextColor = Color.Blue;
                    btnInvoke.Width = 50;
                    btnInvoke.Left = item.panel.Width - btnInvoke.Width - btnInvoke.Width - 5;
                    btnInvoke.Height = item.buttonHeight;
                    btnInvoke.ToolTip.Text = "Invoke Method";
                    btnInvoke.Name = "method_button_invoke";
                    btnInvoke.Text = "Do";
                    item.AddControl(btnInvoke);
                }

                if (inspectorItem.obj == null) return;
                object o = inspectorItem.obj;
                bool isToggle = Utils.isToggle(o);
                
                if (o != null)
                {
                    if (o is Node)
                    {
                        //item.label.Text = "Root";
                        Node n = (Node)o;
                        Button btnEdit = new Button(manager);
                        btnEdit.Init();
                        btnEdit.Parent = item.panel;
                        btnEdit.Width = 30;
                        btnEdit.Left = item.panel.Width - btnEdit.Width - 10;
                        btnEdit.Top = 2;
                        btnEdit.Height = item.buttonHeight;

                        EventHandler editnode = (s, e) =>
                        {
                            //item.isSelected = true;
                            EditNodeWindow editNodeWindow = new EditNodeWindow(sidebar, inspectorItem.Name(), n.name, ViewType.Node);
                            editNodeWindow.componentView.SwitchNode(n, false);
                        };

                        btnEdit.Text = "Edit";
                        btnEdit.ToolTip.Text = "Edit";
                        btnEdit.TextColor = UserInterface.TomShanePuke;

                        btnEdit.Click += editnode;
                    }
                    else if (o is Link)
                    {
                        Link link = (Link)o;
                        Button btnEdit = new Button(manager);
                        btnEdit.Init();
                        btnEdit.Parent = item.panel;
                        btnEdit.Width = 30;
                        btnEdit.Left = item.panel.Width - btnEdit.Width - 10;
                        btnEdit.Top = 2;
                        btnEdit.Height = item.buttonHeight;
                        EventHandler editlink = (s, e) =>
                        {
                            EditLinkWindow editLinkWindow = new EditLinkWindow(sidebar, link, inspectorItem.Name());
                        };
                        btnEdit.Text = "Edit";
                        btnEdit.ToolTip.Text = "Edit";
                        btnEdit.TextColor = UserInterface.TomShanePuke;

                        btnEdit.Click += editlink;
                    }
                    else if (o is Body || o is Component)
                    {
                        item.label.Text = o.GetType().ToString().LastWord('.');
                    }
                    if (o is Component)
                    {
                        Component comp = (Component)o;

                        Button btnEnabled = new Button(manager);
                        btnEnabled.Init();
                        btnEnabled.Parent = item.panel;
                        btnEnabled.TextColor = Color.Red;
                        btnEnabled.Width = 25;
                        btnEnabled.Left = item.panel.Width - btnEnabled.Width - 20;
                        btnEnabled.Top = 3;
                        btnEnabled.Height = item.buttonHeight;
                        btnEnabled.ToolTip.Text = "Toggle Active";
                        btnEnabled.Name = "component_button_enabled";
                        SetButtonBool(btnEnabled, comp.active);
                        item.AddControl(btnEnabled);

                        //check for essential
                        if (!comp.isEssential())
                        {
                            Button btnRemove = new Button(manager);
                            btnRemove.Init();
                            btnRemove.Parent = item.panel;
                            btnRemove.TextColor = Color.Red;
                            btnRemove.Left = btnEnabled.Left - 20;
                            btnRemove.Top = 3;
                            btnRemove.Height = item.buttonHeight;
                            btnRemove.Width = item.buttonWidth;
                            btnRemove.Text = "-";
                            btnRemove.ToolTip.Text = "Remove";
                            btnRemove.Name = "component_button_remove";
                            item.AddControl(btnRemove);
                        }
                    }
                    else if (o is int || o is Single || o is byte || isToggle)
                    {
                        int w = 60;
                        TextBox textbox = new TextBox(manager);
                        textbox.ClientMargins = new Margins();
                        textbox.Init();
                        textbox.Parent = item.panel;
                        textbox.TextColor = UserInterface.TomShanePuke;
                        textbox.Left = backPanel.Width - w - 26;
                        textbox.Width = w;
                        textbox.Height = textbox.Height - 4;
                        
                        textbox.Name = "number_textbox";
                        if (isToggle)
                        {
                            textbox.Name = "toggle_textbox";

                            Button btnEnabled = new Button(manager);
                            btnEnabled.Init();
                            btnEnabled.Parent = item.panel;
                            btnEnabled.TextColor = Color.Red;
                            btnEnabled.Width = 25;
                            btnEnabled.Left = textbox.Left - btnEnabled.Width;
                            //btnEnabled.Top = 3;
                            btnEnabled.Height = item.buttonHeight;
                            btnEnabled.ToolTip.Text = "Toggle Active";
                            btnEnabled.Name = "toggle_button_enabled";
                            SetButtonBool(btnEnabled, (o as dynamic).enabled);
                            item.AddControl(btnEnabled);

                            //item.AddControl(checkbox);

                            //textbox.Text = (o as dynamic).valueString(); // too slow. 
                            textbox.Text = "...";
                        }
                        else textbox.Text = o.ToString();
                        textbox.ClientArea.Top += 2;
                        textbox.ClientArea.Left += 4;
                        textbox.FocusLost += delegate
                        {
                            textbox.SendMessage(Message.KeyUp, new KeyEventArgs(Microsoft.Xna.Framework.Input.Keys.Enter));
                        };
                        textbox.ClientArea.Move += delegate 
                        { 
                            marginalize(textbox); 
                        };
                        item.AddControl(textbox);
                        Type primitiveType = !isToggle ? o.GetType() : ((dynamic)o).value.GetType();

                        Button up = new Button(manager);
                        up.SetSize(textbox.ClientArea.Height, textbox.ClientArea.Height / 2);
                        up.Anchor = Anchors.Right;
                        up.Init();
                        up.Left = textbox.ClientArea.Width - up.Width;
                        sidebar.ui.SetScrollableControl(up, List_ChangeScrollPosition);

                        up.ToolTip.Text = "Increment : RightClick = byOne, MiddleClick = byTen, RightClick = Double";
                        up.MouseDown += (s, e) =>
                        {
                            switch (e.Button)
                            {
                                case MouseButton.Left:
                                    textbox.Text = textbox.Text.increment(primitiveType, NumBoxMode.byOne);
                                    break;
                                case MouseButton.Right:
                                    textbox.Text = textbox.Text.increment(primitiveType, NumBoxMode.quadratic);
                                    break;
                                case MouseButton.Middle:
                                    textbox.Text = textbox.Text.increment(primitiveType, NumBoxMode.byTen);
                                    break;
                            }
                            textbox.SendMessage(Message.KeyUp, new KeyEventArgs(Microsoft.Xna.Framework.Input.Keys.Enter));
                            marginalize(textbox);
                        };
                        textbox.Add(up);

                        Button down = new Button(manager);
                        down.SetSize(textbox.ClientArea.Height, textbox.ClientArea.Height / 2);
                        down.Anchor = Anchors.Right;
                        down.Top = up.Height;
                        down.Init();
                        down.ToolTip.Text = "Decrement : RightClick = byOne, MiddleClick = byTen, RightClick = half";
                        down.MouseDown += (s, e) =>
                        {
                            switch (e.Button)
                            {
                                case MouseButton.Left:
                                    textbox.Text = textbox.Text.decrement(primitiveType, NumBoxMode.byOne);
                                    break;
                                case MouseButton.Right:
                                    textbox.Text = textbox.Text.decrement(primitiveType, NumBoxMode.quadratic);
                                    break;
                                case MouseButton.Middle:
                                    textbox.Text = textbox.Text.decrement(primitiveType, NumBoxMode.byTen);
                                    break;
                            }
                            textbox.SendMessage(Message.KeyUp, new KeyEventArgs(Microsoft.Xna.Framework.Input.Keys.Enter));
                            marginalize(textbox);
                        };
                        down.Left = textbox.ClientArea.Width - down.Width;
                        sidebar.ui.SetScrollableControl(down, List_ChangeScrollPosition);
                        textbox.Add(down);

                        //todo: make tiny + and - buttons
                    }
                    else if (o is string)
                    {
                        int w = 60;
                        TextBox textbox = new TextBox(manager);
                        textbox.ClientMargins = new Margins();
                        textbox.Init();
                        textbox.Parent = item.panel;
                        textbox.TextColor = UserInterface.TomShanePuke;
                        textbox.Left = backPanel.Width - w - 26;
                        textbox.Width = w;
                        textbox.Height = textbox.Height - 4;
                        textbox.Text = o.ToString();
                        textbox.Name = "string_textbox";
                        item.AddControl(textbox);

                        textbox.ClientArea.Top += 2;
                        textbox.ClientArea.Left += 2;
                        textbox.KeyPress += delegate
                        {
                            if (!textbox.Text.Equals(""))
                            {
                                textbox.ClientArea.Top += 2;
                                textbox.ClientArea.Left += 2;
                            }
                        };

                    }
                    else if (o is bool)
                    {
                        Button btnEnabled = new Button(manager);
                        btnEnabled.Init();
                        btnEnabled.Parent = item.panel;
                        btnEnabled.TextColor = Color.Red;
                        btnEnabled.Width = 25;
                        btnEnabled.Left = item.panel.Width - btnEnabled.Width - 20;
                        //btnEnabled.Top = 3;
                        btnEnabled.Height = item.buttonHeight;
                        btnEnabled.ToolTip.Text = "Toggle Enabled";
                        btnEnabled.Name = "bool_button_enabled";
                        SetButtonBool(btnEnabled, (bool)o);
                        item.AddControl(btnEnabled);
                    }
                    else if (o.GetType().IsEnum)
                    {
                        int w = 95;
                        ComboBox combobox = new ComboBox(manager);
                        combobox.ClientMargins = new Margins();
                        combobox.Init();
                        combobox.TextColor = UserInterface.TomShanePuke;
                        combobox.Parent = item.panel;
                        combobox.Left = backPanel.Width - w - 26;
                        combobox.Height = combobox.Height - 4;
                        combobox.Width = w;
                        combobox.MaxItems = 15;
                        int i = 0;
                        foreach(string s in Enum.GetNames(o.GetType()))
                        {
                            combobox.Items.Add(s);
                            if (s.Equals(o.ToString())) combobox.ItemIndex = i;
                            i++;
                        }
                        combobox.Name = "enum_combobox";
                        item.AddControl(combobox);

                        combobox.ClientArea.Top += 2;
                        combobox.ClientArea.Left += 2;
                        combobox.ItemIndexChanged += delegate
                        {
                            if (!combobox.Text.Equals(""))
                            {
                                combobox.ClientArea.Top += 2;
                                combobox.ClientArea.Left += 2;
                            }
                        };
                    }
                    
                }
            }
        }

        public string NumberToString(object o)
        {
            if (o is int)
            {
                return o.ToString();
            }
            else if (o is Single)
            {
                Single single = (Single)o;
                if (single < 0.099)
                {
                    return single.ToString();
                }
                else
                {
                    return string.Format("{0:#.##}", single);
                }
            }
            
            return o.ToString();
        }

        public override void RefreshLight(bool notFocused)
        {
            if (viewItems != null)
            {
                foreach (DetailedItem item in viewItems)
                {
                    if (item.obj == null) continue;
                    if (item.obj is InspectorInfo)
                    {
                        InspectorInfo insItem = (InspectorInfo)item.obj;
                        if (insItem.obj != null && (insItem.obj is Component || insItem.obj is Node || insItem.obj is Body))
                        {
                            continue;
                        }
                        item.label.Text = insItem.ToString().LastWord('.');
                        if (item.itemControls == null) continue;
                        foreach(string name in item.itemControls.Keys)
                        {
                            Control control = item.itemControls[name];
                            if (notFocused && control.Focused) continue;
                            //todo:implement refresh controls
                            if (control is ComboBox)
                            {
                                continue;
                            }
                            else if (control is TextBox)
                            {
                                object val = insItem.GetValue();
                                if (Utils.isToggle(val))
                                {
                                    dynamic tog = val;
                                    control.Text = NumberToString(tog.value);
                                }
                                else
                                {
                                    control.Text = NumberToString(val);
                                }
                            }
                            else if (control is Button)
                            {
                                if (control.Name.Contains("enabled"))
                                {
                                    object bb = insItem.GetValue();
                                    if (Utils.isToggle(bb))
                                    {
                                        dynamic tog = bb;
                                        bool bbb = tog.enabled;
                                        SetButtonBool((Button)control, bbb);
                                    }
                                    else if (bb is bool)
                                    {
                                        SetButtonBool((Button)control, (bool)bb);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void marginalize(TextBox t)
        {
            t.ClientArea.Top = 2;
            t.ClientArea.Left = 4;
        }
    }
}
