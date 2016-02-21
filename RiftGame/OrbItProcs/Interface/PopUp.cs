using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;
using SharpDX;
using SharpOVR;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.DXGI;
using SharpDX.Toolkit.Content;
using TomShane.Neoforce.Controls;
using System.Reflection;


using System.IO;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using EventArgs = System.EventArgs;

namespace OrbItProcs
{

    public static class PopUp
    {
        public enum OptType { info, prompt, textBox, dropDown, radialButton, checkBox};
        public class opt
        {
            private static Dictionary<OptType, Type> tc = new Dictionary<OptType, Type>(){
                {OptType.info,         typeof(String)},
                {OptType.prompt,       typeof(String)},
                {OptType.textBox,      typeof(String)},
                {OptType.dropDown,     typeof(ObservableCollection<object>)},
                {OptType.radialButton, typeof(String[])},
                {OptType.checkBox,     typeof(String)},
            };

            public OptType type;
            public object content;
            public TomShane.Neoforce.Controls.EventHandler action = delegate { };

            public opt(OptType t, object c, TomShane.Neoforce.Controls.EventHandler e = null)
            {
                if (c.GetType() != tc[t]) throw new System.ArgumentException("Content does not match option type");
                type = t; content = c; action = e ?? action;
            }

        }
        public delegate bool ConfirmDelegate(bool confirm, object[] Answers);
        private const int MAX_CHARS_PER_LINE = 25;
        private static int VertPadding = 10;

        public static void makePopup(UserInterface ui, opt[] options, String title = "Hey! Listen!", ConfirmDelegate action = null)
        {
            Manager manager = ui.game.Manager;
            bool confirmed = false;
            object[] answer = new object[options.Length];
            
            bool[] answered = new bool[options.Length];
            ConfirmDelegate emptyDelegate = delegate{ return true;};
            action = action ?? emptyDelegate;
            Dialog window = new Dialog(manager);
            window.Text = title;
            window.Init();
            window.ShowModal();
            window.Caption.Text = "";
            window.Description.Text = "";
            window.Width = 200;
            window.Height = 200;
            window.SetPosition(20, OrbIt.ScreenHeight / 4);
            int heightCounter = window.Caption.Top;
            int i = 0;

            Button btnOk = new Button(manager);
            btnOk.Top = window.Description.Top + window.Description.Height;
            btnOk.Anchor = Anchors.Top;
            btnOk.Parent = window;
            btnOk.Text = "Ok";
            btnOk.Left = btnOk.Left = window.Width / 2 - btnOk.Width / 2;
            btnOk.Init();

            foreach (opt opt in options)
            {
                if (opt.type.In(OptType.info,OptType.prompt))
                {
                    Label info = new Label(manager);
                    info.Init();
                    info.Parent = window;
                    info.Left = VertPadding;
                    info.Width = window.Width - VertPadding * 5;
                    string message = ((string)opt.content).wordWrap(MAX_CHARS_PER_LINE);
                    info.Text = message;
                    info.Height = (info.Text.Count(x => x == '\n')+1) * info.Height;
                    info.Top = heightCounter; heightCounter += info.Height + VertPadding;
                }
                if (opt.type == OptType.dropDown)
                {
                    ComboBox cbBox = new ComboBox(manager);
                    cbBox.Init();
                    cbBox.Parent = window;
                    cbBox.MaxItems = 20; // TODO : ERROR?
                    cbBox.Width = window.Width - VertPadding * 5;
                    cbBox.Left = VertPadding;
                    cbBox.Top = heightCounter; heightCounter += cbBox.Height + VertPadding;
                    ObservableCollection<object> q = (ObservableCollection<object>)opt.content;
                    foreach (object o in q) cbBox.Items.Add(o);
                    q.CollectionChanged += delegate(object s, NotifyCollectionChangedEventArgs e) { cbBox.Items.syncToOCDelegate(e); };
                    int qq = i; answer[qq] = null;
                    cbBox.ItemIndexChanged += delegate {  answer[qq] = (cbBox.Items.ElementAt(cbBox.ItemIndex)); };
                    cbBox.ItemIndexChanged += opt.action;
                }
                if (opt.type == OptType.textBox)
                {
                    TextBox tbName = new TextBox(manager);
                    tbName.Init();
                    tbName.Parent = window;
                    tbName.Width = window.Width - VertPadding * 5;
                    tbName.Left = VertPadding;
                    tbName.Top = heightCounter; heightCounter += tbName.Height + VertPadding;
                    int qq = i; answer[qq] = null;
                    tbName.TextChanged += delegate { answer[qq] = tbName.Text; };
                    tbName.KeyUp += delegate(object sender, KeyEventArgs e)
                    {
                        if (e.Key == Keys.Enter)
                        { confirmed = true; if (action(true, answer)) window.Close(); }
                    };
                    tbName.TextChanged += opt.action;
                }

                if (opt.type == OptType.radialButton)
                {
                    GroupPanel gp = new GroupPanel(manager);
                    gp.Init();
                    gp.Parent = window;
                    gp.Width = window.Width - VertPadding * 5;
                    gp.Left = VertPadding;
                    int qq = i; answer[qq] = -1;
                    for (int j = 0; j < ((string[])opt.content).Length; j++)
                    {
                        RadioButton rb = new RadioButton(manager);
                        rb.Init();
                        rb.Parent = gp;
                        rb.Text = (string)opt.content;
                        int jj = j;
                        rb.Click += delegate { answer[qq] = jj; };
                        rb.Click += opt.action;
                    }
                    gp.Top = heightCounter; heightCounter += gp.Height + VertPadding;
                }
                if (opt.type == OptType.checkBox)
                {
                    CheckBox cb = new CheckBox(manager);
                    cb.Init();
                    cb.Parent = window;
                    cb.Width = window.Width - VertPadding * 5;
                    cb.Left = VertPadding;
                    cb.Top = heightCounter; heightCounter += cb.Height + VertPadding;
                    int qq = i; answer[qq] = false;
                    cb.Text = (string)opt.content;
                    cb.Click += delegate { answer[qq] = cb.Checked; };
                    cb.Click += opt.action;
                }
                i++;
            }
            if (options.Any<opt>(x => x.type != OptType.info))
            {
                btnOk.Left = VertPadding;
                Button btnCancel = new Button(manager);
                btnCancel.Init();
                btnCancel.Parent = window;
                btnCancel.Top = heightCounter;
                btnCancel.Text = "Cancel";
                btnCancel.Left = VertPadding * 2 + btnOk.Width;
                btnCancel.Click += delegate { window.Close(); action(false, answer); };
            }

            btnOk.Click += delegate {
                confirmed = true;
                window.Close(ModalResult.Cancel);
                bool res = action(true, answer);
                if (!res)
                {
                    window.Show();
                };
            };
            btnOk.Top = heightCounter;
            window.Closing += delegate { if (confirmed == false) action(false, answer); };
            window.Height = (btnOk.Top) + 70;
            manager.Add(window);
        }
        public delegate bool singleConfirmDelegate(bool confirm, object answer);

        public static void Toast(string message = "", string title = "Hey! Listen!")
        { makePopup(OrbIt.ui, new opt[]{new opt(OptType.info,message)}, title); }
        public static void Prompt(string message = "", string title = "Hey! Listen!", singleConfirmDelegate action = null)
        { makePopup(OrbIt.ui, new opt[] { new opt(OptType.prompt, message) }, title, delegate(bool c, object[] a) { return action(c, a[0]); }); }
        public static void Select(string message = "", string title = "Hey! Listen!", singleConfirmDelegate action = null, ObservableCollection<object> list = null)
        { makePopup(OrbIt.ui, new opt[] { new opt(OptType.info, message), new opt(OptType.dropDown, list) }, title, delegate(bool c, object[] a) { return action(c, a[1]); }); }
        public static void Text(string message = "", string title = "Hey! Listen!", singleConfirmDelegate action = null, string content = "")
        { makePopup(OrbIt.ui, new opt[] { new opt(OptType.info, message), new opt(OptType.textBox, content) }, title, delegate(bool c, object[] a) { return action(c, a[1]); }); }

        public static void Complex(UserInterface ui)
        {
            makePopup(ui, new opt[] {
            new opt(OptType.info, "Shit son"),
            new opt(OptType.textBox, "Write here")
        }, "THIS IS SO COMPLEX",
                delegate
                {
                    return true;
                
                
                });
        }

    }

}
