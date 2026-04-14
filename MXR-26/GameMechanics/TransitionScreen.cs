/* Charlie Dye - ECT 4440 - 2026.04.03

This is the script for the blinding effect when passing between scenes */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionScreen : MonoBehaviour
{

    [Header("Image Variable")]
    [Tooltip("The blinding effect.")] public Image attachedImage;

    [Header("Float Variables")]
    [Tooltip("The amount to add or subtract from the image's alpha value per coroutine cycle.")] public float amountToIncrement;
    [Tooltip("The refresh rate of the coroutine.")] public float refreshRate;

    [Header("Boolean Variables")]
    [Tooltip("Is the blinding effect fully faded in?")] public bool isFadedIn;
    [Tooltip("Is the blinding effect fully faded out?")] public bool isFadedOut;

    void Awake()
    {

        // If the image variable is left null, then this script will self-destruct
        if (attachedImage == null) Destroy(this);

    }

    void Start()
    {

        // If the floats have bad values, then this will set them to more suitable ones
        if (amountToIncrement == 0f) amountToIncrement = 0.02f;
        else if (amountToIncrement < 0f) amountToIncrement *= -1f;
        if (refreshRate == 0f) refreshRate = 0.03f;
        else if (refreshRate < 0f) refreshRate *= -1f;

    }

    public void InitiateTransition (bool passingFadeArgument)
    {

        // Starts the coroutine (this allows exterior scripts to start this remotely)
        StartCoroutine (TransitionLogic(passingFadeArgument));

    }

    private IEnumerator TransitionLogic(bool fadeArgument)
    {

        // If the image must fade in
        if (fadeArgument)
        {

            // If the fade-out bool is true, then it becomes false
            if (isFadedOut) isFadedOut = false;

        }
        // If the image must fade our
        else
        {

            // If the fade-in bool is true, then it becomes false
            if (isFadedIn) isFadedIn = false;

        }


        // While-loop that returns true as long as the image is not destroyed (this should ALWAYS return true)
        while (attachedImage != null)
        {

            // If the image must fade in
            if (fadeArgument)
            {

                // If the image's alpha value is less than one
                if (attachedImage.color.a < 1f)
                {

                    // Constructs a new color (the only difference is the addition to the alpha value)
                    attachedImage.color = new(attachedImage.color.r, attachedImage.color.g, attachedImage.color.b, attachedImage.color.a + amountToIncrement);

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                // If the image's alpha value equals one
                else
                {

                    // The Boolean denoting a full fade-in becomes true
                    isFadedIn = true;

                    // Breaks the coroutine
                    yield break;

                }

            }
            // If the image must fade out
            else
            {

                // If the image's alpha value is greater than zero
                if (attachedImage.color.a > 0f)
                {

                    // Constructs a new color (the only difference is the subtraction from the alpha value)
                    attachedImage.color = new(attachedImage.color.r, attachedImage.color.g, attachedImage.color.b, attachedImage.color.a - amountToIncrement);

                    // Refreshes the coroutine
                    yield return new WaitForSecondsRealtime(refreshRate);

                }
                // If the image's alpha value equals zero
                else
                {

                    // The Boolean denoting a full fade-out becomes true
                    isFadedOut = true;

                    // Breaks the coroutine
                    yield break;

                }

            }

        }

    }

}
