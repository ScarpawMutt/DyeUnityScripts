/* Charlie Dye, PACE Team - 2025.12.05

This is the script for the normal gauges on the railing puzzle gadgets */

using System.Collections;
using UnityEngine;

public class AnalogClock : MonoBehaviour
{

    [Header("Clock Hands")]
    [Tooltip("The minute hand of the clock.")] public GameObject minuteHand;
    [Tooltip("The hour hand of the clock.")] public GameObject hourHand;

    [Header("Float Variables")]
    [Tooltip("The slowest that the minute hand can spin.")] public float slowestHandSpeed;
    [Tooltip("The fastest that the minute hand can spin.")] public float fastestHandSpeed;
    [Tooltip("The shortest possible time that the hands can remain spinning in one direction.")] public float minimumDirectionTime;
    [Tooltip("The longest possible time that the hands can remain spinning in one direction.")] public float maximumDirectionTime;
    private float randomHandSpeed;
    private float randomStateTime;

    [Header("Boolean Variables")]
    [Tooltip("Are the hands supposed to be spinning clockwise (true) or counterclockwise (false)?")] public bool handsRotateClockwise;
    [Tooltip("Can the hands animate?")] public bool canInvert;
    private bool hasStarted;

    void Start()
    {

        // If either of the clock hands are left null, then this script will self-destruct
        if (minuteHand == null || hourHand == null) Destroy(this);

        // If the float variables are configured incorrectly, then this will correct them
        if (slowestHandSpeed == 0f) slowestHandSpeed = 0.5f;
        else if (slowestHandSpeed < 0f) slowestHandSpeed *= -1f;
        if (fastestHandSpeed == 0f) fastestHandSpeed = 1f;
        else if (fastestHandSpeed < 0f) fastestHandSpeed *= -1f;
        if (fastestHandSpeed <= slowestHandSpeed) fastestHandSpeed = slowestHandSpeed + 1f;
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

        // If the hands must move
        if (canInvert && !hasStarted)
        {

            // Executes the coroutine with the Boolean variable
            StartCoroutine(RandomizeHandMovement());

            // Kill bool for only one execution
            hasStarted = true;

        }

        // Calls the rotation method
        if (canInvert) ClockControl();

    }

    public void ClockControl()
    {

        // If the hands must rotate clockwise
        if (handsRotateClockwise)
        {

            minuteHand.transform.Rotate(randomHandSpeed * Time.fixedDeltaTime * Vector3.up, Space.Self);
            hourHand.transform.Rotate(randomHandSpeed / 12f * Time.fixedDeltaTime * Vector3.up, Space.Self);

        }
        // If the hands must rotate counterclockwise
        else
        {

            minuteHand.transform.Rotate(randomHandSpeed * Time.fixedDeltaTime * Vector3.down, Space.Self);
            hourHand.transform.Rotate(randomHandSpeed / 12f * Time.fixedDeltaTime * Vector3.down, Space.Self);

        }

    }

    public IEnumerator RandomizeHandMovement()
    {

        // While-loop ensuring that the needle object is not null
        while (minuteHand != null && hourHand != null)
        {

            // If the hands can spin
            if (canInvert)
            {

                // Randomizes the coroutine refresh rate using the entered minimum and maximum values
                randomStateTime = Random.Range(minimumDirectionTime, maximumDirectionTime);

                // Randomizes the hand speed using the entered minimum and maximum values
                randomHandSpeed = Random.Range(slowestHandSpeed, fastestHandSpeed);

                // Inverts the current Boolean value
                if (!handsRotateClockwise) handsRotateClockwise = true;
                else handsRotateClockwise = false;

                // Refreshes the coroutine using randomized time
                yield return new WaitForSecondsRealtime(randomStateTime);

            }
            // Breaks the coroutine
            else yield break;

        }

    }

}
