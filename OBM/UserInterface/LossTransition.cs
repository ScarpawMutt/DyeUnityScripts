/* Charlie Dye, PACE Team - 2025.12.09

This is the script for the blinding effect on the camera */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using TestMQTT;

public class LossTransition : MonoBehaviour
{

    [Header("Script Variable")]
    [Tooltip("The script attached to the timer in the HUD.")] public TimerBehavior tbReference;
    public ButtKickerManager buttKickerManager;

    [Header("UI Variables")]
    [Tooltip("The brightness that covers the UI if the time runs out or the player wins.")] public Image blindnessImage;
    [Tooltip("The darkness that covers the UI if the player \"dies\".")] public Image darknessImage;
    [Tooltip("The collection of text displayed in the HUD.")] public GameObject textBoxes;
    [Tooltip("The game over text.")] public TextMeshProUGUI gameOverText;
    [Tooltip("The win subtext.")] public TextMeshProUGUI winText;
    [Tooltip("The loss subtext.")] public TextMeshProUGUI lossText;
    [Tooltip("The death text.")] public TextMeshProUGUI deathText;
    [Tooltip("The ominous subtext.")] public TextMeshProUGUI doomedText;

    [Header("Numerical Variables")]
    [Tooltip("The increment that the coroutine adds each time.")] public float amountToAdd;
    [Tooltip("The divisor (amount that the original volume is divided by) to decrement the gradual volume raise by each time.")] public float soundDecrement;
    [Tooltip("The refresh rate of the coroutines.")] public float refreshRate;
    [Tooltip("The delay before the \"game over\" text appears.")] public float delayBeforeEnd;
    [Tooltip("The delay that the \"game over\" text remains on-screen before repeating the disappearing cycle.")] public float fadeDelay;
    [Tooltip("The amount to increment or decrement the alpha value of the \"game over\" text at the end.")] public float textIncrement;
    [Tooltip("The number of times the ending text cycles before quitting the application.")] public int cyclesBeforeKick;
    private int actualTimesCycled = 0;

    // Boolean variables
    [HideInInspector] public bool playerHasWon = false;
    [HideInInspector] public bool playerHasLost = false;
    [HideInInspector] public bool playerHasDied = false;
    private bool addAlpha = true;
    public bool coroutineCanProceed = true;

    void Start()
    {

        // If the image or its material slot are left null, then this script will self-destruct
        if (blindnessImage == null) Destroy(this);
        else if (blindnessImage.material == null) Destroy(this);

        // If the blinding material is not entirely white and with an initial alpha value of zero, then this will correct it for later
        if (blindnessImage.color != new Color(1f, 1f, 1f, 0f)) blindnessImage.color = new Color(1f, 1f, 1f, 0f);

        // If the float and integer values are left at zero, then this will correct them
        if (amountToAdd == 0f) amountToAdd = 0.01f;
        else if (amountToAdd < 0f) amountToAdd *= -1f;
        if (refreshRate == 0f) refreshRate = 0.03f;
        else if (refreshRate < 0f) refreshRate *= -1f;
        if (soundDecrement == 0f) soundDecrement = 10f;
        else if (soundDecrement < 0f) soundDecrement *= -1f;
        if (delayBeforeEnd == 0f) delayBeforeEnd = 1f;
        else if (delayBeforeEnd < 0f) delayBeforeEnd *= -1f;
        if (fadeDelay < 0f) fadeDelay = 0f;
        if (textIncrement == 0f) textIncrement = 0.03f;
        else if (textIncrement < 0f) textIncrement *= -1f;
        if (cyclesBeforeKick == 0) cyclesBeforeKick = 10;
        else if (cyclesBeforeKick < 0) cyclesBeforeKick *= -1;

        // Sets the game over text alpha values to zero if they are not already
        if (gameOverText.alpha != 0f) gameOverText.alpha = 0f;
        if (winText.alpha != 0f) winText.alpha = 0f;
        if (lossText.alpha != 0f) lossText.alpha = 0f;
        if (deathText.alpha != 0f) deathText.alpha = 0f;
        if (doomedText.alpha != 0f) doomedText.alpha = 0f;

        //sanity check
        //coroutineCanProceed = true;
        
    }

    void FixedUpdate()
    {
        
        // If something has happened and the coroutine can proceed
        if (coroutineCanProceed && (playerHasWon || playerHasLost || playerHasDied))
        {

            // Different color screens fade in depending on the ending
            if (playerHasWon || playerHasLost) StartCoroutine(GradualBlindness(blindnessImage));
            else if (playerHasDied) StartCoroutine(GradualBlindness(darknessImage));

            // This prevents multiple executions
            coroutineCanProceed = false;

        }

    }

    private IEnumerator GradualBlindness(Image whatFadesIn)
    {

        // While the blinding effect is not null
        while (whatFadesIn != null)
        {

            // If the blinding color's alpha value is less than one (i.e., completely opaque)
            if (whatFadesIn.color.a < 1f)
            {

                /* Appends an incremental value to the alpha value;
                if the player has died, then this will increment much faster */
                whatFadesIn.color = new Color(1f, 1f, 1f, whatFadesIn.color.a + amountToAdd * 4f);

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            else
            {

                // Disables the UI text boxes
                textBoxes.SetActive(false);

                // Allows the coroutine sequence to proceed
                SelectCoroutinePath();

                // Breaks the coroutine
                yield break;

            }

        }

    }

    private void SelectCoroutinePath()
    {

        // Depending on how the player ended their experience, different coroutine paths will proceed
        if (playerHasWon) StartCoroutine(MakeTextFlash(gameOverText, winText));
        else if (playerHasLost) StartCoroutine(MakeTextFlash(gameOverText, lossText));
        else if (playerHasDied) StartCoroutine(MakeTextFlash(deathText, doomedText));

    }

    private IEnumerator MakeTextFlash(TextMeshProUGUI textToFlash, TextMeshProUGUI subtextToFlash)
    {

        // While-loop that returns true as long as this script's object is not destroyed
        while (gameObject != null)
        {

            // If the text must fade in
            if (addAlpha)
            {

                /* If the main text alpha is less than full, then it will fade in;
                otherwise, the main Boolean value will change */
                if (textToFlash.alpha < 1f) textToFlash.alpha += textIncrement;
                else addAlpha = false;

                // Forces the argument subtext to gain alpha
                subtextToFlash.alpha += textIncrement;

                // Refreshes the coroutine
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            // If the text must fade out
            else
            {

                /* If the main text alpha is greater than zero, then it will fade in;
                otherwise, the main Boolean value will change and the private integer will increment by one */
                if (textToFlash.alpha > 0f) textToFlash.alpha -= textIncrement;
                else
                {

                    actualTimesCycled++;
                    addAlpha = true;

                }

                // Forces the argument subtext to lose alpha
                subtextToFlash.alpha -= textIncrement;

                /* If the text has cycled enough times and the player has not lost or died, then the application will automatically close on the headset;
                otherwise, the coroutine will refresh */
                if (actualTimesCycled >= cyclesBeforeKick && playerHasWon || playerHasLost || playerHasDied) ExitToCredits();
                else yield return new WaitForSecondsRealtime(refreshRate);

            }

        }

    }

    private void ExitToCredits()
    {
        buttKickerManager.PlayButtKicker("Missile", 0, 2, false);
        // Loads the scene containing the credits
        SceneManager.LoadScene("CreditsScene");

    }

}
