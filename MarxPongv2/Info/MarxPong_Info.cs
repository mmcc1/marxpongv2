using System;
using System.Drawing;

namespace MarxPongv2.Info
{
    class MarxPong_Info
    {
        private float _highestLeftPercentage, _highestRightPercentage;
        private int _highestLeftHit, _highestLeftMiss, _highestRightHit, _highestRightMiss;
        private int _lMiss, _lHit, _rMiss, _rHit, _iteration;
        public MarxPong_Info()
        {
        }

        public void PopulateData(int _lMiss, int _lHit, int _rMiss, int _rHit, int _iteration)
        {
            this._lMiss = _lMiss;
            this._lHit = _lHit;
            this._rMiss = _rMiss;
            this._rHit = _rHit;
            this._iteration = _iteration;

            if (_lHit > 0 && _lMiss < _lHit && _lMiss == 4)
            {
                if (100.0f - (((float)_lMiss / (float)_lHit) * 100.0f) > _highestLeftPercentage)
                {
                    _highestLeftPercentage = 100.0f - (((float)_lMiss / (float)_lHit) * 100.0f);
                    _highestLeftHit = _lHit;
                    _highestLeftMiss = _lMiss;
                }

            }

            if (_rHit > 0 && _rMiss < _rHit && _rMiss == 4)
            {
                if (100.0f - (((float)_rMiss / (float)_rHit) * 100.0f) > _highestRightPercentage)
                {
                    _highestRightPercentage = 100.0f - (((float)_rMiss / (float)_rHit) * 100.0f);
                    _highestRightHit = _rHit;
                    _highestRightMiss = _rMiss;
                }
            }
        }

        public void DrawInfo(Bitmap _e)
        {
            

            Graphics _g = Graphics.FromImage(_e);

            Font _headFont = new Font("Arial", 30, FontStyle.Bold);
            Font _normFont = new Font("Arial", 20, FontStyle.Bold);

            _g.DrawString("Statistics", _headFont, Brushes.Red, new Point(800 / 4, 2));
            _g.DrawString("Iteration: " + (_iteration / 2).ToString(), _normFont, Brushes.Green, new Point(800 / 4, 60));
            
            _g.DrawString("Left Paddle", _normFont, Brushes.Green, new Point(800 / 4, 120));
            _g.DrawString("Missed: " + _lMiss.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 150));
            _g.DrawString("Hit: " + _lHit.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 180));
            _g.DrawString("Highest Accuracy: " + _highestLeftPercentage.ToString() + "%, Hit: " + _highestLeftHit.ToString() + ", Miss: " + _highestLeftMiss.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 210));

            _g.DrawString("Right Paddle", _normFont, Brushes.Green, new Point(800 / 4, 270));
            _g.DrawString("Missed: " + _rMiss.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 300));
            _g.DrawString("Hit: " + _rHit.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 330));
            _g.DrawString("Highest Accuracy: " + _highestRightPercentage.ToString() + "%, Hit: " + _highestRightHit.ToString() + ", Miss: " + _highestRightMiss.ToString(), _normFont, Brushes.Green, new Point(800 / 4, 360));
        }
    }
}
