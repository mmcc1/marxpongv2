using System.Drawing;

namespace MarxPongv2.Background
{
    public class MarxPong_Background
    {
        private int backgroundHeight, backgroundWidth;

        #region Properties

        public int Height { get { return backgroundHeight; } set { backgroundHeight = value; } }
        public int Width { get { return backgroundWidth; } set { backgroundWidth = value; } }

        #endregion

        #region Draw the background

        public void DrawBackground(Bitmap e)
        {
            int centre = backgroundWidth / 2;
            int _index = 0;
            Brush _brush = new SolidBrush(Color.Gray);

            using (Graphics g = Graphics.FromImage(e))
            {
                g.Clear(Color.Black);

                while (_index < backgroundHeight)
                {
                    g.FillRectangle(_brush, new Rectangle(centre, _index, 6, backgroundHeight / 32));
                    _index = _index + ((backgroundHeight / 32) + 10);
                }
            }
        }

        #endregion
    }
}
