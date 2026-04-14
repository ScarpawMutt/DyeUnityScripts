/* Charlie Dye, PACE Team - 2025.12.05

This is the script for the colored lights on the railing puzzle gadgets */

using System.Collections;
using UnityEngine;

public class PsychedeilcLighting : MonoBehaviour
{

    [Header("Material Object")]
    [Tooltip("The material component of the light bulb.")] public Renderer bulbColor;

    [Header("Light Object")]
    [Tooltip("The light object to be animated.")] public Light associatedLight;

    [Header("Float Variables")]
    [Tooltip("The shortest possible time that the light can remain lit or unlit.")] public float minimumStateTime;
    [Tooltip("The longest possible time that the light can remain lit or unlit.")] public float maximumStateTime;
    private float randomStateTime;

    [Header("Boolean Variables")]
    [Tooltip("Is the light supposed to lit (true) or unlit (false)?")] public bool lightIsLit;
    [Tooltip("Can the lights animate?")] public bool canSwitch;
    private bool hasStarted;

    void Start()
    {

        // If the light object is left null, then this script will self-destruct
        if (associatedLight == null) Destroy(this);

        // If the float variables are configured incorrectly, then this will correct them
        if (minimumStateTime == 0f) minimumStateTime = 0.5f;
        else if (minimumStateTime < 0f) minimumStateTime *= -1f;
        if (maximumStateTime == 0f) maximumStateTime = 1f;
        else if (maximumStateTime < 0f) maximumStateTime *= -1f;
        if (maximumStateTime <= minimumStateTime) maximumStateTime = minimumStateTime + 1f;

        // If the Boolean variables are configured incorrectly, then this will correct them
        if (canSwitch) canSwitch = false;
        if (hasStarted) hasStarted = false;

    }

    void FixedUpdate()
    {

        // If the lights have not yet animated but must do so
        if (canSwitch && !hasStarted)
        {

            // Executes the coroutine with the Boolean variable
            StartCoroutine(RandomizeLighting());

            // Kill bool for only one execution
            hasStarted = true;

        }

    }

    public void AlterLighting()
    {

        // If the light must activate
        if (lightIsLit) associatedLight.enabled = true;
        // If the light must deactivate
        else associatedLight.enabled = false;

    }

    public void WinLossLighting (bool playerWonPuzzle)
    {

        /* If the player has won the fourth floor puzzle, then the lights will turn green;
        otherwise, they will turn red */
        if (playerWonPuzzle)
        {

            // Changes the bulb material
            bulbColor.material.color = Color.green;

            // Changes the light color
            associatedLight.color = Color.green;

        }
        else
        {

            // Changes the bulb material
            bulbColor.material.color = Color.red;

            // Changes the light color
            associatedLight.color = Color.red;

        }

    }

    public IEnumerator RandomizeLighting()
    {

        // While-loop ensuring that the light object is not null
        while (associatedLight != null)
        {

            // If the light can animate
            if (canSwitch)
            {

                // Randomizes the coroutine refresh rate using the entered minimum and maximum values
                randomStateTime = Random.Range(minimumStateTime, maximumStateTime);

                // Inverts the current Boolean value
                if (!lightIsLit) lightIsLit = true;
                else lightIsLit = false;

                // Calls the alteration method
                AlterLighting();

                // Refreshes the coroutine using randomized time
                yield return new WaitForSecondsRealtime(randomStateTime);

            }
            // Breaks the coroutine
            else yield break;

        }

    }

}
