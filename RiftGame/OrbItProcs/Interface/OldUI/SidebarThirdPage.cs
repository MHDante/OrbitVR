using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;
using Color = SharpDX.Color;
using SharpDX;
using SharpOVR;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.DXGI;
using SharpDX.Toolkit.Content;

namespace OrbItProcs
{
    public partial class Sidebar
    {
        public StackView stackview2;
        public Panel backPanel2;
        CollapsePanel ConsolePanel;
        CollapsePanel PresetsPanel;
        CollapsePanel CheckBoxes, c14, c15, c16, c17, c18;

        ComboBox cbUserLevel;

        public int HeightCounter4;
        public int VertPadding4;

        public void InitializeThirdPage()
        {
            stackview2 = new StackView();
            tbcMain.AddPage();
            TabPage third = tbcMain.TabPages[2];
            third.Text = "Other";

            backPanel2 = new Panel(manager);
            backPanel2.Height = third.Height;
            backPanel2.Width = third.Width;
            //backPanel2.Width = second.Width + 20;
            backPanel2.AutoScroll = true;
            backPanel2.Init();
            third.Add(backPanel2);

            HeightCounter4 = 0;
            VertPadding4 = 0;

            ConsolePanel = new CollapsePanel(manager, backPanel2, "Console"); stackview2.AddPanel(ConsolePanel);
            PresetsPanel = new CollapsePanel(manager, backPanel2, "Presets"); stackview2.AddPanel(PresetsPanel);
            CheckBoxes = new CollapsePanel(manager, backPanel2, "CheckBoxes", extended: false); stackview2.AddPanel(CheckBoxes);
            //c14 = new CollapsePanel(manager, backPanel2, "fourth", extended: false); stackview2.AddPanel(c14);
            //c15 = new CollapsePanel(manager, backPanel2, "fifth", extended: false); stackview2.AddPanel(c15);
            //c16 = new CollapsePanel(manager, backPanel2, "sixth", extended: false); stackview2.AddPanel(c16);
            //c17 = new CollapsePanel(manager, backPanel2, "seventh", extended: false); stackview2.AddPanel(c17);
            //c18 = new CollapsePanel(manager, backPanel2, "eighth", extended: false); stackview2.AddPanel(c18);

            backPanel2.Color = UserInterface.TomDark;

            tbcMain.SelectedPage = tbcMain.TabPages[2];
            #region  /// Page 3 ///
            GroupPanel parent;
            #region /// Console ///
            parent = ConsolePanel.panel;

            #region  /// Console textbox ///
            consoletextbox = new TextBox(manager);
            consoletextbox.Init();
            consoletextbox.Parent = parent;

            consoletextbox.Left = LeftPadding;
            consoletextbox.Top = HeightCounter2;
            HeightCounter2 += VertPadding + consoletextbox.Height;
            consoletextbox.Width = parent.Width - LeftPadding * 2;
            consoletextbox.Height = consoletextbox.Height + 3;

            consoletextbox.ToolTip.Text = "Enter a command, and push enter";
            consoletextbox.KeyUp += consolePressed;
            #endregion

            #region  /// Enter Button ///
            Button btnEnter = new Button(manager);
            btnEnter.Init();
            btnEnter.Parent = parent;

            btnEnter.Left = LeftPadding;
            btnEnter.Top = HeightCounter2;
            btnEnter.Width = (parent.Width - LeftPadding * 2) / 2;

            btnEnter.Text = "Enter";
            btnEnter.Click += consolePressed;
            #endregion

            #region  /// Clear ///
            Button btnClear = new Button(manager);
            btnClear.Init();
            btnClear.Parent = parent;

            btnClear.Left = LeftPadding + btnEnter.Width;
            btnClear.Top = HeightCounter2; HeightCounter2 += VertPadding + btnClear.Height;
            btnClear.Width = (parent.Width - LeftPadding * 2) / 2;

            btnClear.Text = "Clear";
            btnClear.Click += btnClear_Click;
            #endregion
            #endregion
            ConsolePanel.Collapse();

            #region /// Presets ///
            parent = PresetsPanel.panel;
            PresetsPanel.ExpandedHeight = 175;
            HeightCounter4 = VertPadding4;
            
            lstPresets = new ListBox(manager);
            lstPresets.Init();
            lstPresets.Parent = parent;
            lstPresets.Top = HeightCounter4;
            lstPresets.Left = LeftPadding;
            lstPresets.Width = parent.Width - LeftPadding * 2;
            lstPresets.Height = third.Height / 4; HeightCounter += VertPadding + lstPresets.Height;
            lstPresets.Anchor = Anchors.Top | Anchors.Left | Anchors.Bottom;
            lstPresets.HideSelection = false;
            lstPresets.ItemIndexChanged += lstPresets_ItemIndexChanged;

            // go to cmbPresets to find the preset synching reference.

            #region /// Presets ContextMenu ///
            presetContextMenu = new ContextMenu(manager);
            deletePresetMenuItem = new MenuItem("Delete Preset");
            deletePresetMenuItem.Click += deletePresetMenuItem_Click;
            presetContextMenu.Items.Add(deletePresetMenuItem);
            presetContextMenu.Enabled = false;
            #endregion
            lstPresets.ContextMenu = presetContextMenu;

            CheckBoxes.ExpandedHeight = 150;

            HeightCounter4 = 0;

            cbUserLevel = new ComboBox(manager);
            cbUserLevel.Init();
            cbUserLevel.Parent = CheckBoxes.panel;
            cbUserLevel.Top = HeightCounter4;
            cbUserLevel.Width = 150;
            HeightCounter4 += cbUserLevel.Height;
            foreach(string ul in Enum.GetNames(typeof(UserLevel)))
            {
                cbUserLevel.Items.Add(ul);
            }
            cbUserLevel.ItemIndexChanged += (s, e) =>
            {
                userLevel = (UserLevel)cbUserLevel.ItemIndex;
                
            };
            int count = 0;
            foreach(object s in cbUserLevel.Items)
            {
                if (s.ToString().Equals(userLevel.ToString()))
                {
                    cbUserLevel.ItemIndex = count;
                }
                count++;
            }
            

            #endregion
            

            Dictionary<string, EventHandler> checkBoxHandlers = new Dictionary<string, EventHandler>(){
                { "FullScreen", (o,e) => {
                    if ((o as CheckBox).Checked) game.setResolution(resolutions.AutoFullScreen, true);
                    else game.setResolution(resolutions.WSXGA_1680x1050, false);
                } },
                { "Hide Links", (o,e) => {
                    game.room.DrawLinks = !(o as CheckBox).Checked;
                } },
            };

            foreach (string key in checkBoxHandlers.Keys)
            {
                CreateCheckBox(key, checkBoxHandlers[key]);
            }

            tbcMain.SelectedPage = tbcMain.TabPages[0];
            #endregion
        }

        public void CreateCheckBox(string key, EventHandler ev)
        {
            CheckBox cb = new CheckBox(manager);
            cb.Init();
            cb.Parent = CheckBoxes.panel;
            cb.Text = key;
            cb.Top = HeightCounter4;
            cb.Width = 100;
            cb.Click += ev;
            HeightCounter4 += cb.Height;
        }

    }
}
