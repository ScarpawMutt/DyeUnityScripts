/* Charlie Dye, PACE Team - 2025.12.08

This is the script for the text on the railing puzzle gadgets */

using System.Collections;
using UnityEngine;
using TMPro;

public class PanelText : MonoBehaviour
{

    [Header("Float Variables")]
    [Tooltip("The fading speed of the text alpha.")] public float fadeRate;
    [Tooltip("The rate that the coroutine refreshes.")] public float refreshRate;
    [Tooltip("The duration that non-flashing text stays on the screen.")] public float textDuration;

    [Header("Text Variables")]
    [Tooltip("The text that appears when Steve's hand is accepted by the machine.")] public TextMeshPro unstableText;
    [Tooltip("The text that appears when the player's hand is denied by the machine.")] public TextMeshPro stableText;
    [Tooltip("The text that appears when the machine is idle.")] public TextMeshPro dangerText;

    [Header("Boolean Variables")]
    [Tooltip("Can the text animate?")] public bool canAnimate;
    [Tooltip("The boolean that determines whether the alpha of the text should increase or decrease.")] public bool raiseAlpha;
    [Tooltip("The boolean to override the coroutine entirely.")] public bool overrideAnimation;
    private bool coroutineHasStarted;

    void Start()
    {

        // If any text is left disabled, then this will enable them before setting their alpha values to zero
        if (!unstableText.enabled) unstableText.enabled = true;
        if (!stableText.enabled) unstableText.enabled = true;
        if (!dangerText.enabled) unstableText.enabled = true;

        if (unstableText.alpha != 0f) unstableText.alpha = 0f;
        if (stableText.alpha != 0f) stableText.alpha = 0f;
        if (dangerText.alpha != 0f) dangerText.alpha = 0f;

        // If the Boolean values are improperly configured, then this will correct them
        if (canAnimate) canAnimate = false;
        if (overrideAnimation) overrideAnimation = false;
        if (coroutineHasStarted) coroutineHasStarted = false;

    }

    void FixedUpdate()
    {

        // If the flash coroutine must begin, then this will trigger it once with the kill bool
        if (canAnimate && !coroutineHasStarted)
        {

            StartCoroutine(FlashText());
            coroutineHasStarted = true;

        }

    }

    public void WinOrLoss (bool playerWonPuzzle)
    {

        /* If the local bool returns true, then the "STABLE" text will fade in;
        otherwise, the "DANGER" text will fade in */
        if (playerWonPuzzle) StartCoroutine(FadeInText(stableText));
        else StartCoroutine(FadeInText(dangerText));

        // Overrides the flashing coroutine
        overrideAnimation = true;

    }

    public IEnumerator FlashText()
    {

        // While-loop for the text fade
        while (unstableText != null)
        {

            // If the animation can go on without being overridden
            if (!overrideAnimation)
            {

                // If the text needs to fade in
                if (raiseAlpha)
                {

                    if (unstableText.alpha < 1f) unstableText.alpha += fadeRate;
                    else raiseAlpha = false;

                }
                // If the text needs to fade out
                else
                {

                    if (unstableText.alpha > 0f) unstableText.alpha -= fadeRate;
                    else raiseAlpha = true;

                }

                // Repeats the coroutine using the refresh rate in real time
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            else
            {

                // Fades the text out until it becomes transparent
                if (unstableText.alpha < 1f) unstableText.alpha -= fadeRate;

                // Repeats the coroutine using the refresh rate in real time
                yield return new WaitForSecondsRealtime(refreshRate);

            }

        }

        yield break;

    }

    public IEnumerator FadeInText(TextMeshPro textToFade)
    {

        // While-loop for the text fade
        while (textToFade != null)
        {

            // Fades the text parameter into view
            if (textToFade.alpha < 1f)
            {

                // Increments the alpha with the defined rate
                textToFade.alpha += fadeRate;

                // Repeats the coroutine using the refresh rate in real time
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            else yield break;

        }

    }

}
