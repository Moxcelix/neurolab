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
    private readonly int _observations = 4;

    private float _counter = 0;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        _memoryDamp = new double[_memory];

        _lienarBrain = new LienarBrain(
            new int[] {
                _observations + _memory + 1,    // Input layer
                15,                             // Hidden layer
                30,                             // Hidden layer
                15,                             // Hidden layer
                _directions + _memory });       // Output layer
    }

    private void Update()
    {
        double DistanceToSence(float distance) => System.Math.Pow(1.0 - distance / 10.0, 2);

        _counter += Time.deltaTime;

        for (int i = 0; i < _memory; i++)
        {
            _lienarBrain.Inputs[i].Value = _lienarBrain.Outputs[i];

            _memoryDamp[i] = _lienarBrain.Outputs[i];
        }

        _lienarBrain.Inputs[_memory].Value = Mathf.Sin(_counter * _frequency);
        _lienarBrain.Inputs[_memory + 1].Value = DistanceToSence(HitDirection(transform.forward));
        _lienarBrain.Inputs[_memory + 2].Value = DistanceToSence(HitDirection(transform.right));
        _lienarBrain.Inputs[_memory + 3].Value = DistanceToSence(HitDirection(-transform.forward));
        _lienarBrain.Inputs[_memory + 4].Value = DistanceToSence(HitDirection(-transform.right));

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
