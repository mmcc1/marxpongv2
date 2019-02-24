using System;
using System.Drawing;
using System.Windows.Forms;
using MarxPongv2.Model;
using MarxPongv2.Engine;
using System.Threading;

namespace MarxPongv2
{
    public partial class Form1 : Form
    {
        private MarxPongModel marxPongModel;
        private MarxPong_Engine marxPongEngine;
        private Thread thread;
        private bool runRealTime;

        public Form1()
        {
            InitializeComponent();

            marxPongModel = new MarxPongModel(1024, 768);
            marxPongEngine = new MarxPong_Engine(marxPongModel);
            thread = new Thread(marxPongEngine.StartEngine);
            runRealTime = false;
            timer1.Enabled = false;
            timer1.Interval = 30;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (runRealTime)
                marxPongEngine.RunSTEngine();

            marxPongModel.render();
            pictureBox1.Invalidate();
            pictureBox1.BackgroundImage = marxPongModel.FinalScreen;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            thread.Abort();
            Application.Exit();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!runRealTime)
                thread.Start();
            
            
            timer1.Enabled = true;
        }
    }
}
