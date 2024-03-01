using System;

namespace Core.Neurolab
{
    public class LienarBrain
    {
        private readonly Neuron[][] _neurons;
        private readonly Input[] _inputs;
        private readonly double[] _outputs;

        public Input[] Inputs => _inputs;

        public double[] Output => _outputs;

        public LienarBrain(int[] layers)
        {
            _neurons = new Neuron[layers.Length][];
            _inputs = new Input[layers[0]];
            _outputs = new double[layers[^0]];

            var random = new Random();

            static double Sigmoid(double x)
            {
                return 1.0 / (1.0 + Math.Pow(Math.E, -x));
            }

            for (int i = 0; i < layers.Length; i++)
            {
                _neurons[i] = new Neuron[layers[i]];

                for (int j = 0; j < _neurons[i].Length; j++)
                {
                    _neurons[i][j] = new Neuron(
                        activationFunction: Sigmoid,
                        depressionDelta: random.NextDouble() * 0.05,
                        potentiationDelta: random.NextDouble() * 0.05,
                        energyDelta: random.NextDouble() * 0.05,
                        energyThreshold: random.NextDouble() * 0.5,
                        energyRecoveringDelta: random.NextDouble() * 0.01,
                        energySaturation: random.NextDouble() * 0.5 + 0.5,
                        sensetivity: random.NextDouble() * 10);

                    if (i == 0)
                    {
                        _neurons[i][j].Assign(_inputs[j], random.NextDouble() * 2 - 1);

                        continue;
                    }

                    for (int k = 0; k < _neurons[i - 1].Length; k++)
                    {
                        _neurons[i][j].Assign(_neurons[i - 1][k], random.NextDouble() * 2 - 1);
                    }
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < _neurons.Length; i++)
            {
                for (int j = 0; j < _neurons[i].Length; j++)
                {
                    _neurons[i][j].Update();

                    if (i == _neurons.Length - 1)
                    {
                        _outputs[j] = _neurons[i][j].Value;
                    }
                }
            }
        }
    }
}