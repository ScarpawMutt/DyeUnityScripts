/* Charlie Dye, PACE Team - 2026.03.19

This is the script for the wrench audio when active */

using UnityEngine;

public class WrenchAudio : MonoBehaviour
{

    // Audio source
    private AudioSource metalPipeNoise;

    // Boolean variable
    private bool hasImpacted = false;

    void Awake()
    {

        /* Attempts to fetch the audio source reference by locating it in this object's children;
        if that fails, then this script will self-destruct */
        if (gameObject.GetComponentInChildren<AudioSource>()) metalPipeNoise = gameObject.GetComponentInChildren<AudioSource>();
        else Destroy(this);

    }

    void OnCollisionEnter(Collision barrier)
    {
        
        // If the wrench has not yet impacted the invisible barrier, but is now doing so
        if (!hasImpacted)
        {

            // If the barrier is correctly tagged
            if (barrier.gameObject.CompareTag("Invisible Wall"))
            {

                // Plays the noise
                metalPipeNoise.Play();

                // Switches the Boolean to true
                hasImpacted = true;

            }

        }

    }

}
