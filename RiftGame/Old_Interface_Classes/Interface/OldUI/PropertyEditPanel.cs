using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using TomShane.Neoforce.Controls;
using Component = OrbItProcs.Component;
using Console = System.Console;

namespace OrbItProcs
{
    public class PropertyEditPanel
    {
        //public Sidebar sidebar;
        public InspectorArea insArea;
        public GroupPanel grouppanel;
        public Dictionary<string, Control> panelControls;
        private InspectorInfo activeInspectorItem;
        public Type editType;

        //private int HeightCounter = 0;
        private int LeftPadding = 5;
        //private bool triggerResizeSlider = true;

        public PropertyEditPanel(InspectorArea insArea, GroupPanel grouppanel)
        {
            //this.sidebar = sidebar;
            this.insArea = insArea;
            this.grouppanel = grouppanel;
            panelControls = new Dictionary<string, Control>();

        }

        public void DisableControls(bool erase = true)
        {
            List<string> list = panelControls.Keys.ToList(); // for some reason this isn't updated if you click quickly
            foreach (string key in list)
            {
                grouppanel.Remove(panelControls[key]);
                panelControls.Remove(key);
            }
            grouppanel.Text = "Property";

            if (erase) grouppanel.Visible = false;
        }

        public void UpdatePanel(InspectorInfo inspectorItem)
        {
            //if (activeInspectorItem == inspectorItem) return;
            if (inspectorItem.membertype == member_type.previouslevel) return;

            if (panelControls.Keys.Count > 0) DisableControls();

            grouppanel.Visible = true;
            grouppanel.Refresh();

            activeInspectorItem = inspectorItem;

            //grouppanel.Text = activeInspectorItem.ToString(); //.Name();
            grouppanel.Text = activeInspectorItem.Name();

            if (!activeInspectorItem.HasPanelElements()) return;

            editType = activeInspectorItem.obj.GetType();
            object value = activeInspectorItem.GetValue();
            if (value == null) return;

            if (editType == typeof(int) || editType == typeof(Single) || editType == typeof(string) || editType == typeof(byte))
            {
                //System.Console.WriteLine("It's an int or float.");
                TextBox txtbox = new TextBox(grouppanel.Manager);
                txtbox.Init();
                txtbox.Parent = grouppanel;
                txtbox.Left = LeftPadding;
                txtbox.Top = 10;
                txtbox.Width = 80;
                txtbox.Height = txtbox.Height + 3;
                txtbox.KeyUp += delegate (object sender, KeyEventArgs e) {
                    if (!txtbox.Text.Equals("") && e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                    {
                        btnModify_Click(sender, e);
                    }
                };

                //txtbox.BackColor = Color.Green;

                //txtbox.DrawBorders = true;
                //txtbox.Text = activeInspectorItem.obj.ToString();
                
                txtbox.Text = value.ToString();

                Button btnModify = new Button(grouppanel.Manager);
                btnModify.Init();
                btnModify.Parent = grouppanel;
                btnModify.Left = LeftPadding * 2 + txtbox.Width;
                btnModify.Top = 10;
                btnModify.Width = 80;
                btnModify.Text = "Modify";
                btnModify.Click += btnModify_Click;


                panelControls.Add("txtbox", txtbox);
                panelControls.Add("btnModify", btnModify);

                if (editType == typeof(int) || editType == typeof(Single) || editType == typeof(byte))
                {
                    Slider trkMain = new Slider(grouppanel.Manager);
                    trkMain.Init();
                    trkMain.Parent = grouppanel;
                    trkMain.Left = LeftPadding;
                    trkMain.Top = 20 + btnModify.Height;
                    trkMain.Width = txtbox.Width + btnModify.Width + LeftPadding;
                    trkMain.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
                    int val = Convert.ToInt32(value);
                    
                    //int range = Math.Max(100, val * 2);
                    trkMain.Range = Math.Max(100, val * 2);

                    trkMain.Value = val;
                    //trkMain.
                    trkMain.ValueChanged += new TomShane.Neoforce.Controls.EventHandler(trkMain_ValueChanged);
                    trkMain.Click += delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
                    {
                        MouseEventArgs me = (MouseEventArgs)e;
                        if (me.Button != MouseButton.Right) return;
                        int relpos = me.Position.X - 810; //MAGIC NUMBER HACK OMG
                        int sliderpos = (int)(((float)trkMain.Value / (float)trkMain.Range) * trkMain.Width);
                        if (relpos < sliderpos) trkMain.Range = trkMain.Range / 2;
                        else trkMain.Range = trkMain.Range * 2;
                    };
                    //trkMain.
                    trkMain.btnSlider.MouseUp += trkMain_MouseUp;
                    //trkMain.btnSlider.MouseUp += new TomShane.Neoforce.Controls.MouseEventHandler(trkMain_MouseUp);
                    panelControls.Add("trkMain", trkMain);
                }

            }
            else if (editType == typeof(bool))
            {
                //System.Console.WriteLine("It's a boolean.");
                CheckBox chkbox = new CheckBox(grouppanel.Manager);
                chkbox.Init();
                chkbox.Parent = grouppanel;
                chkbox.Left = LeftPadding;
                chkbox.Top = 10;
                chkbox.Width = 120;
                chkbox.Checked = (bool)value;
                chkbox.Text = activeInspectorItem.Name() + " (" + value + ")";
                chkbox.CheckedChanged += new TomShane.Neoforce.Controls.EventHandler(chkbox_CheckedChanged);
                panelControls.Add("chkbox", chkbox);

            }
            else if (editType.IsSubclassOf(typeof(Enum)))
            {
                //System.Console.WriteLine("ENUM!");
                ComboBox cb = new ComboBox(grouppanel.Manager);
                cb.Init();
                cb.Parent = grouppanel;
                cb.MaxItems = 20;
                cb.Left = LeftPadding;
                cb.Top = 10;
                cb.Width = 120;
                foreach (string enumname in Enum.GetNames(editType))
                {
                    cb.Items.Add(enumname);
                }
                cb.ItemIndex = (int)value;
                cb.ItemIndexChanged += cb_ItemIndexChanged;
                panelControls.Add("cb", cb);
            }

            grouppanel.Refresh();
        }

        

        void cb_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ComboBox cb = (ComboBox) sender;
            //activeInspectorItem.SetValue(cb.ItemIndex);
            //insArea.SetItemValue(activeInspectorItem, cb.ItemIndex);
        }

        void trkMain_MouseUp(object sender, TomShane.Neoforce.Controls.MouseEventArgs e)
        {
            //TrackBar trkbar = (TrackBar)sender;
            Slider trkbar = (Slider)panelControls["trkMain"];
            //if (trkbar.Value == trkbar.Range) trkbar.Range *= 2;

            //if (activeInspectorItem.obj is int)
            //{
            //    insArea.SetItemValue(activeInspectorItem, trkbar.Value);
            //}
            //else if (activeInspectorItem.obj is Single)
            //{
            //    insArea.SetItemValue(activeInspectorItem, (Single)trkbar.Value);
            //}
            //else if (activeInspectorItem.obj is byte)
            //{
            //    insArea.SetItemValue(activeInspectorItem, (byte)trkbar.Value);
            //}
        }

       void trkMain_ValueChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
       {
           Slider trkbar = (Slider)sender;
           //GroupPanel gp = (GroupPanel)(trkbar.Parent.Parent);
       
           if (activeInspectorItem.obj is int)
           {
               activeInspectorItem.SetValue(trkbar.Value);
               panelControls["txtbox"].Text = "" + trkbar.Value;
           }
           else if (activeInspectorItem.obj is Single)
           {
               activeInspectorItem.SetValue((Single)trkbar.Value);
               panelControls["txtbox"].Text = "" + trkbar.Value;
           }
           else if (activeInspectorItem.obj is byte)
           {
               activeInspectorItem.SetValue((byte)trkbar.Value);
               panelControls["txtbox"].Text = "" + trkbar.Value;
           }
       
           //if (trkbar.Value == trkbar.Range)
           //{
           //    trkbar.Range *= 2;
           //}
           
       }

        void chkbox_CheckedChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;

            /*
            if (item.itemtype == treeitem.component)
            {
                item.node.setCompActive(item.component, checkbox.Checked);
                checkbox.Text = item.component + " (" + item.node.isCompActive(item.component) + ")";
            }
            */

            //activeInspectorItem.SetValue(checkbox.Checked);
            //insArea.SetItemValue(activeInspectorItem, checkbox.Checked);
            checkbox.Text = activeInspectorItem.Name() + " (" + activeInspectorItem.GetValue() + ")";

        }

        void btnModify_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            string str;
            Type t = activeInspectorItem.obj.GetType();

            if (t == typeof(int))
            {
                str = panelControls["txtbox"].Text.Trim();
                int integer;
                if (str.Length < 1) return;
                if (Int32.TryParse(str, out integer))
                {
                    //insArea.SetItemValue(activeInspectorItem, integer);
                    activeInspectorItem.SetValue(integer);
                }
            }
            else if (t == typeof(Single))
            {
                str = panelControls["txtbox"].Text.Trim();
                float f;
                if (str.Length < 1) return;
                if (float.TryParse(str, out f))
                {
                    //insArea.SetItemValue(activeInspectorItem, f);
                    activeInspectorItem.SetValue(f);
                }
            }
            else if (t == typeof(string))
            {
                str = panelControls["txtbox"].Text;
                if (str.Length < 1) return;
                //insArea.SetItemValue(activeInspectorItem, str);
                activeInspectorItem.SetValue(str);
            }

        }


    }
}
