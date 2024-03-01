using System;
using System.Collections.Generic;

namespace Core.Neurolab
{
    public class Neuron : ISignal
    {
        public delegate double ActivationFunction(double sum);

        private readonly ActivationFunction _activationFunction;
        private readonly List<ISignal> _assigns;
        private readonly List<double> _weights;

        private readonly float _depressionDelta;
        private readonly float _potentiationDelta;

        public double Value { get; private set; }

        public Neuron(ActivationFunction activationFunction,
            float depressionDelta, float potentiationDelta)
        {
            _activationFunction = activationFunction;
            _depressionDelta = depressionDelta;
            _potentiationDelta = potentiationDelta;

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
            if (weights.Length != _assigns.Count)
            {
                throw new System.ArgumentException(
                    "The number of input weights doesn't match the assings count.");
            }

            _weights.Clear();
            _weights.AddRange(weights);
        }

        public void Update()
        {
            CalculateValue();
            LongTermImpact();
            CorrectWeights();
        }

        private void CalculateValue()
        {
            double sum = 0;

            for (int i = 0; i < _assigns.Count; i++)
            {
                sum += _assigns[i].Value * _weights[i];
            }

            Value = _activationFunction(sum);
        }

        private void LongTermImpact()
        {
            static bool IsActive(double value) => value > 0.5;

            if (!IsActive(Value))
            {
                return;
            }

            for (int i = 0; i < _assigns.Count; i++)
            {
                if (IsActive(_assigns[i].Value))
                {
                    _weights[i] += _potentiationDelta;
                }
                else
                {
                    _weights[i] -= _depressionDelta;
                }
            }
        }

        private void CorrectWeights()
        {
            for (int i = 0; i < _assigns.Count; i++)
            {
                _weights[i] = Math.Clamp(_weights[i], -1, 1);
            }
        }
    }
}
