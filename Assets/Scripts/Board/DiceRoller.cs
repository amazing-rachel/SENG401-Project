using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    [Header("Roll Settings")]
    public float rollDuration = 1.2f;

    [Header("Result")]
    public int LastRoll { get; private set; }
    public bool IsRolling { get; private set; }

    public event Action<int> OnDiceRolled;

    private Quaternion[] faceRotations;

    void Start()
    {
        // Define rotations that put each face on top
        faceRotations = new Quaternion[]
        {
            Quaternion.Euler(-90,0,0),     // 1 up
            Quaternion.Euler(0,0,0),    // 2 up
            Quaternion.Euler(0,0,-90),   // 3 up
            Quaternion.Euler(0,0,90),    // 4 up
            Quaternion.Euler(180,0,0),   // 5 up
            Quaternion.Euler(90,0,0)    // 6 up
        };
    }

    void Update()
    {
        if (IsRolling) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    StartCoroutine(RollDice());
                }
            }
        }
    }

    IEnumerator RollDice()
    {
        IsRolling = true;

        // Random dice result
        LastRoll = UnityEngine.Random.Range(1, 7);

        float elapsed = 0f;

        while (elapsed < rollDuration)
        {
            elapsed += Time.deltaTime;

            // Spin dice randomly while rolling
            transform.Rotate(
                UnityEngine.Random.Range(400,700) * Time.deltaTime,
                UnityEngine.Random.Range(400,700) * Time.deltaTime,
                UnityEngine.Random.Range(400,700) * Time.deltaTime
            );

            yield return null;
        }

        // Snap to final face
        transform.rotation = faceRotations[LastRoll - 1];

        IsRolling = false;

        Debug.Log("Dice result: " + LastRoll);

        // Notify other systems
        OnDiceRolled?.Invoke(LastRoll);
    }
}