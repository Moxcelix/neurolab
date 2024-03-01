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

        private readonly double _depressionDelta;
        private readonly double _potentiationDelta;
        private readonly double _energyDelta;
        private readonly double _energyThreshold;
        private readonly double _energySaturation;
        private readonly double _energyRecoveringDelta;
        private readonly double _sensetivity;

        private double _energyLevel = 1.0f;
        private bool _isRecovering = false;

        public double Value { get; private set; }

        public Neuron(ActivationFunction activationFunction,
            double depressionDelta, double potentiationDelta,
            double energyDelta, double energyThreshold, 
            double energyRecoveringDelta, double energySaturation, double sensetivity)
        {
            _activationFunction = activationFunction;
            _depressionDelta = depressionDelta;
            _potentiationDelta = potentiationDelta;
            _energyDelta = energyDelta;
            _energyThreshold = energyThreshold;
            _energyRecoveringDelta = energyRecoveringDelta;
            _energySaturation = energySaturation;
            _sensetivity = sensetivity;

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
                throw new ArgumentException(
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
            CalculateEnergy();

        }

        private void CalculateValue()
        {
            double sum = 0;

            for (int i = 0; i < _assigns.Count; i++)
            {
                sum += _assigns[i].Value * _weights[i];
            }

            Value = _activationFunction(sum * _sensetivity);
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

        private void CalculateEnergy()
        {
            if (_isRecovering)
            {
                Value = 1;

                _energyLevel += _energyRecoveringDelta;

                if(_energyLevel > _energySaturation)
                {
                    _energyLevel = _energySaturation;

                    _isRecovering = false;
                }

                return;
            }

            var level = Math.Clamp(Value, 0, 1);

            _energyLevel -= level * _energyDelta;

            if (_energyLevel < _energyThreshold)
            {
                _isRecovering = true;
            }
        }
    }
}
