using System;
using System.Threading;
using MarxPongv2.Model;
using MarxPongv2.Collision;
using MarxPongv2.Physics;


namespace MarxPongv2.Engine
{
    class MarxPong_Engine
    {
        MarxPongModel _mpm;
        MarxPong_Collision _mpc;
        MarxPong_Physics _mpp;
        
        bool _end, _pause, _resetBall, _isBallGoingForward;

        public MarxPong_Engine(MarxPongModel _mpm)
        {
            this._mpm = _mpm;
            _mpc = new MarxPong_Collision(_mpm);
            _mpp = new MarxPong_Physics();

            _resetBall = true;
            _isBallGoingForward = true;
        }

        public bool Pause { set { _pause = value; } }
        public bool End { set { _end = value; } }

        public void StartEngine()
        {
            while (!_end)
            {
                while (_pause)  //Pause execution and be nice
                {
                    //Thread.Sleep(500);
                }

                ExecuteGameStep();
            }
        }

        public void RunSTEngine()
        {
            ExecuteGameStep();
        }

        public void ExecuteGameStep()
        {
            //Exec AI
            _mpm.ExecuteAI();
            //Test for collision and set flags
            SetCollisionInfo();

            if (_resetBall)  //reset the ball position to start
            {
                ResetBall();
                _resetBall = false;
            }
            else  //update ball position
            {
                UpdateBall();
            }  
        }

        public void ResetBall()
        {
            Random _rand = new Random((int)DateTime.Now.Ticks);

            _mpm.BallX = _mpm.FinalScreenWidth / 2;
            _mpm.BallY = _mpm.FinalScreenHeight / 2;

            if(_isBallGoingForward)  //Toggle ball direction left/right depending on who last scored
                _mpm.BallXVelocity = 5;
            else
                _mpm.BallXVelocity = -5;

            _mpm.BallYVelocity = _rand.Next(-10, 11);
        }

        public void UpdateBall()
        {
            _mpm.BallX += _mpm.BallXVelocity;
            _mpm.BallY += _mpm.BallYVelocity;
        }

        private void SetCollisionInfo()
        {
            int _collisionResult = _mpc.CollisionDetection();

            switch (_collisionResult)
            {
                case 0:  //nothing
                    return;
                case 1:  //hit top of screen
                    _mpm.BallYVelocity = _mpp.BounceReverse(_mpm.BallYVelocity);
                    break;
                case 2:  //hit top of screen
                    _mpm.BallYVelocity = _mpp.BounceReverse(_mpm.BallYVelocity);
                    break;
                case 3: //Hit left paddle
                    {
                        _mpm.BallXVelocity = _mpp.BounceReverse(_mpm.BallXVelocity);
                        _mpm.LPNumHits++;
                    }
                    break;
                case 4: //Missed left paddle
                    {
                        _mpm.Player1Score++;
                        _mpm.LeftPaddleDeathrate++;
                        _mpm.LPNumMiss++;
                        _resetBall = true;
                        _isBallGoingForward = true;  //Reverse direction of reset ball
                    }
                    break;
                case 5:  //Hit right paddle
                    {
                        _mpm.BallXVelocity = _mpp.BounceReverse(_mpm.BallXVelocity);
                        _mpm.RPNumHits++;
                    }
                    break;
                case 6:  //Miss right paddle
                    {
                        _mpm.Player2Score++;
                        _mpm.RightPaddleDeathrate++;
                        _mpm.RPNumMiss++;
                        _resetBall = true;
                        _isBallGoingForward = false;  //Reverse direction of reset ball
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
