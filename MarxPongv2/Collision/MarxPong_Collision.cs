using MarxPongv2.Model;

namespace MarxPongv2.Collision
{
    public class MarxPong_Collision
    {
        private MarxPongModel marxPongModel;

        public MarxPong_Collision(MarxPongModel marxPongModel)
        {
            this.marxPongModel = marxPongModel;
        }

        public int CollisionDetection()
        {
            //hit top of screen (status 1)
            if (marxPongModel.BallY <= 0)
                return 1;

            //hit bottom if screen
            if (marxPongModel.BallY >= marxPongModel.FinalScreenHeight)
                return 2;

            
            //left threshold
            if (marxPongModel.BallX <= 10)  //Left edge
            {
                //Did we hit the paddle?
                if (marxPongModel.BallY >= marxPongModel.LeftPaddleY && marxPongModel.BallY <= (marxPongModel.LeftPaddleY + (marxPongModel.FinalScreenHeight / 8)))
                    return 3;
                else
                    return 4;  
            }

            //right threshold
            if (marxPongModel.BallX >= marxPongModel.FinalScreenWidth - 10)  //right edge
            {
                //Did we hit the Paddle?
                if (marxPongModel.BallY >= marxPongModel.RightPaddleY && marxPongModel.BallY <= (marxPongModel.RightPaddleY + (marxPongModel.FinalScreenHeight / 8)))
                    return 5;
                else  //Missed it,
                    return 6;
            }

            return 0;
        }
    }
}
