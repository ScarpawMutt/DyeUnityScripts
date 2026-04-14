/* Charlie Dye - 2025.04.18

This is the script for noise created when walking */

using Oculus.Interaction.Locomotion;
using UnityEngine;

public class MovementNoise : MonoBehaviour
{

    [Header("Script References")]
    public FirstPersonLocomotor fplReference;
    public WaterPhysics wpReference;

    [Header("Audio Sources")]
    public AudioSource walkingSource;
    public AudioSource swimmingSource;
    public AudioSource[] outsideAudioArray;

    [Header("Float Variable")]
    [Range(1f, 10f)] public float amountToAttenuateBy;
    private float[] correspondingAudioLevels;

    [Header("Boolean Variables")]
    public bool buttonDetected;
    public bool buttonReleased;
    public bool walkingNoiseHasStarted;
    public bool walkingNoiseHasStopped;
    public bool swimmingNoiseHasStarted;
    public bool swimmingNoiseHasStopped;

    void Start()
    {

        // Sets the length of the volume array to that of the audio array
        correspondingAudioLevels = new float[outsideAudioArray.Length];

        // Sets the appropriate booleans so that sound can play
        if (walkingNoiseHasStarted) walkingNoiseHasStarted = false;
        if (!walkingNoiseHasStopped) walkingNoiseHasStopped = true;
        if (swimmingNoiseHasStarted) swimmingNoiseHasStarted = false;
        if (!swimmingNoiseHasStopped) swimmingNoiseHasStopped = true;
        
        // Sets the volume levels as immutable for each element in the audio array
        for (int i = 0; i < outsideAudioArray.Length; i++)
        {

            correspondingAudioLevels[i] = outsideAudioArray[i].volume;

        }

    }

    void FixedUpdate()
    {

        // If the player is in the air or standing still
        if (!fplReference.IsGrounded ||
           !(OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown) ||
            OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)))
        {

            if (walkingNoiseHasStarted && !walkingNoiseHasStopped)
            {

                // Pauses the walking audio
                walkingSource.Pause();

                // Sets the appropriate bools
                walkingNoiseHasStarted = false;
                walkingNoiseHasStopped = true;

            }

        }
        // If the player is underwater
        else if (wpReference.isSubmerged)
        {

            // Stops walking audio
            if (walkingNoiseHasStarted && !walkingNoiseHasStopped)
            {

                walkingSource.Stop();

                // Sets the appropriate bools
                walkingNoiseHasStarted = false;
                walkingNoiseHasStopped = true;

            }

            // Begins underwater ambience
            {

                if (!swimmingNoiseHasStarted && swimmingNoiseHasStopped)
                {

                    swimmingSource.Play();

                    // Decreases the outside noise to simulate an underwater environment
                    for (int j = 0; j < outsideAudioArray.Length; j++)
                    {

                        outsideAudioArray[j].volume = correspondingAudioLevels[j] / amountToAttenuateBy;

                    }

                    // Sets the appropriate bools
                    swimmingNoiseHasStarted = true;
                    swimmingNoiseHasStopped = false;

                }

            }

        }
        // If the player is on land and moving around (via controller input)
        else if (fplReference.IsGrounded && !wpReference.isSubmerged &&
            (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown) ||
            OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)))
        {

            if (!walkingNoiseHasStarted && walkingNoiseHasStopped)
            {

                // Plays the walking audio
                walkingSource.Play();

                // Stops underwater ambience
                swimmingSource.Stop();

                // Returns outside noise to its original volume
                if (swimmingNoiseHasStarted && !swimmingNoiseHasStopped)
                {

                    for (int k = 0; k < outsideAudioArray.Length; k++)
                    {

                        outsideAudioArray[k].volume = correspondingAudioLevels[k];

                    }

                }

                // Sets the appropriate bools
                walkingNoiseHasStarted = true;
                walkingNoiseHasStopped = false;
                swimmingNoiseHasStarted = false;
                swimmingNoiseHasStopped = true;

            }

        }

    }

}
