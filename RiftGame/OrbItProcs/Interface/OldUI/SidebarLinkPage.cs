
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;

using Component = OrbItProcs.Component;

namespace OrbItProcs
{
    public partial class Sidebar
    {
        public StackView stackview;
        public Panel backPanel;
        public int HeightCounter3;
        public int VertPadding3;

        public ObservableHashSet<Link> PaletteLinks = new ObservableHashSet<Link>();
        public ObservableHashSet<Link> AllActiveLinks = new ObservableHashSet<Link>();


        //panels
        CollapsePanel SourceTarget;
        CollapsePanel LinkPalette;
        //CollapsePanel c3, c4, c5, c6, c7, c8;
        //SourceTarget
        Label lblSource, lblTarget, lblGroupS, lblGroupT, lblNodeS, lblNodeT;
        public ComboBox cbGroupS, cbGroupT, cbNodeS, cbNodeT;
        RadioButton rdGroupS, rdGroupT, rdNodeS, rdNodeT;//, rdSelectionS, rdSelectionT;
        CheckBox chSelectionS, chSelectionT;

        //LinkPalette
        public ComboBox cbLinkList;
        Button btnCreateLink, btnOpenGenerator;
        public InspectorArea insArea2;


        public void InitializeSecondPage()
        {
            stackview = new StackView();
            TabPage second = tbcMain.TabPages[1];
            second.Text = "Links";

            backPanel = new Panel(manager);
            backPanel.Height = second.Height;
            backPanel.Width = second.Width;
            //backPanel.Width = second.Width + 20;
            backPanel.AutoScroll = true;
            backPanel.Init();
            //backPanel.Color = master.BackColor;
            second.Add(backPanel);

            HeightCounter3 = 0;
            VertPadding3 = 0;

            SourceTarget = new CollapsePanel(manager, backPanel, "Source      |   Target"); stackview.AddPanel(SourceTarget);
            LinkPalette = new CollapsePanel(manager, backPanel, "Link Palette"); stackview.AddPanel(LinkPalette);
            //c3 = new CollapsePanel(manager, backPanel, "third", extended: false); stackview.AddPanel(c3);
            //c4 = new CollapsePanel(manager, backPanel, "fourth", extended: false); stackview.AddPanel(c4);
            //c5 = new CollapsePanel(manager, backPanel, "fifth", extended: false); stackview.AddPanel(c5);
            //c6 = new CollapsePanel(manager, backPanel, "sixth", extended: false); stackview.AddPanel(c6);
            //c7 = new CollapsePanel(manager, backPanel, "seventh", extended: false); stackview.AddPanel(c7);
            //c8 = new CollapsePanel(manager, backPanel, "eighth", extended: false); stackview.AddPanel(c8);

            backPanel.Color = UserInterface.TomDark;

            tbcMain.SelectedPage = tbcMain.TabPages[1];

            #region /// Source | Target ///

            int left = backPanel.Left;
            int middle = backPanel.Left + backPanel.Width / 2 - 15;
            int right = backPanel.Width - 15;

            SourceTarget.ExpandedHeight += 60;

            

            #region /// Source Side ///

            lblSource = new Label(manager);
            lblSource.Init();
            lblSource.Left = left;
            lblSource.Top = HeightCounter3; HeightCounter3 += lblSource.Height + VertPadding3;
            lblSource.Text = "   Source";
            lblSource.Parent = SourceTarget.panel;

            lblGroupS = new Label(manager);
            lblGroupS.Init();
            lblGroupS.Left = left;
            lblGroupS.Top = HeightCounter3; HeightCounter3 += lblGroupS.Height + VertPadding3;
            lblGroupS.Text = "Group";
            lblGroupS.Parent = SourceTarget.panel;

            cbGroupS = new ComboBox(manager);
            cbGroupS.Init();
            cbGroupS.Left = left;
            cbGroupS.Top = HeightCounter3; HeightCounter3 += cbGroupS.Height + VertPadding3;
            cbGroupS.Width = middle;
            cbGroupS.Parent = SourceTarget.panel;
            cbGroupS.ItemIndexChanged += cbGroupS_ItemIndexChanged;
            cbGroupS.MaxItems = 100;

            lblNodeS = new Label(manager);
            lblNodeS.Init();
            lblNodeS.Left = left;
            lblNodeS.Top = HeightCounter3; HeightCounter3 += lblNodeS.Height + VertPadding3;
            lblNodeS.Text = "Node";
            lblNodeS.Parent = SourceTarget.panel;

            cbNodeS = new ComboBox(manager);
            cbNodeS.Init();
            cbNodeS.Left = left;
            cbNodeS.Top = HeightCounter3; HeightCounter3 += cbNodeS.Height + VertPadding3;
            cbNodeS.Width = middle;
            cbNodeS.Parent = SourceTarget.panel;

            GroupBox radioBoxSource = new GroupBox(manager);
            radioBoxSource.Init();
            radioBoxSource.Left = left;
            radioBoxSource.Top = HeightCounter3 - 7;
            radioBoxSource.Width = middle;
            radioBoxSource.Height = 75;
            radioBoxSource.Text = "";
            radioBoxSource.Parent = SourceTarget.panel;

            HeightCounter3 = 5;

            rdGroupS = new RadioButton(manager);
            rdGroupS.Init();
            rdGroupS.Left = left;
            rdGroupS.Top = HeightCounter3; HeightCounter3 += rdGroupS.Height + VertPadding3;
            rdGroupS.Width = middle;
            rdGroupS.Text = "Group";
            rdGroupS.Parent = radioBoxSource;
            rdGroupS.Click += rdGroupS_Click;

            rdNodeS = new RadioButton(manager);
            rdNodeS.Init();
            rdNodeS.Left = left;
            rdNodeS.Top = HeightCounter3; HeightCounter3 += rdNodeS.Height + VertPadding3;
            rdNodeS.Width = middle;
            rdNodeS.Text = "Node";
            rdNodeS.Parent = radioBoxSource;
            rdNodeS.Click += rdNodeS_Click;

            chSelectionS = new CheckBox(manager);
            chSelectionS.Init();
            chSelectionS.Left = left;
            chSelectionS.Top = HeightCounter3; HeightCounter3 += chSelectionS.Height + VertPadding3;
            chSelectionS.Width = middle;
            chSelectionS.Text = "Selection";
            chSelectionS.Parent = radioBoxSource;
            chSelectionS.Click += chSelectionS_Click;

            #endregion

            #region /// Target Side ///

            HeightCounter3 = 0;

            lblTarget = new Label(manager);
            lblTarget.Init();
            lblTarget.Left = middle;
            lblTarget.Top = HeightCounter3; HeightCounter3 += lblTarget.Height + VertPadding3;
            lblTarget.Text = "|   Target";
            lblTarget.Parent = SourceTarget.panel;

            lblGroupT = new Label(manager);
            lblGroupT.Init();
            lblGroupT.Left = middle;
            lblGroupT.Top = HeightCounter3; HeightCounter3 += lblGroupT.Height + VertPadding3;
            lblGroupT.Text = "Group";
            lblGroupT.Parent = SourceTarget.panel;

            cbGroupT = new ComboBox(manager);
            cbGroupT.Init();
            cbGroupT.Left = middle;
            cbGroupT.Top = HeightCounter3; HeightCounter3 += cbGroupT.Height + VertPadding3;
            cbGroupT.Width = middle;
            cbGroupT.Parent = SourceTarget.panel;
            cbGroupT.ItemIndexChanged += cbGroupT_ItemIndexChanged;
            cbGroupT.MaxItems = 100;

            lblNodeT = new Label(manager);
            lblNodeT.Init();
            lblNodeT.Left = middle;
            lblNodeT.Top = HeightCounter3; HeightCounter3 += lblNodeT.Height + VertPadding3;
            lblNodeT.Text = "Node";
            lblNodeT.Parent = SourceTarget.panel;

            cbNodeT = new ComboBox(manager);
            cbNodeT.Init();
            cbNodeT.Left = middle;
            cbNodeT.Top = HeightCounter3; HeightCounter3 += cbNodeT.Height + VertPadding3;
            cbNodeT.Width = middle;
            cbNodeT.Parent = SourceTarget.panel;


            GroupBox radioBoxTarget = new GroupBox(manager);
            radioBoxTarget.Init();
            radioBoxTarget.Left = middle;
            radioBoxTarget.Top = HeightCounter3 - 7;
            radioBoxTarget.Width = middle;
            radioBoxTarget.Height = 75;
            radioBoxTarget.Text = "";
            radioBoxTarget.Parent = SourceTarget.panel;

            HeightCounter3 = 5;

            rdGroupT = new RadioButton(manager);
            rdGroupT.Init();
            rdGroupT.Left = left;
            rdGroupT.Top = HeightCounter3; HeightCounter3 += rdGroupT.Height + VertPadding3;
            rdGroupT.Width = middle;
            rdGroupT.Text = "Group";
            rdGroupT.Parent = radioBoxTarget;
            rdGroupT.Click += rdGroupT_Click;

            rdNodeT = new RadioButton(manager);
            rdNodeT.Init();
            rdNodeT.Left = left;
            rdNodeT.Top = HeightCounter3; HeightCounter3 += rdNodeT.Height + VertPadding3;
            rdNodeT.Width = middle;
            rdNodeT.Text = "Node";
            rdNodeT.Parent = radioBoxTarget;
            rdNodeT.Click += rdNodeT_Click;

            chSelectionT = new CheckBox(manager);
            chSelectionT.Init();
            chSelectionT.Left = left;
            chSelectionT.Top = HeightCounter3; HeightCounter3 += chSelectionT.Height + VertPadding3;
            chSelectionT.Width = middle;
            chSelectionT.Text = "Selection";
            chSelectionT.Parent = radioBoxTarget;
            chSelectionT.Click += chSelectionT_Click;

            #endregion

            #endregion

            #region /// Link Palette ///

            LinkPalette.ExpandedHeight += 130;
            HeightCounter3 = 5;
            GroupPanel parent3 = LinkPalette.panel;

            cbLinkList = new ComboBox(manager);
            cbLinkList.Init();
            cbLinkList.Top = HeightCounter3; HeightCounter3 += cbLinkList.Height;
            cbLinkList.Left = 0;
            cbLinkList.Width = 150; 
            cbLinkList.Parent = parent3;
            cbLinkList.Items.AddRange(new List<object>() { "Palette Links", "Source's Links", "Target's Links", "All Active Links" });
            cbLinkList.ItemIndexChanged += cbLinkList_ItemIndexChanged;
            


            btnCreateLink = new Button(manager);
            btnCreateLink.Init();
            btnCreateLink.Top = HeightCounter3; //HeightCounter3 += btnCreateLink.Height;
            btnCreateLink.Left = 0;
            btnCreateLink.Width = (parent3.Width - 10) / 2;
            btnCreateLink.Text = "Create Link";
            btnCreateLink.Parent = parent3;
            btnCreateLink.Click += btnCreateLink_Click;

            btnOpenGenerator = new Button(manager);
            btnOpenGenerator.Init();
            btnOpenGenerator.Top = HeightCounter3; HeightCounter3 += btnOpenGenerator.Height;
            btnOpenGenerator.Left = btnCreateLink.Width;
            btnOpenGenerator.Width = btnCreateLink.Width;
            btnOpenGenerator.Text = "Generator";
            btnOpenGenerator.Parent = parent3;
            btnOpenGenerator.Click += btnOpenGenerator_Click;
            
            insArea2 = new InspectorArea(this, parent3, 0, HeightCounter3);
            //insArea2.backPanel.AutoScroll = true;
            LinkPalette.ExpandedHeight = HeightCounter3 + insArea2.Height + 20;

            cbLinkList.ItemIndex = 0;

            rdGroupS.Checked = true;
            rdGroupT.Checked = true;
            rdGroupS_Click(null, null);
            rdGroupT_Click(null, null);

            #endregion

            backPanel.Refresh();
            tbcMain.SelectedPage = tbcMain.TabPages[0];

        }

        void cbGroupS_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (cbGroupS.ItemIndex == -1) return;
            string name = cbGroupS.Items.ElementAt(cbGroupS.ItemIndex).ToString();
            if (name.Equals("")) return;

            Group g = room.masterGroup.FindGroup(name);
            if (g == null) return;

            cbNodeS.Items.RemoveRange(0, cbNodeS.Items.Count);
            g.ForEachFullSet((Node n) => cbNodeS.Items.Add(n));


        }

        void cbGroupT_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (cbGroupT.ItemIndex == -1) return;
            string name = cbGroupT.Items.ElementAt(cbGroupT.ItemIndex).ToString();
            if (name.Equals("")) return;

            Group g = room.masterGroup.FindGroup(name);
            if (g == null) return;

            cbNodeT.Items.RemoveRange(0, cbNodeT.Items.Count);
            g.ForEachFullSet((Node n) => cbNodeT.Items.Add(n));
        }

        void rdGroupS_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (chSelectionS.Checked) return;
            cbGroupS.Enabled = true;
            cbNodeS.Enabled = false;
        }

        void rdNodeS_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (chSelectionS.Checked) return;
            cbGroupS.Enabled = true;
            cbNodeS.Enabled = true;
        }

        void chSelectionS_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (chSelectionS.Checked)
            {
                cbGroupS.Enabled = false;
                cbNodeS.Enabled = false;
            }
            else
            {
                if (rdGroupS.Checked)
                {
                    cbGroupS.Enabled = true;
                    cbNodeS.Enabled = false;
                }
                else if (rdNodeS.Checked)
                {
                    cbGroupS.Enabled = true;
                    cbNodeS.Enabled = true;
                }
            }
        }

        void rdGroupT_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (chSelectionT.Checked) return;
            cbGroupT.Enabled = true;
            cbNodeT.Enabled = false;
        }

        void rdNodeT_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (chSelectionT.Checked) return;
            cbGroupT.Enabled = true;
            cbNodeT.Enabled = true;
        }

        void chSelectionT_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (chSelectionT.Checked)
            {
                cbGroupT.Enabled = false;
                cbNodeT.Enabled = false;
            }
            else
            {
                if (rdGroupT.Checked)
                {
                    cbGroupT.Enabled = true;
                    cbNodeT.Enabled = false;
                }
                else if (rdNodeT.Checked)
                {
                    cbGroupT.Enabled = true;
                    cbNodeT.Enabled = true;
                }
            }
        }
        //palette
        void cbLinkList_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            insArea2.ClearInspectorBox();
            string str = cbLinkList.SelectedItem();
            //remove panelControl elements (from groupPanel at the bottom)
            if (insArea2.propertyEditPanel.panelControls.Keys.Count > 0)
            {
                insArea2.propertyEditPanel.DisableControls();
            }

            if (str.Equals("Palette Links"))
            {
                insArea2.ResetInspectorBox(PaletteLinks);
            }
            else if (str.Equals("Source's Links"))
            {
                if (rdGroupS.Checked)
                {
                    string s = cbGroupS.SelectedItem();
                    if (s == null || s.Equals("")) return;
                    Group g = room.masterGroup.FindGroup(s);
                    if (g == null) return;

                    insArea2.ResetInspectorBox(g.SourceLinks);
                }
                else if (rdNodeS.Checked)
                {
                    if (cbNodeS.ItemIndex < 0 || cbNodeS.ItemIndex > cbNodeS.Items.Count) return;
                    object o = cbNodeS.Items.ElementAt(cbNodeS.ItemIndex);
                    if (!(o is Node)) return;
                    Node n = (Node)o;

                    insArea2.ResetInspectorBox(n.SourceLinks);
                }
                else if (chSelectionS.Checked)
                {
                    //todo: implement selection linking
                }
            }
            else if (str.Equals("Target's Links"))
            {
                if (rdGroupT.Checked)
                {
                    string s = cbGroupT.SelectedItem();
                    if (s == null || s.Equals("")) return;
                    Group g = room.masterGroup.FindGroup(s);
                    if (g == null) return;

                    insArea2.ResetInspectorBox(g.TargetLinks);
                }
                else if (rdNodeT.Checked)
                {
                    if (cbNodeT.ItemIndex < 0 || cbNodeT.ItemIndex > cbNodeT.Items.Count) return;
                    object o = cbNodeT.Items.ElementAt(cbNodeT.ItemIndex);
                    if (!(o is Node)) return;
                    Node n = (Node)o;

                    insArea2.ResetInspectorBox(n.TargetLinks);
                }
                else if (chSelectionT.Checked)
                {
                    //todo: implement selection linkin
                }
            }
            else if (str.Equals("All Active Links"))
            {
                insArea2.ResetInspectorBox(room.AllActiveLinks);
            }
        }

        void btnCreateLink_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            //InspectorBox box = insArea2.InsBox;
            ListBox box = insArea2.InsBox;
            if (box.ItemIndex < 0 || box.ItemIndex > box.Items.Count) return;
            if (box.ItemIndex >= box.Items.Count) return;

            object i = box.Items.ElementAt(box.ItemIndex);
            if (!(i is InspectorInfo)) return;
            InspectorInfo item = (InspectorInfo)i;
            if (!(item.obj is Link)) return;
            Link link = (Link)item.obj;

            //source
            dynamic source = null;
            if (chSelectionS.Checked)
            {
                //todo: implement selection linking
                if (rdGroupS.Checked)
                {
                    HashSet<Node> groupset = room.processManager.GetProcess<GroupSelect>().groupSelectSet;
                    if (groupset != null && groupset.Count > 0)
                    {
                        source = groupset;
                    }
                }
                else if (rdNodeS.Checked)
                {
                    if (room.targetNode != null)
                    {
                        source = room.targetNode;
                    }
                }

            }
            else if (rdGroupS.Checked)
            {
                string s = cbGroupS.SelectedItem();
                if (s == null || s.Equals("")) return;
                Group g = room.masterGroup.FindGroup(s);
                if (g == null) return;

                source = g;
            }
            else if (rdNodeS.Checked)
            {
                if (cbNodeS.ItemIndex < 0 || cbNodeS.ItemIndex > cbNodeS.Items.Count) return;
                object o = cbNodeS.Items.ElementAt(cbNodeS.ItemIndex);
                if (!(o is Node)) return;
                Node n = (Node)o;
                source = n;
            }
            

            //target
            dynamic target = null;
            if (chSelectionT.Checked)
            {
                //todo: implement selection linking
                if (rdGroupT.Checked)
                {
                    HashSet<Node> groupset = room.processManager.GetProcess<GroupSelect>().groupSelectSet;
                    
                    if (groupset != null && groupset.Count > 0)
                    {
                        target = groupset;
                    }
                }
                else if (rdNodeT.Checked)
                {
                    if (room.targetNode != null)
                    {
                        target = room.targetNode;
                    }
                }
            }
            else if (rdGroupT.Checked)
            {
                string s = cbGroupT.SelectedItem();
                if (s == null || s.Equals("")) return;
                Group g = room.masterGroup.FindGroup(s);
                if (g == null) return;

                target = g;
            }
            else if (rdNodeT.Checked)
            {
                if (cbNodeT.ItemIndex < 0 || cbNodeT.ItemIndex > cbNodeT.Items.Count) return;
                object o = cbNodeT.Items.ElementAt(cbNodeT.ItemIndex);
                if (!(o is Node)) return;
                Node n = (Node)o;
                target = n;
            }
            

            //create link
            if (source == null || target == null) return;
            /*
            dynamic newComponent = Activator.CreateInstance(link.linkComponent.GetType());
            Component.CloneComponent((Component)link.linkComponent, newComponent);
            //newComponent.Initialize();
            newComponent.active = true;
            if (newComponent.GetType().GetProperty("activated") != null) newComponent.activated = true;
            */

            Link newLink = new Link(link, source, target);
            //Link newLink = new Link(source, target, newComponent, link.formation);
            room.AllActiveLinks.Add(newLink);

            cbLinkList.ItemIndex = 1;
        }

        void btnOpenGenerator_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            LinkGeneratorWindow gen = new LinkGeneratorWindow(manager,this);
        }

        

        void b_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            backPanel.Refresh();
        }
    }
}
