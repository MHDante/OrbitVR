using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TomShane.Neoforce.Controls;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using OrbItProcs;


using Component = OrbItProcs.Component;
using Console = System.Console;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;

namespace OrbItProcs
{
    public partial class Sidebar
    {
        EventHandler NotImplemented;
        public OrbIt game;
        public Room room { get { return game.room; } }
        
        public UserInterface ui;
        private UserLevel _userLevel = UserLevel.Debug;
        public UserLevel userLevel
        {
            get { return _userLevel; }
            set
            {
                if (value == _userLevel) return;
                _userLevel = value;

                //call refresh on inspector areas
                if (ui.detailedViews != null)
                {
                    foreach(DetailedView dv in ui.detailedViews)
                    {
                        if (dv is ComponentView)
                        {
                            (dv as ComponentView).RefreshComponents();
                        }
                        if (dv.OnUserLevelChanged != null)
                        {
                            dv.OnUserLevelChanged(value);
                        }
                    }
                }
            }
        }
        public bool CreatingGroup = false;
        public Group GetActiveGroup()
        {
            if (activeTabControl == tbcMain)
            {
                if (cbListPicker != null && cbListPicker.ItemIndex != -1 && room != null && room.masterGroup != null)
                {
                    string name = cbListPicker.Text;
                    return room.masterGroup.FindGroup(name);
                }
                return room.masterGroup;
            }

            if (tbcViews.SelectedIndex == 0 || tbcViews.SelectedIndex == 1 || tbcViews.SelectedIndex == 3) //groups (or players or processes, for now)
            {
                if (groupsView == null || presetsView == null)// groupsView.selectedItem == null)
                {
                    return room.groups.general.childGroups.ElementAt(0).Value;
                }
                else
                {
                    if (tbcGroups.SelectedIndex == 0) //custom groups
                    {
                        if (!(groupsView.selectedItem.obj is Group)) return null;
                        return (Group)groupsView.selectedItem.obj;
                    }
                    else if (tbcGroups.SelectedIndex == 1) //preset groups
                    {
                        if (!(presetsView.selectedItem.obj is Group)) return null;
                        return (Group)presetsView.selectedItem.obj;
                    }
                }
            }
            else if (tbcViews.SelectedIndex == 1) //players
            {
                return null;
            }
            else if (tbcViews.SelectedIndex == 2) //items
            {
                if (itemsView == null || itemsView.selectedItem == null)
                {
                    return room.groups.items.childGroups.ElementAt(0).Value;
                }
                else
                {
                    if (!(itemsView.selectedItem.obj is Group)) return null;
                    return (Group)itemsView.selectedItem.obj;
                }
            }
            else if (tbcViews.SelectedIndex == 3) //processes
            {
                return null;
            }
            return null;
        }

        public Node ActiveDefaultNode
        {
            get
            {
                Group g = GetActiveGroup();
                if (g != null && g.defaultNode != null)
                    return g.defaultNode;
                return null;
            }
        }
        //public InspectorInfo ActiveInspectorParent;
        
        private int _Width = 250;
        public int Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
                if (ui.SidebarActive)
                {
                    OrbIt.game.room.camera.CameraOffset = value;
                }
            }
        }
        #region /// Neoforce Fields///
        public Manager manager;
        public SideBar master;
        public TabControl tbcMain;
        public Label title1;
        TextBox consoletextbox;
        public ListBox lstMain;
        public ComboBox cbListPicker;
        Button btnRemoveNode, btnRemoveAllNodes, btnAddComponent, btnDefaultNode, btnApplyToAll, btnSaveNode, btnDeleteGroup;
        public ListBox lstPresets;
        public ComboBox cbPresets;
        public ContextMenu presetContextMenu;
        public MenuItem deletePresetMenuItem;

        public ContextMenu mainNodeContextMenu;
        public MenuItem ConvertIntoList, PromoteToDefault;
        
        #endregion

        public InspectorArea inspectorArea;

        #region /// Layout Fields///
        private int HeightCounter = 0, HeightCounter2 = 0;
        private int lstMainScrollPosition = 0;
        //private int lstCompScrollPosition = 0;
        private int LeftPadding = 5;
        private int VertPadding = 4;
        #endregion

        public Sidebar(UserInterface ui)
        {
            this.game = ui.game;
            //this.room = ui.game.room;
            this.ui = ui;
            NotImplemented = delegate {
                PopUp.Toast("Not Implemented. Take a hike.");
                //throw new NotImplementedException();
            };
            manager = game.Manager;

            manager.SetSkin("Green");
        }

        public void Initialize()
        {
            manager.Initialize();

            #region /// Master ///
            master = new SideBar(manager);
            master.Init();
            master.Name = "Sidebar";
            master.Width = Width;
            master.Height = OrbIt.game.MainWindow.ClientArea.Height;
            master.Visible = true;
            master.Anchor = Anchors.Top | Anchors.Left | Anchors.Bottom;

            ui.game.MainWindow.Add(master);
            #endregion

            #region  /// tabcontrol ///
            tbcMain = new TabControl(manager);
            tbcMain.Init();
            //tbcMain.BackColor = Color.Transparent;
            //tbcMain.Color = Color.Transparent;
            tbcMain.Parent = master;
            tbcMain.Left = 0;
            tbcMain.Top = 0;
            tbcMain.Width = master.Width - 5;
            tbcMain.Height = master.Height - 40;
            tbcMain.Anchor = Anchors.All;
            activeTabControl = tbcMain;
            #endregion

            #region  /// Page1 ///

            tbcMain.AddPage();
            tbcMain.TabPages[0].Text = "First";
            TabPage first = tbcMain.TabPages[0];
            tbcMain.SelectedIndex = 0;

            #region  /// Title ///
            title1 = new Label(manager);
            title1.Init();
            title1.Parent = first;

            title1.Top = HeightCounter;
            title1.Text = "Node List";
            title1.Width = 130;
            title1.Left = first.Width / 2 - title1.Width / 2; //TODO : Center auto
            HeightCounter += VertPadding + title1.Height;
            title1.Anchor = Anchors.Left;
            #endregion

            #region  /// List Main ///
            lstMain = new ListBox(manager);
            lstMain.Init();
            lstMain.Parent = first;

            lstMain.Top = HeightCounter;
            lstMain.Left = LeftPadding;
            lstMain.Width = first.Width - LeftPadding * 2;
            lstMain.Height = first.Height / 5; HeightCounter += VertPadding + lstMain.Height;
            lstMain.Anchor = Anchors.Top | Anchors.Left | Anchors.Bottom;

            lstMain.HideSelection = false;
            lstMain.ItemIndexChanged += lstMain_ItemIndexChanged;
            lstMain.Click += lstMain_Click;
            lstMain.Refresh();
            //room.nodes.CollectionChanged += nodes_Sync;

            mainNodeContextMenu = new ContextMenu(manager);
            ConvertIntoList = new MenuItem("Make Default of new Group.");
            ConvertIntoList.Click += ConvertIntoList_Click;
            PromoteToDefault = new MenuItem("Make Default of current Group");
            PromoteToDefault.Click += PromoteToDefault_Click;
            mainNodeContextMenu.Items.Add(ConvertIntoList);
            lstMain.ContextMenu = mainNodeContextMenu;

            
            ui.SetScrollableControl(lstMain, lstMain_ChangeScrollPosition);
            #endregion

            #region  /// List Picker ///

            cbListPicker = new ComboBox(manager);
            cbListPicker.Init();
            cbListPicker.Parent = first;
            cbListPicker.MaxItems = 20;

            cbListPicker.Width = first.Width - LeftPadding * 6;
            cbListPicker.Left = LeftPadding;
            cbListPicker.Top = HeightCounter; 
            cbListPicker.Items.Add("Other Objects");
            cbListPicker.ItemIndex = 0;
            //cbListPicker.
            cbListPicker.Click += cbListPicker_Click;
            cbListPicker.ItemIndexChanged += cbListPicker_ItemIndexChanged;

            #endregion

            #region /// Delete Group Button ///

            btnDeleteGroup = new Button(manager);
            btnDeleteGroup.Init();
            btnDeleteGroup.Parent = first;
            btnDeleteGroup.Left = LeftPadding + cbListPicker.Width + 5;
            btnDeleteGroup.Width = 15;
            btnDeleteGroup.Height = cbListPicker.Height;
            btnDeleteGroup.Top = HeightCounter; HeightCounter += VertPadding + cbListPicker.Height;
            btnDeleteGroup.Text = "X";
            btnDeleteGroup.Click += btnDeleteGroup_Click;

            #endregion

            #region  /// Remove Node Button ///
            btnRemoveNode = new Button(manager);
            btnRemoveNode.Init();
            btnRemoveNode.Parent = first;

            btnRemoveNode.Top = HeightCounter;
            btnRemoveNode.Width = first.Width / 2 - LeftPadding;
            btnRemoveNode.Height = 24;
            btnRemoveNode.Left = LeftPadding;

            btnRemoveNode.Text = "Remove Node";
            btnRemoveNode.Click += btnRemoveNode_Click;
            #endregion

            #region  /// Remove All Nodes Button ///
            btnRemoveAllNodes = new Button(manager);
            btnRemoveAllNodes.Init();
            btnRemoveAllNodes.Parent = first;

            btnRemoveAllNodes.Top = HeightCounter;
            //btnRemoveAllNodes.Width = first.Width / 2 - LeftPadding;
            btnRemoveAllNodes.Width = first.Width / 2 - LeftPadding;
            btnRemoveAllNodes.Height = 24; HeightCounter += VertPadding + btnRemoveAllNodes.Height;
            btnRemoveAllNodes.Left = LeftPadding + btnRemoveNode.Width;

            btnRemoveAllNodes.Text = "Remove All";
            btnRemoveAllNodes.Click += btnRemoveAllNodes_Click;
            #endregion

            #region  /// Add Componenet ///
            btnAddComponent = new Button(manager);
            btnAddComponent.Init();
            btnAddComponent.Parent = first;

            btnAddComponent.Top = HeightCounter;
            btnAddComponent.Width = first.Width / 2 - LeftPadding;
            btnAddComponent.Height = 20;
            btnAddComponent.Left = LeftPadding;

            btnAddComponent.Text = "Add Component";
            btnAddComponent.Click += btnAddComponent_Click;
            #endregion

            #region  /// Default Node ///
            btnDefaultNode = new Button(manager);
            btnDefaultNode.Init();
            btnDefaultNode.Parent = first;

            btnDefaultNode.Top = HeightCounter;
            btnDefaultNode.Width = first.Width / 2 - LeftPadding;
            btnDefaultNode.Height = 20; HeightCounter += VertPadding + btnDefaultNode.Height;
            btnDefaultNode.Left = LeftPadding + btnRemoveNode.Width;

            btnDefaultNode.Text = "Default Node";
            btnDefaultNode.Click += btnDefaultNode_Click;
            #endregion

            #region  /// Presets Dropdown ///
            cbPresets = new ComboBox(manager);
            cbPresets.Init();
            cbPresets.Parent = first;
            cbPresets.MaxItems = 20;
            cbPresets.Width = 160;
            cbPresets.Left = LeftPadding;
            cbPresets.Top = HeightCounter; HeightCounter += cbPresets.Height;
            Assets.NodePresets.CollectionChanged += NodePresets_Sync;
            cbPresets.ItemIndexChanged += cbPresets_ItemIndexChanged;
            cbPresets.Click += cmbPresets_Click;
            #endregion


            inspectorArea = new InspectorArea(this, first, LeftPadding, HeightCounter);

            HeightCounter += inspectorArea.Height;

            #region  /// Apply to Group ///
            btnApplyToAll = new Button(manager);
            btnApplyToAll.Init();
            btnApplyToAll.Parent = first;

            btnApplyToAll.Text = "Apply To Group";
            btnApplyToAll.Top = HeightCounter;
            btnApplyToAll.Width = first.Width / 2 - LeftPadding;
            btnApplyToAll.Height = 20; //HeightCounter += VertPadding + btnApplyToAll.Height;
            btnApplyToAll.Left = LeftPadding;
            btnApplyToAll.Click += applyToAllNodesMenuItem_Click;
            #endregion

            #region  /// Save as Preset ///
            btnSaveNode = new Button(manager);
            btnSaveNode.Init();
            btnSaveNode.Text = "Save Node";
            btnSaveNode.Top = HeightCounter;
            btnSaveNode.Width = first.Width / 2 - LeftPadding;
            btnSaveNode.Height = 20; HeightCounter += VertPadding + btnSaveNode.Height;
            btnSaveNode.Left = LeftPadding + btnApplyToAll.Width;
            btnSaveNode.Parent = first;
            btnSaveNode.Click += btnSaveNode_Click;
            #endregion

            #endregion

            #region  /// Page 2 ///
            tbcMain.AddPage();
            tbcMain.TabPages[1].Text = "Second";
            TabPage second = tbcMain.TabPages[1];
            HeightCounter = 0;

            
            
            #endregion

            

            inspectorArea.ResetInspectorBox(ActiveDefaultNode);

            InitializeSecondPage();
            InitializeThirdPage();

            
            
        }

        void btnDeleteGroup_Click(object sender, EventArgs e)
        {

            string item = cbListPicker.Items.ElementAt(cbListPicker.ItemIndex).ToString();
            if (item.Equals("Other Objects") || item.Equals("master") || item.Equals("Link Groups") || item.Equals("Normal Groups"))
            {
                return;
            }
            else if (!item.Equals(""))
            {
                Group g = GetActiveGroup();
                if (g == null) return;
                if (g.Name.Equals("[G0]")) return;

                foreach (object o in lstMain.Items.ToList())
                {
                    lstMain.Items.Remove(o);
                }


                if (g.fullSet.Contains(room.targetNode)) room.targetNode = null;
                if (g.fullSet.Contains(inspectorArea.editNode) && inspectorArea.editNode != g.defaultNode)
                {
                    inspectorArea.InsBox.Items.Clear();
                    inspectorArea.rootitem = null;
                    inspectorArea.editNode = null;
                }
                g.DeleteGroup();

                cbListPicker.Items.Remove(g.Name);

                cbListPicker.ItemIndex = 2;

                

                //UpdateGroupComboBoxes();
                SyncTitleNumber();

                

            }
            lstMain.ScrollTo(0);
        }

        void lstMain_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button != MouseButton.Right) return;
            ListBox listbox = (ListBox)sender;
            listbox.ContextMenu.Items.ToList().ForEach(o => listbox.ContextMenu.Items.Remove(o));
            //foreach (MenuItem m in listbox.ContextMenu.Items.ToList())
            //{
            //    listbox.ContextMenu.Items.Remove(m);
            //}
            Color c = listbox.ContextMenu.Color;
            listbox.ContextMenu.Color = new Color(0f, 0f, 0f, 0f);
            if (listbox.ItemIndex >= 0 && listbox.Items.ElementAt(listbox.ItemIndex) is Node)
            {
                listbox.ContextMenu.Color = new Color(1f, 1f, 1f, 1.0f);
                listbox.ContextMenu.Items.Add(ConvertIntoList);
                listbox.ContextMenu.Items.Add(PromoteToDefault);
            }
        }

        void PromoteToDefault_Click(object sender, EventArgs e)
        {
            Node n = (Node)lstMain.Items.ElementAt(lstMain.ItemIndex);
            Node newdefault = n.CreateClone(n.room);
            Group g = GetActiveGroup();
            if (g == null) return;
            g.defaultNode = newdefault;
            g.fullSet.Remove(n);
            SetDefaultNodeAsEdit();
        }

        void ConvertIntoList_Click(object sender, EventArgs e)
        {
            Node n = (Node)lstMain.Items.ElementAt(lstMain.ItemIndex);
            if (n == room.targetNode)
            {
                room.targetNode = null;
            }

            Node newdefault = n.CreateClone(n.room);
            //Node.cloneNode(n, newdefault);
            newdefault.body.velocity = new Vector2(0, 0);
            Group g = new Group(room, newdefault, room.masterGroup.childGroups["General Groups"]);
            newdefault.name = g.Name;
            //room.masterGroup.childGroups["General Groups"].AddGroup(g.Name, g);
            //room.masterGroup.UpdateComboBox();
            UpdateGroupComboBoxes();

            Group active = GetActiveGroup();
            if (active == null) return;
            //active.fullSet.Remove(n);
            active.DeleteEntity(n);

            //g.fullSet.Add(n);

            int index = cbListPicker.Items.IndexOf(newdefault.name);
            cbListPicker.ItemIndex = index;
            //cmbListPicker.Refresh();
            //cmbListPicker.ItemIndex = 0;


        }

        public void UpdateGroupComboBoxes()
        {
            UpdateGroupComboBox(cbListPicker, "Other Objects");

            UpdateGroupComboBox(cbGroupS);
            UpdateGroupComboBox(cbGroupT);

            //componentView.UpdateGroupComboBox();
        }

        public void UpdateGroupComboBox(ComboBox cb, params string[] additionalItems)
        {
            if (room == null || cb == null) return;
            string tempName = "";
            if (cb.ItemIndex >= 0) tempName = cb.Items.ElementAt(cb.ItemIndex).ToString();
            cb.ItemIndex = 0;
            List<object> list = cb.Items;
            list.ToList().ForEach((o) => list.Remove(o));
            room.masterGroup.GroupNamesToList(list);

            for (int i = 0; i < additionalItems.Length; i++)
            {
                list.Add(additionalItems[i]);
            }

            if (!tempName.Equals("")) cb.ItemIndex = cb.Items.IndexOf(tempName);
        }

        
            
        void cbListPicker_Click(object sender, EventArgs e)
        {
            int t = cbListPicker.ItemIndex;
            cbListPicker.ItemIndex = 0;
            cbListPicker.ItemIndex = t;
        }
        void cbListPicker_ItemIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            if (cmb.ItemIndex < 0) return;
            string item = cmb.Items.ElementAt(cmb.ItemIndex).ToString();
            if (item.Equals("Other Objects"))
            {
                foreach (object o in lstMain.Items.ToList())
                {
                    lstMain.Items.Remove(o);
                }
                lstMain.Items.Add(OrbIt.game);
                lstMain.Items.Add(room);
                lstMain.Items.Add(room.masterGroup);
            }
            else if (!item.Equals(""))
            {
                foreach (object o in lstMain.Items.ToList())
                {
                    lstMain.Items.Remove(o);
                }

                Group find = room.masterGroup.FindGroup(item);
                if (find == null) return;
                lstMain.Items.AddRange(find.fullSet);
                SyncTitleNumber(find);

                SetDefaultNodeAsEdit();

            }
            lstMain.ScrollTo(0);
        }
        
        void NodePresets_Sync(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (((ObservableCollection<Object>)sender).Count() < 1) presetContextMenu.Enabled = false;
            else presetContextMenu.Enabled = true;
            cbPresets.Items.syncToOCDelegate(e);
            lstPresets.Items.syncToOCDelegate(e);
        }

        public void SyncTitleNumber(Group group = null)
        {
            if (group == null)
            {
                group = GetActiveGroup();
                if (group == null) return;
            }

            int count = group == null ? 0 : group.entities.Count;
            title1.Text = group.Name + " : " + count;
        }

        void btnSaveNode_Click(object sender, EventArgs e)
        {
            if (inspectorArea.editNode == null)
                PopUp.Toast("You haven't selected a Node.");
            else
                PopUp.Text("Pick a preset name", "Name preset", delegate(bool c, object input)
                {
                                    if (c) Assets.saveNode(inspectorArea.editNode, (string)input);
                    return true;
                });
        }

        void applyToAllNodesMenuItem_Click(object sender, TomShane.Neoforce.Controls.EventArgs e) //TODO: fix the relection copying reference types
        {
            List<InspectorInfo> itemspath = new List<InspectorInfo>();
            InspectorInfo item = (InspectorInfo)inspectorArea.InsBox.Items.ElementAt(inspectorArea.InsBox.ItemIndex);
            object value = item.GetValue();

            BuildItemsPath(item, itemspath);

            Group activeGroup = GetActiveGroup();
            if (activeGroup == null) return;
            activeGroup.ForEachAllSets(delegate(Node o)
            {
                Node n = (Node)o;
                if (n == itemspath.ElementAt(0).obj) return;
                InspectorInfo temp = new InspectorInfo(null, n, this);
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
                        //else if (next.membertype == member_type.collectionentry)
                        //{
                        //    dynamic coll = temp.obj;
                        //    
                        //}
                        else
                        {
                            if (next.fpinfo.propertyInfo == null)
                            {
                                temp = new InspectorInfo(null, temp, next.fpinfo.GetValue(temp.obj), next.fpinfo.fieldInfo);
                            }
                            else
                            {
                                temp = new InspectorInfo(null, temp, next.fpinfo.GetValue(temp.obj), next.fpinfo.propertyInfo);
                            }
                        }
                    }
                    count++;
                }
            });
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

        void consolePressed(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (sender is Button || (sender is TextBox && (((KeyEventArgs)e).Key == Keys.Enter))) 
                ProcessConsoleCommand(consoletextbox.Text);
        }

        public void ProcessConsoleCommand(String text)
        {
            text = text.Trim();

            if (text.Equals(""))
            {
                PopUp.Toast("No Command Provided");
                consoletextbox.Text = "";
                return;
            }
            object currentObj = game.room;



            List<String> args = text.Split(' ').ToList();
            String methodname;
            if (args.Count > 0)
            {
                methodname = args.ElementAt(0);
                args.RemoveAt(0);
            }
            else
            {
                PopUp.Toast("No Command Provided");
                return;
            }

            MethodInfo methinfo = currentObj.GetType().GetMethod(methodname);

            if (methinfo == null || methinfo.IsPrivate)
            {
                PopUp.Toast("Invalid method specification.");
                return;
            }

            ParameterInfo[] paraminfos = methinfo.GetParameters();

            int paramNum = paraminfos.Length;
            object[] finalargs = new object[paramNum];

            for(int i = 0; i < paramNum; i++)
            {

                Type ptype = paraminfos[i].ParameterType;
                if (i >= args.Count)
                {
                    if (paraminfos[i].IsOptional)
                    {
                        finalargs[i] = Type.Missing;
                        continue;
                    }
                    PopUp.Toast("Parameter Inconsistenc[ies].");
                    return;
                }
                try
                {
                  finalargs[i] = TypeDescriptor.GetConverter(ptype).ConvertFromInvariantString(args[i]);
                }
                catch (Exception e)
                {
                    PopUp.Toast("Casting exception: " + e.Message);
                    throw e;
                }

            }
            if (methinfo.IsStatic) currentObj = null;
            try
            {
                methinfo.Invoke(currentObj, finalargs);
            }
            catch (Exception e)
            {
                PopUp.Toast("Invoking exception: " + e.Message);
                throw e;
            }
        }

        void btnClear_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (e is KeyEventArgs)
            {
                KeyEventArgs ke = (KeyEventArgs)e;
                if (ke.Key == Keys.Enter)
                {
                    TextBox textbox = (TextBox)sender;
                    textbox.Text = "";
                }
            }
            else
            {
                consoletextbox.Text = "";
            }
        }
        
        public void lstMain_ChangeScrollPosition(int change)
        {
            if (lstMainScrollPosition + change < 0) lstMainScrollPosition = 0;
            else if (lstMainScrollPosition + change > lstMain.Items.Count-7) lstMainScrollPosition = lstMain.Items.Count-7;
            else lstMainScrollPosition += change;
            lstMain.ScrollTo(lstMain.Items.Count - 1);
            lstMain.ScrollTo(lstMainScrollPosition);
        }

        void lstMain_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            //remove panelControl elements (from groupPanel at the bottom)
            if (inspectorArea.propertyEditPanel.panelControls.Keys.Count > 0)
            {
                inspectorArea.propertyEditPanel.DisableControls();
            }
            

            if (listbox.ItemIndex >= 0 && listbox.Items.ElementAt(listbox.ItemIndex) is Node)
            {
                SetTargetNode((Node)listbox.Items.ElementAt(listbox.ItemIndex));
            }
            else if (listbox.ItemIndex >= 0)
            {
                //ResetInspectorBox(inspectorArea.InsBox, listbox.Items.ElementAt(listbox.ItemIndex));
                inspectorArea.ResetInspectorBox(listbox.Items.ElementAt(listbox.ItemIndex));
            }

        }

        public void SetTargetNode(Node target)
        {
            room.targetNode = target;
            
            if (inspectorArea.editNode != target)
            {
                inspectorArea.ResetInspectorBox(room.targetNode);
            }
            
            //if (EditSelectedNode)
            //{
            //    componentView.SwitchNode(room.targetNode, false);
            //}
        }

        void lstPresets_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            //remove panelControl elements (from groupPanel at the bottom)
            if (inspectorArea.propertyEditPanel.panelControls.Keys.Count > 0) //DisableControls(groupPanel);
            {
                inspectorArea.propertyEditPanel.DisableControls();
            }
            if (listbox.ItemIndex < 0) return;
            Node newEdit = (Node)listbox.Items.ElementAt(listbox.ItemIndex);
            //lblEditNodeName.Text = ui.editNode.name;
            inspectorArea.lblInspectorAddress.Text = "/" + newEdit.ToString();
            ui.spawnerNode = newEdit;

            //ResetInspectorBox(inspectorArea.InsBox, ui.editNode);
            inspectorArea.ResetInspectorBox(newEdit);

            if (cbPresets.ItemIndex != lstPresets.ItemIndex)
            {
                cbPresets.ItemIndex = lstPresets.ItemIndex;
            }

        }

        void deletePresetMenuItem_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {

            String presetName = ((Node)lstPresets.selected()).name + ".xml";
            string message = "Are you sure you want to delete this preset file? : " + presetName;
            PopUp.Prompt(message, action:
            delegate(bool del, object ans)
            {
                if (del)
                {
                    Assets.deletePreset((Node)lstPresets.selected());
                }
                return true;
            });
        }

        void cmbPresets_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            if (combobox.ItemIndex >= 0)
            {
                lstPresets_ItemIndexChanged(lstPresets, e); //HACKs
            }
        }

        void cbPresets_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {

            ComboBox combobox = (ComboBox)sender;
            if (combobox.ItemIndex != lstPresets.ItemIndex)
            {
                lstPresets.ItemIndex = combobox.ItemIndex;
            }
        }
        
        void btnAddComponent_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (inspectorArea.editNode == null)
                PopUp.Toast("You haven't selected a Node.");
            else
            {
                new AddComponentWindow(this, tbcMain, inspectorArea.editNode, null, false);
                //ObservableCollection<dynamic> nodecomplist = new ObservableCollection<dynamic>((Enum.GetValues(typeof(comp)).Cast<dynamic>().Where(c => !inspectorArea.editNode.HasComp(c))));
                //List<dynamic> missingcomps = new List<dynamic>(Enum.GetValues(typeof(comp)).Cast<dynamic>().Where(c => inspectorArea.editNode.HasComp(c)));
                //
                //PopUp.opt[] options = new PopUp.opt[]{
                //    new PopUp.opt(PopUp.OptType.info, "Add component to: " + inspectorArea.editNode.name),
                //    new PopUp.opt(PopUp.OptType.dropDown, nodecomplist),
                //    new PopUp.opt(PopUp.OptType.checkBox, "Add to all", 
                //        delegate(object s, TomShane.Neoforce.Controls.EventArgs a){
                //            if ((s as CheckBox).Checked) nodecomplist.AddRange(missingcomps);
                //            else nodecomplist.RemoveRange(missingcomps);})};
                //
                //PopUp.makePopup(ui, options, "Add Component", delegate(bool a, object[] o)
                //{
                //    if (a) return addComponent(o);
                //    else return false;
                //});
            }
        }
        void btnDefaultNode_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            SetDefaultNodeAsEdit();
        }

        public void SetDefaultNodeAsEdit()
        {
            Node activeDef = ActiveDefaultNode;
            if (inspectorArea.editNode == activeDef) return;
            //inspectorArea.editNode = ActiveDefaultNode;
            ui.spawnerNode = activeDef;

            inspectorArea.ResetInspectorBox(activeDef);

            //lblEditNodeName.Text = inspectorArea.editNode.name + "(DEFAULT)";
            //inspectorArea.lblInspectorAddress.Text = "/" + activeDef.ToString();
        }
        public void btnRemoveAllNodes_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (sender != null)
                (sender as Button).Focused = false;

            Group g = GetActiveGroup();
            if (g == null) return;
            if (room.targetNode != null)
                if (g.fullSet.Contains(room.targetNode)) room.targetNode = null;
            if (g.fullSet.Contains(inspectorArea.editNode) && inspectorArea.editNode != g.defaultNode)
            {
                inspectorArea.InsBox.Items.Clear();
                inspectorArea.rootitem = null;
                inspectorArea.editNode = null;
            }
            //int size = g.fullSet.Count, count = 0;
            g.EmptyGroup();
            
            lstMain.ItemIndex = -1;
        }

        void btnRemoveNode_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Group g = GetActiveGroup();
            if (g == null) return;
            if (g != null && g.fullSet.Contains(room.targetNode))
                g.DeleteEntity(room.targetNode);
            if (room.targetNode != null)
            {
                //room.targetNode.active = false;
                room.targetNode.IsDeleted = true;
                room.targetNode = null;
            }
            if (inspectorArea.editNode != ActiveDefaultNode && !lstPresets.Items.Contains(inspectorArea.editNode))
            {
                inspectorArea.InsBox.Items.Clear();
                inspectorArea.rootitem = null;
                inspectorArea.editNode = null;
            }
            inspectorArea.propertyEditPanel.DisableControls();
        }

        public void Update()
        {
        }
    }
}
