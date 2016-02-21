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
    public class AddComponentView : DetailedView
    {
        public Label lblDescription, lblCompName;
        public AddComponentView(Sidebar sidebar, Control parent, int Left, int Top, int Height)
            : base(sidebar, parent, Left, Top, false)
        {
            this.Height = Height;
            base.Initialize();
            ItemCreator = Creator;
            ColorChangeOnSelect = false;
            Width = parent.Width - 15;
            sidebar.ui.detailedViews.Remove(this);

            lblCompName = new Label(manager);
            lblCompName.Init();
            lblCompName.Parent = parent;
            lblCompName.Width = 400;
            //lblCompName.Height = 250;
            lblCompName.Top = backPanel.Height + Top;
            lblCompName.Left = 10;
            lblCompName.Text = "";
            lblCompName.TextColor = UserInterface.TomShanePuke;

            lblDescription = new Label(manager);
            lblDescription.Init();
            lblDescription.Parent = parent;
            lblDescription.Width = 400;
            lblDescription.Top = lblCompName.Top + lblCompName.Height;//backPanel.Height + Top + 10;
            lblDescription.Left = 10;
            lblDescription.Height = 70;
            lblDescription.Text = "l";
        }
        public void InitNode(Node node)
        {
            int heightcounter = 0;
            List<Type> compTypes = new List<Type>();
            foreach (Type ctype in Component.compTypes)
            {
                Info info = Utils.GetInfoType(ctype);
                if (info == null || (int)sidebar.userLevel < (int)info.userLevel) continue;
                if (node.HasComp(ctype)) continue;
                if ((Utils.GetCompTypes(ctype) & mtypes.exclusiveLinker) == mtypes.exclusiveLinker) continue;
                compTypes.Add(ctype);
            }

            compTypes.Sort((t1, t2) => t1.Name.CompareTo(t2.Name));

            foreach(Type ctype in compTypes)
            {
                DetailedItem ditem = new DetailedItem(manager, this, ctype, backPanel, heightcounter, 0);
                SetupScroll(ditem);
                ditem.label.Text = ditem.label.Text;
                ditem.label.Left += 30;
                CreateItem(ditem);
                heightcounter += ditem.panel.Height;
            }
            backPanel.Refresh();
        }
        public void InitLink(Link link)
        {
            int heightcounter = 0;
            List<Type> compTypes = new List<Type>();
            foreach (Type ctype in Component.compTypes)
            {
                Info info = Utils.GetInfoType(ctype);
                if (info == null || (int)sidebar.userLevel < (int)info.userLevel) continue;
                if (link.components.ContainsKey(ctype)) continue;
                //if ((Utils.GetCompTypes(ctype) & mtypes.exclusiveLinker) != mtypes.exclusiveLinker) continue;
                if (ctype.GetInterface(typeof(ILinkable).Name, true) == null) continue;
                compTypes.Add(ctype);
            }

            compTypes.Sort((t1, t2) => t1.Name.CompareTo(t2.Name));

            foreach (Type ctype in compTypes)
            {
                DetailedItem ditem = new DetailedItem(manager, this, ctype, backPanel, heightcounter, 0);
                SetupScroll(ditem);
                ditem.label.Text = ditem.label.Text;
                ditem.label.Left += 30;
                CreateItem(ditem);
                heightcounter += ditem.panel.Height;
            }
            backPanel.Refresh();
        }
        public void Creator(DetailedItem item, object obj)
        {
            if (item == null || obj == null) return;
            item.panel.DoubleClick += (s, e) =>
            {
                item.panel.SendMessage(Message.Click, new MouseEventArgs());
            };
            item.panel.Click += (s, e) =>
            {
                CheckBox cb = (CheckBox)item.itemControls["checkbox"];
                cb.Checked = !cb.Checked;
            };
            CheckBox checkbox = new CheckBox(manager);
            checkbox.Init();
            checkbox.Parent = item.panel;
            checkbox.Left = 6;
            checkbox.Top = 3;
            checkbox.Checked = false;
            checkbox.Text = "";
            checkbox.Name = "checkbox";
            item.AddControl(checkbox);

            mtypes types = Utils.GetCompTypes((Type)item.obj);
            if (types == mtypes.none) return;
            bool AO = (types & mtypes.affectother) == mtypes.affectother;
            bool AS = (types & mtypes.affectself) == mtypes.affectself;
            bool D = ((types & mtypes.draw) == mtypes.draw);
            bool Q = (types & mtypes.tracer) == mtypes.tracer;
            bool TREE = (Type)item.obj == typeof(Tree);
            //int weight = 0;
            //if (AO) weight += 10;
            //if (AS) weight += 1;
            //if (D) weight += 1;
            //if (Q) weight += 3;
            //if (TREE) weight = 50;
            int leftcounter = 135;
            //NewLabel(weight.ToString(), hc, item, "label1");
            //leftcounter += 100;
            if (AO) NewLabel(UserInterface.Checkmark, leftcounter, item, "label2"); else NewLabel(UserInterface.Cross, leftcounter, item, "label2");
            leftcounter += 20;
            if (AS) NewLabel(UserInterface.Checkmark, leftcounter, item, "label3"); else NewLabel(UserInterface.Cross, leftcounter, item, "label3");
            leftcounter += 20;
            if (D)  NewLabel(UserInterface.Checkmark, leftcounter, item, "label4"); else NewLabel(UserInterface.Cross, leftcounter, item, "label4");

            Info info = Utils.GetInfoType((Type)item.obj);
            if (info == null) Console.WriteLine("Info was null on component type " + item.obj);
            else
            {
                string summary = info.summary.wordWrap(32);
                item.panel.MouseOver += (s, e) =>
                {
                    lblCompName.Text = item.obj.ToString().LastWord('.');
                    lblDescription.Text = summary;
                };
            }
        }

        public void NewLabel(string s, int left, DetailedItem item, string name)
        {
            Label label = new Label(manager);
            label.Init();
            label.Parent = item.panel;
            label.Top = 1;
            label.Left = left;
            label.TextColor = Color.Black;
            label.Width = 30;
            label.Text = s;
            label.Name = name;
            if (s.Equals(UserInterface.Checkmark)) label.TextColor = UserInterface.TomShanePuke;
            else if (s.Equals(UserInterface.Cross)) label.TextColor = Color.Red;
            item.AddControl(label);
        }
    }
}
