/* Charlie Dye, PACE Team - 2024.11.20

This is the script for the alarm that sounds during the meltdown */

using System.Collections;
using UnityEngine;

public class AlarmBehavior : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the lugnut puzzle on the fifth floor.")] public NutBehavior nbReference;
    [Tooltip("The script attached to the flashing light behavior for the fire alarm.")] public FlashingLight flReference;

    [Header("Audio")]
    [Tooltip("The audio that sounds as the fire alarm goes off.")] public AudioSource alarmNoise;

    [Header("Check Interval")]
    [Tooltip("The interval, in seconds, that the coroutine checks for any active fire objects in the scene.")] public float checkInSeconds;

    // Boolean variables
    private bool alarmIsActive;
    private bool onEnable;
    private bool onDisable;

    void Start()
    {

        // Sets the alarm activity status to false
        alarmIsActive = false;

        // Begins the coroutine that checks for fire at intervals
        StartCoroutine(CheckForFire());

    }

    void FixedUpdate()
    {

        // Bool logic depending on whether the alarm is on or not
        if (alarmIsActive)
        {

            if (!onEnable)
            {

                // Makes the siren play
                alarmNoise.Play();

                // Makes the light source in the other script pulse
                flReference.pulseIsActive = true;
                flReference.startCoroutine = true;

                // Modifies the private bools so the block only executes once
                onDisable = false;
                onEnable = true;

            }

        }
        else
        {

            if (!onDisable)
            {

                // Makes the siren stop
                alarmNoise.Stop();

                // Makes the light source in the other script switch off
                flReference.pulseIsActive = false;

                // Modifies the private bools so the block only executes once
                onDisable = true;
                onEnable = false;

            }

        }

    }

    public IEnumerator CheckForFire()
    {

        /* If fire is active in the scene and the win condition has not been met, then the alarm will activate;
        if there is no fire, then it will shut off. If the win condition has been met, then the coroutine will stop */
        while (gameObject != null)
        {

            if (!nbReference.winConditionHasExecuted)
            {

                if (GameObject.FindGameObjectsWithTag("Fire").Length != 0) alarmIsActive = true;
                else alarmIsActive = false;

                // Repeats after the float has passed in real time
                yield return new WaitForSecondsRealtime(checkInSeconds);

            }
            else yield break;

        }

    }

}