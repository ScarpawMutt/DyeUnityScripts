/* Charlie Dye, PACE Team - 2024.10.31

This is the spooky script for the doomed man in the project */

using System.Collections;
using UnityEngine;

public class SteveArm : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the first floor puzzle.")] public FirstFloor ffReference;

    [Header("Object Array")]
    [Tooltip("The array of GameObjects that form steve's arm joints, in sequential order, from shoulder to wrist.")] public GameObject[] armJoints;
    [Tooltip("The ideal position of the arm object when extended out.")] public Transform targetedExtension;
    [Tooltip("The ideal position of the arm object when retracted.")] public Transform targetedRetraction;

    [Header("Float Variables")]
    [Tooltip("The duration that Steve's arm moves in a single direction before alternating.")] public float durationOfWavePhase;
    [Tooltip("The speed of the waving animation.")] public float waveSpeed;
    [Tooltip("The amount of time that Steve waves to the player.")] public float wavingDuration;
    [Tooltip("How fast Steve extends his arm out.")] public float extensionSpeed;
    [Tooltip("The refresh rate of the lateral motion coroutine.")] public float extendRefreshRate;
    private float waveTime;
    private int phaseIndexer;

    private bool coroutinesHaveExecuted;
    private bool reverseWave;

    void Start()
    {

        /* If the wave parameters are less than or equal to zero (an unworkable value),
        they will automatically be replaced by positive values */
        if (durationOfWavePhase == 0f) durationOfWavePhase = 1f;
        else if (durationOfWavePhase < 0f) durationOfWavePhase *= -1f;
        if (waveSpeed == 0f) waveSpeed = 1f;
        else if (waveSpeed < 0f) waveSpeed *= -1f;
        if (extendRefreshRate == 0f) extendRefreshRate = 0.03f;
        else if (extendRefreshRate < 0f) extendRefreshRate *= -1f;

        // Sets the Booleans to false, if not already so
        coroutinesHaveExecuted = false;
        reverseWave = false;

        // Sets the value of the phase indexer to negative one (this is important for proper coroutine cycling)
        phaseIndexer = -1;

        // Moves the arm to the retracted position
        gameObject.transform.position = targetedRetraction.position;

    }

    void FixedUpdate()
    {

        // If the wave animation must start, then the appropriate method will execute
        if (ffReference.steveIsWaving) WaveMotion();

    }

    private void WaveMotion()
    {

        // If the coroutine has not already started
        if (!coroutinesHaveExecuted)
        {

            // Starts the coroutines
            StartCoroutine(ChangeWaveValue());
            StartCoroutine(ExtendReach(true));

            // Switches the value of the kill Boolean to prevent excessive executions
            coroutinesHaveExecuted = true;

        }

        // Logic that determines the arm direction and acceleration depending on the index value
        if (phaseIndexer % 2 == 0) waveTime += waveSpeed * Time.fixedDeltaTime;
        else if (phaseIndexer % 2 == 1) waveTime -= waveSpeed * Time.fixedDeltaTime;
        if (phaseIndexer < 2) reverseWave = true;
        else reverseWave = false;

        // For-loop that runs through the length of the arm joint array
        for (int j = 0; j < armJoints.Length; j++)
        {

            // If the joint is not the last in the array
            if (j != armJoints.Length - 1)
            {

                /* If the reverse wave Boolean is false, then the arm will wave forward;
                otherwise, it will wave backward */
                if (!reverseWave) armJoints[j].transform.Rotate(Vector3.forward * waveTime, Space.Self);
                else armJoints[j].transform.Rotate(Vector3.back * waveTime, Space.Self);

            }

        }

    }

    private IEnumerator ChangeWaveValue()
    {

        // While-loop that returns true as long as the array is not wiped and the arm must wave
        while (ffReference.steveIsWaving)
        {

            /* If the phase index value is less than three, then it will increment by one;
            otherwise, it will reset to zero */
            if (phaseIndexer < 3) phaseIndexer++;
            else phaseIndexer = 0;

            // Refreshes the coroutine
            yield return new WaitForSecondsRealtime(durationOfWavePhase);

        }

        // Breaks the coroutine
        yield break;

    }

    private IEnumerator ExtendReach(bool extendOut)
    {

        // While-loop that returns true as long as the object is not nullified
        while (gameObject != null && ffReference.steveIsWaving)
        {

            // If the arm must extend out
            if (extendOut)
            {

                // If the arm's current position is not at its target position
                if (gameObject.transform.position != targetedExtension.position)
                {

                    // Interpolates toward the target position
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetedExtension.position, extensionSpeed);

                    // Refreshes the coroutine
                    yield return new WaitForSeconds(extendRefreshRate);

                }
                else
                {

                    // Sets a timer for a refresh of the coroutine, stopping this cycle beforehand
                    Invoke(nameof(LoopAroundCoroutine), wavingDuration);

                    // Breaks the coroutine
                    yield break;

                }

            }
            // If the arm must retract back
            else
            {

                // If the arm's current position is not at its target position
                if (gameObject.transform.position != targetedRetraction.position)
                {

                    // Interpolates toward the target position
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetedRetraction.position, extensionSpeed);

                    // Refreshes the coroutine
                    yield return new WaitForSeconds(extendRefreshRate);

                }
                else
                {

                    // Disables the arm object
                    gameObject.SetActive(false);

                    // Breaks the coroutine
                    yield break;

                }

            }

        }

    }

    private void LoopAroundCoroutine()
    {

        // Restarts the coroutine
        StartCoroutine(ExtendReach(false));

    }

}
