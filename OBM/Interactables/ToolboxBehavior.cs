/* Charlie Dye, PACE Team - 2024.10.04

This is the script for the wrench box */

using UnityEngine;

public class ToolboxBehavior : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the master dialogue module.")] public DialogueControl dcReference;
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;
    [Tooltip("The script attached to the lugnut puzzle.")] public NutBehavior nbReference;
    [Tooltip("The script attached to the player's timer.")] public TimerBehavior tbReference;

    [Header("Wrench GameObjects")]
    [Tooltip("The wrench object to be placed in the toolbox.")] public GameObject wrenchInteractable;
    [Tooltip("The wrench that appears in the toolbox once the interactable is placed.")] public GameObject wrenchDummy;
    [Tooltip("The wrench that spawns when pre-launch restarts.")] public GameObject wrenchClone;

    [Header("Transforms")]
    [Tooltip("The transform, which rests on the axis, that the toolbox lid revolves around.")] public Transform boxHinge;
    [Tooltip("The point in world space where the wrench clone appears.")] public Transform wrenchRespawnPoint;

    [Header("Numerical Attributes")]
    [Tooltip("The amount, in a float from 0 to 1, that the toolbox lid opens or closes.")] public float normalizedDegreesOfRotation;
    [Tooltip("The speed that the toolbox lid opens or closes.")] public float movementSpeed;
    [Tooltip("The amount of time that passes between Major Miner's introduction to the new \"newbie\" and the restarted meltdown.")] public float secondaryCutsceneDelay;
    private float primaryCutsceneDelay;
    private readonly int minerMonologueStart = 4;
    private readonly int minerMonologueEnd = 7;

    [Header("Boolean Variables")]
    [Tooltip("Is the toolbox lid is in the opened pose or not?")] public bool isOpened = false;
    [Tooltip("Should the lid execute the animation?")] public bool executeAnimation = false;
    [Tooltip("Is the timer ticking down to restart pre-launch?")] public bool countTimeToPreLaunch = false;
    [Tooltip("Should pre-launch restart?")] public bool storyHasRestarted = false;

    void Start()
    {

        // If the floats have unworkable values, then this will correct them
        if (normalizedDegreesOfRotation <= 0f) normalizedDegreesOfRotation = 0.75f;
        if (secondaryCutsceneDelay < 0f) secondaryCutsceneDelay = 0f;

        // Makes the dummy and clone wrenches inactive
        wrenchDummy.SetActive(false);
        wrenchClone.SetActive(false);

        /* Loads time into the primary cutscene delay decimal by indexing through the exterior array,
         adding the lengths of the associated clips and their associated pauses, and then adding the secondary value */
        for (int i = minerMonologueStart; i < minerMonologueEnd + 1; i++)
        {

            primaryCutsceneDelay += dcReference.dialogueFifthFloor[i].length;
            primaryCutsceneDelay += dcReference.pausesFifthFloor[i];

        }
        if (secondaryCutsceneDelay > 0f) primaryCutsceneDelay += secondaryCutsceneDelay;

    }

    void FixedUpdate()
    {

        // If the animation bool is true, then the animation will play
        if (executeAnimation) LidMotion();

        // If pre-launch has not started yet but is slated to do so
        if (countTimeToPreLaunch && !storyHasRestarted)
        {

            /* If the cutscene delay still has time loaded on it, it will subtract unscaled time;
            otherwise, the meltdown will reignite */
            if (primaryCutsceneDelay > 0f) primaryCutsceneDelay -= Time.fixedDeltaTime;
            else
            {

                // If the story has not yet restarted
                if (!storyHasRestarted)
                {

                    // The meltdown resumes
                    RestartMeltdown();

                    // The kill Boolean switches to true
                    storyHasRestarted = true;

                }

            }

        }

    }

    void OnTriggerEnter(Collider wrench)
    {

        // If the toolbox lid is already opened
        if (isOpened)
        {

            // If the wrench has a collider component attached
            if (wrench == wrenchInteractable.GetComponent<Collider>())
            {

                // Activates the dummy and deactivates the interactable
                wrenchDummy.SetActive(true);
                wrenchInteractable.SetActive(false);

                // Closes the toolbox
                executeAnimation = true;

                // Continues dialogue
                dcReference.PrepareSpeechBlock(dcReference.dialogueFifthFloor, dcReference.pausesFifthFloor, 4, 7, false);

                // Starts the hidden timer until pre-launch
                countTimeToPreLaunch = true;

            }

        }    

    }

    public void LidMotion()
    {

        if (!isOpened)
        {

            if (boxHinge.localRotation.x < normalizedDegreesOfRotation)
                boxHinge.Rotate(Vector3.right * (movementSpeed * Time.fixedDeltaTime), Space.Self);
            else StopAnimation();

        }
        else
        {

            if (boxHinge.localRotation.x > 0f)
                boxHinge.Rotate(Vector3.left * (movementSpeed * Time.fixedDeltaTime), Space.Self);
            else StopAnimation();

        }

    }

    public void StopAnimation()
    {

        // Stops the animation from playing
        executeAnimation = false;

        /* If the bool for determining the direction of lid movement returned true, it will become false;
        conversely, the same opposite logic applies for the bool if it initially returns false */
        if (isOpened) isOpened = false;
        else isOpened = true;

    }

    public void RestartMeltdown()
    {

        // Respawns the wrench by making its gameObject active
        wrenchClone.SetActive(true);

        // Restarts the narrative, allowing the timer to progress
        storyHasRestarted = true;
        nbReference.storyIsPaused = false;

        // Manipulates the timer
        tbReference.timeUntilCountdown = tbReference.originalTimeCountdown;
        tbReference.timeRemaining = tbReference.originalTimeLimit;
        tbReference.timerIsGlitching = true;
        tbReference.timerField.color = Color.red;

        // Allows the player to continue the story
        ebReference.floorHasBeenCompleted = true;

        // Continues dialogue
        //dcReference.PrepareSpeechBlock(dcReference.dialogueFifthFloor, dcReference.pausesFifthFloor, minerMonologueStart, minerMonologueEnd, true);

    }

}
