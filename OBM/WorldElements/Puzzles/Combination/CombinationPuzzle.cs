/* Charlie Dye, PACE Team - 2024.11.22

This is the script for the combination puzzle on the third floor */

using UnityEngine;

public class CombinationPuzzle : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the spinner button responsible for turning the spinner clockwise.")] public CombinationButton cbReferenceClockwise;
    [Tooltip("The script attached to the spinner button responsible for turning the spinner counterclockwise.")] public CombinationButton cbReferenceCounterclockwise;
    [Tooltip("The script attached to the master dialogue module.")] public DialogueControl dcReference;
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;
    [Tooltip("The script attached to the master railing control panel on the fourth floor.")] public RailingControls rcReference;
    [Tooltip("The script attached to the player timer.")] public TimerBehavior tbReference;
    [Tooltip("The array of scripts attached to the pressure panel needles.")] public NeedleMovement[] nmReferences;
    [Tooltip("The array of scripts attached to the correct characters on this panel's wheel.")] public TriggerZone[] tzReferences;

    [Header("GameObject Variables")]
    [Tooltip("The part of the combination puzzle that spins.")] public GameObject rotatingSpinner;
    [Tooltip("The rotating arrow around the puzzle.")] public GameObject spinnerArrow;

    [Header("Renderer Array")]
    [Tooltip("The renderers attached to the light bulbs.")] public Renderer[] bulbRenderers;

    [Header("Trigger GameObjects")]
    [Tooltip("The player's left controller trigger.")] public GameObject triggerCubeLeft;
    [Tooltip("The player's right controller trigger.")] public GameObject triggerCubeRight;

    [Header("Numerical Variables")]
    [Tooltip("The speed at which the dial rotates.")] public float rotationSpeed;
    [Tooltip("The speed at which the arrow rotates.")] public float arrowSpeed;
    [Tooltip("The delay, in seconds, before the lights change color when the puzzle is won.")] public float changeDelay;
    [Tooltip("The index in the array that the target is located.")] public int arrayIndexer;

    [Header("Light Array")]
    [Tooltip("The array of lights that correspond to the puzzle characters.")] public Light[] puzzleLights;

    [Header("Boolean Variables")]
    [Tooltip("Whether the dial can rotate counterclockwise.")] public bool rotateLeft = false;
    [Tooltip("Whether the dial can rotate clockwise.")] public bool rotateRight = false;
    [Tooltip("Whether the dial can rotate at all.")] public bool canRotate = true;
    [Tooltip("Whether or not the puzzle has been completed.")] public bool puzzleIsComplete = false;
    private bool arrowHasAppeared = false;
    private bool arrowHasDisappeared = false;

    void Start()
    {

        // If the floats are left at zero, then this will correct them
        if (rotationSpeed == 0f) rotationSpeed = 1f;
        if (arrowSpeed == 0f) arrowSpeed = 1f;
        if (changeDelay == 0f) changeDelay = 1f;

        // Sets the array indexer to zero
        if (arrayIndexer != 0) arrayIndexer = 0;

        // Hides the arrow
        if (spinnerArrow != null) spinnerArrow.SetActive(false);

        /* Sets the directional bool for each array element depending on its position;
        it also deactivates all elements except the first one. and switches off all lights */
        for (int j = 0; j < tzReferences.Length; j++)
        {

            if (j % 2 == 0) tzReferences[j].clockwiseNotCounterclockwise = true;
            else tzReferences[j].clockwiseNotCounterclockwise = false;

            if (j >= 1) tzReferences[j].gameObject.SetActive(false);

            puzzleLights[j].enabled = false;

        }

    }

    void FixedUpdate()
    {

        // Depending on which button is being interacted with, different Booleans will alter their values
        if (cbReferenceClockwise.isActive) rotateRight = true;
        else rotateRight = false;
        if (cbReferenceCounterclockwise.isActive) rotateLeft = true;
        else rotateLeft = false;

        // If one but not main bools return true, then the spinner will rotate
        if ((rotateLeft || rotateRight) && canRotate) RotateSpinner();

        // If the elevator is set correctly and the arrow has not appeared already, then it will do so
        if (ebReference.arrayIndexer == 3 && !arrowHasAppeared && !puzzleIsComplete)
        {

            spinnerArrow.SetActive(true);
            arrowHasAppeared = true;

        }

        // Rotates the arrow
        if (arrowHasAppeared && !puzzleIsComplete) RotateArrow();

        // If both are true, however, then rotation is disabled
        if (rotateLeft && rotateRight) canRotate = false;
        else canRotate = true;
        
    }

    public void RotateSpinner()
    {

        // Changes the direction of the spinner depending on the trigger
        if (rotateRight) rotatingSpinner.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward);
        else if (rotateLeft) rotatingSpinner.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.back);

    }

    public void RotateArrow()
    {

        // Rotates the arrow in the correct direction of the spinner
        if (arrayIndexer % 2 == 0) spinnerArrow.transform.Rotate(arrowSpeed * Time.deltaTime * Vector3.forward);
        else if (arrayIndexer % 2 == 1) spinnerArrow.transform.Rotate(arrowSpeed * Time.deltaTime * Vector3.back);

    }

    public void AdvanceInPuzzle(int trigger_indexer)
    {

        // Switches on one light at a time
        puzzleLights[trigger_indexer].enabled = true;

        // Increments the array indexer by one
        arrayIndexer++;

        // Inverts the arrow's X-scale so that it points in the correct direction each time a character is entered
        spinnerArrow.transform.localScale = new(spinnerArrow.transform.localScale.x * -1f, 1f, 1f);

        /* If the local int variable is below a certain value, then it will cause the puzzle to move forward;
        otherwise, if it is large enough, then the player will win */
        if (trigger_indexer < tzReferences.Length - 1) tzReferences[trigger_indexer + 1].gameObject.SetActive(true);
        else CompletePuzzle();

    }

    public void CompletePuzzle()
    {

        // Sends a win and RTE message to the HUD
        tbReference.SendWinMessage();
        tbReference.returnToElevatorText.enabled = true;

        // Triggers the railing control panel animations through the master script
        rcReference.SetOffInstruments();

        // Turns the lights green after a short delay
        Invoke(nameof(LightDelay), changeDelay);

        // Causes the arrow to disappear as governed by the kill Boolean
        if (!arrowHasDisappeared)
        {

            spinnerArrow.SetActive(false);
            arrowHasDisappeared = true;

        }

        // Modifies the bool value
        puzzleIsComplete = true;

        // Causes the needles on the this level's pressure panel to stop
        for (int m = 0; m < nmReferences.Length; m++)
        {

            if (nmReferences[m].canRotate) nmReferences[m].canRotate = false;

        }

        // Allows the player to advance
        ebReference.floorHasBeenCompleted = true;

        // Allows dialogue to continue
        dcReference.PrepareSpeechBlock(dcReference.dialogueThirdFloor, dcReference.pausesThirdFloor, 5, 5, true);

    }

    public void LightDelay()
    {

        // Causes the light objects and bulbs to turn green
        for (int k = 0; k < puzzleLights.Length; k++)
        {

            puzzleLights[k].color = Color.green;

        }
        for (int l = 0; l < bulbRenderers.Length; l++)
        {

            bulbRenderers[l].material.color = Color.green;

        }

    }

    public void ResetPuzzle()
    {

        // Resets the array indexer
        arrayIndexer = 0;

        // Resets the arrow's scale
        spinnerArrow.transform.localScale = new(1f, 1f, 1f);

        for (int l = 0; l < tzReferences.Length; l++)
        {

            // Deactivates all scripts' associated boolean
            tzReferences[l].isTriggered = false;

            // Switches all lights off
            puzzleLights[l].enabled = false;

            // Disables all scripts' GameObjects except for the first, restarting the puzzle
            if (l >= 1) tzReferences[l].gameObject.SetActive(false);

        }

    }

}
