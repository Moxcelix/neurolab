using System.Collections.Generic;

namespace Core.Neurolab
{
    public class Neuron : ISignal
    {
        public delegate double ActivationFunction(double sum);

        private readonly ActivationFunction _activationFunction;
        private readonly List<ISignal> _assigns;
        private readonly List<double> _weights;

        public double Value { get; private set; }

        public Neuron(ActivationFunction activationFunction)
        {
            _activationFunction = activationFunction;
            _assigns = new List<ISignal>();
            _weights = new List<double>();
        }

        public void Assign(ISignal signal, double weight = 0)
        {
            _assigns.Add(signal);
            _weights.Add(weight);
        }

        public void SetWeights(double[] weights)
        {
            if(weights.Length != _assigns.Count) 
            {
                throw new System.ArgumentException(
                    "The number of input weights doesn't match the assings count.");
            }

            _weights.Clear();
            _weights.AddRange(weights);
        }

        public void Update()
        {
            double sum = 0;

            for (int i = 0; i < _assigns.Count; i++)
            {
                sum += _assigns[i].Value * _weights[i];
            }

            Value = _activationFunction(sum);
        }
    }
}
