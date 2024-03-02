using UnityEngine;
using Core.Neurolab;
using Unity.Collections;

[RequireComponent(typeof(CharacterController))]
public class SimpleAnimal : MonoBehaviour
{
    private LienarBrain _lienarBrain;

    [SerializeField] private int _memory = 4;
    [SerializeField] private float _frequency = 1;

    [ReadOnly][SerializeField] private double[] _memoryDamp;

    private CharacterController _characterController;

    private readonly int _directions = 4;
    private readonly int _feelings = 4;
    private readonly int _observations = 4;

    private float _counter = 0;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        _memoryDamp = new double[_memory];

        _lienarBrain = new LienarBrain(
            new int[] {
                _observations + _feelings + _memory + 1,    // Input layer
                15,                                         // Hidden layer
                30,                                         // Hidden layer
                30,                                         // Hidden layer
                15,                                         // Hidden layer
                _directions + _memory });                   // Output layer
    }

    private void Update()
    {
        double DistanceToSence(float distance) => System.Math.Pow(1.0 - distance / 10.0, 2);
        double SpeedToSence(float speed) => System.Math.Pow(speed / 2.0, 2);
        double WaveSignal(double signal, double frequency) => System.Math.Sin(frequency * _counter) * signal;

        _counter += Time.deltaTime;

        for (int i = 0; i < _memory; i++)
        {
            _lienarBrain.Inputs[i].Value = _lienarBrain.Outputs[i];

            _memoryDamp[i] = _lienarBrain.Outputs[i];
        }

        _lienarBrain.Inputs[_memory].Value = Mathf.Sin(_counter * _frequency);
        _lienarBrain.Inputs[_memory + 1].Value = WaveSignal(DistanceToSence(HitDirection(transform.forward)), _frequency);
        _lienarBrain.Inputs[_memory + 2].Value = WaveSignal(DistanceToSence(HitDirection(transform.right)), _frequency);
        _lienarBrain.Inputs[_memory + 3].Value = WaveSignal(DistanceToSence(HitDirection(-transform.forward)), _frequency);
        _lienarBrain.Inputs[_memory + 4].Value = WaveSignal(DistanceToSence(HitDirection(-transform.right)), _frequency);
        _lienarBrain.Inputs[_memory + 5].Value = WaveSignal(SpeedToSence(FeelMoveDirection(transform.forward)), _frequency);
        _lienarBrain.Inputs[_memory + 6].Value = WaveSignal(SpeedToSence(FeelMoveDirection(transform.right)), _frequency);
        _lienarBrain.Inputs[_memory + 7].Value = WaveSignal(SpeedToSence(FeelMoveDirection(-transform.forward)), _frequency);
        _lienarBrain.Inputs[_memory + 8].Value = WaveSignal(SpeedToSence(FeelMoveDirection(-transform.right)), _frequency);

        _lienarBrain.Update();

        var direction =
            (float)_lienarBrain.Outputs[_memory + 0] * Vector3.forward +
            (float)_lienarBrain.Outputs[_memory + 1] * Vector3.right +
            (float)_lienarBrain.Outputs[_memory + 2] * Vector3.back +
            (float)_lienarBrain.Outputs[_memory + 3] * Vector3.left;

        Move(direction, Time.deltaTime);
    }

    private void Move(Vector3 direction, float deltaTime)
    {
        _characterController.Move(direction * deltaTime);
    }

    private float FeelMoveDirection(Vector3 direction)
    {
        return Vector3.Dot(_characterController.velocity, direction);
    }

    private float HitDirection(Vector3 direction)
    {
        var length = 10.0f;

        if (Physics.Raycast(transform.position + direction * 0.5f, direction, out RaycastHit hit, length))
        {
            Debug.DrawLine(transform.position + direction, hit.point);

            return hit.distance;
        }

        return length;
    }
}
