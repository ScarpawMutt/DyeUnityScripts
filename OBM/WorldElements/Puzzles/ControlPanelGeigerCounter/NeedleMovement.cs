/* Charlie Dye, PACE Team - 2024.03.20

This is the script for randomized needle movement within puzzles */

using System.Collections;
using UnityEngine;

public class NeedleMovement : MonoBehaviour
{

    [Header("Numerical Variables")]
    [Tooltip("The maximum angle that the needle can rotate.")] public float maximumNeedleAangle;
    [Tooltip("The slowest the needle can move at.")] public float mimimumMovementRate;
    [Tooltip("The fastest the needle can move at.")] public float maximumMovementRate;
    [Tooltip("The rate at which speed and direction are rerolled.")] public float rerollRate;
    private float rotationRate;
    private int correspondingBoolValue;

    [Header("Boolean Variable")]
    [Tooltip("Whether or not the needle can rotate.")] public bool canRotate = false;
    private bool hasExecuted = false;

    void FixedUpdate()
    {
        
        // If the needles must rotate
        if (canRotate)
        {
            // If they have not moved prior
            if (!hasExecuted)
            {

                // The coroutine begins
                StartCoroutine(RandomizeVectors());

                // The kill bool switches to true, preventing excessive code block executions
                hasExecuted = true;

            }

            // Creates movement
            MoveNeedle();

        }

    }

    public void MoveNeedle()
    {

        // Depending on the bool integer, the needle will rotate either clockwise or counterclockwise around its pivot
        if (correspondingBoolValue == 0) gameObject.transform.Rotate(rotationRate * Time.deltaTime * Vector3.forward, Space.Self);
        else if (correspondingBoolValue == 1) gameObject.transform.Rotate(rotationRate * Time.deltaTime * Vector3.back, Space.Self);

        // If the needle hits its extreme rotation limits
        if (correspondingBoolValue == 1 && gameObject.transform.localRotation.eulerAngles.z > maximumNeedleAangle) correspondingBoolValue = 0;
        else if (gameObject.transform.localRotation.eulerAngles.z > maximumNeedleAangle) correspondingBoolValue = 1;

    }

    public IEnumerator RandomizeVectors()
    {

        // If the needle is allowed to rotate
        while (canRotate)
        {

            // Randomizes the bool integer, outputting either zero or one
            correspondingBoolValue = Random.Range(0, 2);

            // Randomizes the private rotation using given min and max values
            rotationRate = Random.Range(mimimumMovementRate, maximumMovementRate);    

            // Repeats the randomization after a set amount of time
            yield return new WaitForSecondsRealtime(rerollRate);

        }

    }

}
