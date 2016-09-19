using System;
using System.Drawing;
using MarxPongv2.Background;
using MarxPongv2.Ball;
using MarxPongv2.Paddle;
using MarxPongv2.AI;
using MarxPongv2.Info;

namespace MarxPongv2.Model
{
    class MarxPongModel
    {
        private Bitmap _finalScreen;
        private MarxPong_Background _mpb;
        private MarxPong_Ball _mpba;
        private MarxPong_Paddle _mpp;
        private MarxPong_Info _mpi;
        private bool _togglePlayer;
        MarxPong_AI _mpaiPlayer1;
        MarxPong_AI _mpaiPlayer2;
        private int _player1Score, _player2Score;
        private int _leftPaddleNumberHits, _leftPaddleCurrentMaxNumberHits, _rightPaddleNumberHits, _rightPaddleCurrentMaxNumberHits;  //Track successful hits of the ball by the paddle
        private int _leftPaddleNumberMiss, _rightPaddleNumberMiss;
        private int _selectedDeathrate, _actualDeathrateLeft, _actualDeathrateRight, _totalDeathrateLeft, _totalDeathrateRight;  //Deathrates: goal and current
        private int _fsheight, _fsWidth;
        private int _iterationCount;  //total count of executions

        public Bitmap FinalScreen { get { return _finalScreen; } }
        public int FinalScreenHeight { get { return _fsheight; } }
        public int FinalScreenWidth { get { return _fsWidth; } }
        public int BallX { get { return _mpba.BallX; } set { _mpba.BallX = value; } }
        public int BallY { get { return _mpba.BallY; } set { _mpba.BallY = value; } }
        public int BallXVelocity { get { return _mpba.BallXVelocity; } set { _mpba.BallXVelocity = value; } }
        public int BallYVelocity { get { return _mpba.BallYVelocity; } set { _mpba.BallYVelocity = value; } }
        public int LeftPaddleY { get { return _mpp.LeftPaddleY; } set { _mpp.LeftPaddleY = value; } }
        public int RightPaddleY { get { return _mpp.RightPaddleY; } set { _mpp.RightPaddleY = value; } }
        public int Player1Score { get { return _player1Score; } set { _player1Score = value; } }  //Left Paddle
        public int Player2Score { get { return _player2Score; } set { _player2Score = value; } }  //Right Paddle
        public int LPNumHits { get { return _leftPaddleNumberHits; } set { _leftPaddleNumberHits = value; } }
        public int LPNumMiss { get { return _leftPaddleNumberMiss; } set { _leftPaddleNumberMiss = value; } }
        public int LPCurMaxNumHits { get { return _leftPaddleCurrentMaxNumberHits; } set { _leftPaddleCurrentMaxNumberHits = value; } }
        public int RPNumHits { get { return _rightPaddleNumberHits; } set { _rightPaddleNumberHits = value; } }
        public int RPNumMiss { get { return _rightPaddleNumberMiss; } set { _rightPaddleNumberMiss = value; } }
        public int RPCurMaxNumHits { get { return _rightPaddleCurrentMaxNumberHits; } set { _rightPaddleCurrentMaxNumberHits = value; } }
        public int SelectedDeathrate { get { return _selectedDeathrate; } set { _selectedDeathrate = value; } }
        public int LeftPaddleDeathrate { get { return _actualDeathrateLeft; } set { _actualDeathrateLeft = value; } }
        public int RightPaddleDeathrate { get { return _actualDeathrateRight; } set { _actualDeathrateRight = value; } }
        public int TotalRightPaddleDeathrate { get { return _totalDeathrateRight; } set { _totalDeathrateRight = value; } }
        public int TotalLeftPaddleDeathrate { get { return _totalDeathrateLeft; } set { _totalDeathrateLeft = value; } }
        public int IterationCount { get { return _iterationCount; } set { _iterationCount = value; } }

        public MarxPongModel(int _bmX, int _bmY)
        {
            initMarxPong(_bmX, _bmY);
            _selectedDeathrate = 4;
        }

        public void ExecuteAI()
        {
            if (!_togglePlayer)
            {
                LeftPaddleY += _mpaiPlayer1.ExecuteAI();

                if (LeftPaddleY < 0)
                    LeftPaddleY = 0;

                if (LeftPaddleY > FinalScreenHeight)
                    LeftPaddleY = FinalScreenHeight;

                _togglePlayer = true;
            }
            else
            {
                RightPaddleY += _mpaiPlayer2.ExecuteAI();

                if (RightPaddleY < 0)
                    RightPaddleY = 0;

                if (RightPaddleY > FinalScreenHeight)
                    RightPaddleY = FinalScreenHeight;

                _togglePlayer = false;
            }
        }

        private void initMarxPong(int _bmX, int _bmY)
        {
            //Setup Bitmap
            _finalScreen = new Bitmap(_bmX, _bmY);
            _fsWidth = _finalScreen.Width;
            _fsheight = _finalScreen.Height;

            //Setup Background
            _mpb = new MarxPong_Background();
            _mpb.Height = _finalScreen.Height;
            _mpb.Width = _finalScreen.Width;

            //Setup Ball
            _mpba = new MarxPong_Ball();
            _mpba.BallX = 50;
            _mpba.BallY = 50;

            //Setup Paddle
            _mpp = new MarxPong_Paddle();
            _mpp.Height = _finalScreen.Height;
            _mpp.LeftPaddleX = 0;
            _mpp.LeftPaddleY = _finalScreen.Height / 2;
            _mpp.RightPaddleX = _finalScreen.Width - 10;
            _mpp.RightPaddleY = _finalScreen.Height / 2;

            //Setup Info
            _mpi = new MarxPong_Info();

            //Setup AI
            _mpaiPlayer1 = new MarxPong_AI(this, 0);
            _mpaiPlayer2 = new MarxPong_AI(this, 1);
        }

        public void render()
        {
            _mpb.DrawBackground(_finalScreen);
            _mpba.DrawBall(_finalScreen);
            _mpp.DrawPaddles(_finalScreen);
            _mpi.DrawInfo(_finalScreen);
        }

        public void PopulateInfo()
        {
            _mpi.PopulateData(LPNumMiss, LPNumHits, RPNumMiss, RPNumHits, IterationCount);
        }
    }
}
