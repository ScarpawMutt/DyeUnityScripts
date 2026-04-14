/* Charlie Dye, PACE Team - 2026.01.29

This is the script for the analog "Doomsday" clock */

using UnityEngine;

public class ClockBehavior : MonoBehaviour
{

    [Header("Script Variable")]
    [Tooltip("The script attached to the player timer.")] public TimerBehavior tbReference;

    [Header("Clock Hands")]
    [Tooltip("The hour hand.")] public GameObject handHour;
    [Tooltip("The minute hand.")] public GameObject handMinute;
    [Tooltip("The second hand.")] public GameObject handSecond;

    // Vector-3 variable
    private Vector3 rotationDirection;

    void Start()
    {

        // Snaps the hands to the correct positions once the scene loads
        handSecond.transform.localRotation = Quaternion.identity;
        handMinute.transform.localRotation = Quaternion.Euler(tbReference.originalTimeLimit / 10f, 0f, 0f);
        handHour.transform.localRotation = Quaternion.Euler(tbReference.originalTimeLimit / 120f, 0f, 0f);

        // Establishes the rotation direction as a function of time and a leftward vector
        rotationDirection = 6f * Time.fixedDeltaTime * Vector3.left;

    }

    void FixedUpdate()
    {

        // If the timer still has an amount greater than zero and is not frozen, then the seconds in the player's digital timer will convert to hand rotation
        if (tbReference.isActiveAndEnabled && tbReference.timeRemaining > 0f && !tbReference.timerIsFrozen && !tbReference.timerIsYellow
            && !tbReference.timerIsSoftlocked && !tbReference.timerIsGlitching) ConvertToRotation();
        
    }

    private void ConvertToRotation()
    {

        // Rotates the hands in sync with real time
        handSecond.transform.Rotate(rotationDirection);
        handMinute.transform.Rotate(rotationDirection / 60f);
        handHour.transform.Rotate(rotationDirection / 3600f);

    }

    public void ReadjustHands()
    {

        // Snaps the hands to the correct positions
        handSecond.transform.localRotation = Quaternion.identity;
        handMinute.transform.localRotation = Quaternion.Euler(tbReference.originalTimeLimit / 10f, 0f, 0f);
        handHour.transform.localRotation = Quaternion.Euler(tbReference.originalTimeLimit / 120f, 0f, 0f);

    }

}
