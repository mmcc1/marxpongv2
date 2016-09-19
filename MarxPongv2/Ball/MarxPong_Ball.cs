using System;
using System.Windows.Forms;
using System.Drawing;

namespace MarxPongv2.Ball
{
    class MarxPong_Ball
    {
        private int _ballY, _ballX, _ballXVelocity, _ballYVelocity;

        #region Constructor and Deconstructor

        public MarxPong_Ball()
        {
        }

        ~MarxPong_Ball()
        {
        }

        #endregion

        #region Properties

        public int BallY { get { return _ballY; } set { _ballY = value; } }
        public int BallX { get { return _ballX; } set { _ballX = value; } }
        public int BallYVelocity { get { return _ballYVelocity; } set { _ballYVelocity = value; } }
        public int BallXVelocity { get { return _ballXVelocity; } set { _ballXVelocity = value; } }

        #endregion

        #region Draw the ball

        public void DrawBall(Bitmap _e)
        {
            Graphics _g = Graphics.FromImage(_e);

            Brush _brush = new SolidBrush(Color.White);
            _g.FillRectangle(_brush, new Rectangle(_ballX, _ballY, 10, 10));
        }

        #endregion
    }
}
