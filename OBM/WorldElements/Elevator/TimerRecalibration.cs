/* Charlie Dye, PACE Team - 2024.09.05

This is the script for timer control on the elevator */

using UnityEngine;

public class TimerRecalibration : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the timer in the player's HUD.")] public TimerBehavior tbReference;

    [Header("Time to Recalibrate to")]
    [Tooltip("The amount of time, specific to this script, that the timer recalibrates to once it has finished glitching.")] public float recalibrateToThisAmount;

    [Tooltip("Whether or not the function has executed. Kill bool.")]
    public bool hasExecuted;

    public void RecalibrateTimer()
    {

        if (!hasExecuted && gameObject != null)
        {

            tbReference.timeRemaining = recalibrateToThisAmount;
            tbReference.timeUntilCountdown = tbReference.originalTimeCountdown;
            hasExecuted = true;
            gameObject.SetActive(false);

        }

    }

}
