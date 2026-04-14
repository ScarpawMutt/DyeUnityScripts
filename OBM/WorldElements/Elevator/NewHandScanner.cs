/* Charlie Dye, PACE Team - 2025.10.17

This is the script for the updated hand scanner */

using System.Collections;
using UnityEngine;
using TMPro;

public class NewHandScanner : MonoBehaviour
{

    [Header("Script Variable")]
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;

    [Header("Collider Interactibles")]
    [Tooltip("The player's left hand.")] public GameObject leftHandMesh;
    [Tooltip("The player's right hand.")] public GameObject rightHandMesh;
    [Tooltip("Steve's severed hand.")] public GameObject steveHandMesh;
    [Tooltip("The collider used to register GameObject collision.")] public Collider meshTarget;

    [Header("Float Variables")]
    [Tooltip("The fading speed of the text alpha.")] public float fadeRate;
    [Tooltip("The rate that the coroutine refreshes.")] public float refreshRate;
    [Tooltip("The duration that non-flashing text stays on the screen.")] public float textDuration;

    [Header("Text Variables")]
    [Tooltip("The text that appears when Steve's hand is accepted by the machine.")] public TextMeshPro acceptedText;
    [Tooltip("The text that appears when the player's hand is denied by the machine.")] public TextMeshPro deniedText;
    [Tooltip("The text that appears when the machine is idle.")] public TextMeshPro levelFiveText;

    [Header("Boolean Variables")]
    [Tooltip("The boolean that determines whether the alpha of the text should increase or decrease.")] public bool raiseAlpha = false;
    [Tooltip("The boolean to override the coroutine entirely.")] public bool overrideAnimation = false;
    [HideInInspector] public bool coroutineHasStarted;

    void Start()
    {

        // If the collider is enabled, then this will disable it until later
        if (meshTarget.enabled) meshTarget.enabled = false;

        // If the text at the beginning is not enabled properly, then this will correct it
        if (acceptedText.enabled) acceptedText.enabled = false;
        if (deniedText.enabled) deniedText.enabled = false;
        if (levelFiveText.alpha != 0f) levelFiveText.alpha = 0f;
        
    }

    void FixedUpdate()
    {

        // If the elevator can move up to the fifth floor
        if (ebReference.arrayIndexer == 4 && ebReference.floorHasBeenCompleted && !coroutineHasStarted)
        {

            // Enables the collider
            meshTarget.enabled = true;

            // The digital text prompting so will activate via coroutine and the kill bool
            StartCoroutine(FadeText());
            coroutineHasStarted = true;

        }

    }

    void OnTriggerEnter(Collider hand)
    {

        // The trigger only works if the elevator is on the correct floor
        if (ebReference.arrayIndexer == 4 && ebReference.floorHasBeenCompleted)
        {

            if (hand == leftHandMesh.GetComponent<Collider>() || hand == rightHandMesh.GetComponent<Collider>())
            {

                // Makes the denied text visible for a short time
                deniedText.enabled = true;
                Invoke(nameof(DisableText), textDuration);

                // Attempts to make the elevator ascend (it does not work)
                ebReference.AscendOneFloor();

            }
            else if (hand == steveHandMesh.GetComponent<Collider>())
            {

                // Overrides the coroutine
                overrideAnimation = true;

                // Makes the accepted text visible for a short time
                acceptedText.enabled = true;
                Invoke(nameof(DisableText), textDuration);

                // Disables the collider
                meshTarget.enabled = false;

                // Causes the elevator to ascend to the top floor
                ebReference.AscendToFifthFloor();

            }

        }

    }

    public IEnumerator FadeText()
    {

        // If the text is not enabled yet, this will do so
        if (!levelFiveText.enabled) levelFiveText.enabled = true;

        // While-loop for the text fade
        while (levelFiveText != null)
        {

            if (!overrideAnimation)
            {

                // If the text needs to fade in
                if (raiseAlpha)
                {

                    if (levelFiveText.alpha < 1f) levelFiveText.alpha += fadeRate;
                    else raiseAlpha = false;

                }
                // If the text needs to fade out
                else
                {

                    if (levelFiveText.alpha > 0f) levelFiveText.alpha -= fadeRate;
                    else raiseAlpha = true;

                }

                // Repeats the coroutine using the refresh rate in real time
                yield return new WaitForSecondsRealtime(refreshRate);

            }
            else
            {

                // Fades the text out until it becomes transparent
                if (levelFiveText.alpha < 1f) levelFiveText.alpha -= fadeRate;

                // Repeats the coroutine using the refresh rate in real time
                yield return new WaitForSecondsRealtime(refreshRate);

            }

        }

        yield break;

    }

    public void DisableText()
    {

        // If either of the status text boxes are enabled, this will disable them
        if (acceptedText.enabled) acceptedText.enabled = false;
        if (deniedText.enabled) deniedText.enabled = false;

    }

}
