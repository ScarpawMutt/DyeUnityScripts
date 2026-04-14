/* Charlie Dye, PACE Team - 2024.11.22

This is the script for the updated Geiger counter */

using System.Collections;
using UnityEngine;

public class GeigerCounter : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script used by the first floor of the play area.")] public FirstFloor ffReference;
    [Tooltip("The script used by the red radiation lantern.")] public FlashingLight flReference;
    [Tooltip("The script used by the lugnut puzzle on the fifth floor.")] public NutBehavior nbReference;

    [Header("Needle GameObjects")]
    [Tooltip("The gameObject used to represent the ideal position of the needle. Do not confuse this with the needle itself.")] public GameObject counterNeedleTarget;
    [Tooltip("The gameObject used to represent the actual needle that is visible to the player.")] public GameObject counterNeedle;

    [Header("Float Variables")]
    [Tooltip("The rotation speed of the needle.")] public float needleSpeed;
    [Tooltip("The maximum angle from the resting position that the needle can reach before it must stop.")] public float terminationAngle;
    [Tooltip("The maximum difference, in degrees, that the needle can deviate from its ideal rotation when quivering.")] public float quiverIntensity;
    [Tooltip("The rate at which the clicking noise volume changes per coroutine execution.")] public float volumeGrowth;
    private float originalNeedleLocation;
    private float newNeedleLocation;

    [Header("Audio")]
    [Tooltip("The noise that plays when active.")] public AudioSource counterClick;

    [Header("Boolean Variables")]
    [Tooltip("Determines whether the needle can move at all.")] public bool needleCanMove = true;
    [Tooltip("Determines whether the needle can quiver, or shake.")] public bool needleCanQuiver = false;
    [Tooltip("Determines when to activate needle movement.")] public bool signalToActivate = false;
    [Tooltip("Returns true once the main command has already executed. Kill bool.")] public bool alarmCommandHasExecuted = false;

    void Start()
    {

        // Creates values for starting and new needle positions based on given arithmetic
        originalNeedleLocation = counterNeedleTarget.transform.localRotation.eulerAngles.z;
        newNeedleLocation = originalNeedleLocation + terminationAngle;

        // Sets the initial volume to zero
        counterClick.volume = 0f;

    }

    void FixedUpdate()
    {

        // If the meltdown is underway, then the subsequent bool will allow access to the rest of the script
        if (ffReference.timerHasStarted) signalToActivate = true;

        // Boolean logic for operation of the Geiger counter and alarm
        if (signalToActivate && !nbReference.winConditionHasExecuted)
        {

            // Controlled loop for alarm and sound components
            if (!alarmCommandHasExecuted)
            {
                // Activates the warning lantern
                flReference.pulseIsActive = true;

                // Starts the clicking coroutine
                StartCoroutine(SoundControl(true));

                // Kills the loop
                alarmCommandHasExecuted = true;

            }

            // Methods for moving the needle
            if (needleCanMove) RotateNeedle(true);
            if (needleCanQuiver) QuiverNeedle(true);

        }
        // If the game has been won
        else if (nbReference.winConditionHasExecuted)
        {

            // Re-enables movement of the needle
            needleCanMove = true;

            // Controlled loop for alarm and sound components
            if (alarmCommandHasExecuted)
            {

                // Deactivates the warning lantern
                flReference.pulseIsActive = false;

                // Restarts the clicking coroutine
                StartCoroutine(SoundControl(false));

                // Kills the loop
                alarmCommandHasExecuted = false;

            }

            // Method for rotating the needle
            if (needleCanMove) RotateNeedle(false);

        }

    }

    public void RotateNeedle(bool increaseRadiation)
    {

        // If the game has not been won yet
        if (increaseRadiation)
        {

            // Enables quivering movement
            needleCanQuiver = true;

            // Rotates the needle's transform until it has reached the target Euler value
            if (counterNeedleTarget.transform.localRotation.eulerAngles.z < newNeedleLocation)
            {

                // Creates rotation
                counterNeedleTarget.transform.Rotate(needleSpeed * Time.deltaTime * Vector3.forward);

                // Creates deviation
                QuiverNeedle(true);

            }
            else needleCanMove = false;

        }
        // Otherwise, if the win condition has been satisfied
        else
        {

            // Snaps the needle's position back to "identity", i.e., default position
            counterNeedleTarget.transform.localRotation = Quaternion.identity;

        }

    }

    public void QuiverNeedle(bool causeQuivering)
    {

        // Local float variable used to randomize quiver
        float randomNeedleQuiver = Random.Range(-quiverIntensity, quiverIntensity);

        // If the needle can quiver, then it will deviate from its "ideal" local rotation
        if (causeQuivering) counterNeedle.transform.localRotation = Quaternion.Euler(0f, 0f, randomNeedleQuiver);
        // If the needle cannot quiver, then it will snap back to its "ideal" local rotation
        else counterNeedle.transform.localRotation = Quaternion.identity;

    }

    public IEnumerator SoundControl (bool volumeUp)
    {

        // If the volume must rise
        if (volumeUp)
        {

            // Begins the sound
            counterClick.Play();

            while (counterClick.volume < 1f)
            {

                // Increments the volume and pitch/speed until it has reached 100 percent
                counterClick.volume += volumeGrowth;
                if (counterClick.pitch < 3f) counterClick.pitch += volumeGrowth;

                yield return new WaitForSecondsRealtime(0.5f);

            }

            yield break;

        }
        // Otherwise, if the volume must fall
        else
        {

            while (counterClick.volume > 0f)
            {

                // Decrements the volume and pitch/speed until it has reached zero
                counterClick.volume -= volumeGrowth;
                if (counterClick.pitch > 1f) counterClick.pitch -= volumeGrowth;

                yield return new WaitForSecondsRealtime(0.5f);

            }

            // Stops the noise properly
            counterClick.Stop();
            yield break;

        }

    }

}
