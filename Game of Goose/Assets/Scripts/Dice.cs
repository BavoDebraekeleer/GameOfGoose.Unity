using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Dice : MonoBehaviour
{
    [SerializeField] private Transform[] _diceFaces;
    [SerializeField] private int _delayResultCheck = 1000;
    private Rigidbody _rigidbody;
    private bool _isDelayFinished, _isRolling;

    public int DiceId { get; set; } = -1;

    public static UnityAction<int, int> OnDiceResult;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Update()
    {
        if (_isDelayFinished)
        {
            if (_isRolling && _rigidbody.velocity.sqrMagnitude == 0f)
            {
                _isRolling = false;
                GetDiceTopFace();
            }
        }
    }

    [ContextMenu("Get Top Face")]
    private int GetDiceTopFace()
    {
        if (_diceFaces == null)
        {
            throw new Exception("Dice faces not assigned!");
        }
        else
        {
            int topFace = 0;
            float lastYPosition = _diceFaces[0].position.y;

            for (int i = 0; i < _diceFaces.Length; i++)
            {
                if (_diceFaces[i].position.y > lastYPosition)
                {
                    lastYPosition = _diceFaces[i].position.y;
                    topFace = i;
                }
            }
            topFace++; // Index starts at 0 for face 1, so + 1.
            Debug.Log($"Dice {DiceId} top face is {topFace}");
            OnDiceResult?.Invoke(DiceId, topFace);
            return topFace;
        }

    }

    public void RollDice(float throwForce, float rollForce, int diceId)
    {
        DiceId = diceId;
        _isRolling = true;

        // Throw
        float randomVariance = UnityEngine.Random.Range(-0.5f, 0.5f);
        _rigidbody.AddForce(transform.forward * (throwForce + randomVariance), ForceMode.Impulse);

        // Roll/Twist
        float x = UnityEngine.Random.Range(0f, 1f);
        float y = UnityEngine.Random.Range(0f, 1f);
        float z = UnityEngine.Random.Range(0f, 1f);
        _rigidbody.AddTorque(new Vector3(x, y, z) * (rollForce + randomVariance), ForceMode.Impulse);

        // Wait till roll is finished
        DelayResult();
    }

    private async void DelayResult()
    {
        await Task.Delay(_delayResultCheck);
        _isDelayFinished = true;
    }
}
