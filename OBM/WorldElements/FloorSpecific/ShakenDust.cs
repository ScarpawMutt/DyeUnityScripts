/* Charlie Dye, PACE Team - 2025.12.09

This is the script for the dust falling from the ceiling on some floors */

using UnityEngine;

public class ShakenDust : MonoBehaviour
{

    [Header("Dust Object Array")]
    [Tooltip("The array of particle systems that emulate falling dust.")] public ParticleSystem[] dustColumns;

    [Header("Float Variable")]
    [Tooltip("The duration, in seconds, that the dust falls.")] public float dustDuration;

    void Start()
    {

        // If the array has a length of zero (i.e., it is completely null), then this script will self-destruct
        if (dustColumns.Length == 0) Destroy(this);

        // If any of the array's elements are left null, then this script will self-destruct
        for (int i = 0; i < dustColumns.Length; i++)
        {

            if (dustColumns[i] == null) Destroy(this);

        }

        /* If the duration float is less than zero, the negative sign will be negated;
        if it is zero, meanwhile, it will be set to a positive value */
        if (dustDuration < 0f) dustDuration *= -1f;
        else if (dustDuration == 0f) dustDuration = 1f;

    }

    [Tooltip("Causes the array of dust particles to begin falling.")] public void StartDustfall()
    {

        // Goes through the particle system array, starting each one
        for (int j = 0; j < dustColumns.Length; j++)
        {

            dustColumns[j].Play();

        }

        // Initiates the stopping method after a given time
        Invoke(nameof(StopDustfall), dustDuration);

    }

    public void StopDustfall()
    {

        // Goes through the particle system array, stopping each one
        for (int k = 0; k < dustColumns.Length; k++)
        {

            dustColumns[k].Stop();

        }

    }

}
