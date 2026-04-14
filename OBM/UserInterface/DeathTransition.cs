/* Charlie Dye, PACE Team - 2026.02.27

This is the script for if the player dies on the elevator */

using System.Collections.Generic;
using UnityEngine;

public class DeathTransition : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the loss animation.")] public LossTransition ltReference;

    [Header("Audio Source and Array")]
    [Tooltip("A list of all other audio sources. Do not modify this directly.")] public List<AudioSource> otherAudio;
    [Tooltip("The overriding scream audio that plays right when the player is killed.")] public AudioSource[] deathAudio;

    // Boolean variable
    private bool soundHasExecuted = false;

    void Awake()
    {

        // If any overriding audio is not correctly tagged, then this script will self-destruct
        for (int i = 0; i < deathAudio.Length; i++)
        {

            if (!deathAudio[i].CompareTag("Insta-Kill")) Destroy(this);

        }

    }

    void Start()
    {

        // Assembles the list
        GetAudioList();

    }

    private void GetAudioList()
    {

        // Local array that stores all audio sources
        AudioSource[] preliminaryArray = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        // For-loop that executes the same number of times as the array's length
        for (int j = 0; j < preliminaryArray.Length; j++)
        {

            // If the audio source is not specially tagged, then it will be appended to the public list
            if (preliminaryArray[j].CompareTag("Untagged")) otherAudio.Add(preliminaryArray[j]);

        }

    }

    void FixedUpdate()
    {
        
        // If the player "dies", then the below method will execute
        if (ltReference.playerHasDied && !soundHasExecuted)
        {

            PlayKillSound();
            soundHasExecuted = true;

        }

    }

    private void PlayKillSound()
    {

        // Goes through the list of audio sources and mutes each one suddenly
        for (int k = 0; k < otherAudio.Count; k++)
        {

            otherAudio[k].mute = true;

        }

        // Plays the only sounds the "dead" player hears otherwise
        for (int l = 0; l < deathAudio.Length; l++)
        {

            deathAudio[l].Play();

        }

    }

}
