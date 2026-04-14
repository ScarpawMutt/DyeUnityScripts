/* Charlie Dye, PACE Team - 2025.01.14

This is the script for flashing light behavior */

using System.Collections;
using UnityEngine;

public class FlashingLight : MonoBehaviour
{

    [Header("Light Variable")]
    [Tooltip("The fire alarm's light source.")] public Light lightObject;

    [Header("Float Variables")]
    [Tooltip("The increment at which the light source increases/decreases in intensity per coroutine cycle.")] public float fadeRate;
    [Tooltip("The refresh rate of the coroutine.")] public float refreshRate;
    private float originalIntensity;

    [Header("Boolean Variables")]
    [Tooltip("Should the light source be visibly pulsing?")] public bool pulseIsActive;
    [Tooltip("Should the coroutine begin?")] public bool startCoroutine;
    private bool coroutineHasStarted;
    private bool lightIntensityIncreases;

    void Start()
    {

        // Records the original intensity of the light source
        originalIntensity = lightObject.intensity;

        // If the floats are configured incorrectly, then this will correct them
        lightObject.intensity = 0f;
        if (fadeRate == 0f) fadeRate = 1f;
        else if (fadeRate < 0f) fadeRate *= -1f;
        if (refreshRate == 0f) refreshRate = 1f;
        else if (refreshRate < 0f) refreshRate *= -1f;

        // If the bools are configured incorrectly, then this will correct them
        if (pulseIsActive) pulseIsActive = false;
        if (startCoroutine) startCoroutine = false;
        coroutineHasStarted = false;
        lightIntensityIncreases = true;

    }

    void FixedUpdate()
    {

        // If the coroutine must start, then it will execute here via the kill bool
        if (startCoroutine && !coroutineHasStarted)
        {

            StartCoroutine(CreateFlashingEffect());
            coroutineHasStarted = true;

        }

    }

    public IEnumerator CreateFlashingEffect()
    {

        while (lightObject != null)
        {

            // If the light must increase in intensity
            if (lightIntensityIncreases)
            {

                // If the intensity is less than the original intensity (its target)
                if (lightObject.intensity < originalIntensity)
                {

                    lightObject.intensity += fadeRate;
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                else
                {

                    lightIntensityIncreases = false;
                    yield return new WaitForSecondsRealtime(refreshRate);

                }

            }
            // If the light must decrease in intensity
            else
            {

                // If the intensity is greater than zero (its target)
                if (lightObject.intensity > 0f)
                {

                    lightObject.intensity -= fadeRate;
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                else
                {

                    /* If the pulse is still active, then the coroutine cycle will repeat and the light will reignite;
                    otherwise, the coroutine will break the cycle */
                    if (pulseIsActive)
                    {

                        lightIntensityIncreases = true;
                        yield return new WaitForSecondsRealtime(refreshRate);

                    }
                    else yield break;

                }

            }

        }

    }

}
