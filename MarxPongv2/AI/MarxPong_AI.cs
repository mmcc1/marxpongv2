using System;
using MarxPongv2.Model;

namespace MarxPongv2.AI
{
    public struct Weights
    {
        public double w0;
        public double w1;
        public double w2;
        public double w3;
        public double w4;
        public double w5;
    }

    class MarxPong_AI
    {
        MarxPongModel marxPongModel;
        int playerNumber;  // 0 or 1 - Left or right respectively
        bool isInitialised;

        //Neural Network
        private Weights[] nnWeights;  //Weights for neural network
        private int paddleY;
        private double[] hnOutput;  //output activations from hidden networks

        //Genetic Algorithm
        private Weights[] gaParent1;  //weights/genome - most successful weights from NN
        private Weights[] gaParent2;  //weights/genome - most successful weights from NN
        private int currentHits;
        private int previousHits;
        private bool updatePreviousHits;
        private bool initialParent1Found, initialParent2Found;  //need valid weights/genome (a random success) before we can begin evolution
        private bool updateParentToggle;
        private bool[] shouldKeepValue;  //Flags to determine which part of genome to keep
        private int paddleDeathrate;
        //private int _totalDeathrate;
        private bool updatePaddleDeathrate;

        //Output to paddle
        int result;

        public MarxPong_AI(MarxPongModel _mpm, int _playernumber)
        {
            this.marxPongModel = _mpm;
            this.playerNumber = _playernumber;

            gaParent1 = new Weights[7];
            gaParent2 = new Weights[7];
            nnWeights = new Weights[7];
            hnOutput = new double[6];
            shouldKeepValue = new bool[42];
        }

        public int ExecuteAI()
        {
            CopyValuesFromModel();  //Move some values into variables in this class

            if (!isInitialised)  //When first executed, we populate the weights randomly.
            {
                InitNNWeights();
                isInitialised = true;
            }

            ExecuteGA();
            HiddenNetwork(6);

            marxPongModel.PopulateInfo();

            CopyValuesToModel();  //Move some values from this class back to the model.

            return OutputNetwork();
        }

        #region Copy values to and from MarxPongModel

        public void CopyValuesFromModel()
        {
            if (playerNumber == 0)
            {
                currentHits = marxPongModel.LPNumHits;
                paddleY = marxPongModel.LeftPaddleY;
                previousHits = marxPongModel.LPCurMaxNumHits;
                paddleDeathrate = marxPongModel.LeftPaddleDeathrate;
            }
            else
            {
                currentHits = marxPongModel.RPNumHits;
                paddleY = marxPongModel.RightPaddleY;
                previousHits = marxPongModel.RPCurMaxNumHits;
                paddleDeathrate = marxPongModel.RightPaddleDeathrate;
            }
        }

        public void CopyValuesToModel()
        {
            if (updatePreviousHits && playerNumber == 0)
            {
                marxPongModel.LPCurMaxNumHits = previousHits;
                marxPongModel.LPNumHits = 0;               
            }

            if (updatePreviousHits && playerNumber == 1)
            {
                marxPongModel.RPCurMaxNumHits = previousHits;
                marxPongModel.RPNumHits = 0;  
            }

            if (updatePaddleDeathrate && playerNumber == 0)
            {
                marxPongModel.LeftPaddleDeathrate = 0;
                marxPongModel.LPNumMiss = 0;
            }

            if (updatePaddleDeathrate && playerNumber == 1)
            {
                marxPongModel.RightPaddleDeathrate = 0;
                marxPongModel.RPNumMiss = 0;
            }

            updatePreviousHits = false;
            updatePaddleDeathrate = false;
        }

        #endregion

        #region Neural Network

        private void HiddenNetwork(int _numLayers)
        {
            for (int i = 0; i < _numLayers; i++)
            {
                double activation = (marxPongModel.BallX * nnWeights[i].w0) + (marxPongModel.BallY * nnWeights[i].w1) + (marxPongModel.BallXVelocity * nnWeights[i].w2) + (marxPongModel.BallYVelocity * nnWeights[i].w3) + (paddleY * nnWeights[i].w4);
                hnOutput[i] = 1.0 / (1.0 + Math.Exp(-activation));
            }
        }

        private int OutputNetwork()
        {
            double activation = (hnOutput[0] * nnWeights[6].w0) + (hnOutput[1] * nnWeights[6].w1) + (hnOutput[2] * nnWeights[6].w2) + (hnOutput[3] * nnWeights[6].w3) + (hnOutput[4] * nnWeights[6].w4) + (hnOutput[5] * nnWeights[6].w5);

            double output = 1.0 / (1.0 + Math.Exp(-activation));

            if (output <= 0.5)
                result = -10; //Move paddle upwards
            else
                result = 10;  //Move paddle downwards

            return result;
        }

        #endregion

        private void InitNNWeights()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 7; i++)
            {
                nnWeights[i].w0 = rand.NextDouble();
                nnWeights[i].w1 = rand.NextDouble();
                nnWeights[i].w2 = rand.NextDouble();
                nnWeights[i].w3 = rand.NextDouble();
                nnWeights[i].w4 = rand.NextDouble();
                nnWeights[i].w5 = rand.NextDouble();

                if (rand.Next(1, 3) == 1)
                    nnWeights[i].w0 = -nnWeights[i].w0;
                if (rand.Next(1, 3) == 1)
                    nnWeights[i].w1 = -nnWeights[i].w1;
                if (rand.Next(1, 3) == 1)
                    nnWeights[i].w2 = -nnWeights[i].w2;
                if (rand.Next(1, 3) == 1)
                    nnWeights[i].w3 = -nnWeights[i].w3;
                if (rand.Next(1, 3) == 1)
                    nnWeights[i].w4 = -nnWeights[i].w4;
                if (rand.Next(1, 3) == 1)
                    nnWeights[i].w5 = -nnWeights[i].w5;
            }
        }

        private void ExecuteGA()
        {
            if (paddleDeathrate >= marxPongModel.SelectedDeathrate && initialParent1Found && initialParent2Found)  //Can't begin evolution if we don't have two successful sets of weights/genomes
            {
                //If the paddle misses the ball a greater number of times than the set limit (1-4), then evolve a new solution.
                FitnessAndCrossover();
                RandomMutation();
                paddleDeathrate = 0; //Reset counter
                updatePaddleDeathrate = true;  //flag to indicate that model should be updated
                marxPongModel.IterationCount++;
            }
            else
            {
                if (currentHits > previousHits)  //If the current weights of the NN are better
                {
                    UpdateParents();  //Set current weights as the new parents
                    previousHits = currentHits;  //set the new high hit rate to beat
                    currentHits = 0;  //reset the number of times the paddle has hit the ball in a row
                    updatePreviousHits = true;  //flag to indicate that model should be updated
                }

                for(int i = 0; i < shouldKeepValue.Length; i++)
                    shouldKeepValue[i] = false;

                RandomMutation();
            }
        }

        private void UpdateParents()  //Copies current NN weights to parents
        {
            if (!updateParentToggle)
            {
                for (int i = 0; i < 7; i++)
                {
                    gaParent1[i].w0 = nnWeights[i].w0;
                    gaParent1[i].w1 = nnWeights[i].w1;
                    gaParent1[i].w2 = nnWeights[i].w2;
                    gaParent1[i].w3 = nnWeights[i].w3;
                    gaParent1[i].w4 = nnWeights[i].w4;
                    gaParent1[i].w5 = nnWeights[i].w5;
                }

                updateParentToggle = true;
                initialParent1Found = true;
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    gaParent2[i].w0 = nnWeights[i].w0;
                    gaParent2[i].w1 = nnWeights[i].w1;
                    gaParent2[i].w2 = nnWeights[i].w2;
                    gaParent2[i].w3 = nnWeights[i].w3;
                    gaParent2[i].w4 = nnWeights[i].w4;
                    gaParent2[i].w5 = nnWeights[i].w5;
                }

                updateParentToggle = false;
                initialParent2Found = true;
            }
        }

        private void FitnessAndCrossover()  //Determine individual weights to keep and which to evolve (Randomly mutate)
        {
            int index = 0;

            for (int i = 0; i < 7; i++)
            {
                if (((gaParent1[i].w0 - gaParent2[i].w0) < 0.2 && (gaParent1[i].w0 - gaParent2[i].w0) >= 0) || ((gaParent1[i].w0 - gaParent2[i].w0) > -0.2 && (gaParent1[i].w0 - gaParent2[i].w0) <= 0))
                    shouldKeepValue[index] = true;
                else
                    shouldKeepValue[index] = false;

                index++;

                if (((gaParent1[i].w1 - gaParent2[i].w1) < 0.2 && (gaParent1[i].w1 - gaParent2[i].w1) >= 0) || ((gaParent1[i].w1 - gaParent2[i].w1) > -0.2 && (gaParent1[i].w1 - gaParent2[i].w1) <= 0))
                    shouldKeepValue[index] = true;
                else
                    shouldKeepValue[index] = false;

                index++;

                if (((gaParent1[i].w2 - gaParent2[i].w2) < 0.2 && (gaParent1[i].w2 - gaParent2[i].w2) >= 2) || ((gaParent1[i].w2 - gaParent2[i].w2) > -0.2 && (gaParent1[i].w2 - gaParent2[i].w2) <= 0))
                    shouldKeepValue[index] = true;
                else
                    shouldKeepValue[index] = false;

                index++;

                if (((gaParent1[i].w3 - gaParent2[i].w3) < 0.2 && (gaParent1[i].w3 - gaParent2[i].w3) >= 0) || ((gaParent1[i].w3 - gaParent2[i].w3) > -0.2 && (gaParent1[i].w3 - gaParent2[i].w3) <= 0))
                    shouldKeepValue[index] = true;
                else
                    shouldKeepValue[index] = false;

                index++;

                if (((gaParent1[i].w4 - gaParent2[i].w4) < 0.2 && (gaParent1[i].w4 - gaParent2[i].w4) >= 0) || ((gaParent1[i].w4 - gaParent2[i].w4) > -0.2 && (gaParent1[i].w4 - gaParent2[i].w4) <= 0))
                    shouldKeepValue[index] = true;
                else
                    shouldKeepValue[index] = false;

                index++;

                if (((gaParent1[i].w5 - gaParent2[i].w5) < 0.2 && (gaParent1[i].w5 - gaParent2[i].w5) >= 0) || ((gaParent1[i].w5 - gaParent2[i].w5) > -0.2 && (gaParent1[i].w5 - gaParent2[i].w5) <= 0))
                    shouldKeepValue[index] = true;
                else
                    shouldKeepValue[index] = false;

                index++;
            }

            //Prevent infinite loop
            bool _falseSolutionReset = true;

            for (int i = 0; i < shouldKeepValue.Length; i++)
            {
                if (shouldKeepValue[i] == false)
                    _falseSolutionReset = false;
            }

            if (_falseSolutionReset)  //if all values are kept, dump the entire genome as a false solution, a real solution would not execute this function
            {
                InitNNWeights();
                initialParent1Found = false;
                initialParent2Found = false;
                previousHits = 0;  //set the new high hit rate to beat
                currentHits = 0;  //reset the number of times the paddle has hit the ball in a row
                updatePreviousHits = true;  //flag to indicate that model should be updated
                updatePaddleDeathrate = true;
            }
        }

        private void RandomMutation()  //Random Mutations in values we should not keep
        {
            Random _rand = new Random((int)DateTime.Now.Ticks);
            int index = 0;

            for (int i = 0; i < 7; i++)
            {
                if (!shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w0 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w0 = -nnWeights[i].w0;
                }

                index++;

                if (!shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w1 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w1 = -nnWeights[i].w1;
                }

                index++;

                if (!shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w2 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w2 = -nnWeights[i].w2;
                }

                index++;

                if (!shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w3 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w3 = -nnWeights[i].w3;
                }

                index++;

                if (!shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w4 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w4 = -nnWeights[i].w4;
                }

                index++;

                if (!shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w5 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        nnWeights[i].w5 = -nnWeights[i].w5;
                }

                index++;
            }
        }
    }
}
