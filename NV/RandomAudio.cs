/* Charlie Dye - 2025.04.19

This is the script for random ambience in the scene */

using UnityEngine;

public class RandomAudio : MonoBehaviour
{

    // Audio source this script is attached to
    private AudioSource attachedSource;

    [Header("Tracks to Choose From")]
    public AudioClip[] clipArray;

    [Header("Float Variable")]
    [Range(0f, 1f)] public float marginOfRandomization;

    void Awake()
    {
        
        // Fetches the audio source
        attachedSource = GetComponent<AudioSource>();

        // If the settings are incorrect, this script automatically fixes them
        if (!attachedSource.playOnAwake) attachedSource.playOnAwake = true;
        if (!attachedSource.loop) attachedSource.loop = true;

        // Randomizes audio using the method
        SelectSound();

    }

    public void SelectSound()
    {

        // Randomizes pitch and volume of audio
        attachedSource.pitch = Random.Range(1f - marginOfRandomization, 1f + marginOfRandomization);
        attachedSource.volume = Random.Range(1f - marginOfRandomization, 1f);

        // Local integer for indexing the array variables
        int randomIndexer = Random.Range(0, clipArray.Length);

        // Plays audio using that randomization
        attachedSource.clip = clipArray[randomIndexer];

    }

}
