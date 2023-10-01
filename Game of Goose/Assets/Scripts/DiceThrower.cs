using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DiceThrower : MonoBehaviour
{
    [SerializeField] private Dice _dice;
    [SerializeField] private int _numberOfDice = 2;
    [SerializeField] private float _throwForce = 5f;
    [SerializeField] private float _rollForce = 10f;

    private List<GameObject> _spawnedDice = new List<GameObject>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }
    }

    private async void RollDice()
    {
        if (_dice == null)
        {
            throw new Exception("No Dice assigned for Dice Thrower!");
        }
        else
        {
            foreach(GameObject dice in _spawnedDice)
            {
                Destroy(dice); // Destroy previous dice.
            }

            for (int i = 0; i < _numberOfDice; i++)
            {
                Dice dice = Instantiate(_dice, transform.position, transform.rotation);
                _spawnedDice.Add(dice.gameObject);
                Debug.Log($"Dice {dice.DiceId} spawned.");
                dice.RollDice(_throwForce, _rollForce, i);
                await Task.Yield(); // Wait a frame and do the next iteration of the loop.
            }
        }
    }
}
