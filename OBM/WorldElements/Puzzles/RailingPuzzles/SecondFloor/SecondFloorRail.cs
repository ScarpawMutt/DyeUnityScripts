/* Charlie Dye, PACE Team - 2024.11.20

This is the script for the railing puzzle on the second floor */

using UnityEngine;

public class SecondFloorRail : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the Elevator.")] public ElevatorBehavior ebReference;
    [Tooltip("The script attached to the camera quaking effect.")] public CameraQuake cqReference;
    [Tooltip("The script attached to the master dialogue module.")] public DialogueControl dcReference;
    [Tooltip("The script attached to the ominous ambient glow on the fourth floor")] public FireGlow fgReference;
    [Tooltip("The script attached to the fire spawning mechanism.")] public FireSpawner fsReference;
    [Tooltip("The script attached to the wrench pointer.")] public PointerBehavior pbReference;
    [Tooltip("The script attached to the shaking dust effect.")] public ShakenDust sdReference;
    [Tooltip("The script attached to the needles on the third floor pressure gauge.")] public NeedleMovement[] nmReferences;

    [Header("Particle Systems")]
    [Tooltip("The array of particle systems that simulate the explosion.")] public ParticleSystem[] explosionJets;

    [Header("Float Variable")]
    [Tooltip("The duration of the explosion.")] public float explosionLength;

    [Header("Audio")]
    [Tooltip("The audio source responsible for the explosion noise.")] public AudioSource explosionNoise;
    [Tooltip("The audio source responsible for the Steve screaming.")] public AudioSource steveScream;

    [Header("Boolean Variable")]
    [Tooltip("Whether or not any of the buttons on the panel has been pressed.")] public bool buttonHasBeenPressed = false;
    private bool hasExecuted = false;

    void FixedUpdate()
    {
        
        // If any button in the puzzle is pressed, then the method will execute
        if (buttonHasBeenPressed && !hasExecuted && ebReference.arrayIndexer == 2)
        {

            // Continues the meltdown
            ContinueMeltdown();

            // Stops the meltdown after a set amount of time
            Invoke(nameof(StopMeltdown), explosionLength);

            // Makes the auxiliary bool true to prevent multiple executions in FixedUpdate()
            hasExecuted = true;

        }

    }

    public void ContinueMeltdown()
    {

        // Creates the explosion
        for (int j = 0; j < explosionJets.Length; j++)
        {

            explosionJets[j].Play();

        }

        // Causes the quaking effect
        cqReference.canQuake = true;

        // Plays audio
        explosionNoise.Play();
        steveScream.Play();

        // Causes the dust on that floor to get kicked up from the ceiling
        sdReference.StartDustfall();

        // Forces the fire glow to disappear both as a GameObject and a script
        fgReference.enabled = false;
        fgReference.gameObject.SetActive(false);

        // Causes the needles on the level 3 pressure panel to go haywire
        for (int k = 0; k < nmReferences.Length; k++)
        {

            nmReferences[k].canRotate = true;

        }

        // Allows fire to begin spawning on the fifth floor
        fsReference.canBeginSpawning = true;

    }

    public void StopMeltdown()
    {

        // Stops the quaking effect
        cqReference.canQuake = false;

        // Enables the wrench's pointer
        pbReference.renderPointer = true;

        // Stops the explosion
        for (int l = 0; l < explosionJets.Length; l++)
        {

            explosionJets[l].Stop();

        }

        // Causes Major Miner to continue his rant
        dcReference.PrepareSpeechBlock(dcReference.dialogueSecondFloor, dcReference.pausesSecondFloor, 2, 3, true);

    }

}
