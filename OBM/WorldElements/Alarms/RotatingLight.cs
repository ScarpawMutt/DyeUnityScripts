/* Charlie Dye, PACE Team - 2025.01.14

This is the script for flashing light behavior */

using System.Collections;
using UnityEngine;

public class RotatingLight : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the fourth floor rail puzzle.")] public FourthFloorRail ffReference;
    [Tooltip("The script attached to the fifth floor lugnut gimmick.")] public NutBehavior nbReference;

    [Header("Spinning Component")]
    [Tooltip("The piece of the lantern that rotates when active. All warning lights are children of this.")] public GameObject alarmSpindle;

    [Header("Light GameObjects")]
    [Tooltip("The array of warning lanterns to be enabled/disabled when the warning activates/deactivates.")] public Light[] warningLanterns;

    [Header("Float Variables")]
    [Tooltip("The maximim allowed speed that the spindle can rotate.")] public float maxRotationRate;
    [Tooltip("The currrent speed, in this frame, that the spindle rotates.")] public float currentRotationRate;
    [Tooltip("The rate at which the spindle speeds up or slows down via the coroutine.")] public float rateOfAcceleration;
    [Tooltip("The rate, in seconds, at which the coroutine refreshes.")] public float refreshRate;

    // Boolean variables
    private bool alarmHasActivated = false;
    private bool alarmHasStopped = false;
    private bool alarmHasDeactivated = false;

    void Start()
    {

        // If the warning lights are switched on when the scene loads, this will disable them
        for (int i = 0; i < warningLanterns.Length; i++)
        {

            warningLanterns[i].enabled = false;

        }

        // If the maximum rotation speed is set to zero, this will set it to a positive value
        if (maxRotationRate == 0f) maxRotationRate = 1.0f;

        // If the starting rotation speed is anything but zero, this will set it to zero temporarily
        if (currentRotationRate != 0f) currentRotationRate = 0f;

    }

    void FixedUpdate()
    {

        // If the alarm is active, then rotation is created through the method
        if (alarmHasActivated && !alarmHasDeactivated) CreateRotation();

        // If the alarm must activate as a result of the fourth floor gimmick being completed unsuccessfully
        if (ffReference.puzzleHasFailed && !nbReference.winConditionHasExecuted && !alarmHasActivated)
        {

            // Activates the rotating strobes
            for (int j = 0; j < warningLanterns.Length; j++)
            {

                warningLanterns[j].enabled = true;

            }

            // Controls the spindle's speed via the coroutine
            StartCoroutine(ControlRate(true));

            // Freezes the infinite loop with the kill bool
            alarmHasActivated = true;

        }

        // If the alarm must shut off as a result of the fifth floor's lugnut gimmick being completed
        if (nbReference.winConditionHasExecuted && alarmHasActivated && !alarmHasStopped)
        {

            // Deactivates the rotating strobes
            for (int k = 0; k < warningLanterns.Length; k++)
            {

                warningLanterns[k].enabled = false;

            }

            // Controls the spindle's speed via the coroutine
            StartCoroutine(ControlRate(false));

            // Freezes the infinite loop with the kill bool
            alarmHasStopped = true;

        }

    }

    public void CreateRotation()
    {

        // Rotates the spindle around its local axis, adjusted to real time
        alarmSpindle.transform.Rotate(currentRotationRate * Time.fixedDeltaTime * Vector3.up, Space.Self);

    }

    public IEnumerator ControlRate(bool speedRateUp)
    {

        // If the spindle has a legitimate GameObject in its slot, then the coroutine will play as normal
        while (alarmSpindle != null)
        {

            // If the spindle's rotation rate must speed up
            if (speedRateUp)
            {

                // If the spindle's current rotation rate is less than the maximum
                if (currentRotationRate < maxRotationRate)
                {

                    // Incrementally adds to the current speed
                    currentRotationRate += rateOfAcceleration;

                    // Refreshes the coroutine with the new speed
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                else yield break;

            }
            // If the spindle must slow to a stop
            else
            {

                // If the spindle's current rotation rate is greater than zero
                if (currentRotationRate > 0f)
                {

                    // Subtracts from the current speed via a decrement
                    currentRotationRate -= rateOfAcceleration;

                    // Refreshes the coroutine with the new speed
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                else
                {

                    // Deactivates the alarm permanently and breaks the coroutine
                    alarmHasDeactivated = true;
                    yield break;

                }

            }

        }

    }

}
