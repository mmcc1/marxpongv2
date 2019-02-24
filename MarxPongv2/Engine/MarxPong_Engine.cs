using System;
using System.Threading;
using MarxPongv2.Model;
using MarxPongv2.Collision;
using MarxPongv2.Physics;


namespace MarxPongv2.Engine
{
    class MarxPong_Engine
    {
        MarxPongModel marxPongModel;
        MarxPong_Collision marxPongCollision;
        MarxPong_Physics marxPongPhysics;
        
        private bool end, pause, resetBall, isBallGoingForward;

        public MarxPong_Engine(MarxPongModel marxPongModel)
        {
            this.marxPongModel = marxPongModel;
            marxPongCollision = new MarxPong_Collision(this.marxPongModel);
            marxPongPhysics = new MarxPong_Physics();

            resetBall = true;
            isBallGoingForward = true;
        }

        public bool Pause { set { pause = value; } }
        public bool End { set { end = value; } }

        public void StartEngine()
        {
            while (!end)
            {
                while (pause)  //Pause execution and be nice
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
            marxPongModel.ExecuteAI();
            //Test for collision and set flags
            SetCollisionInfo();

            if (resetBall)  //reset the ball position to start
            {
                ResetBall();
                resetBall = false;
            }
            else  //update ball position
            {
                UpdateBall();
            }  
        }

        public void ResetBall()
        {
            Random _rand = new Random((int)DateTime.Now.Ticks);

            marxPongModel.BallX = marxPongModel.FinalScreenWidth / 2;
            marxPongModel.BallY = marxPongModel.FinalScreenHeight / 2;

            if(isBallGoingForward)  //Toggle ball direction left/right depending on who last scored
                marxPongModel.BallXVelocity = 5;
            else
                marxPongModel.BallXVelocity = -5;

            marxPongModel.BallYVelocity = _rand.Next(-10, 11);
        }

        public void UpdateBall()
        {
            marxPongModel.BallX += marxPongModel.BallXVelocity;
            marxPongModel.BallY += marxPongModel.BallYVelocity;
        }

        private void SetCollisionInfo()
        {
            int _collisionResult = marxPongCollision.CollisionDetection();

            switch (_collisionResult)
            {
                case 0:  //nothing
                    return;
                case 1:  //hit top of screen
                    marxPongModel.BallYVelocity = marxPongPhysics.BounceReverse(marxPongModel.BallYVelocity);
                    break;
                case 2:  //hit top of screen
                    marxPongModel.BallYVelocity = marxPongPhysics.BounceReverse(marxPongModel.BallYVelocity);
                    break;
                case 3: //Hit left paddle
                    {
                        marxPongModel.BallXVelocity = marxPongPhysics.BounceReverse(marxPongModel.BallXVelocity);
                        marxPongModel.LPNumHits++;
                    }
                    break;
                case 4: //Missed left paddle
                    {
                        marxPongModel.Player1Score++;
                        marxPongModel.LeftPaddleDeathrate++;
                        marxPongModel.LPNumMiss++;
                        resetBall = true;
                        isBallGoingForward = true;  //Reverse direction of reset ball
                    }
                    break;
                case 5:  //Hit right paddle
                    {
                        marxPongModel.BallXVelocity = marxPongPhysics.BounceReverse(marxPongModel.BallXVelocity);
                        marxPongModel.RPNumHits++;
                    }
                    break;
                case 6:  //Miss right paddle
                    {
                        marxPongModel.Player2Score++;
                        marxPongModel.RightPaddleDeathrate++;
                        marxPongModel.RPNumMiss++;
                        resetBall = true;
                        isBallGoingForward = false;  //Reverse direction of reset ball
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
