using System;
using System.Threading;
using System.Windows.Forms;

namespace RiftGame
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            var gameThread = new Thread(new ThreadStart(Run))
            {
                Name = "Game Thread",
                IsBackground = true,
            };
            gameThread.SetApartmentState(ApartmentState.STA);
            gameThread.Start();
        }

        public void Run()
        {
            using (var program = new RiftGame())
            {
                program.Run();
            }
        }
    }
}
