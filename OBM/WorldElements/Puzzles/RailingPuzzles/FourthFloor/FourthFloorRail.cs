/* Charlie Dye, PACE Team - 2024.12.12

This is the script for the railing puzzle on the fourth floor */

using System;
using System.Linq;
using UnityEngine;

public class FourthFloorRail : MonoBehaviour
{

    [Header("Script References and Arrays")]
    [Tooltip("The script attached to the master dialogue module.")] public DialogueControl dcReference;
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;
    [Tooltip("The script for the text on the panel screen.")] public PanelText ptReference;
    [Tooltip("The script attached to the master railing control panel on the fourth floor.")] public RailingControls rcReference;
    [Tooltip("The script attached to the rotating yellow \"loss\" light.")] public RotatingLight rlReference;
    [Tooltip("The script attached to the timer in the player's HUD.")] public TimerBehavior tbReference;
    [Tooltip("The script attached to the buttons associated with this puzzle.")] public FFRailButtons[] ffrbReferences;
    [Tooltip("The script array attached to the light array on the railing control panel.")] public PsychedeilcLighting[] plReferences;

    [Header("Combinmation Arrays")]
    [Tooltip("The array of numbers, assorted in the intended permutation, to be pressed in order to win the puzzle.")] public int[] correctCombination;
    [Tooltip("The array of numbers actually entered, in chronological order, by the player.")] public int[] enteredCombination;

    [Header("Float Variable")]
    [Tooltip("The time that the timer is frozen for if the puzzle is completed successfully.")] public float timeToFreezeTimer;

    [Header("Boolean Variables")]
    [Tooltip("The boolean that determines if the combination has been checked. Kill bool.")] public bool checkHasExecuted = false;
    [Tooltip("Whether the puzzle was completed successfully or failed.")] public bool puzzleHasFailed = false;
    [Tooltip("Whether the timer has unfrozen if the puzzle was won.")] public bool timerHasUnfrozen = false;

    // Integer variable for checking button statuses
    [HideInInspector] public int orderIndexer = 0;

    void Start()
    {

        // If the integer arrays are not set to an equal length as the script array, this will correct them automatically
        if (correctCombination.Length != ffrbReferences.Length) correctCombination = new int[ffrbReferences.Length];
        if (enteredCombination.Length != ffrbReferences.Length) enteredCombination = new int[ffrbReferences.Length];

        // If the "frozen" duration of the timer is at or below zero, it will automatically be set to a positive value
        if (timeToFreezeTimer <= 0f) timeToFreezeTimer = 1.0f;

        // Corresponds the integer of each script in the below array to one in the local "correct" array
        for (int i = 0; i < correctCombination.Length; i++)
        {

            correctCombination[i] = i;

        }

    }

    void FixedUpdate()
    {
        
        // If the indexer is high enough, then the script will check if the buttons are in the correct order
        if (orderIndexer == ffrbReferences.Length && !checkHasExecuted)
        {

            // Calls the method
            CheckButtonOrder();

            // Switches the kill bool to true to prevent excessive calls of the method
            checkHasExecuted = true;

        }

        /* If the puzzle has been won and the timer is frozen, then the respective float value will tick down in real time;
        otherwise, if the time is up or the elevator is interacted with, then the clock will unfreeze */
        if (tbReference.timerIsYellow) timeToFreezeTimer -= Time.fixedDeltaTime;
        if ((timeToFreezeTimer < 0f || ((ebReference.elevatorIsMoving || tbReference.timerIsGlitching)
            && checkHasExecuted && ebReference.arrayIndexer >= 4)) && !timerHasUnfrozen)
        {

            timeToFreezeTimer = 0f;
            UnfreezeCountdown();

            timerHasUnfrozen = true;

        }

    }

    public void CheckButtonOrder()
    {

        // Compares the two arrays - if their elements match, then success is declared; otherwise, failure results
        if (enteredCombination.SequenceEqual(correctCombination)) WinCondition();
        else
        {

            puzzleHasFailed = true;
            LossCondition();

        }

    }

    public void WinCondition()
    {

        // Sends a congratulatory message and the prompt to the headset
        tbReference.SendWinMessage();
        tbReference.returnToElevatorText.enabled = true;

        // Causes the lights on the control panel to change
        for(int j = 0; j < plReferences.Length; j++)
        {

            plReferences[j].WinLossLighting(true);

        }

        // Alters the gadget behaviors via the master script
        rcReference.StopInstruments();

        // Causes the "STABLE" text to appear
        ptReference.WinOrLoss(true);

        // Freezes the timer via the method and turns it yellow
        FreezeCountdown();
        tbReference.timerField.color = Color.yellow;

        // Makes the elevator operable
        ebReference.floorHasBeenCompleted = true;

    }

    public void LossCondition()
    {

        // Prompts the player to return to the elevator
        tbReference.returnToElevatorText.enabled = true;

        // Causes the lights on the control panel to change
        for (int k = 0; k < plReferences.Length; k++)
        {

            plReferences[k].WinLossLighting(false);

        }

        // Causes Major Miner to panic
        dcReference.PrepareSpeechBlock(dcReference.dialogueFourthFloor, dcReference.pausesFourthFloor, 2, 2, true);

        // Causes the "DANGER" text to appear
        ptReference.WinOrLoss(false);

        // Makes the elevator operable
        ebReference.floorHasBeenCompleted = true;

    }

    public void FreezeCountdown()
    {

        // Makes the timer yellow, freezing it for a given time
        tbReference.timerIsYellow = true;

    }

    public void UnfreezeCountdown()
    {

        // Unfreezes the timer, allowing it to function normally again
        tbReference.timerIsYellow = false;

    }

}
