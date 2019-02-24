using System.Drawing;

namespace MarxPongv2.Paddle
{
    public class MarxPong_Paddle
    {
        private int pictureBoxHeight, leftPaddleY, rightPaddleY, leftPaddleX, rightPaddleX;

        #region Properties

        public int Height { get { return pictureBoxHeight; } set { pictureBoxHeight = value; } }
        public int LeftPaddleY { get { return leftPaddleY; } set { leftPaddleY = value; } }
        public int RightPaddleY { get { return rightPaddleY; } set { rightPaddleY = value; } }
        public int LeftPaddleX { get { return leftPaddleX; } set { leftPaddleX = value; } }
        public int RightPaddleX { get { return rightPaddleX; } set { rightPaddleX = value; } }

        #endregion

        #region Draw the paddles

        public void DrawPaddles(Bitmap e)
        {
            using (Graphics g = Graphics.FromImage(e))
            {
                Brush brush = new SolidBrush(Color.White);
                g.FillRectangle(brush, new Rectangle(leftPaddleX, leftPaddleY, 10, pictureBoxHeight / 8));
                g.FillRectangle(brush, new Rectangle(rightPaddleX, rightPaddleY, 10, pictureBoxHeight / 8));
            }
        }

        #endregion
    }
}
