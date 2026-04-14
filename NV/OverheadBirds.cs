/*Charlie Dye - 2025.04.26

This is the script for bird audio from overhead */

using System.Collections;
using UnityEngine;

public class OverheadBirds : MonoBehaviour
{

    // Audio source this script is attached to
    private AudioSource attachedAudio;

    [Header("Transform Variable")]
    public Transform newAudioLocation;

    [Header("Float Variables")]
    [Range(0f, 500f)] public float rangeOfOccurrence;
    [Range(0f, 1f)] public float rangeOfRandomness;
    [Range(1f, 10f)] public float speedOfTransit;

    void Start()
    {

        // Fetches the audio component
        attachedAudio = GetComponent<AudioSource>();

        // If the range is equal to zero, then it will be randomized
        if (rangeOfOccurrence == 0f) rangeOfOccurrence = Random.Range(50f, 500f);

        // Randomizes the starting position of the audio
        attachedAudio.transform.position = new Vector3(Random.Range(-rangeOfOccurrence, rangeOfOccurrence), attachedAudio.transform.position.y, Random.Range(-rangeOfOccurrence, rangeOfOccurrence));

        // Starts the coroutine via the associated method
        RandomizeCoroutineParameters();

    }

    public void RandomizeCoroutineParameters()
    {

        // Creates a new randomized position for the audio source to move to
        newAudioLocation.position = new Vector3(Random.Range(-rangeOfOccurrence, rangeOfOccurrence), attachedAudio.transform.position.y, Random.Range(-rangeOfOccurrence, rangeOfOccurrence));

        // Randomizes pitch and volume
        attachedAudio.pitch = Random.Range(attachedAudio.pitch - rangeOfRandomness, attachedAudio.pitch + rangeOfRandomness);
        attachedAudio.volume = Random.Range(attachedAudio.volume - rangeOfRandomness, attachedAudio.volume);

        // Starts the coroutine
        StartCoroutine(MoveAudio());

    }

    public IEnumerator MoveAudio()
    {

        while (attachedAudio.transform.position != newAudioLocation.position)
        {

            attachedAudio.transform.position = Vector3.MoveTowards(attachedAudio.transform.position, newAudioLocation.position, speedOfTransit * Time.fixedDeltaTime);

            yield return new WaitForSecondsRealtime(0.01f);

        }

        RandomizeCoroutineParameters();

    }

}
