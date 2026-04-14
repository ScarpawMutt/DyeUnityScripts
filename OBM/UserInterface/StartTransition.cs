/* Charlie Dye, PACE Team - 2025.12.09

This is the script for the darkening effect on the camera */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartTransition : MonoBehaviour
{

    [Header("UI Variables")]
    [Tooltip("The family of text that is on-screen before the main experience begins.")] public GameObject preambleText;
    [Tooltip("The darkness that covers the UI.")] public Image darknessImage;

    [Header("Numerical Variables")]
    [Tooltip("The amount that the screen stays black before brightening.")] public float initialPause;
    [Tooltip("The increment that the coroutine subtracts from the darkness each time.")] public float amountToSubtract;
    [Tooltip("The divisor (amount that the original volume is divided by) to increment the gradual volume raise by each time.")] public float soundIncrement;
    [Tooltip("The refresh rate of the coroutines.")] public float refreshRate;
    [Tooltip("The period of time that the preamble text is on-screen per cycle.")] public float textFlashRate;
    [Tooltip("The number of times that the preamble text cycles before the prologue continues.")] public int timesToCycleText;
    private int actualTimesCycled = 0;

    // Boolean variable
    private bool textIsVisible = false;

    void Start()
    {

        // If the image or its material slot are left null, then this script will self-destruct
        if (darknessImage == null) Destroy(this);
        else if (darknessImage.material == null) Destroy(this);
        else if (preambleText == null) Destroy(this);

        // If the blinding material is not entirely black and with an initial alpha value of one, then this will correct it for later
        if (darknessImage.color != new Color(0f, 0f, 0f, 1f)) darknessImage.color = new Color(0f, 0f, 0f, 1f);

        // If the preamble text parent is active, then this will deactivate it temporarily
        if (preambleText.activeInHierarchy) preambleText.SetActive(false); 

        // If the float and integer values are set improperly, then this will correct them
        if (amountToSubtract == 0f) amountToSubtract = 0.01f;
        else if (amountToSubtract < 0f) amountToSubtract *= -1f;
        if (refreshRate == 0f) refreshRate = 0.03f;
        else if (refreshRate < 0f) refreshRate *= -1f;
        if (soundIncrement == 0f) soundIncrement = 10f;
        else if (soundIncrement < 0f) soundIncrement *= -1f;
        if (textFlashRate == 0f) textFlashRate = 1f;
        else if (textFlashRate < 0f) textFlashRate *= -1f;
        if (timesToCycleText == 0) timesToCycleText = 5;
        else if (timesToCycleText < 0) timesToCycleText *= -1;

        // Transitions to the coroutine after the pause completes
        Invoke(nameof(StartPreamble), initialPause);

    }

    private void StartPreamble()
    {

        // Starts the associated coroutine
        StartCoroutine(Preamble());

    }

    private IEnumerator Preamble()
    {

        // While-loop that returns true as long as the preamble text is not destroyed
        while (preambleText != null)
        {

            // If the preamble text is currently visible in the player's HUD
            if (textIsVisible)
            {

                // Deactivates the text
                preambleText.SetActive(false);

                // Changes the associated Boolean value
                textIsVisible = false;

                /* If the text has cycled enough times, then the next coroutine will activate and this coroutine will break its cycle;
                otherwise, it will refresh as normal */
                if (actualTimesCycled >= timesToCycleText)
                {

                    StartCoroutine(GradualBrightening());
                    yield break;

                }
                else yield return new WaitForSecondsRealtime(textFlashRate);

            }
            // If the preamble text is currently invisible in the player's HUD
            else
            {

                // Activates the text
                preambleText.SetActive(true);

                // Changes the associated Boolean value
                textIsVisible = true;

                // Increments the private integer by one
                actualTimesCycled++;

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(textFlashRate);

            }

        }

    }

    private IEnumerator GradualBrightening()
    {

        // While the blinding effect is not null
        while (darknessImage != null)
        {

            // If the blinding color's alpha value is less than one (i.e., completely opaque)
            if (darknessImage.color.a > 0f)
            {

                // Appends an incremental value to the alpha value
                darknessImage.color = new Color(0f, 0f, 0f, darknessImage.color.a - amountToSubtract);

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            else
            {

                // Renders the darkness image transparent
                darknessImage.color = new(0f, 0f, 0f, 0f);

                // Breaks the coroutine
                yield break;

            }

        }

    }

}
