/* Charlie Dye, PACE Team - 2026.01.12

This is the script for the "secret" text at the end */

using UnityEngine;
using TMPro;

public class SecretText : MonoBehaviour
{

    [Header("Script Variable")]
    [Tooltip("The script attached to the fifth floor lugnut.")] public NutBehavior nbReference;

    [Header("Float Variable")]
    [Tooltip("The amount that a color changes by per each update.")] public float lerpAmount;
    [HideInInspector] [Range(0, 6)] public int colorIndexer;

    [Header("Text Variable Array")]
    [Tooltip("An array of text objects to be manipulated.")] public TextMeshPro[] tmpSecrets;

    // Color variables
    private Color colorRed = new(1f, 0f, 0f, 1f);
    private Color colorYellow = new(1f, 1f, 0f, 1f);
    private Color colorGreen = new(0f, 1f, 0f, 1f);
    private Color colorCyan = new(0f, 1f, 1f, 1f);
    private Color colorBlue = new(0f, 0f, 1f, 1f);
    private Color colorViolet = new(1f, 0f, 1f, 1f);
    private Color colorTarget;

    void Start()
    {

        // If the float is an invalid value, then this will correct it
        if (lerpAmount == 0f) lerpAmount = 0.1f;
        else if (lerpAmount < 0f) lerpAmount *= -1f;

        // Sets the color index value to zero, indicating a red target
        colorIndexer = 0;

    }

    void FixedUpdate()
    {

        // If the game has been won
        if (nbReference.winConditionHasExecuted)
        {

            // Causes the text's color to change
            ChangeTextColor();

            // Applies the updated color onto the text
            for (int i = 0; i < tmpSecrets.Length; i++)
            {

                tmpSecrets[i].color = colorTarget;

            }

        }

    }

    public void ChangeTextColor()
    {

        // To create red
        if (colorIndexer == 0)
        {

            if (colorTarget.b > lerpAmount) colorTarget = Color.Lerp(colorTarget, colorRed, lerpAmount);
            else colorIndexer++;

        }
        // To create yellow
        else if (colorIndexer == 1)
        {

            if (colorTarget.g < 1f - lerpAmount) colorTarget = Color.Lerp(colorTarget, colorYellow, lerpAmount);
            else colorIndexer++;

        }
        // To create green
        else if (colorIndexer == 2)
        {

            if (colorTarget.r > lerpAmount) colorTarget = Color.Lerp(colorTarget, colorGreen, lerpAmount);
            else colorIndexer++;

        }
        // To create cyan
        else if (colorIndexer == 3)
        {

            if (colorTarget.b < 1f - lerpAmount) colorTarget = Color.Lerp(colorTarget, colorCyan, lerpAmount);
            else colorIndexer++;

        }
        // To create blue
        else if (colorIndexer == 4)
        {

            if (colorTarget.g > lerpAmount) colorTarget = Color.Lerp(colorTarget, colorBlue, lerpAmount);
            else colorIndexer++;

        }
        // To create violet
        else if (colorIndexer == 5)
        {

            if (colorTarget.r < 1f - lerpAmount) colorTarget = Color.Lerp(colorTarget, colorViolet, lerpAmount);
            else colorIndexer = 0;

        }

    }

}
