/* Charlie Dye, PACE Team - 2024.09.20

This is the updated control panel script */

using UnityEngine;

public class MasterControlPanel : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the steel door on the first floor.")] public DoorBehavior dbReference;
    [Tooltip("The script attached to the master dialogue module.")] public DialogueControl dcReference;
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;
    [Tooltip("The script attached to the pointer.")] public PointerBehavior pbReference;
    [Tooltip("The script attached to the timer.")] public TimerBehavior tbReference;
    [Tooltip("The array of scripts attached to the buttons of the control panel.")] public ButtonsUpdated[] buReferences;

    [Header("Audio Sources")]
    [Tooltip("The audio that plays when the puzzle is completed.")] public AudioSource puzzleSuccess;
    [Tooltip("The audio that plays when the puzzle is failed.")] public AudioSource puzzleFailure;

    [Header("Numerical Variable")]
    [Tooltip("The amount of time to wait until the reward for completing the puzzle is given.")] public float winDelay;

    [Header("Boolean Variables")]
    [Tooltip("Whether or not the puzzle can be interacted with entirely.")] public bool canInteractWithPuzzle = false;
    [Tooltip("Whether or not the reward to the player has been given.")] public bool winConditionHasExecuted = false;
    [Tooltip("The array of booleans that keep track of the buttons pressed.")] public bool[] colliderArray;
    [Tooltip("This needs to be disabled untill the puzzle is solved to avoid diolouge playing too early")] public DialogueTrigger FloorOnePuzzleDoneDiolougeCollider;

    // Integer variable for checking button statuses
    [HideInInspector] public int orderIndexer;

    void Start()
    {

        FloorOnePuzzleDoneDiolougeCollider.canInteract = false;

        // If the boolean array is set to the incorrect length, then it will be corrected
        if (colliderArray.Length != buReferences.Length) colliderArray = new bool[buReferences.Length];
        
        // Corresponds the bool of each script in the below array to one in the local bool array
        for (int i = 0; i < buReferences.Length; i++)
        {

            buReferences[i].thisButtonIsLit = colliderArray[i];

        }

        // Automatically resets the puzzle
        ResetPuzzle(false);

    }

    public void CheckButtonOrder()
    {

        if (canInteractWithPuzzle)
        {

            /* If a button is pressed, then it will increment the order integer by one.
            The next button in the array will become the "correct button" (the one that
            the player must press next) that advances the puzzle. If the incorrect button
            is pressed after that, then the puzzle will reset */
            if (buReferences[orderIndexer].thisButtonIsLit) orderIndexer++;
            else if (!buReferences[orderIndexer].thisButtonIsLit) ResetPuzzle(true);

            /* If the order integer reaches a certain number (a result only achieved by
            pressing all buttons in a set pattern), then the win condition will execute */
            if (orderIndexer == 8 && !winConditionHasExecuted)
            {

                WinCondition();
                winConditionHasExecuted = true;
                FloorOnePuzzleDoneDiolougeCollider.canInteract = true;
            }

        }

    }

    public void WinCondition()
    {

        // Sends a congratulatory message to the headset and prompts the player to return to the elevator with the extinguisher
        tbReference.SendWinMessage();
        tbReference.returnToElevatorText.enabled = true;
        tbReference.takeExtinguisherText.enabled = true;

        // Makes the elevator operable
        ebReference.floorHasBeenCompleted = true;

        // Activates the fire extinguisher's pointer
        pbReference.renderPointer = true;

        // Continues with dialogue
        dcReference.PrepareSpeechBlock(dcReference.dialogueFirstFloor, dcReference.pausesFirstFloor, 9, 12, true);

        // Plays audio
        puzzleSuccess.Play();

        // Opens the door to the elevator corridor
        dbReference.executeAnimation = true;

    }

    public void ResetPuzzle(bool playSoundEffect)
    {

        // Deactivates all buttons, restarting the puzzle
        for (int i = 0; i < buReferences.Length; i++)
        {

            buReferences[i].DeactivateButton();

        }

        // Sets the order integer back to 0
        orderIndexer = 0;

        // Plays losing audio if the paramter is true
        if (playSoundEffect) puzzleFailure.Play();

    }

}
