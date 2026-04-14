/* Charlie Dye, PACE Team - 2026.01.22

This is the script for dialogue coming through the player's helmet */

using UnityEngine;

public class DialogueControl : MonoBehaviour
{

    [Header("Object Variables and Array")]
    [Tooltip("The player's head object.")] public GameObject playerHead;
    [Tooltip("The array of all collider objects found throughout the Unity world.")] public GameObject[] triggerObjects;

    [Header("Audio Source")]
    [Tooltip("The audio source that audio lines play through.")] public AudioSource dialogueOrigin;

    [Header("Audio Clips (Arrays)")]
    [Tooltip("The array containing all lines of dialogue to be played on the ground floor.")] public AudioClip[] dialogueArrivalFloor;
    [Tooltip("The array containing all lines of dialogue to be played on the first elevator ride.")] public AudioClip[] dialogueElevator0To1;
    [Tooltip("The array containing all lines of dialogue to be played on the first floor.")] public AudioClip[] dialogueFirstFloor;
    [Tooltip("The array containing all lines of dialogue to be played on the second elevator ride.")] public AudioClip[] dialogueElevator1To2;
    [Tooltip("The array containing all lines of dialogue to be played on the second floor.")] public AudioClip[] dialogueSecondFloor;
    [Tooltip("The array containing all lines of dialogue to be played on the third elevator ride.")] public AudioClip[] dialogueElevator2To3;
    [Tooltip("The array containing all lines of dialogue to be played on the third floor.")] public AudioClip[] dialogueThirdFloor;
    [Tooltip("The array containing all lines of dialogue to be played on the fourth elevator ride.")] public AudioClip[] dialogueElevator3To4;
    [Tooltip("The array containing all lines of dialogue to be played on the fourth floor.")] public AudioClip[] dialogueFourthFloor;
    [Tooltip("The array containing all lines of dialogue to be played on the fifth elevator ride.")] public AudioClip[] dialogueElevator4To5;
    [Tooltip("The array containing all lines of dialogue to be played on the fifth floor.")] public AudioClip[] dialogueFifthFloor;
    [Tooltip("The array containing all lines of dialogue to be played on the sixth elevator ride.")] public AudioClip[] dialogueElevator5To1;
    private AudioClip[] audioArrayToPass;
    private AudioClip[] waitingArrayToPass;

    [Header("Numerical Variables (Single)")]
    [Tooltip("The default value that each pause value should be set to if left at zero.")] public float defaultInitialValue;
    [Tooltip("The current time left in a dialogue pause, if one is in effect.")] public float timeLeftInPause;
    [Tooltip("The index value of the current block of speech in the sequence.")] public int speechIndexer;
    [Tooltip("The index value of the triggers in the Unity world.")] public int triggerIndexer;
    private int beginningAudioInteger;
    private int endingAudioInteger;
    private int waitingBeginningAudioInteger;
    private int waitingEndingAudioInteger;

    [Header("Float Variables (Arrays)")]
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the ground floor.")] public float[] pausesArrivalFloor;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the first elevator ride.")] public float[] pausesElevator0To1;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the first floor.")] public float[] pausesFirstFloor;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the second elevator ride.")] public float[] pausesElevator1To2;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the second floor.")] public float[] pausesSecondFloor;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the third elevator ride.")] public float[] pausesElevator2To3;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the third floor.")] public float[] pausesThirdFloor;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the fourth elevator ride.")] public float[] pausesElevator3To4;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the fourth floor.")] public float[] pausesFourthFloor;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the fifth elevator ride.")] public float[] pausesElevator4To5;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the fifth floor.")] public float[] pausesFifthFloor;
    [Tooltip("The array containing all pauses between lines of dialogue to be played on the sixth elevator ride.")] public float[] pausesElevator5To1;
    private float[] pauseArrayToPass;
    private float[] waitingPausesToPass;

    [Header("Boolean Variables")]
    [Tooltip("Is there currently a string of speech playing?")] public bool speechIsPlaying;
    [Tooltip("Is the timer able to count down?")] public bool timerCanCountDown;
    [Tooltip("Is incoming audio waiting for the current block to finish?")] public bool newSpeechIsWaiting;

    void Start()
    {

        // Sets some of the values to zero if not already
        if (speechIndexer != 0) speechIndexer = 0;
        if (triggerIndexer != 0) triggerIndexer = 0;
        if (timeLeftInPause != 0f) timeLeftInPause = 0f;

        // Modifies the Booleans
        speechIsPlaying = false;
        timerCanCountDown = false;
        newSpeechIsWaiting = false;

        // Starts the methods
        ConfigureTriggers();
        ListCorrection();

    }

    private void ConfigureTriggers()
    {

        /* For each trigger element in the array, if it does not contain a proper script, then one will be added automatically;
        additionally, the player's head variable is passed on into these scripts if it is left null */
        for (int i = 0; i < triggerObjects.Length; i++)
        {

            if (!triggerObjects[i].GetComponent<DialogueTrigger>()) triggerObjects[i].AddComponent<DialogueTrigger>();
            if (triggerObjects[i].GetComponent<DialogueTrigger>().passedHeadObject == null)
                triggerObjects[i].GetComponent<DialogueTrigger>().passedHeadObject = playerHead;

        }

    }

    public void UpdateTriggers(bool continueSequence)
    {

        /* For each trigger element in the array, the one element with the matching index value as the respective indexer is enabled; 
        likewise, all other triggers are disabled */
        for (int j = 0; j < triggerObjects.Length; j++)
        {

            if (j == triggerIndexer && triggerIndexer + 1 <= triggerObjects.Length) triggerObjects[j].GetComponent<Collider>().enabled = true;
            else triggerObjects[j].GetComponent<Collider>().enabled = false;

        }

        // Goeds on to the next method if the local Boolean allows
        if (continueSequence) ExecuteDialogue();

    }

    public void SkipCorrection(int parserArgument)
    {

        // Logic checking for the value of the integer argument
        if (parserArgument == 1)
        {

            // If the trigger indexer does not equal the hard-coded "target" integer
            if (triggerIndexer != 2)
            {

                // It will change accordingly
                triggerIndexer = 2;

                // Refreshes the triggers
                UpdateTriggers(false);

            }

        }
        else if (parserArgument == 2)
        {

            // If the trigger indexer does not equal the hard-coded "target" integer
            if (triggerIndexer != 6)
            {

                // It will change accordingly
                triggerIndexer = 6;

                // Refreshes the triggers
                UpdateTriggers(false);

            }

        }
        else if (parserArgument == 3)
        {

            // If the trigger indexer does not equal the hard-coded "target" integer
            if (triggerIndexer != 10)
            {

                // It will change accordingly
                triggerIndexer = 10;

                // Refreshes the triggers
                UpdateTriggers(false);

            }

        }
        else if (parserArgument == 4)
        {

            // If the trigger indexer does not equal the hard-coded "target" integer
            if (triggerIndexer != 13)
            {

                // It will change accordingly
                triggerIndexer = 13;

                // Refreshes the triggers
                UpdateTriggers(false);

            }

        }
        else if (parserArgument == 5)
        {

            // If the trigger indexer does not equal the hard-coded "target" integer
            if (triggerIndexer != 15)
            {

                // It will change accordingly
                triggerIndexer = 15;

                // Refreshes the triggers
                UpdateTriggers(false);

            }

        }

    }

    private void ExecuteDialogue()
    {

        // Logic for determining which audio to play, depending on which trigger was triggered (this accounts for the incremented trigger value per method call)
        if (triggerIndexer == 1) PrepareSpeechBlock(dialogueElevator0To1, pausesElevator0To1, 0, 0, true);
        else if (triggerIndexer == 2) PrepareSpeechBlock(dialogueFirstFloor, pausesFirstFloor, 0, 0, true);
        else if (triggerIndexer == 3) PrepareSpeechBlock(dialogueFirstFloor, pausesFirstFloor, 1, 7, false);
        else if (triggerIndexer == 4) PrepareSpeechBlock(dialogueFirstFloor, pausesFirstFloor, 8, 8, false);
        else if (triggerIndexer == 5) PrepareSpeechBlock(dialogueElevator1To2, pausesElevator1To2, 0, 0, true);
        else if (triggerIndexer == 6) PrepareSpeechBlock(dialogueSecondFloor, pausesSecondFloor, 0, 0, true);
        else if (triggerIndexer == 7) PrepareSpeechBlock(dialogueSecondFloor, pausesSecondFloor, 1, 1, true);
        else if (triggerIndexer == 8) PrepareSpeechBlock(dialogueSecondFloor, pausesSecondFloor, 8, 8, false);
        else if (triggerIndexer == 9) PrepareSpeechBlock(dialogueElevator2To3, pausesElevator2To3, 0, 1, false);
        else if (triggerIndexer == 10) PrepareSpeechBlock(dialogueThirdFloor, pausesThirdFloor, 0, 0, false);
        else if (triggerIndexer == 11) PrepareSpeechBlock(dialogueThirdFloor, pausesThirdFloor, 1, 4, true);
        else if (triggerIndexer == 12) PrepareSpeechBlock(dialogueElevator3To4, pausesElevator3To4, 0, 2, false);
        else if (triggerIndexer == 13) PrepareSpeechBlock(dialogueFourthFloor, pausesFourthFloor, 0, 0, false);
        else if (triggerIndexer == 14) PrepareSpeechBlock(dialogueFourthFloor, pausesFourthFloor, 1, 1, false);
        else if (triggerIndexer == 15) PrepareSpeechBlock(dialogueFifthFloor, pausesFifthFloor, 0, 0, false);
        else if (triggerIndexer == 16) PrepareSpeechBlock(dialogueFifthFloor, pausesArrivalFloor, 1, 1, true);

    }

    private void ListCorrection()
    {

        // If the pause and dialogue array lengths are unequal, then this will adjust them
        if (pausesArrivalFloor.Length != dialogueArrivalFloor.Length) pausesArrivalFloor = new float[dialogueArrivalFloor.Length];
        if (pausesElevator0To1.Length != dialogueElevator0To1.Length) pausesElevator0To1 = new float[dialogueElevator0To1.Length];
        if (pausesFirstFloor.Length != dialogueFirstFloor.Length) pausesFirstFloor = new float[dialogueFirstFloor.Length];
        if (pausesElevator1To2.Length != dialogueElevator1To2.Length) pausesElevator1To2 = new float[dialogueElevator1To2.Length];
        if (pausesSecondFloor.Length != dialogueSecondFloor.Length) pausesSecondFloor = new float[dialogueSecondFloor.Length];
        if (pausesElevator2To3.Length != dialogueElevator2To3.Length) pausesElevator2To3 = new float[dialogueElevator2To3.Length];
        if (pausesThirdFloor.Length != dialogueThirdFloor.Length) pausesThirdFloor = new float[dialogueThirdFloor.Length];
        if (pausesElevator3To4.Length != dialogueElevator3To4.Length) pausesElevator3To4 = new float[dialogueElevator3To4.Length];
        if (pausesFourthFloor.Length != dialogueFourthFloor.Length) pausesFourthFloor = new float[dialogueFourthFloor.Length];
        if (pausesElevator4To5.Length != dialogueElevator4To5.Length) pausesElevator4To5 = new float[dialogueElevator4To5.Length];
        if (pausesFifthFloor.Length != dialogueFifthFloor.Length) pausesFifthFloor = new float[dialogueFifthFloor.Length];
        if (pausesElevator5To1.Length != dialogueElevator5To1.Length) pausesElevator5To1 = new float[dialogueElevator5To1.Length];
        
        // Proceeds to assign the default value
        SetDefaultValues();

    }

    private void SetDefaultValues()
    {

        // If any pause value is equal to zero, then the default value will be substituted through the for-loop
        for (int collectionI = 0; collectionI < pausesArrivalFloor.Length; collectionI++)
        {

            if (pausesArrivalFloor[collectionI] == 0f) pausesArrivalFloor[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesElevator0To1.Length; collectionI++)
        {

            if (pausesElevator0To1[collectionI] == 0f) pausesElevator0To1[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesFirstFloor.Length; collectionI++)
        {

            if (pausesFirstFloor[collectionI] == 0f) pausesFirstFloor[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesElevator1To2.Length; collectionI++)
        {

            if (pausesElevator1To2[collectionI] == 0f) pausesElevator1To2[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesSecondFloor.Length; collectionI++)
        {

            if (pausesSecondFloor[collectionI] == 0f) pausesSecondFloor[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesElevator2To3.Length; collectionI++)
        {

            if (pausesElevator2To3[collectionI] == 0f) pausesElevator2To3[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesThirdFloor.Length; collectionI++)
        {

            if (pausesThirdFloor[collectionI] == 0f) pausesThirdFloor[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesElevator3To4.Length; collectionI++)
        {

            if (pausesElevator3To4[collectionI] == 0f) pausesElevator3To4[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesFourthFloor.Length; collectionI++)
        {

            if (pausesFourthFloor[collectionI] == 0f) pausesFourthFloor[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesElevator4To5.Length; collectionI++)
        {

            if (pausesElevator4To5[collectionI] == 0f) pausesElevator4To5[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesFifthFloor.Length; collectionI++)
        {

            if (pausesFifthFloor[collectionI] == 0f) pausesFifthFloor[collectionI] = defaultInitialValue;

        }
        for (int collectionI = 0; collectionI < pausesElevator5To1.Length; collectionI++)
        {

            if (pausesElevator5To1[collectionI] == 0f) pausesElevator5To1[collectionI] = defaultInitialValue;

        }

        // Prepares the first block of dialogue to play
        PrepareSpeechBlock(dialogueArrivalFloor, pausesArrivalFloor, 0, 3, true);

    }

    void FixedUpdate()
    {

        // If the timer is allowed to diminish
        if (timerCanCountDown)
        {

            // If there is still some time left before the pause ends, then it will tick down using unscaled time
            if (timeLeftInPause > 0f) timeLeftInPause -= Time.fixedDeltaTime;
            // Otherwise, the speech train will continue on
            else ResetSpeech();

        }

    }

    /// <summary>
    /// Plays a piece or string of pieces of dialogue from a given audio array and its corresponding float array, the index values to begin and end on, and determines if this sequence of dialogue can override any dialogue already playing.
    /// </summary>
    /// <param name="audioToPlayFrom"></param>
    /// <param name="pausesToPlay"></param>
    /// <param name="beginningAudioClip"></param>
    /// <param name="endingAudioClip"></param>
    /// <param name="canOverrideSpeech"></param>
    public void PrepareSpeechBlock(AudioClip[] audioToPlayFrom, float[] pausesToPlay, int beginningAudioClip, int endingAudioClip, bool canOverrideSpeech)
    {

        if (canOverrideSpeech)
        {

            // Sets the speech indexer to the value of the beginning block of text
            speechIndexer = beginningAudioClip;

            // If the Boolean indicating that speech is waiting is set to true, then it will switch to false
            if (newSpeechIsWaiting) newSpeechIsWaiting = false;

            /* If the defined ending integer is greater than the parent audio array's length minus one (to account for the "0" value),
            then it will be corrected; this is a failsafe to prevent null executions */
            if (endingAudioClip > audioToPlayFrom.Length - 1) endingAudioClip = audioToPlayFrom.Length - 1;

            // Copies the values of the local arguments into the private variables for storage purposes
            audioArrayToPass = audioToPlayFrom;
            pauseArrayToPass = pausesToPlay;
            beginningAudioInteger = beginningAudioClip;
            endingAudioInteger = endingAudioClip;

            // Returns to the method
            ProceduralDialogue();

        }
        else
        {

            if (speechIsPlaying)
            {

                waitingArrayToPass = audioToPlayFrom;
                waitingPausesToPass = pausesToPlay;
                waitingBeginningAudioInteger = beginningAudioClip;
                waitingEndingAudioInteger = endingAudioClip;

                newSpeechIsWaiting = true;

            }
            else
            {

                // Sets the speech indexer to the value of the beginning block of text
                speechIndexer = beginningAudioClip;

                /* If the defined ending integer is greater than the parent audio array's length minus one (to account for the "0" value),
                then it will be corrected; this is a failsafe to prevent null executions */
                if (endingAudioClip > audioToPlayFrom.Length - 1) endingAudioClip = audioToPlayFrom.Length - 1;

                // Copies the values of the local arguments into the private variables for storage purposes
                audioArrayToPass = audioToPlayFrom;
                pauseArrayToPass = pausesToPlay;
                beginningAudioInteger = beginningAudioClip;
                endingAudioInteger = endingAudioClip;

                // Returns to the method
                ProceduralDialogue();

            }

        }

    }

    private void ProceduralDialogue()
    {

        // Sets the Booleans to true
        speechIsPlaying = true;
        timerCanCountDown = true;

        // Loads the proper audio clip into the audio source and then plays it
        dialogueOrigin.clip = audioArrayToPass[speechIndexer];
        dialogueOrigin.Play();

        // Loads/reloads a value into the float timer; this is equal to the audio clip's length plus the following pause, in seconds
        timeLeftInPause = audioArrayToPass[speechIndexer].length + pauseArrayToPass[speechIndexer];

    }

    private void ResetSpeech()
    {

        // If the speech index value is less than that of the ending audio integer stored
        if (speechIndexer < endingAudioInteger)
        {

            // Increments the value by one
            speechIndexer++;

            // Repeats the method cycle
            ProceduralDialogue();

        }
        else
        {

            if (newSpeechIsWaiting)
            {

                // Transfers the data from the "waiting" argument slots to the "active" slots
                audioArrayToPass = waitingArrayToPass;
                pauseArrayToPass = waitingPausesToPass;
                beginningAudioInteger = waitingBeginningAudioInteger;
                endingAudioInteger = waitingEndingAudioInteger;

                // Wipes the array and integer arguments of their values
                waitingArrayToPass = null;
                waitingPausesToPass = null;
                waitingBeginningAudioInteger = 0;
                waitingEndingAudioInteger = 0;

                // Sets the speech indexer to the newly valued beginning index integer
                speechIndexer = beginningAudioInteger;

                // Renews the method chain
                PrepareSpeechBlock(audioArrayToPass, pauseArrayToPass, beginningAudioInteger, endingAudioInteger, true);

                // Switches the "waiting" Boolean to false
                newSpeechIsWaiting = false;

            }
            else
            {

                // Nullifies the audio clip in the source
                dialogueOrigin.clip = null;

                // Resets the speech indexer to zero
                speechIndexer = 0;

                // Sets the Booleans to false
                speechIsPlaying = false;
                timerCanCountDown = false;

            }

        }

    }

}
