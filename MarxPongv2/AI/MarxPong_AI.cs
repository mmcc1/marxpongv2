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
        MarxPongModel _mpm;
        int _playernumber;  // 0 or 1 - Left or right respectively
        bool _isInitialised;

        //Neural Network
        private Weights[] _nnWeights;  //Weights for neural network
        private int _paddleY;
        private double[] _hnOutput;  //output activations from hidden networks

        //Genetic Algorithm
        private Weights[] _gaParent1;  //weights/genome - most successful weights from NN
        private Weights[] _gaParent2;  //weights/genome - most successful weights from NN
        private int _currentHits;
        private int _previousHits;
        private bool _updatePreviousHits;
        private bool _initialParent1Found, _initialParent2Found;  //need valid weights/genome (a random success) before we can begin evolution
        private bool _updateParentToggle;
        private bool[] _shouldKeepValue;  //Flags to determine which part of genome to keep
        private int _paddleDeathrate;
        //private int _totalDeathrate;
        private bool _updatePaddleDeathrate;

        //Output to paddle
        int _result;

        public MarxPong_AI(MarxPongModel _mpm, int _playernumber)
        {
            this._mpm = _mpm;
            this._playernumber = _playernumber;

            _gaParent1 = new Weights[7];
            _gaParent2 = new Weights[7];
            _nnWeights = new Weights[7];
            _hnOutput = new double[6];
            _shouldKeepValue = new bool[42];
        }

        public int ExecuteAI()
        {
            CopyValuesFromModel();  //Move some values into variables in this class

            if (!_isInitialised)  //When first executed, we populate the weights randomly.
            {
                InitNNWeights();
                _isInitialised = true;
            }

            ExecuteGA();
            HiddenNetwork(6);

            _mpm.PopulateInfo();

            CopyValuesToModel();  //Move some values from this class back to the model.

            return OutputNetwork();
        }

        #region Copy values to and from MarxPongModel

        public void CopyValuesFromModel()
        {
            if (_playernumber == 0)
            {
                _currentHits = _mpm.LPNumHits;
                _paddleY = _mpm.LeftPaddleY;
                _previousHits = _mpm.LPCurMaxNumHits;
                _paddleDeathrate = _mpm.LeftPaddleDeathrate;
            }
            else
            {
                _currentHits = _mpm.RPNumHits;
                _paddleY = _mpm.RightPaddleY;
                _previousHits = _mpm.RPCurMaxNumHits;
                _paddleDeathrate = _mpm.RightPaddleDeathrate;
            }
        }

        public void CopyValuesToModel()
        {
            if (_updatePreviousHits && _playernumber == 0)
            {
                _mpm.LPCurMaxNumHits = _previousHits;
                _mpm.LPNumHits = 0;               
            }

            if (_updatePreviousHits && _playernumber == 1)
            {
                _mpm.RPCurMaxNumHits = _previousHits;
                _mpm.RPNumHits = 0;  
            }

            if (_updatePaddleDeathrate && _playernumber == 0)
            {
                _mpm.LeftPaddleDeathrate = 0;
                _mpm.LPNumMiss = 0;
            }

            if (_updatePaddleDeathrate && _playernumber == 1)
            {
                _mpm.RightPaddleDeathrate = 0;
                _mpm.RPNumMiss = 0;
            }

            _updatePreviousHits = false;
            _updatePaddleDeathrate = false;
        }

        #endregion

        #region Neural Network

        private void HiddenNetwork(int _numLayers)
        {
            for (int i = 0; i < _numLayers; i++)
            {
                double activation = (_mpm.BallX * _nnWeights[i].w0) + (_mpm.BallY * _nnWeights[i].w1) + (_mpm.BallXVelocity * _nnWeights[i].w2) + (_mpm.BallYVelocity * _nnWeights[i].w3) + (_paddleY * _nnWeights[i].w4);
                _hnOutput[i] = 1.0 / (1.0 + Math.Exp(-activation));
            }
        }

        private int OutputNetwork()
        {
            double activation = (_hnOutput[0] * _nnWeights[6].w0) + (_hnOutput[1] * _nnWeights[6].w1) + (_hnOutput[2] * _nnWeights[6].w2) + (_hnOutput[3] * _nnWeights[6].w3) + (_hnOutput[4] * _nnWeights[6].w4) + (_hnOutput[5] * _nnWeights[6].w5);

            double output = 1.0 / (1.0 + Math.Exp(-activation));

            if (output <= 0.5)
                _result = -10; //Move paddle upwards
            else
                _result = 10;  //Move paddle downwards

            return _result;
        }

        #endregion

        private void InitNNWeights()
        {
            Random _rand = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 7; i++)
            {
                _nnWeights[i].w0 = _rand.NextDouble();
                _nnWeights[i].w1 = _rand.NextDouble();
                _nnWeights[i].w2 = _rand.NextDouble();
                _nnWeights[i].w3 = _rand.NextDouble();
                _nnWeights[i].w4 = _rand.NextDouble();
                _nnWeights[i].w5 = _rand.NextDouble();

                if (_rand.Next(1, 3) == 1)
                    _nnWeights[i].w0 = -_nnWeights[i].w0;
                if (_rand.Next(1, 3) == 1)
                    _nnWeights[i].w1 = -_nnWeights[i].w1;
                if (_rand.Next(1, 3) == 1)
                    _nnWeights[i].w2 = -_nnWeights[i].w2;
                if (_rand.Next(1, 3) == 1)
                    _nnWeights[i].w3 = -_nnWeights[i].w3;
                if (_rand.Next(1, 3) == 1)
                    _nnWeights[i].w4 = -_nnWeights[i].w4;
                if (_rand.Next(1, 3) == 1)
                    _nnWeights[i].w5 = -_nnWeights[i].w5;
            }
        }

        private void ExecuteGA()
        {
            if (_paddleDeathrate >= _mpm.SelectedDeathrate && _initialParent1Found && _initialParent2Found)  //Can't begin evolution if we don't have two successful sets of weights/genomes
            {
                //If the paddle misses the ball a greater number of times than the set limit (1-4), then evolve a new solution.
                FitnessAndCrossover();
                RandomMutation();
                _paddleDeathrate = 0; //Reset counter
                _updatePaddleDeathrate = true;  //flag to indicate that model should be updated
                _mpm.IterationCount++;
            }
            else
            {
                if (_currentHits > _previousHits)  //If the current weights of the NN are better
                {
                    UpdateParents();  //Set current weights as the new parents
                    _previousHits = _currentHits;  //set the new high hit rate to beat
                    _currentHits = 0;  //reset the number of times the paddle has hit the ball in a row
                    _updatePreviousHits = true;  //flag to indicate that model should be updated
                }

                for(int i = 0; i < _shouldKeepValue.Length; i++)
                    _shouldKeepValue[i] = false;

                RandomMutation();
            }
        }

        private void UpdateParents()  //Copies current NN weights to parents
        {
            if (!_updateParentToggle)
            {
                for (int i = 0; i < 7; i++)
                {
                    _gaParent1[i].w0 = _nnWeights[i].w0;
                    _gaParent1[i].w1 = _nnWeights[i].w1;
                    _gaParent1[i].w2 = _nnWeights[i].w2;
                    _gaParent1[i].w3 = _nnWeights[i].w3;
                    _gaParent1[i].w4 = _nnWeights[i].w4;
                    _gaParent1[i].w5 = _nnWeights[i].w5;
                }

                _updateParentToggle = true;
                _initialParent1Found = true;
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    _gaParent2[i].w0 = _nnWeights[i].w0;
                    _gaParent2[i].w1 = _nnWeights[i].w1;
                    _gaParent2[i].w2 = _nnWeights[i].w2;
                    _gaParent2[i].w3 = _nnWeights[i].w3;
                    _gaParent2[i].w4 = _nnWeights[i].w4;
                    _gaParent2[i].w5 = _nnWeights[i].w5;
                }

                _updateParentToggle = false;
                _initialParent2Found = true;
            }
        }

        private void FitnessAndCrossover()  //Determine individual weights to keep and which to evolve (Randomly mutate)
        {
            int index = 0;

            for (int i = 0; i < 7; i++)
            {
                if (((_gaParent1[i].w0 - _gaParent2[i].w0) < 0.2 && (_gaParent1[i].w0 - _gaParent2[i].w0) >= 0) || ((_gaParent1[i].w0 - _gaParent2[i].w0) > -0.2 && (_gaParent1[i].w0 - _gaParent2[i].w0) <= 0))
                    _shouldKeepValue[index] = true;
                else
                    _shouldKeepValue[index] = false;

                index++;

                if (((_gaParent1[i].w1 - _gaParent2[i].w1) < 0.2 && (_gaParent1[i].w1 - _gaParent2[i].w1) >= 0) || ((_gaParent1[i].w1 - _gaParent2[i].w1) > -0.2 && (_gaParent1[i].w1 - _gaParent2[i].w1) <= 0))
                    _shouldKeepValue[index] = true;
                else
                    _shouldKeepValue[index] = false;

                index++;

                if (((_gaParent1[i].w2 - _gaParent2[i].w2) < 0.2 && (_gaParent1[i].w2 - _gaParent2[i].w2) >= 2) || ((_gaParent1[i].w2 - _gaParent2[i].w2) > -0.2 && (_gaParent1[i].w2 - _gaParent2[i].w2) <= 0))
                    _shouldKeepValue[index] = true;
                else
                    _shouldKeepValue[index] = false;

                index++;

                if (((_gaParent1[i].w3 - _gaParent2[i].w3) < 0.2 && (_gaParent1[i].w3 - _gaParent2[i].w3) >= 0) || ((_gaParent1[i].w3 - _gaParent2[i].w3) > -0.2 && (_gaParent1[i].w3 - _gaParent2[i].w3) <= 0))
                    _shouldKeepValue[index] = true;
                else
                    _shouldKeepValue[index] = false;

                index++;

                if (((_gaParent1[i].w4 - _gaParent2[i].w4) < 0.2 && (_gaParent1[i].w4 - _gaParent2[i].w4) >= 0) || ((_gaParent1[i].w4 - _gaParent2[i].w4) > -0.2 && (_gaParent1[i].w4 - _gaParent2[i].w4) <= 0))
                    _shouldKeepValue[index] = true;
                else
                    _shouldKeepValue[index] = false;

                index++;

                if (((_gaParent1[i].w5 - _gaParent2[i].w5) < 0.2 && (_gaParent1[i].w5 - _gaParent2[i].w5) >= 0) || ((_gaParent1[i].w5 - _gaParent2[i].w5) > -0.2 && (_gaParent1[i].w5 - _gaParent2[i].w5) <= 0))
                    _shouldKeepValue[index] = true;
                else
                    _shouldKeepValue[index] = false;

                index++;
            }

            //Prevent infinite loop
            bool _falseSolutionReset = true;

            for (int i = 0; i < _shouldKeepValue.Length; i++)
            {
                if (_shouldKeepValue[i] == false)
                    _falseSolutionReset = false;
            }

            if (_falseSolutionReset)  //if all values are kept, dump the entire genome as a false solution, a real solution would not execute this function
            {
                InitNNWeights();
                _initialParent1Found = false;
                _initialParent2Found = false;
                _previousHits = 0;  //set the new high hit rate to beat
                _currentHits = 0;  //reset the number of times the paddle has hit the ball in a row
                _updatePreviousHits = true;  //flag to indicate that model should be updated
                _updatePaddleDeathrate = true;
            }
        }

        private void RandomMutation()  //Random Mutations in values we should not keep
        {
            Random _rand = new Random((int)DateTime.Now.Ticks);
            int index = 0;

            for (int i = 0; i < 7; i++)
            {
                if (!_shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w0 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w0 = -_nnWeights[i].w0;
                }

                index++;

                if (!_shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w1 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w1 = -_nnWeights[i].w1;
                }

                index++;

                if (!_shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w2 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w2 = -_nnWeights[i].w2;
                }

                index++;

                if (!_shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w3 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w3 = -_nnWeights[i].w3;
                }

                index++;

                if (!_shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w4 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w4 = -_nnWeights[i].w4;
                }

                index++;

                if (!_shouldKeepValue[index])
                {
                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w5 = _rand.NextDouble();

                    if (_rand.Next(1, 3) == 1)
                        _nnWeights[i].w5 = -_nnWeights[i].w5;
                }

                index++;
            }
        }
    }
}
