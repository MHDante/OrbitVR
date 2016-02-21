using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Component = OrbItProcs.Component;
using Console = System.Console;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;
using Microsoft.Xna.Framework.Input;
using System.Collections.ObjectModel;


namespace OrbItProcs
{
    public class InspectorArea
    {
        public OrbIt game;
        public Room room { get { return game.room; } }
        public UserInterface ui;
        public Sidebar sidebar;
        public InspectorInfo ActiveInspectorParent;
        public Node editNode;

        public UserLevel userLevel { get { return sidebar.userLevel; } set { sidebar.userLevel = value; } }

        public int Left;
        public int Top;
        public int Width;
        public int Height { get { return (propertyEditPanel.grouppanel.Top + propertyEditPanel.grouppanel.Height);} }
        public int HeightCounter;
        public int LeftPadding = 5;
        public int VertPadding = 4;
        public int ScrollPosition = 0;
        public int InsBoxParentTop = 0;

        public bool GenerateFields { get; set; }

        public Manager manager;
        public Control parent;
        public Panel backPanel;
        public Label lblInspectorAddress;


        //public InspectorBox InsBox;
        public ListBox InsBox;
        public InspectorInfo rootitem;

        public PropertyEditPanel propertyEditPanel;
        public ContextMenu contextMenuInsBox;
        public MenuItem applyToAllNodesMenuItem, toggleComponentMenuItem, removeComponentMenuItem, toggleBoolMenuItem;
        public MenuItem toggleLinkMenuItem, removeLinkMenuItem, addComponentToLinkMenuItem;

        public Tuple<bool, string> OverrideString = new Tuple<bool, string>(false, "");

        public ComponentView compView;
        //public Group activeGroup = null;

        public InspectorArea(Sidebar sidebar, Control parent, int Left, int Top)
        {
            //get rid of these if not needed.
            this.game = sidebar.game;
            this.ui = sidebar.ui;
            this.sidebar = sidebar;
            this.manager = sidebar.manager;
            this.parent = parent;

            this.Left = Left;
            this.Top = Top;

            this.GenerateFields = true; //todo: set to true when usermode is set up (so you can see fields in debug)
            //this.ActiveInspectorParent = sidebar.ActiveInspectorParent;
            
            Initialize();
            
        }

        public void ChangeUserLevel()
        {
            if (userLevel == UserLevel.Debug)
            {
            }
        }

        public void SetOverrideString(string name)
        {
            bool alreadyTrue = OverrideString.Item1;

            OverrideString = new Tuple<bool, string>(true, name);
            lblInspectorAddress.Text = OverrideString.Item2;
            if (alreadyTrue) return;
            if (lblInspectorAddress.Height == labelDoubleHeight)
            {
                InsBox.Top -= labelDoubleHeight / 2;
                propertyEditPanel.grouppanel.Top -= labelDoubleHeight / 2;
            }
            lblInspectorAddress.Height = labelDoubleHeight / 2;
        }

        public void RemoveOverrideString()
        {
            OverrideString = new Tuple<bool, string>(false, OverrideString.Item2);
            lblInspectorAddress.Text = "__";
            if (lblInspectorAddress.Height == labelDoubleHeight / 2)
            {
                InsBox.Top += labelDoubleHeight / 2;
                propertyEditPanel.grouppanel.Top += labelDoubleHeight / 2;
            }
            lblInspectorAddress.Height = labelDoubleHeight;
        }

        private int labelDoubleHeight = 0;

        public void Initialize()
        {
            HeightCounter = 0;
            Width = parent.Width - LeftPadding * 2;

            #region /// GroupBox (back panel) ///
            backPanel = new Panel(manager);
            backPanel.Init();
            //backPanel.Left = Left;//change
            backPanel.Top = Top;
            backPanel.Width = Width;
            backPanel.Parent = parent;
            backPanel.Text = "";
            backPanel.Color = sidebar.master.BackColor;
            
            #endregion

            int WidthReduction = 15; //change

            #region  /// Inspector Address Label ///
            lblInspectorAddress = new Label(manager);
            lblInspectorAddress.Init();
            //lblInspectorAddress.Visible = false; //change
            lblInspectorAddress.Parent = backPanel;
            lblInspectorAddress.Top = HeightCounter; //HeightCounter += VertPadding + lblInspectorAddress.Height + 10;
            lblInspectorAddress.Width = Width - WidthReduction;
            lblInspectorAddress.Height = lblInspectorAddress.Height * 2; //HeightCounter += lblInspectorAddress.Height;
            labelDoubleHeight = lblInspectorAddress.Height;
            //lblInspectorAddress.Left = LeftPadding;
            //lblInspectorAddress.Anchor = Anchors.Left;
            lblInspectorAddress.Text = ">No Node Selected<";
            bool changed = false;
            lblInspectorAddress.TextChanged += delegate(object s, EventArgs e)
            {
                if (!changed)
                {
                    changed = true;
                    Label l = (Label)s;
                    l.Text = l.Text.wordWrap(25);
                }
                changed = false;
            };
            #endregion
            //btnToggleFields = new Button(manager);
            //btnToggleFields.Init();
            //btnToggleFields.Parent = backPanel;
            //btnToggleFields.Left = LeftPadding + backPanel.Width - 30;
            //btnToggleFields.Width = 15;
            //btnToggleFields.Height = 20;
            //btnToggleFields.Top = HeightCounter + 10; 
            //btnToggleFields.Text = "F";
            //btnToggleFields.Click += delegate(object s, EventArgs e)
            //{
            //    this.GenerateFields = !this.GenerateFields;
            //    //ResetInspectorBox(ActiveInspectorParent.obj);
            //    ActiveInspectorParent.DoubleClickItem(this);
            //};

            HeightCounter += lblInspectorAddress.Height;


            #region  /// Component List ///
            InsBox = new ListBox(manager);
            InsBox.Init();
            //manager.Add(InsBox);
            InsBox.Parent = backPanel;

            InsBox.Top = HeightCounter;
            InsBox.Left = 10;
            InsBox.Width = Width - WidthReduction;
            InsBox.Height = 140; HeightCounter += InsBox.Height;

            InsBox.HideSelection = false;
            InsBox.ItemIndexChanged += InsBox_ItemIndexChanged;
            InsBox.Click += InsBox_Click;
            InsBox.DoubleClick += InsBox_DoubleClick;
            InsBox.Refresh();

            sidebar.ui.SetScrollableControl(InsBox, InsBox_ChangeScrollPosition);

            #region  /// Context Menu ///
            contextMenuInsBox = new ContextMenu(manager);
            applyToAllNodesMenuItem = new MenuItem("Apply to Group");
            applyToAllNodesMenuItem.Click += applyToAllNodesMenuItem_Click;
            toggleComponentMenuItem = new MenuItem("Toggle Component");
            toggleComponentMenuItem.Click += toggleComponentMenuItem_Click;
            removeComponentMenuItem = new MenuItem("Remove Component");
            removeComponentMenuItem.Click += removeComponentMenuItem_Click;
            toggleBoolMenuItem = new MenuItem("Toggle");
            toggleBoolMenuItem.Click += toggleBoolMenuItem_Click;
            toggleLinkMenuItem = new MenuItem("Toggle link");
            toggleLinkMenuItem.Click += toggleLinkMenuItem_Click;
            removeLinkMenuItem = new MenuItem("Delete link");
            removeLinkMenuItem.Click += removeLinkMenuItem_Click;
            addComponentToLinkMenuItem = new MenuItem("Add Link Component");
            addComponentToLinkMenuItem.Click += AddComponentToLinkMenuItem_Click;


            contextMenuInsBox.Items.Add(applyToAllNodesMenuItem);

            InsBox.ContextMenu = contextMenuInsBox;
            #endregion

            #endregion

            #region  /// GroupPanel ///
            GroupPanel groupPanel = new GroupPanel(manager);
            groupPanel.Init();
            groupPanel.Parent = backPanel;
            //groupPanel.Visible = false;//change
            groupPanel.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            groupPanel.Width = Width - WidthReduction;
            groupPanel.Top = HeightCounter;
            groupPanel.Height = 90; HeightCounter += VertPadding + groupPanel.Height;
            groupPanel.Left = InsBox.Left;
            groupPanel.Text = "Inspector Property";
            #endregion


            #region  /// PropertyEditPanel ///
            propertyEditPanel = new PropertyEditPanel(this, groupPanel);
            #endregion

            backPanel.Height = groupPanel.Top + groupPanel.Height;

        }

        public void ClearInspectorBox()
        {
            InsBox.ItemIndex = 0;
            rootitem = null;
            ActiveInspectorParent = null;
            foreach (object o in InsBox.Items.ToList())
            {
                InsBox.Items.Remove(o);
            }
            lblInspectorAddress.Text = "/";

            propertyEditPanel.DisableControls();
        }

        public void ResetInspectorBox(object rootobj)
        {
            if (rootobj == null)
            {
                ClearInspectorBox();
                return;
            }
            InsBox.ItemIndex = 0;
            InspectorInfo root = new InspectorInfo(InsBox.Items, rootobj, sidebar);
            root.showValueToString = true;
            this.rootitem = root;
            root.GenerateChildren(GenerateFields, userLevel: UserLevel.Debug);
            ActiveInspectorParent = root;
            

            foreach (object o in InsBox.Items.ToList())
            {
                InsBox.Items.Remove(o);
            }
            foreach (object o in root.children.ToList())
            {
                InsBox.Items.Add(o);
            }
            InsBox.Refresh();
            if (rootobj is Node)
            {
                editNode = (Node)rootobj;
                lblInspectorAddress.Text = "/" + editNode.ToString();
            }
            else
            {
                lblInspectorAddress.Text = rootobj.GetType().ToString();
            }

            if (OverrideString.Item1)
            {
                lblInspectorAddress.Text = OverrideString.Item2;
                lblInspectorAddress.Height = labelDoubleHeight / 2;
            }
            else
            {
                lblInspectorAddress.Height = labelDoubleHeight;
            }

        }

        public void BuildItemsPath(InspectorInfo item, List<InspectorInfo> itemspath)
        {
            InspectorInfo temp = item;
            itemspath.Insert(0, temp);
            while (temp.parentItem != null)
            {
                temp = temp.parentItem;
                itemspath.Insert(0, temp);
            }
        }

        void InsBox_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ListBox InsBox = (ListBox)sender;

            if (InsBox.ItemIndex < 0 || InsBox.Items.Count == 0) return;
            InspectorInfo item = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);
            
            //UpdateGroupPanel(item, groupPanel);
        }

        void InsBox_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            //InspectorBox InsBox = (InspectorBox)sender;
            MouseEventArgs mouseArgs = (MouseEventArgs)e;

            if (mouseArgs.Button == MouseButton.Right)
            {
                contextMenuInsBox.Items.RemoveRange(0, contextMenuInsBox.Items.Count);
                if (InsBox.ItemIndex < 0 || InsBox.Items.Count == 0) return;


                InspectorInfo litem = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);

                if (litem.obj is Component)
                {
                    contextMenuInsBox.Items.Add(toggleComponentMenuItem);
                    contextMenuInsBox.Items.Add(removeComponentMenuItem);
                    contextMenuInsBox.Items.Add(applyToAllNodesMenuItem);
                }
                else if (litem.obj is Link)
                {
                    contextMenuInsBox.Items.Add(toggleLinkMenuItem);
                    contextMenuInsBox.Items.Add(addComponentToLinkMenuItem);
                    contextMenuInsBox.Items.Add(removeLinkMenuItem);
                    
                }
                else
                {
                    if (litem.obj is bool)
                    {
                        contextMenuInsBox.Items.Add(toggleBoolMenuItem);
                    }

                    contextMenuInsBox.Items.Add(applyToAllNodesMenuItem);
                }

            }
            else if (mouseArgs.Button == MouseButton.Left)
            {
                if (InsBox.ItemIndex < 0 || InsBox.Items.Count == 0 || InsBox.ItemIndex >= InsBox.Items.Count) return;
                InspectorInfo item = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);
                if (item.obj == null) return;
                //item.ClickItem(InsBox.ItemIndex);
                Type t = item.obj.GetType();

                //if (activeInspectorItem != item)
                propertyEditPanel.UpdatePanel(item);
            }
        }

        void InsBox_DoubleClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            //InspectorBox InsBox = (InspectorBox)sender;
            MouseEventArgs mouseArgs = (MouseEventArgs)e;
            if (mouseArgs.Button == MouseButton.Left)
            {
                if (InsBox.ItemIndex < 0 || InsBox.Items.Count == 0 || InsBox.ItemIndex >= InsBox.Items.Count) return;
                InspectorInfo item = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);
                item.DoubleClickItem(this);
                InspectorInfo temp = ActiveInspectorParent;
                string lbltext = "/" + temp.Name();
                while (temp.parentItem != null)
                {
                    temp = temp.parentItem;
                    lbltext = lbltext.Insert(0, "/" + temp.Name());
                }
                if (!OverrideString.Item1)
                {
                    lblInspectorAddress.Text = lbltext;
                }
                //Type t = item.obj.GetType();

                //if (activeInspectorItem != item)
                //    propertyEditPanel.UpdatePanel(item);

            }
        }

        void applyToAllNodesMenuItem_Click(object sender, TomShane.Neoforce.Controls.EventArgs e) //TODO: fix the relection copying reference types
        {
            Group g = sidebar.GetActiveGroup();
            if (g == null) return;
            ApplyToAllNodes(g);
        }

        public void ApplyToAllNodes(Group group)
        {
            if (group == null) return;
            List<InspectorInfo> itemspath = new List<InspectorInfo>();
            InspectorInfo item = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);
            object value = item.GetValue();

            BuildItemsPath(item, itemspath);

            group.ForEachAllSets(delegate(Node n)
            {
                if (n == itemspath.ElementAt(0).obj) return;
                InspectorInfo temp = new InspectorInfo(null, n, sidebar);
                int count = 0;
                foreach (InspectorInfo pathitem in itemspath)
                {
                    if (temp.obj.GetType() != pathitem.obj.GetType())
                    {
                        Console.WriteLine("The paths did not match while applying to all. {0} != {1}", temp.obj.GetType(), pathitem.obj.GetType());
                        break;
                    }
                    if (count == itemspath.Count - 1) //last item
                    {
                        if (pathitem.membertype == member_type.dictentry)
                        {
                            dynamic dict = temp.parentItem.obj;
                            dynamic key = pathitem.key;
                            if (!dict.ContainsKey(key)) break;
                            if (dict[key] is Component)
                            {
                                dict[key].active = ((Component)value).active;
                            }
                            else if (temp.IsPanelType())
                            {
                                dict[key] = value;
                            }
                        }
                        else
                        {
                            if (value is Component)
                            {
                                ((Component)temp.obj).active = ((Component)value).active;
                            }
                            else if (temp.IsPanelType())
                            {
                                temp.fpinfo.SetValue(value, temp.parentItem.obj);
                            }
                        }
                    }
                    else
                    {
                        InspectorInfo next = itemspath.ElementAt(count + 1);
                        if (next.membertype == member_type.dictentry)
                        {
                            dynamic dict = temp.obj;
                            dynamic key = next.key;
                            if (!dict.ContainsKey(key)) break;
                            temp = new InspectorInfo(null, temp, dict[key], key);
                        }
                        else
                        {
                            temp = new InspectorInfo(null, temp, next.fpinfo.GetValue(temp.obj), next.fpinfo.propertyInfo);
                        }
                    }
                    count++;
                }
            });
        }

        void removeLinkMenuItem_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            InspectorInfo item = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);
            if (!(item.obj is Link))
            {
                Console.WriteLine("Error: The list item was not a link.");
                return;
            }

            Link link = (Link)item.obj;
            if (item.parentItem.obj is ObservableHashSet<Link>)
            {
                //Console.WriteLine("ObservableHashSet<Link> has been observed.");
                ObservableHashSet<Link> set = (ObservableHashSet<Link>)item.parentItem.obj;
                set.Remove(link);
                link.DeleteLink();

                item.parentItem.DoubleClickItem(this);

                
            }
        }

        void toggleLinkMenuItem_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            InspectorInfo item = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);
            if (!(item.obj is Link))
            {
                Console.WriteLine("Error: The list item was not a link.");
                return;
            }

            Link link = (Link)item.obj;
            //link.linkComponent.active = !link.linkComponent.active;
            link.active = !link.active;
        }

        void toggleComponentMenuItem_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            InspectorInfo item = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);
            if (!(item.obj is Component))
            {
                Console.WriteLine("Error: The list item was not a component.");
                return;
            }

            Component component = (Component)item.obj;
            component.active = !component.active;
        }

        void toggleBoolMenuItem_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            InspectorInfo item = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);
            if (!(item.obj is bool))
            {
                Console.WriteLine("Error: The list item was not a bool.");
                return;
            }

            //item.SetValue(!(bool)item.GetValue());
            //SetItemValue(item, !(bool)item.GetValue());
        }


        void removeComponentMenuItem_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            InspectorInfo item = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);
            if (!(item.obj is Component))
            {
                Console.WriteLine("Error: The list item was not a component.");
                return;
            }
            Component component = (Component)item.obj;
            if (component is Movement || component is Collision)
            {
                return;
            }

            if (item.parentItem.parentItem.obj is Node)
            {
                component.active = false;
                editNode.RemoveComponent(component.GetType());
                item.RemoveChildren();
                InsBox.Items.Remove(item);
            }
            else if (item.parentItem.parentItem.obj is Link)
            {
                component.active = false;
                Link link = (Link)item.parentItem.parentItem.obj;
                //link.components.Remove(component as ILinkable);
                link.components.Remove(component.GetType());
                item.RemoveChildren();
                InsBox.Items.Remove(item);
            }
        }

        public void InsBox_ChangeScrollPosition(int change)
        {
            if (ScrollPosition + change < 0) ScrollPosition = 0;
            else if (ScrollPosition + change > InsBox.Items.Count - 9) ScrollPosition = InsBox.Items.Count - 9;
            else ScrollPosition += change;
            InsBox.ScrollTo(InsBox.Items.Count - 1);
            InsBox.ScrollTo(ScrollPosition);
        }


        void AddComponentToLinkMenuItem_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            InspectorInfo item = (InspectorInfo)InsBox.Items.ElementAt(InsBox.ItemIndex);
            if (!(item.obj is Link))
            {
                //Console.WriteLine("Error: The list item was not a link.");
                PopUp.Toast("You haven't selected a Link.");
                return;
            }
            Link link = (Link)item.obj;


            ObservableCollection<dynamic> Ilinkables = new ObservableCollection<dynamic>();
            List<object> list = new List<object>();
            Link.GetILinkableEnumVals(list);
            foreach (object obj in list) Ilinkables.Add(obj);


            PopUp.opt[] options = new PopUp.opt[]{
                new PopUp.opt(PopUp.OptType.info, "Add link component to: " + link.ToString()),
                new PopUp.opt(PopUp.OptType.dropDown, Ilinkables)};
                /*new PopUp.opt(PopUp.OptType.checkBox, "Add to all", 
                    delegate(object s, TomShane.Neoforce.Controls.EventArgs a){
                        if ((s as CheckBox).Checked) nodecomplist.AddRange(missingcomps);
                        else nodecomplist.RemoveRange(missingcomps);})};
                */
            PopUp.makePopup(ui, options, "Add Link Component", delegate(bool a, object[] o)
            {
                if (a) return AddLinkComponent(o, link);
                else return false;
            });
            
        }
        private bool AddLinkComponent(object[] o, Link link)
        {
            //bool writeable = false;
            Type t = (Type)o[1];

            object linkComp = Activator.CreateInstance(t);


            link.AddLinkComponent((ILinkable)linkComp);
            //ILinkable l = (ILinkable)linkComp;
            //link.components.Add(l);

            ActiveInspectorParent.DoubleClickItem(this);
            return true;
            /*
            if (!inspectorArea.editNode.comps.ContainsKey((comp)o[1]))
            {
                inspectorArea.editNode.addComponent((comp)o[1], true);
                inspectorArea.ActiveInspectorParent.DoubleClickItem(inspectorArea);
            }
            else PopUp.Prompt(ui,
                        "The node already contains this component. Overwrite to default component?",
                        action: delegate(bool k, object ans) { writeable = k; return true; });
            
            if (writeable)
            {
                inspectorArea.editNode.addComponent((comp)o[1], true);
                inspectorArea.ActiveInspectorParent.DoubleClickItem(inspectorArea);
                return true;
            }
            else
            {
                return false;
            }
            */
        }
    }
}
