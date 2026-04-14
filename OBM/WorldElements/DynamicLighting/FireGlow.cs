/* Charlie Dye, PACE Team - 2025.01.14

This is the script for flashing light behavior */

using System.Collections;
using UnityEngine;

public class FireGlow : MonoBehaviour
{

    [Header("Light Variable")]
    [Tooltip("The light to control.")] public Light fireLight;

    [Header("Color Variables")]
    [Tooltip("The maximum allowed green attribute of the glow.")] [Range(0, 255)] public int maxGreen;
    [Tooltip("The maximum allowed range of the glow.")] public float maxIntensity;
    [Tooltip("The actual target value of the fire's green hue as a product of the maximum allowed value.")] public float actualGreen;
    [Tooltip("The new color that the actual fire color should shift towards.")] public Color newColor;

    [Header("Float Variables")]
    [Tooltip("The slowest possible change that the fire's color values can change.")] public float minTransitionRange;
    [Tooltip("The fastest possible change that the fire's color values can change.")] public float maxTransitionRange;
    [Tooltip("The least amount of time that the color attributes can stagnate at before it must change again.")] public float minRefreshRate;
    [Tooltip("The most amount of time that the color attributes can stagnate at before it must change again.")] public float maxRefreshRate;
    [Tooltip("The actual random float generated per coroutine cycle as a product of the two above refresh bounds.")] public float actualRefreshRate;
    [Tooltip("The amount at which the fire's intensity increments per cycle.")] public float incrementRate;
    [Tooltip("The fixed rate at which the fire glow fades into view.")] public float transitionRefreshRate;
    private float initialIntensity;

    [Header("Boolean Variables")]
    [Tooltip("Should the coroutine start now?")] public bool signalToActivate = false;
    [Tooltip("Has the glow fully faded into view yet?")] public bool transitionIsComplete = false;
    [Tooltip("Should the fire change its hue?")] public bool continueChange = false;
    private bool coroutineHasActivated = false;

    void Start()
    {

        // If the color maxima are set to zero, then this will automatically set them to the highest RGBA value
        if (maxGreen == 0) maxGreen = 255;
        if (maxIntensity == 0) maxIntensity = 1f;

        // If the float values are improperly set, then this will configure them properly
        if (minTransitionRange <= 0f) minTransitionRange = 0.1f;
        if (maxTransitionRange <= 0f || maxTransitionRange < minTransitionRange) maxTransitionRange = minTransitionRange + 0.1f;
        if (minRefreshRate <= 0f) minRefreshRate = 0.1f;
        if (maxRefreshRate <= 0f || maxRefreshRate < minRefreshRate) maxRefreshRate = minRefreshRate + 0.1f;
        if (incrementRate <= 0f) incrementRate = 0.01f;
        if (transitionRefreshRate <= 0f) transitionRefreshRate = 0.1f;

        // Sets the private intensity float to the value loaded when the scene is started before setting it to zero
        initialIntensity = fireLight.intensity;
        if (fireLight.intensity != 0f) fireLight.intensity = 0f;

    }

    void FixedUpdate()
    {
        
        // If the coroutine must begin
        if (signalToActivate && !coroutineHasActivated)
        {

            // Starts the coroutine and switches the kill bool
            StartCoroutine(DynamicGlow());
            coroutineHasActivated = true;

        }

        // If the main section of the coroutine is active, then color control will kick in as a separate method
        if (transitionIsComplete && continueChange && fireLight.color.g != newColor.g) ColorControl();

    }

    public void ColorControl()
    {

        // If the current fire's green value is lower than the target value
        if (fireLight.color.g < newColor.g)
        {

            // Adjusts the color until it matches, at which point the prohobitive bool will become false
            if (fireLight.color.g < newColor.g) fireLight.color = Color.Lerp(fireLight.color, newColor, transitionRefreshRate);
            else continueChange = false;

        }
        // If the current fire's green value is higher than the target value
        else if (fireLight.color.g > newColor.g)
        {

            // Adjusts the color until it matches, at which point the prohobitive bool will become false
            if (fireLight.color.g > newColor.g) fireLight.color = Color.Lerp(fireLight.color, newColor, transitionRefreshRate);
            else continueChange = false;

        }

    }

    public IEnumerator DynamicGlow()
    {

        // While the fire light source is not null
        while (fireLight != null)
        {

            // If the transition is incomplete
            if (!transitionIsComplete)
            {

                /* If the glow's intensity is not at its benchmark yet, it will increment until it reaches it;
                otherwise, the bool will switch and the coroutine will reroute */
                if (fireLight.intensity < initialIntensity) fireLight.intensity += incrementRate;
                else transitionIsComplete = true;

                // Repeats the cycle using the designated refresh rate
                yield return new WaitForSecondsRealtime(transitionRefreshRate);

            }
            // If the transition is complete
            else
            {

                // Randomizes the refresh rate of this coroutine segment using the player-entered parameters
                actualRefreshRate = Random.Range(minRefreshRate, maxRefreshRate);

                // If the fire cannot change color, this will enable it
                if (!continueChange) continueChange = true;

                // Randomizes the "green float" to a value between zero and the maximum player-entered value
                actualGreen = Random.Range(0f, maxGreen / 255f);

                // Constructs the new color using the randomized "green float"
                newColor = new(1f, actualGreen, 0f, 1f);

                // Repeats the cycle using the randomized refresh rate
                yield return new WaitForSecondsRealtime(actualRefreshRate);

            }

        }

        yield break;

    }

}
