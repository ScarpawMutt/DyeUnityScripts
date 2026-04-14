/* Charlie Dye - ECT 4440 - 2026.04.02

This is the script for the statistics display after the game has ended */

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class Postmortem : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the game controls.")] public GameControls gcReference;
    [Tooltip("The script attached to the player death mechanic.")] public DeathSequence dsReference;
    [Tooltip("The script attached to the master level mechanics.")] public LevelCounter lcReference;
    [Tooltip("The script attached to the player's power up bank.")] public PowerUpBank pubReference;
    [Tooltip("The script attached to the text in the player UI.")] public TMPBehavior tmpbReference;
    [Tooltip("The script attached to the transition animation.")] public TransitionScreen tsReference;

    [Header("Input Actions")]
    [Tooltip("The keyboard binding to fire bullets (the first keyboard binding to exit the postmortem screen).")] public InputAction exitButtonOne;
    [Tooltip("The keyboard binding to fire missiles (the second keyboard binding to exit the postmortem screen).")] public InputAction exitButtonTwo;

    [Header("String/Text Variables")]
    [Tooltip("The string name for the conventional/keyboard fire buttons.")] public string fireKeysConventional;
    [Tooltip("The string name for the VR fire buttons.")] public string fireKeysVR;
    [Tooltip("The text that prompts the player to continue.")] public TextMeshProUGUI continueText;

    [Header("Float Variable")]
    [Tooltip("The delay before the postmortem text begins to appear.")] public float postmortemDelay;
    [Tooltip("The amount of alpha to increment per coroutine cycle.")] public float amountToIncrement;
    [Tooltip("The refresh rate of the coroutine.")] public float refreshRate;
    private float hitMissRatio;
    private int playerScoreCopy;
    private int playerLevelCopy;
    private int bulletCounterCopy;
    private int missileCounterCopy;
    private int asteroidsShotDownCopy;
    private int hostileShipsShotDownCopy;
    private int powerUpsCollectedCopy;

    // Boolean variables
    private bool delayHasEnded = false;
    private bool isDisplayingStats = false;
    private bool fadeTextIn = true;
    private bool gameMayBeExited = false;
    private bool gameHasBeenExited = false;

    void Awake()
    {

        /* If any of the public script references are left null, then this script will attempt to locate them;
        if that fails for any of them, then this script will self-destruct */
        if (gcReference == null)
        {

            if (FindFirstObjectByType<GameControls>()) gcReference = FindFirstObjectByType<GameControls>();
            else Destroy(this);

        }
        if (dsReference == null)
        {

            if (FindFirstObjectByType<DeathSequence>()) dsReference = FindFirstObjectByType<DeathSequence>();
            else Destroy(this);

        }
        if (lcReference == null)
        {

            if (FindFirstObjectByType<LevelCounter>()) lcReference = FindFirstObjectByType<LevelCounter>();
            else Destroy(this);

        }
        if (pubReference == null)
        {

            if (FindFirstObjectByType<PowerUpBank>()) pubReference = FindFirstObjectByType<PowerUpBank>();
            else Destroy(this);

        }
        if (tmpbReference == null)
        {

            if (FindFirstObjectByType<TMPBehavior>()) tmpbReference = FindFirstObjectByType<TMPBehavior>();
            else Destroy(this);

        }
        if (tsReference == null)
        {
            
            if (FindFirstObjectByType<TransitionScreen>()) tsReference = FindFirstObjectByType<TransitionScreen>();
            else Destroy(this);

        }

    }

    void Start()
    {

        // If the continue text is not transparent, then it will become so
        if (continueText.color.a > 0f) continueText.color = new(continueText.color.r, continueText.color.g, continueText.color.b, 0f);

        // If the floats have bad values, then this will fix them
        if (postmortemDelay < 0f) postmortemDelay = 0f;
        if (amountToIncrement == 0f) amountToIncrement = 0.02f;
        else if (amountToIncrement < 0f) amountToIncrement *= -1f;
        if (refreshRate == 0f) refreshRate = 0.03f;
        else if (refreshRate < 0f) refreshRate *= -1f;

        /* If the game is using VR controls, then the continue text will prompt the player to exit using a set of buttons on their joysticks;
        otherwise, it will prompt the player to exit using a completely different set */
        if (gcReference.useXRControls) continueText.text = $"PRESS {fireKeysVR} TO CONTINUE";
        else continueText.text = $"PRESS {fireKeysConventional} TO CONTINUE";

    }

    void FixedUpdate()
    {

        // If the statistics must be displayed
        if (dsReference.cueStats)
        {

            /* If the timer has a value greater than zero, then it will subtract unscaled time;
            otherwise, the Boolean necessary for text appearance will become true */
            if (postmortemDelay > 0f) postmortemDelay -= Time.fixedDeltaTime;
            else delayHasEnded = true;

            // If the delay has ended, but it has not yet begun doing so
            if (delayHasEnded && !isDisplayingStats)
            {

                // Activates the appearance method
                DisplayPostmortemText();

                // Switches the kill Boolean to true
                isDisplayingStats = true;

            }

        }

        // If the game may be exited
        if (gameMayBeExited)
        {

            // If the game is using VR controls
            if (gcReference.useXRControls)
            {

                if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0f || OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0f)
                {

                    // If the game has not registered it yet
                    if (!gameHasBeenExited)
                    {

                        // Starts the transition coroutine
                        tsReference.InitiateTransition(true);

                        // Switches the kill Boolean to true
                        gameHasBeenExited = true;

                    }

                }

            }
            // If the game is using keyboard controls
            else
            {

                // If either exit button is pressed
                if (exitButtonOne.IsPressed() || exitButtonTwo.IsPressed())
                {

                    // If the game has not registered it yet
                    if (!gameHasBeenExited)
                    {

                        // Starts the transition coroutine
                        tsReference.InitiateTransition(true);

                        // Switches the kill Boolean to true
                        gameHasBeenExited = true;

                    }

                }

            }

        }

        // If the transition into blindness is complete and the game has been exited, then the player will go back to the title screen
        if (tsReference.isFadedIn && gameHasBeenExited) SceneManager.LoadScene(0);

    }

    public void CopyVariables()
    {

        // Copies the values of the referenced scripts onto the private integer variables
        playerScoreCopy = lcReference.playerScore;
        playerLevelCopy = lcReference.playerLevel;
        bulletCounterCopy = lcReference.numberOfBulletsFired;
        missileCounterCopy = lcReference.numberOfMissilesFired;
        asteroidsShotDownCopy = lcReference.asteroidsShotDown;
        hostileShipsShotDownCopy = lcReference.hostileShipsShotDown;
        powerUpsCollectedCopy = lcReference.powerUpsCollected;

        // Calculates the hit-miss ratio by dividing the sum of obstacles shot down by the sum of projectiles fired (protection against a denominator of zero is accounted for)
        if (asteroidsShotDownCopy + hostileShipsShotDownCopy > 0) hitMissRatio = (float)(asteroidsShotDownCopy + hostileShipsShotDownCopy) / (float)(bulletCounterCopy + missileCounterCopy);
        else hitMissRatio = 0f;

        // Assembles the strings
        AssemblePostmortemText();

    }

    private void AssemblePostmortemText()
    {

        // Combines the postmortem text from the TMP behavior script array and the private numbers from this script into a final string
        tmpbReference.postmortemTexts[0].text += playerScoreCopy.ToString();
        tmpbReference.postmortemTexts[1].text += playerLevelCopy.ToString();
        tmpbReference.postmortemTexts[2].text += bulletCounterCopy.ToString();
        tmpbReference.postmortemTexts[3].text += missileCounterCopy.ToString();
        tmpbReference.postmortemTexts[4].text += asteroidsShotDownCopy.ToString();
        tmpbReference.postmortemTexts[5].text += hostileShipsShotDownCopy.ToString();
        tmpbReference.postmortemTexts[6].text += powerUpsCollectedCopy.ToString();
        tmpbReference.postmortemTexts[7].text += (hitMissRatio * 100f).ToString("F2") + "%";

    }

    private void DisplayPostmortemText()
    {

        // Causes the congratulatory text to appear
        tmpbReference.RemoteStart(tmpbReference.congratulationsText, true);

        // For-loop that indexes the TMP postmortem array
        for (int i = 0; i < tmpbReference.postmortemTexts.Length; i++)
        {

            // Causes each element to appear
            tmpbReference.RemoteStart(tmpbReference.postmortemTexts[i], true);

        }

        // Enables input
        exitButtonOne.Enable();
        exitButtonTwo.Enable();

        // Allows the player to exit the game
        gameMayBeExited = true;

        // Starts the coroutine
        StartCoroutine(FlashPromptText());

    }

    private IEnumerator FlashPromptText()
    {

        // While-loop that returns true as long as the continue text is not wiped (this should ALWAYS return true)
        while (continueText != null)
        {

            // If the text must fade in
            if (fadeTextIn)
            {

                // If the text's alpha value is less than one
                if (continueText.color.a < 1f)
                {

                    // Constructs a "new" color; the only change is the increment in alpha
                    continueText.color = new(continueText.color.r, continueText.color.g, continueText.color.b, continueText.color.a + amountToIncrement);

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                // If the text's alpha value is equal to one
                else
                {

                    // The Boolean becomes true, reversing the coroutine
                    fadeTextIn = false;

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }

            }
            // If the text must fade our
            else
            {

                // If the text's alpha value is greater than zero
                if (continueText.color.a > 0f)
                {

                    // Constructs a "new" color; the only change is the decrement in alpha
                    continueText.color = new(continueText.color.r, continueText.color.g, continueText.color.b, continueText.color.a - amountToIncrement);

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                // If the text's alpha value is equal to zero
                else
                {

                    // The Boolean becomes false, reversing the coroutine
                    fadeTextIn = true;

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }

            }

        }

    }

}
