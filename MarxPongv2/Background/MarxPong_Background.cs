using System;
using System.Windows.Forms;
using System.Drawing;

namespace MarxPongv2.Background
{
    class MarxPong_Background
    {
        private int _backgroundHeight, _backgroundWidth;

        #region Constructer and Destructor

        public MarxPong_Background()
        {
        }

        ~MarxPong_Background()
        {
        }

        #endregion

        #region Properties

        public int Height { get { return _backgroundHeight; } set { _backgroundHeight = value; } }
        public int Width { get { return _backgroundWidth; } set { _backgroundWidth = value; } }

        #endregion

        #region Draw the background

        public void DrawBackground(Bitmap _e)
        {
            int _centre = _backgroundWidth / 2;
            int _index = 0;

            Brush _brush = new SolidBrush(Color.Gray);
            Graphics _g = Graphics.FromImage(_e);
            _g.Clear(Color.Black);

            while (_index < _backgroundHeight)
            { 
                _g.FillRectangle(_brush, new Rectangle(_centre, _index, 6, _backgroundHeight / 32));
                _index = _index + ((_backgroundHeight /32) + 10);
            }
        }

        #endregion
    }
}
