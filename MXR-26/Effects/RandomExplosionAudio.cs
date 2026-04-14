/* Charlie Dye - ECT 4440 - 2026.03.19

This is the script for random explosion audio */

using UnityEngine;

public class RandomExplosionAudio : MonoBehaviour
{

    [Header("Audio Clips")]
    [Tooltip("An array of explosion sound effects to choose from.")] public AudioClip[] explosionSounds;

    [Header("Float Variable")]
    [Tooltip("The amount of pitch that the played sound effect can vary in relative to the default value entered.")] public float rangeOfRandomPitch;
    private float enteredPitch;
    private float lengthOfClip;

    // Audio source
    private AudioSource attachedSource;

    void Awake()
    {
        
        // If there are no clips to choose from, then this object will self-destruct
        if (explosionSounds == null) Destroy(gameObject);

    }

    void OnEnable()
    {

        // Attempts to fetch the audio source component within this object; if that fails, then this object will self-destruct
        if (gameObject.GetComponent<AudioSource>()) attachedSource = gameObject.GetComponent<AudioSource>();
        else Destroy(gameObject);

        // Records the default pitch value (the one entered in; it does not need to be 1)
        enteredPitch = attachedSource.pitch;

        // Selects a random audio clip
        attachedSource.clip = explosionSounds[Random.Range(0, explosionSounds.Length)];

        // Alters the selected clip's pitch at random
        attachedSource.pitch = enteredPitch + Random.Range(-rangeOfRandomPitch, rangeOfRandomPitch);

        // Records the length of the clip
        lengthOfClip = attachedSource.clip.length;

        // Plays the audio
        attachedSource.Play();

        // Destroys this object after the clip has finished
        Destroy(gameObject, lengthOfClip);

    }

}
