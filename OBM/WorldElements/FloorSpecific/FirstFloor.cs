/* Charlie Dye, PACE Team - 2024.10.11

This is the script unique to the first floor */

using UnityEngine;

public class FirstFloor : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the camera quaking effect.")] public CameraQuake cqReference;
    [Tooltip("The script attached to the master dialogue system.")] public DialogueControl dcReference;
    [Tooltip("The script attached to the delay for the control panel discovery dialogue.")] public DialogueDelay ddReference;
    [Tooltip("The script attached to the missile.")] public MissileBehavior mbReference;
    [Tooltip("The script attached to the control panel gimmick on the first floor.")] public MasterControlPanel mcpReference;

    [Header("Objects")]
    [Tooltip("Steve's arm.")] public GameObject steveArm;
    [Tooltip("The player timer.")] public GameObject textBoxes;
    [Tooltip("Steve's wrench.")] public GameObject wrenchInteractable;

    [Header("Transform Array")]
    [Tooltip("The array of transforms that Steve's arm moves between.")] public Transform[] armArray;
    [Tooltip("The target position that Steve's arm must move to.")] public Transform armTargetPosition;

    [Header("Collider")]
    [Tooltip("The collider that segues into a winning ending.")] public Collider victoryBox;

    [Header("Audio Source")]
    [Tooltip("The audio source that plays the vine boom.")] public AudioSource boomSource;

    [Header("Audio Clips")]
    [Tooltip("The block of dialogue that triggers the Steve animation.")] public AudioClip stevePrompt;
    [Tooltip("The block of dialogue that triggers the shaking animation for the missile.")] public AudioClip shakePrompt;
    [Tooltip("The block of dialogue that triggers the vine boom.")] public AudioClip boomPrompt;
    [Tooltip("The block of dialogue that triggers the wrench falling onto the second floor.")] public AudioClip wrenchPrompt;
    [Tooltip("The block of dialogue that triggers the timer.")] public AudioClip timerPrompt;

    [Header("Numerical Variables")]
    [Tooltip("The duration of the missile animation.")] public float durationOfMissileAnimation;
    [Tooltip("The duration between the start of the prompting audio clip and Steve waving.")] public float durationUntilWave;
    [Tooltip("The duration between the start of the prompting audio clip and the vine boom playing.")] public float durationUntilBoomPlays;
    [Tooltip("The duration between the start of the prompting audio clip and the wrench falling.")] public float durationUntilWrenchFalls;

    // Boolean variables
    [HideInInspector] public bool steveIsWaving = false;
    [HideInInspector] public bool timerHasStarted = false;
    private bool steveMustWave = false;
    private bool boomMustPlay = false;
    private bool boomHasPlayed = false;
    private bool wrenchMustFall = false;
    private bool missileIsShaking = false;
    private bool missileIsStill = false;
    private bool wrenchHasFallen = false;

    void Start()
    {

        // If the numerical variables have unworkable values, then this will correct them
        if (durationUntilWave < 0f) durationUntilWave = 0f;
        if (durationUntilWrenchFalls < 0f) durationUntilWrenchFalls = 0f;

        // Moves Steve's arm to where it is needed
        steveArm.transform.position = armArray[0].position;

        // Disables the victory box
        if (victoryBox.enabled) victoryBox.enabled = false;

        // If certain objects are enabled, this will disable them until they are properly called
        if (textBoxes.activeInHierarchy) textBoxes.SetActive(false);
        if (wrenchInteractable.activeInHierarchy) wrenchInteractable.SetActive(false);

    }

    void FixedUpdate()
    {

        // If the dialogue clip matches the one that prompts the Steve animation, then he will wave
        if (stevePrompt == dcReference.dialogueOrigin.clip) steveMustWave = true;

        // If Steve must wave but has not started doing so yet
        if (steveMustWave)
        {

            /* Subtracts time from the counter if it is greater than zero
            otherwise, the Boolean will switch off and the animation will eventually stop */
            if (durationUntilWave > 0f) durationUntilWave -= Time.fixedDeltaTime;
            else steveIsWaving = true;

        }

        // If the dialogue clip matches the one that prompts the missile animation, and the missile is not already shaking
        if (shakePrompt == dcReference.dialogueOrigin.clip)
        {

            if (!missileIsShaking)
            {

                // Toggles the shaking once (the kill Boolean regulates this)
                mbReference.awakenMissile = true;
                mbReference.emitSteam = true;
                cqReference.canQuake = true;
                missileIsShaking = true;

            }

        }

        // If the missile is undergoing its shaking animation
        if (missileIsShaking)
        {

            /* If the missile animation has time left and the kill Boolean is active, then it will subtract time from itself;
            otherwise, the missile will shut off */
            if (durationOfMissileAnimation > 0f) durationOfMissileAnimation -= Time.fixedDeltaTime;
            else
            {

                // If the kill Boolean has not changed values yet
                if (!missileIsStill)
                {

                    // Reverts the shaking to stop it
                    mbReference.awakenMissile = false;
                    mbReference.emitSteam = false;
                    cqReference.canQuake = false;
                    missileIsStill = true;

                }

            }

        }

        // If the dialogue clip matches the one that prompts the vine boom
        if (boomPrompt == dcReference.dialogueOrigin.clip) boomMustPlay = true;

        // If the vine boom must play
        if (boomMustPlay)
        {

            /* If the timer has time left, then it will subtract unscaled time;
            otherwise, the sound effect will play */
            if (durationUntilBoomPlays > 0f) durationUntilBoomPlays -= Time.fixedDeltaTime;
            else
            {

                // If the kill Boolean has not changed values yet
                if (!boomHasPlayed)
                {

                    // Plays the sound
                    boomSource.Play();

                    // Switches the kill Boolean's value
                    boomHasPlayed = true;

                }

            }

        }

        // If the dialogue clip matches the one that prompts Steve's wrench falling down
        if (wrenchPrompt == dcReference.dialogueOrigin.clip) wrenchMustFall = true;

        // If the wrench must fall but has not done so yet
        if (wrenchMustFall && !wrenchHasFallen)
        {

            /* If the timer has time left, then it will subtract unscaled time;
            otherwise, the wrench will fall */
            if (durationUntilWrenchFalls > 0f) durationUntilWrenchFalls -= Time.fixedDeltaTime;
            else
            {

                // Activates the wrench, allowing it to fall
                wrenchInteractable.SetActive(true);

                // Modifies the kill Boolean
                wrenchHasFallen = true;

            }

        }

        // If the dialogue clip matches the one that prompts the timer to begin counting down
        if (timerPrompt == dcReference.dialogueOrigin.clip && !timerHasStarted)
        {

            // Activates the timer
            textBoxes.SetActive(true);

            // Allows Major Miner to play his line when the player goes to the control stand
            ddReference.EnableTrigger();

            // Allows interaction with the control panel
            mcpReference.canInteractWithPuzzle = true;

            // Modifies the kill Boolean
            timerHasStarted = true;

        }

    }

}
