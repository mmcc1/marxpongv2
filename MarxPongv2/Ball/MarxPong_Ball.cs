using System.Drawing;

namespace MarxPongv2.Ball
{
    public class MarxPong_Ball
    {
        private int ballY, ballX, ballXVelocity, ballYVelocity;

        #region Properties

        public int BallY { get { return ballY; } set { ballY = value; } }
        public int BallX { get { return ballX; } set { ballX = value; } }
        public int BallYVelocity { get { return ballYVelocity; } set { ballYVelocity = value; } }
        public int BallXVelocity { get { return ballXVelocity; } set { ballXVelocity = value; } }

        #endregion

        #region Draw the ball

        public void DrawBall(Bitmap e)
        {
            using (Graphics g = Graphics.FromImage(e))
            {
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(ballX, ballY, 10, 10));
            }
        }

        #endregion
    }
}
