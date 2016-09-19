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
        MarxPongModel _mpm;
        MarxPong_Engine _mpe;
        Thread _met;
        bool _singleThread;
        int _frameCount;

        public Form1()
        {
            InitializeComponent();
            _mpm = new MarxPongModel(1024, 768);
            _mpe = new MarxPong_Engine(_mpm);
            _met = new Thread(_mpe.StartEngine);
            _singleThread = false;
            timer1.Enabled = false;
            timer1.Interval = 5000;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_singleThread)
                _mpe.RunSTEngine();

            //if (_frameCount > 20)
            //{
                _mpm.render();
                pictureBox1.Invalidate();
                pictureBox1.BackgroundImage = _mpm.FinalScreen;
                

           //     if (_frameCount > 21)
           //         _frameCount = 0;
           // }

           // _frameCount++;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _met.Abort();
            Application.Exit();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_singleThread)
                _met.Start();
            
            
            timer1.Enabled = true;
        }
    }
}
