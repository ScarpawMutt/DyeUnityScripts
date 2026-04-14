/* Charlie Dye, PACE Team - 2024.03.07

This is the script for the UI Timer */

using System.Collections;
using UnityEngine;
using TMPro;

public class TimerBehavior : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the master dialogue module.")] public DialogueControl dcReference;
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;
    [Tooltip("The script attached to the missile.")] public MissileBehavior mbReference;
    [Tooltip("The script attached to the fifth floor nut puzzle.")] public NutBehavior nbReference;
    [Tooltip("The script attached to the toolbox object.")] public ToolboxBehavior tbReference;

    [Header("Text Variables")]
    [Tooltip("The text that displays the timer value.")] public TextMeshProUGUI timerField;
    [Tooltip("The text that displays a winning message when a puzzle is completed.")] public TextMeshProUGUI winText;
    [Tooltip("The text that prompts the player to return to the elevator.")] public TextMeshProUGUI returnToElevatorText;
    [Tooltip("The text that prompts the player to carry the fire extinguisher to the elevator.")] public TextMeshProUGUI takeExtinguisherText;
    [Tooltip("The text that prompts the player to carry the wrench to the elevator.")] public TextMeshProUGUI takeWrenchText;
    [Tooltip("An array containing all possible characters to choose from when the timer is in a glitched state.")] public string[] glitchCharacters;
    [Tooltip("The message to be displayed should the timer deplete to zero.")] public string gameOverText;
    [Tooltip("The length, in characters, of a glitching message.")] public int glitchMessageLength;
    private Color newYellow = new(1f, 1f, 0f, 1f);

    [Header("Time Parameters")]
    [Tooltip("The amount of time left for the player to complete their current objective.")] public float timeRemaining;
    [Tooltip("The amount of time to be expended just before the timer begins ticking down.")] public float timeUntilCountdown;
    [Tooltip("The amount of time that a message in the HUD is visible for.")] public float messageDuration;
    [Tooltip("The amount of time that the timer displays a glitched state.")] public float glitchDuration;
    [Tooltip("The original time limit as defined by the proper float.")] public float originalTimeLimit;
    [Tooltip("The original preliminary countdown as defined by the proper float.")] public float originalTimeCountdown;


    [Header("Boolean Variable")]
    [Tooltip("Whether or not the timer is in a frozen state.")] public bool timerIsFrozen = false;
    [Tooltip("Whether or not the times is in a temporarily frozen (yellow) state.")] public bool timerIsYellow = false;
    [Tooltip("Whether or nor the timer is in a glitched state.")] public bool timerIsGlitching = false;
    [Tooltip("Whether or not the timer has fully depleted.")] public bool timerIsUp = false;
    [Tooltip("Whether or not the timer has been frozen permanently due to a successful game.")] public bool timerIsSoftlocked = false;
    [Tooltip("Whether or not the coroutine has started. Kill bool.")] public bool coroutineHasStarted = false;
    [Tooltip("Whether or not the missile's launch sequence has started. Kill bool.")] public bool launchHasStarted = false;
    [Tooltip("Whether or not the timer's glitch effect has started. kill bool.")] public bool glitchHasStarted = false;
    private bool minerIsImpatient = false;

    void Start()
    {

        // Hides certain messages
        winText.enabled = false;
        returnToElevatorText.enabled = false;
        takeExtinguisherText.enabled = false;
        takeWrenchText.enabled = false;

        // Sets the hidden public floats to the original time alloted for use later
        originalTimeLimit = timeRemaining;
        originalTimeCountdown = timeUntilCountdown;

        // Sets the text messages to specific colors
        timerField.color = Color.red;
        returnToElevatorText.color = Color.red;
        winText.color = Color.green;
        takeExtinguisherText.color = newYellow;
        takeWrenchText.color = newYellow;

    }

    void FixedUpdate()
    {

        // Controls the timer
        TimerControl();

        // If the timer text coroutine has not yet started
        if (!coroutineHasStarted)
        {

            // Begins coroutines for certain messages
            StartCoroutine(MakeTextFlash(winText, Color.green, Color.clear, true));
            StartCoroutine(MakeTextFlash(returnToElevatorText, Color.red, Color.clear, true));
            StartCoroutine(MakeTextFlash(takeExtinguisherText, newYellow, Color.clear, true));
            StartCoroutine(MakeTextFlash(takeWrenchText, newYellow, Color.clear, true));

            // Switches the bool value to prevent multiple executions in FixedUpdate()
            coroutineHasStarted = true;

        }

        // If the timer dips below one minute while the player is on the fourth floor
        if (timeRemaining < 60f && ebReference.arrayIndexer == 4 && !minerIsImpatient)
        {

            // Miner will reprimand them
            dcReference.PrepareSpeechBlock(dcReference.dialogueFourthFloor, dcReference.pausesFourthFloor, 3, 3, true);

            // Sets the kill Boolean to true
            minerIsImpatient = true;

        }

    }

    public void TimerControl()
    {

        // Introduces the local second, minute, and hour variables
        int hours = Mathf.FloorToInt(timeRemaining / 3600);
        int minutes = Mathf.FloorToInt(timeRemaining % 3600 / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Introduces a local variable and empty string for the glitch array
        int glitchIndexer = glitchCharacters.Length;
        string glitchedMessage = null;

        // If the conditions are correct, then the timer will glitch
        if (timerIsGlitching)
        {

            // Repeats the number of times as specified in the Inspector
            for (int i = 0; i < glitchMessageLength; i++)
            {

                // Selects a random value each time the for loop is called
                int random_value = Random.Range(0, glitchIndexer);

                // Appends the selected value into the string and displays it where the timer is
                glitchedMessage += glitchCharacters[random_value];
                timerField.text = glitchedMessage;

            }

            // If the timer has begun to glitch
            if (!glitchHasStarted)
            {

                Invoke(nameof(TimerCorrection), glitchDuration);
                glitchHasStarted = true;

            }

        }

        // If the level four puzzle is won, then the timer will freeze and turn yellow
        if (timerIsYellow)
        {

            timerIsFrozen = true;
            timerField.color = new Color (1f, 1f, 0f, 1f);

        }

        // If the game is won but the timer has not softlocked, then it will appear green
        if (nbReference.storyIsPaused)
        {

            timerIsFrozen = true;
            timerField.color = Color.green;

        }

        // Otherwise, it will function as normal
        if (!timerIsGlitching && !timerIsYellow && !nbReference.storyIsPaused)
        {

            if (!timerIsSoftlocked)
            {

                // Assembles a string to display the local variables in an hour-minute-second format
                timerField.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

                // Subtracts from the delay before the timer activates, if the delay remaining is greater than zero and the elevator is stationary
                if (timeUntilCountdown > 0f && !ebReference.elevatorIsMoving)
                {

                    // Freezes the timer
                    timerIsFrozen = true;

                    // Counts the preliminary countdown down
                    timeUntilCountdown -= Time.deltaTime;
                    timerField.color = Color.red;

                }
                else if (timeUntilCountdown <= 0f)
                {

                    // Counts the regular timer down
                    timeUntilCountdown = 0f;
                    timerField.color = Color.red;

                    // Unfreezes the timer
                    timerIsFrozen = false;

                }

                // Logic for the timer when the elevator is in motion or if level five has been cleared
                if (ebReference.elevatorIsMoving || (nbReference.winConditionHasExecuted
                    && !tbReference.storyHasRestarted)) timerIsFrozen = true;

                // Subtracts from the allotted time in real time if the time remaining is greater than zero
                if (!timerIsFrozen)
                {

                    if (timeRemaining > 0f && !timerIsUp) timeRemaining -= Time.deltaTime;
                    else if (timeRemaining <= 0f && !timerIsUp)
                    {

                        timeRemaining = 0f;
                        timerIsUp = true;

                    }

                    // If the timer hits zero, the out-of-time method will be called
                    if (timerIsUp) OutOfTime();

                }

            }
            else timerField.text = gameOverText;

        }

    }

    public IEnumerator MakeTextFlash(TextMeshProUGUI textToFlash, Color textColorDefault, Color textColorChanged, bool canFlash)
    {

        while (canFlash)
        {

            if (textToFlash.color == textColorDefault) textToFlash.color = textColorChanged;
            else textToFlash.color = textColorDefault;

            yield return new WaitForSeconds(0.5f);

        }

        yield break;

    }

    public void OutOfTime()
    {

        timerField.text = gameOverText;

        if (!launchHasStarted)
        {

            mbReference.InitiateLaunchSequence();
            launchHasStarted = true;

        }

    }

    public void SendWinMessage()
    {

        winText.enabled = true;
        Invoke(nameof(DisableMessage), messageDuration);

    }

    public void DisableMessage()
    {

        StartCoroutine(MakeTextFlash(winText, Color.green, Color.clear, false));
        winText.enabled = false;

    }

    public void TimerCorrection()
    {

        // Resets booleans
        timerIsGlitching = false;
        glitchHasStarted = false;

    }

}