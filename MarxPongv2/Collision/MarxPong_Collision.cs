using System;
using MarxPongv2.Model;

namespace MarxPongv2.Collision
{
    class MarxPong_Collision
    {
        MarxPongModel _mpm;

        public MarxPong_Collision(MarxPongModel _mpm)
        {
            this._mpm = _mpm;
        }

        public int CollisionDetection()
        {
            //hit top of screen (status 1)
            if (_mpm.BallY <= 0)
            {
                return 1;
            }

            //hit bottom if screen
            if (_mpm.BallY >= _mpm.FinalScreenHeight)
            {
                return 2;
            }

            
            //left threshold
            if (_mpm.BallX <= 10)  //Left edge
            {
                //Did we hit the paddle?
                if (_mpm.BallY >= _mpm.LeftPaddleY && _mpm.BallY <= (_mpm.LeftPaddleY + (_mpm.FinalScreenHeight / 8)))
                {
                    return 3;
                }
                else  //Missed it,
                {
                    return 4;
                }  
            }

            //right threshold
            if (_mpm.BallX >= _mpm.FinalScreenWidth - 10)  //right edge
            {
                //Did we hit the Paddle?
                if (_mpm.BallY >= _mpm.RightPaddleY && _mpm.BallY <= (_mpm.RightPaddleY + (_mpm.FinalScreenHeight / 8)))
                {
                    return 5;
                }
                else  //Missed it,
                {
                    return 6;
                }  
            }

            return 0;
        }
    }
}
