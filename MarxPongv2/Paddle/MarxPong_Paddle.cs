using System;
using System.Windows.Forms;
using System.Drawing;

namespace MarxPongv2.Paddle
{
    class MarxPong_Paddle
    {
        private int _pictureBoxHeight, _leftPaddleY, _rightPaddleY, _leftPaddleX, _rightPaddleX;

        #region Constructor and Deconstructor

        public MarxPong_Paddle()
        {
        }

        ~MarxPong_Paddle()
        {
        }

        #endregion

        #region Properties

        public int Height { get { return _pictureBoxHeight; } set { _pictureBoxHeight = value; } }
        public int LeftPaddleY { get { return _leftPaddleY; } set { _leftPaddleY = value; } }
        public int RightPaddleY { get { return _rightPaddleY; } set { _rightPaddleY = value; } }
        public int LeftPaddleX { get { return _leftPaddleX; } set { _leftPaddleX = value; } }
        public int RightPaddleX { get { return _rightPaddleX; } set { _rightPaddleX = value; } }

        #endregion

        #region Draw the paddles

        public void DrawPaddles(Bitmap _e)
        {
            Graphics _g = Graphics.FromImage(_e);

            Brush brush = new SolidBrush(Color.White);
            _g.FillRectangle(brush, new Rectangle(_leftPaddleX, _leftPaddleY, 10, _pictureBoxHeight / 8));

            Brush brush2 = new SolidBrush(Color.White);
            _g.FillRectangle(brush2, new Rectangle(_rightPaddleX, _rightPaddleY, 10, _pictureBoxHeight / 8));
        }

        #endregion
    }
}
