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
    public class GroupsView : DetailedView
    {
        public Label lblGroupLabel;
        public Button btnCreateGroup, btnEmptyGroup, btnEmptyAll;
        public Group parentGroup;
        public CreateGroupWindow createGroupWindow;
        public EditNodeWindow editGroupWindow;
        public GroupsView(Sidebar sidebar, Control parent, int Left, int Top, Group parentGroup)
            : base(sidebar, parent, Left, Top, false)
        {
            Height += 50;
            this.parentGroup = parentGroup;
            HeightCounter = Top + 45;
            lblGroupLabel = new Label(manager);
            lblGroupLabel.Init();
            lblGroupLabel.Parent = parent;
            lblGroupLabel.Left = LeftPadding;
            lblGroupLabel.Top = 10;// HeightCounter;
            lblGroupLabel.Width = sidebar.Width / 2;
            lblGroupLabel.Text = "";
            lblGroupLabel.TextColor = Color.Black;
            //HeightCounter += lblGroupLabel.Height + LeftPadding;

            ItemCreator += ItemCreatorDelegate;

            Initialize();

            btnCreateGroup = new Button(manager);
            btnCreateGroup.Init();
            btnCreateGroup.Parent = parent;
            btnCreateGroup.Top = HeightCounter;
            btnCreateGroup.Text = "Create Group";
            btnCreateGroup.Height = (int)(btnCreateGroup.Height * 1.5);
            btnCreateGroup.Width = (parent.Width - LeftPadding * 2) / 2;
            btnCreateGroup.Left = LeftPadding;//parent.Width / 2 - btnCreateGroup.Width / 2;
            btnCreateGroup.Click += (s, e) =>
            {
                createGroupWindow = new CreateGroupWindow(sidebar);
            };
            Margins m = btnCreateGroup.ClientMargins;
            btnCreateGroup.ClientMargins = new Margins(m.Left, 1, m.Right, 1);

            btnEmptyGroup = new Button(manager);
            btnEmptyGroup.Init();
            btnEmptyGroup.Parent = parent;
            btnEmptyGroup.Top = HeightCounter;
            btnEmptyGroup.Text = "Empty Group";
            btnEmptyGroup.Height = (int)(btnEmptyGroup.Height * 1.5);
            btnEmptyGroup.Width = btnCreateGroup.Width;
            btnEmptyGroup.Left = parent.Width - LeftPadding * 2 - btnEmptyGroup.Width;//parent.Width / 2 - btnCreateGroup.Width / 2;
            btnEmptyGroup.Click += (s, e) =>
            {
                sidebar.btnRemoveAllNodes_Click(null, null);
            };
            Margins m2 = btnEmptyGroup.ClientMargins;
            btnEmptyGroup.ClientMargins = new Margins(m2.Left, 1, m2.Right, 1);

            HeightCounter += btnCreateGroup.Height + LeftPadding;

            btnEmptyAll = new Button(manager);
            btnEmptyAll.Init();
            btnEmptyAll.Parent = parent;
            btnEmptyAll.Top = HeightCounter;
            btnEmptyAll.Text = "Empty All Groups";
            //btnEmptyAll.Height = (int)(btnEmptyGroup.Height * 1.5);
            btnEmptyAll.Width = parent.Width / 2;
            btnEmptyAll.Left = parent.Width / 2 - btnEmptyAll.Width / 2;//parent.Width / 2 - btnCreateGroup.Width / 2;
            btnEmptyAll.Click += (s, e) =>
            {
                EmptyAllGroups();
            };
        }

        public void EmptyAllGroups()
        {
            foreach(Group g in room.masterGroup.childGroups.Values)
            {
                if (g == room.groups.player) continue;
                g.EmptyGroup();
            }
        }
        public void UpdateGroups()
        {
            if (parentGroup == null) return;
            Group currentlySelected = null;
            if (selectedItem != null && selectedItem.obj is Group)
            {
                currentlySelected = (Group)selectedItem.obj;
            }

            ClearView();
            showRemoveButton = parentGroup.childGroups.Count > 1;
            foreach (Group g in parentGroup.childGroups.Values)
            {
                CreateNewItem(g);
            }
            if (viewItems.Count > 0)
            {
                foreach(DetailedItem item in viewItems)
                {
                    if (item.obj == currentlySelected)
                    {
                        SelectItem(item);
                        return;
                    }
                }
                SelectItem(viewItems.ElementAt(0));
            }
        }
        private bool showRemoveButton = false;
        public void CreateNewItem(Group g)
        {
            int top = 0;
            if (viewItems.Count > 0)
            {
                top = (viewItems[0].itemHeight - 4) * viewItems.Count;
            }
            DetailedItem detailedItem = new DetailedItem(manager, this, g, backPanel, top, LeftPadding);
            base.CreateItem(detailedItem);
        }

        private void ItemCreatorDelegate(DetailedItem item, object obj)
        {
            if (item.obj is Group)
            {
                Group g = (Group)item.obj;
                item.label.Text = g.Name;
                Button btnEdit = new Button(manager);
                btnEdit.Init();
                btnEdit.Parent = item.panel;
                btnEdit.Width = 30;
                btnEdit.Left = item.panel.Width - btnEdit.Width - 10;
                btnEdit.Top = 2;
                btnEdit.Height = item.buttonHeight;

                EventHandler editgroup = (s, e) =>
                {
                    item.isSelected = true;
                    if (parentGroup == room.groups.items)
                    {
                        editGroupWindow = new EditNodeWindow(sidebar, "Item Group", g.Name, ViewType.Group);
                    }
                    else
                    {
                        editGroupWindow = new EditNodeWindow(sidebar, g);
                    }
                    editGroupWindow.componentView.SwitchGroup(g);
                    
                };
                
                btnEdit.Text = "Edit";
                btnEdit.ToolTip.Text = "Edit";
                btnEdit.TextColor = UserInterface.TomShanePuke;

                btnEdit.Click += editgroup;

                item.panel.DoubleClick += editgroup;
                

                Button btnEnabled = new Button(manager);
                btnEnabled.Init();
                btnEnabled.Parent = item.panel;
                btnEnabled.Width = 30;
                btnEnabled.Left = btnEdit.Left - btnEnabled.Width - 5;
                btnEnabled.Top = 2;
                btnEnabled.Height = item.buttonHeight;
                //btnEnabled.Draw += btnEnabled_Draw;

                //btnEnabled.Text = "On";
                SetButtonBool(btnEnabled, !g.Disabled);
                btnEnabled.ToolTip.Text = "Group Enabled";
                btnEnabled.TextColor = UserInterface.TomShanePuke;
                btnEnabled.Click += (s, e) =>
                {
                    g.Disabled = !GetButtonBool(btnEnabled);
                    SetButtonBool(btnEnabled, !g.Disabled);
                };

                if (!showRemoveButton) return;

                Button btnRemove = new Button(manager);
                btnRemove.Init();
                btnRemove.Parent = item.panel;
                btnRemove.Width = item.buttonWidth;
                btnRemove.Top = 2;
                btnRemove.Left = btnEnabled.Left - btnRemove.Width - 5;
                btnRemove.Height = item.buttonHeight;
                
                btnRemove.TextColor = Color.Red;
                btnRemove.Text = "-";
                btnRemove.ToolTip.Text = "Remove";
                btnRemove.Click += (s, e) =>
                {
                    g.EmptyGroup();
                    g.DeleteGroup();
                    UpdateGroups();
                };
            }
        }

        void btnEnabled_Draw(object sender, DrawEventArgs e)
        {
            //Button b = (Button)sender;
            //new Rectangle(500, 500, 600, 600)
            e.Renderer.Draw(Assets.textureDict[textures.blackorb], e.Rectangle, new Rectangle(0,0,25,25), Color.White);
        }
    }
}
