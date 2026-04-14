/* Charlie Dye, PACE Team - 2025.03.17

This is the script that controls leaking steam from pipes */

using System.Collections;
using UnityEngine;

public class LeakingSteam : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the third floor combination puzzle.")] public CombinationPuzzle cpReference;
    [Tooltip("The script attached tp the second floor railing puzzle.")] public SecondFloorRail sfrReference;
    [Tooltip("The script attached to the Elevator.")] public ElevatorBehavior ebReference;
    public FanManager fanManager;

    [Header("Numerical Variable")]
    [Tooltip("The rate at which the steam activation/deactivation is shut off.")] public float rerollRate;
    [Tooltip("The maximum amount from the reroll rate that the activation/deactivation can deviate from.")] public float randomizationVariable;
    private int corresponding_value;

    // Particle and audio variables
    private AudioSource leakNoise;
    private ParticleSystem thisLeak;
    public int OutletNumber;

    // Boolean variables
    private bool hasStarted = false;
    private bool hasStopped = false;

    [Header("On/Off Type")]
    [Tooltip("Does the steam turn on and off randomly (Off) or is it turned on untill the puzzle is solved (On)")] public bool isStatic = false;

    void Start()
    {
        // Fetches the FanManager script
        fanManager = FindAnyObjectByType<FanManager>();

        // Fetches the particle system and audio source components
        leakNoise = gameObject.GetComponent<AudioSource>();
        thisLeak = gameObject.GetComponent<ParticleSystem>();

        // Stops the flow of steam
        thisLeak.Stop();
    }

    void FixedUpdate()
    {
        // If the meltdown has started
        if (ebReference.arrayIndexer == 3 && !hasStarted)
        {
            if (isStatic)
            {
                thisLeak.Play();
                
                fanManager.ToggleFan(OutletNumber, "1");

                hasStarted = true;
            }
            else
            {
                // The coroutine starts
                StartCoroutine(RandomizeLeak());

                // The kill bool prevents excessive executions of the code block
                hasStarted = true;
            }
        }

        // If the combination lock has been cracked
        if (cpReference.puzzleIsComplete && !hasStopped)
        {

            // Stops the leak permanently
            thisLeak.Stop();

            // The kill bool prevents excessive executions of the code block
            hasStopped = true;

            fanManager.ToggleFan(OutletNumber, "0");

        }

    }

    public void ProcessOutput()
    {

        /* Logic for the determined random number.
        if the output is zero, then steam will flow out and sound will play;
        otherwise, if it is one, then steam and sound will stop */
        if (corresponding_value == 0)
        {

            leakNoise.Stop();
            thisLeak.Play();

        }
        else if (corresponding_value == 1)
        {

            leakNoise.Stop();
            thisLeak.Stop();

        }

    }

    public IEnumerator RandomizeLeak()
    {

        while (thisLeak != null && !cpReference.puzzleIsComplete)
        {

            /* Randomizes the corresponding "boolean" value to be either zero or one;
            the two is exclusive and will never be chosen on its own */
            corresponding_value = Random.Range(0, 2);

            // Takes that random value into the processing method
            ProcessOutput();

            // Rerolls again in a randomized amount of real-time seconds
            yield return new WaitForSecondsRealtime(Random.Range(rerollRate - randomizationVariable, rerollRate + randomizationVariable));

        }

    }

}
