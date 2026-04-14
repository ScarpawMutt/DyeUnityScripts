/* Charlie Dye, PACE Team - 2025.12.05

This is the script for the normal gauges on the railing puzzle gadgets */

using System.Collections;
using UnityEngine;

public class GaugeBehavior : MonoBehaviour
{

    [Header("Gauge Objects")]
    [Tooltip("The needle's ideal position that this script controls.")] public GameObject needleHolder;
    [Tooltip("The needle's actual position that this script controls.")] public GameObject needleObject;

    [Header("Float Variables")]
    [Tooltip("The slowest that the needle can spin.")] public float slowestNeedleSpeed;
    [Tooltip("The fastest that the needle can spin.")] public float fastestNeedleSpeed;
    [Tooltip("The shortest possible time that the needles can remain spinning in one direction.")] public float minimumDirectionTime;
    [Tooltip("The longest possible time that the needles can remain spinning in one direction.")] public float maximumDirectionTime;
    [Tooltip("The maximum difference, in degrees, that the needle can deviate from its ideal rotation when quivering.")] public float quiverIntensity;
    private float randomNeedleSpeed;
    private float randomStateTime;
    private float quiveredDifference;

    [Header("Boolean Variables")]
    [Tooltip("Is the needle supposed to be spinning clockwise (true) or counterclockwise (false)?")] public bool needleRotatesClockwise;
    [Tooltip("Can the needles animate?")] public bool canInvert;
    private bool hasStarted;

    void Start()
    {

        // If either of the clock hands are left null, then this script will self-destruct
        if (needleHolder == null || needleObject == null) Destroy(this);

        // If the float variables are configured incorrectly, then this will correct them
        if (slowestNeedleSpeed == 0f) slowestNeedleSpeed = 0.5f;
        else if (slowestNeedleSpeed < 0f) slowestNeedleSpeed *= -1f;
        if (fastestNeedleSpeed == 0f) fastestNeedleSpeed = 1f;
        else if (fastestNeedleSpeed < 0f) fastestNeedleSpeed *= -1f;
        if (fastestNeedleSpeed <= slowestNeedleSpeed) fastestNeedleSpeed = slowestNeedleSpeed + 1f;
        if (minimumDirectionTime == 0f) minimumDirectionTime = 0.5f;
        else if (minimumDirectionTime < 0f) minimumDirectionTime *= -1f;
        if (maximumDirectionTime == 0f) maximumDirectionTime = 1f;
        else if (maximumDirectionTime < 0f) maximumDirectionTime *= -1f;
        if (maximumDirectionTime <= minimumDirectionTime) maximumDirectionTime = minimumDirectionTime + 1f;

        // If the Boolean variables are configured incorrectly, then this will correct them
        if (canInvert) canInvert = false;
        if (hasStarted) hasStarted = false;

    }

    void FixedUpdate()
    {

        // If the needles must move
        if (canInvert && !hasStarted)
        {

            // Executes the coroutine with the Boolean variable
            StartCoroutine(RandomizeNeedleMovement());

            // Kill bool for only one execution
            hasStarted = true;

        }

        // Calls the rotation and quivering methods
        if (canInvert)
        {

            InvertRotation();
            QuiveringMovement();

        }

    }

    public void InvertRotation()
    {

        // If the needles must spin clockwise
        if (needleRotatesClockwise) needleHolder.transform.Rotate(randomNeedleSpeed * Time.fixedDeltaTime * Vector3.up, Space.Self);
        // If the needles must spin counterclockwise
        else needleHolder.transform.Rotate(randomNeedleSpeed * Time.fixedDeltaTime * Vector3.down, Space.Self);

    }

    public void QuiveringMovement()
    {

        // Uses a random number generator to determine the deviation
        quiveredDifference = Random.Range(quiverIntensity / 2f, quiverIntensity);

        // Causes the needle to deviate from its "ideal" position
        needleObject.transform.localRotation = Quaternion.Euler(0f, quiveredDifference, 0f);


    }

    public IEnumerator RandomizeNeedleMovement()
    {

        // While-loop ensuring that the needle object is not null
        while (needleHolder != null && needleObject != null)
        {

            // If the needles can spin
            if (canInvert)
            {

                // Randomizes the coroutine refresh rate using the entered minimum and maximum values
                randomStateTime = Random.Range(minimumDirectionTime, maximumDirectionTime);

                // Randomizes the needle speed using the entered minimum and maximum values
                randomNeedleSpeed = Random.Range(slowestNeedleSpeed, fastestNeedleSpeed);

                // Inverts the current Boolean value
                if (!needleRotatesClockwise) needleRotatesClockwise = true;
                else needleRotatesClockwise = false;

                // Refreshes the coroutine using randomized time
                yield return new WaitForSecondsRealtime(randomStateTime);

            }
            // Breaks the coroutine
            else yield break;

        }

    }

}
