using System.Drawing;

namespace MarxPongv2.Info
{
    public class MarxPong_Info
    {
        private float highestLeftPercentage, highestRightPercentage;
        private int highestLeftHit, highestLeftMiss, highestRightHit, highestRightMiss;
        private int lMiss, lHit, rMiss, rHit, iteration;
        public MarxPong_Info()
        {
        }

        public void PopulateData(int lMiss, int lHit, int rMiss, int rHit, int iteration)
        {
            this.lMiss = lMiss;
            this.lHit = lHit;
            this.rMiss = rMiss;
            this.rHit = rHit;
            this.iteration = iteration;

            if (lHit > 0 && lMiss < lHit && lMiss == 4)
            {
                if (100.0f - (((float)lMiss / (float)lHit) * 100.0f) > highestLeftPercentage)
                {
                    highestLeftPercentage = 100.0f - (((float)lMiss / (float)lHit) * 100.0f);
                    highestLeftHit = lHit;
                    highestLeftMiss = lMiss;
                }

            }

            if (rHit > 0 && rMiss < rHit && rMiss == 4)
            {
                if (100.0f - (((float)rMiss / (float)rHit) * 100.0f) > highestRightPercentage)
                {
                    highestRightPercentage = 100.0f - (((float)rMiss / (float)rHit) * 100.0f);
                    highestRightHit = rHit;
                    highestRightMiss = rMiss;
                }
            }
        }

        public void DrawInfo(Bitmap _e)
        {
            using (Graphics _g = Graphics.FromImage(_e))
            {
                Font _headFont = new Font("Arial", 30, FontStyle.Bold);
                Font _normFont = new Font("Arial", 20, FontStyle.Bold);

                _g.DrawString("Statistics", _headFont, Brushes.Red, new Point(800 / 4, 2));
                _g.DrawString("Iteration: " + (iteration / 2).ToString(), _normFont, Brushes.Green, new Point(800 / 4, 60));

                _g.DrawString("Left Paddle", _normFont, Brushes.Green, new Point(800 / 4, 120));
                _g.DrawString("Missed: " + lMiss.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 150));
                _g.DrawString("Hit: " + lHit.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 180));
                _g.DrawString("Highest Accuracy: " + highestLeftPercentage.ToString() + "%, Hit: " + highestLeftHit.ToString() + ", Miss: " + highestLeftMiss.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 210));

                _g.DrawString("Right Paddle", _normFont, Brushes.Green, new Point(800 / 4, 270));
                _g.DrawString("Missed: " + rMiss.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 300));
                _g.DrawString("Hit: " + rHit.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 330));
                _g.DrawString("Highest Accuracy: " + highestRightPercentage.ToString() + "%, Hit: " + highestRightHit.ToString() + ", Miss: " + highestRightMiss.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 360));
            }
        }
    }
}
