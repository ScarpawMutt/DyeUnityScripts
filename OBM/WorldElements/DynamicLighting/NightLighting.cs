/* Charlie Dye, PACE Team - 2024.12.10

This is the script for the lighting in the play area */

using System;
using UnityEngine;

public class NightLighting : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the world clock.")] public DigitalClock dcReference;

    [Header("Integer Variables")]
    [Tooltip("The time, converted to seconds, that this light should switch on at.")] [Range(0, 86400)] public int switchOnAtThisTime;
    [Tooltip("The time, converted to seconds, that this light should switch off at.")] [Range(0, 86000)] public int switchOffAtThisTime;

    [Header("Light Variable")]
    [Tooltip("The light source to be handled.")] public Light nightLight;

    void FixedUpdate()
    {

        // If the sun is between two given angles, then the lights will come on; otherwise, they will shut off
        if (dcReference.timeInSeconds >= switchOnAtThisTime || dcReference.timeInSeconds <= switchOffAtThisTime) nightLight.enabled = true;
        else nightLight.enabled = false;

    }

}
