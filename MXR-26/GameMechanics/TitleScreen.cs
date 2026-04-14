/* Charlie Dye - ECT 4440 - 2026.04.02

This is the script for the title screen UI text */

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreen : MonoBehaviour
{

    [Header("Script Variables")]
    [Tooltip("The script attached to the game controls.")] public GameControls gcReference;
    [Tooltip("The script attached to the transition screen.")] public TransitionScreen tsReference;

    [Header("Input Actions")]
    [Tooltip("The keyboard binding to fire bullets (the first keyboard binding to exit the title screen).")] public InputAction exitButtonOne;
    [Tooltip("The keyboard binding to fire missiles (the second keyboard binding to exit the title screen).")] public InputAction exitButtonTwo;

    [Header("String/Text Variables")]
    [Tooltip("The string name for the conventional/keyboard fire buttons.")] public string fireKeysConventional;
    [Tooltip("The string name for the VR fire buttons.")] public string fireKeysVR;
    [Tooltip("The flashing prompt text.")] public TextMeshProUGUI promptText;

    [Header("Float Variables")]
    [Tooltip("The amount of alpha to increment per coroutine cycle.")] public float amountToIncrement;
    [Tooltip("The refresh rate of the coroutine.")] public float refreshRate;

    // Boolean variables
    private bool textMustFadeIn;
    private bool buttonHasBeenPressed = false;
    private bool transitionMayProceed = false;

    void Awake()
    {

        /* If the public script references are null, then this script will attempt to locate them;
        if that fails for either of them, then this script will self-destruct */
        if (gcReference == null)
        {

            if (FindFirstObjectByType<GameControls>()) gcReference = gameObject.AddComponent<GameControls>();
            else Destroy(this);

        }
        if (tsReference == null)
        {

            if (FindFirstObjectByType<TransitionScreen>()) tsReference = gameObject.AddComponent<TransitionScreen>();
            else Destroy(this);

        }

        // If the prompt text is left null, then this script will self-destruct
        if (promptText == null) Destroy(this);

    }

    void Start()
    {

        // Enables input
        exitButtonOne.Enable();
        exitButtonTwo.Enable();

        // If the floats have bad values, then this will fix them
        if (amountToIncrement == 0f) amountToIncrement = 0.02f;
        else if (amountToIncrement < 0f) amountToIncrement *= -1f;
        if (refreshRate == 0f) refreshRate = 0.03f;
        else if (refreshRate < 0f) refreshRate *= -1f;

        // Starts the coroutine
        StartCoroutine(FlashingText());

        /* If the game is utilizing VR controls, then the prompt text displays one string of text;
        if it is using keyboard controls, then the fire [button] prompt changes accordingly */
        if (gcReference.useXRControls) promptText.text = $"PRESS {fireKeysVR} TO PLAY";
        else promptText.text = $"PRESS {fireKeysConventional} TO PLAY";

    }

    void FixedUpdate()
    {
        
        // If the game is using VR controls
        if (gcReference.useXRControls)
        {

            // If either of the buttons to play are pressed
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0f || OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0f)
            {

                // If the button press has not registered yet
                if (!buttonHasBeenPressed)
                {

                    // Begins the transition process
                    tsReference.InitiateTransition(true);

                    // Switches the kill Boolean
                    buttonHasBeenPressed = true;

                }

            }

        }
        // If the game is using keyboard controls
        else
        {

            // If either of the buttons to play are pressed
            if (exitButtonOne.IsPressed() || exitButtonTwo.IsPressed())
            {

                // If the button press has not registered yet
                if (!buttonHasBeenPressed)
                {

                    // Begins the transition process
                    tsReference.InitiateTransition(true);

                    // Switches the kill Boolean
                    buttonHasBeenPressed = true;

                }

            }

        }

        // If the transition has fully faded in and the main scene must be loaded
        if (tsReference.isFadedIn && !transitionMayProceed)
        {

            // Activates the exit method
            ExitTitleScreen();

            // Switches the kill Boolean to true
            transitionMayProceed = true;

        }
        
    }

    private void ExitTitleScreen()
    {

        // Loads the play area as its own separate scene
        SceneManager.LoadScene(1);

    }

    private IEnumerator FlashingText()
    {

        // While-loop that returns true as long as the prompt text is not wiped (this should ALWAYS return true)
        while (promptText != null)
        {

            // If the text must fade in
            if (textMustFadeIn)
            {

                // If the text's alpha value is less than one
                if (promptText.color.a < 1f)
                {

                    // Constructs a "new" color; the only change is the increment in alpha
                    promptText.color = new (promptText.color.r, promptText.color.g, promptText.color.b, promptText.color.a + amountToIncrement);

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                // If the text's alpha value is equal to one
                else
                {

                    // The Boolean becomes true, reversing the coroutine
                    textMustFadeIn = false;

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }

            }
            // If the text must fade our
            else
            {

                // If the text's alpha value is greater than zero
                if (promptText.color.a > 0f)
                {

                    // Constructs a "new" color; the only change is the decrement in alpha
                    promptText.color = new (promptText.color.r, promptText.color.g, promptText.color.b, promptText.color.a - amountToIncrement);

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                // If the text's alpha value is equal to zero
                else
                {

                    // The Boolean becomes false, reversing the coroutine
                    textMustFadeIn = true;

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }

            }

        }

    }

}
