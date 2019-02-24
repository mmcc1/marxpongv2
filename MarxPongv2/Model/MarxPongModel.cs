using System.Drawing;
using MarxPongv2.Background;
using MarxPongv2.Ball;
using MarxPongv2.Paddle;
using MarxPongv2.AI;
using MarxPongv2.Info;

namespace MarxPongv2.Model
{
    public class MarxPongModel
    {
        private Bitmap finalScreen;
        private MarxPong_Background marxPongBackground;
        private MarxPong_Ball marxPongBall;
        private MarxPong_Paddle marxPongPaddle;
        private MarxPong_Info marxPongInfo;
        private bool togglePlayer;
        MarxPong_AI mpaiPlayer1;
        MarxPong_AI mpaiPlayer2;
        private int player1Score, player2Score;
        private int leftPaddleNumberHits, leftPaddleCurrentMaxNumberHits, rightPaddleNumberHits, rightPaddleCurrentMaxNumberHits;  //Track successful hits of the ball by the paddle
        private int leftPaddleNumberMiss, rightPaddleNumberMiss;
        private int selectedDeathrate, actualDeathrateLeft, actualDeathrateRight, totalDeathrateLeft, totalDeathrateRight;  //Deathrates: goal and current
        private int fsheight, fsWidth;
        private int iterationCount;  //total count of executions


        public Bitmap FinalScreen { get { return finalScreen; } }
        public int FinalScreenHeight { get { return fsheight; } }
        public int FinalScreenWidth { get { return fsWidth; } }
        public int BallX { get { return marxPongBall.BallX; } set { marxPongBall.BallX = value; } }
        public int BallY { get { return marxPongBall.BallY; } set { marxPongBall.BallY = value; } }
        public int BallXVelocity { get { return marxPongBall.BallXVelocity; } set { marxPongBall.BallXVelocity = value; } }
        public int BallYVelocity { get { return marxPongBall.BallYVelocity; } set { marxPongBall.BallYVelocity = value; } }
        public int LeftPaddleY { get { return marxPongPaddle.LeftPaddleY; } set { marxPongPaddle.LeftPaddleY = value; } }
        public int RightPaddleY { get { return marxPongPaddle.RightPaddleY; } set { marxPongPaddle.RightPaddleY = value; } }
        public int Player1Score { get { return player1Score; } set { player1Score = value; } }  //Left Paddle
        public int Player2Score { get { return player2Score; } set { player2Score = value; } }  //Right Paddle
        public int LPNumHits { get { return leftPaddleNumberHits; } set { leftPaddleNumberHits = value; } }
        public int LPNumMiss { get { return leftPaddleNumberMiss; } set { leftPaddleNumberMiss = value; } }
        public int LPCurMaxNumHits { get { return leftPaddleCurrentMaxNumberHits; } set { leftPaddleCurrentMaxNumberHits = value; } }
        public int RPNumHits { get { return rightPaddleNumberHits; } set { rightPaddleNumberHits = value; } }
        public int RPNumMiss { get { return rightPaddleNumberMiss; } set { rightPaddleNumberMiss = value; } }
        public int RPCurMaxNumHits { get { return rightPaddleCurrentMaxNumberHits; } set { rightPaddleCurrentMaxNumberHits = value; } }
        public int SelectedDeathrate { get { return selectedDeathrate; } set { selectedDeathrate = value; } }
        public int LeftPaddleDeathrate { get { return actualDeathrateLeft; } set { actualDeathrateLeft = value; } }
        public int RightPaddleDeathrate { get { return actualDeathrateRight; } set { actualDeathrateRight = value; } }
        public int TotalRightPaddleDeathrate { get { return totalDeathrateRight; } set { totalDeathrateRight = value; } }
        public int TotalLeftPaddleDeathrate { get { return totalDeathrateLeft; } set { totalDeathrateLeft = value; } }
        public int IterationCount { get { return iterationCount; } set { iterationCount = value; } }

        public MarxPongModel(int bmX, int bmY)
        {
            initMarxPong(bmX, bmY);
            selectedDeathrate = 4;
        }

        public void ExecuteAI()
        {
            if (!togglePlayer)
            {
                LeftPaddleY += mpaiPlayer1.ExecuteAI();

                if (LeftPaddleY < 0)
                    LeftPaddleY = 0;

                if (LeftPaddleY > FinalScreenHeight)
                    LeftPaddleY = FinalScreenHeight;

                togglePlayer = true;
            }
            else
            {
                RightPaddleY += mpaiPlayer2.ExecuteAI();

                if (RightPaddleY < 0)
                    RightPaddleY = 0;

                if (RightPaddleY > FinalScreenHeight)
                    RightPaddleY = FinalScreenHeight;

                togglePlayer = false;
            }
        }

        private void initMarxPong(int bmX, int bmY)
        {
            //Setup Bitmap
            finalScreen = new Bitmap(bmX, bmY);
            fsWidth = finalScreen.Width;
            fsheight = finalScreen.Height;

            //Setup Background
            marxPongBackground = new MarxPong_Background();
            marxPongBackground.Height = finalScreen.Height;
            marxPongBackground.Width = finalScreen.Width;

            //Setup Ball
            marxPongBall = new MarxPong_Ball();
            marxPongBall.BallX = 50;
            marxPongBall.BallY = 50;

            //Setup Paddle
            marxPongPaddle = new MarxPong_Paddle();
            marxPongPaddle.Height = finalScreen.Height;
            marxPongPaddle.LeftPaddleX = 0;
            marxPongPaddle.LeftPaddleY = finalScreen.Height / 2;
            marxPongPaddle.RightPaddleX = finalScreen.Width - 10;
            marxPongPaddle.RightPaddleY = finalScreen.Height / 2;

            //Setup Info
            marxPongInfo = new MarxPong_Info();

            //Setup AI
            mpaiPlayer1 = new MarxPong_AI(this, 0);
            mpaiPlayer2 = new MarxPong_AI(this, 1);
        }

        public void render()
        {
            marxPongBackground.DrawBackground(finalScreen);
            marxPongBall.DrawBall(finalScreen);
            marxPongPaddle.DrawPaddles(finalScreen);
            marxPongInfo.DrawInfo(finalScreen);
        }

        public void PopulateInfo()
        {
            marxPongInfo.PopulateData(LPNumMiss, LPNumHits, RPNumMiss, RPNumHits, IterationCount);
        }
    }
}
