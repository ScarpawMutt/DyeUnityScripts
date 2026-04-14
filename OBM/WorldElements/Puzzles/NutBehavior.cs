/* Charlie Dye - 2024.10.03

This is the script for the nut/screw puzzle */

using System.Collections;
using UnityEngine;

public class NutBehavior : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the master dialogue module.")] public DialogueControl dcReference;
    [Tooltip("The script attached to the first floor trigger.")] public FirstFloor ffRefernce;
    [Tooltip("The script attached to the toolbox GameObject.")] public ToolboxBehavior toolBReference;
    [Tooltip("The script attached to the player's timer.")] public TimerBehavior timerBReference;

    [Header("GameObjects")]
    [Tooltip("The wrench object.")] public GameObject wrenchObject;
    [Tooltip("The HUD that appears on the player's visor.")] public GameObject hudDisplay;
    [Tooltip("The trophy that appears if the player wins.")] public GameObject obmTrophy;

    [Header("Target Screw")]
    [Tooltip("The rigid body attached to the nut.")] public Rigidbody nutRigidbody;

    [Header("Movement Attributes")]
    [Tooltip("The rate that the nut spins.")] public float rotationRate;
    [Tooltip("The value that multiplies the rate of nut rotation.")] public float rotationMultiplier;
    [Tooltip("The rate at which the nut descends on the shaft.")] public float threadingRate;
    [Tooltip("The amount to move downwards.")] public float threadingDistance;

    [Header("Boolean Variables")]
    [Tooltip("Whether or not rotation is allowed.")] public bool canScrewIn = false;
    [Tooltip("Whether or not the nut is in the process of screwing in.")] public bool isScrewingIn = false;
    [Tooltip("Whether or not the nut has already screwed in fully.")] public bool isScrewedIn = false;
    [Tooltip("Whether or not the coroutine has fully executed. Kill bool.")] public bool coroutineHasExecuted = false;
    [Tooltip("Whether or not the game has been won due to the successful completion of the puzzle. Kill bool.")] public bool winConditionHasExecuted = false;
    [Tooltip("Whether or not the timer is paused due to successful completion.")] public bool storyIsPaused = false;

    // Vector3 variables
    private Vector3 nutRotation;
    private Vector3 targetNutPosition;

    // Start is called before the first frame update
    void Start()
    {

        // Sets parameters for the Vector3 variables
        nutRotation = new(0f, 0f, rotationMultiplier);
        targetNutPosition = new Vector3(nutRigidbody.position.x, nutRigidbody.position.y - threadingDistance, nutRigidbody.position.z);

        // Makes the trophy inactive
        obmTrophy.SetActive(false);

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // If the screw can be turned
        if (canScrewIn)
        {

            // Once the screw reaches its target position, the bool becomes true
            if (nutRigidbody.position == targetNutPosition) isScrewedIn = true;

            // If the nut is screwed in fully, then a win condition will activate
            if (isScrewedIn && !winConditionHasExecuted) GameVictory();

        }
        
    }

    void OnTriggerStay(Collider wrench)
    {

        // Returns true as long as the wrench and nut triggers collide
        if (wrench == wrenchObject.GetComponent<Collider>() && canScrewIn)
        {

            isScrewingIn = true;

            if (!isScrewedIn && !coroutineHasExecuted)
            {

                StartCoroutine(NutMotion());
                coroutineHasExecuted = true;

            }

            // If the nut is screwed in fully
            if (isScrewedIn)
            {

                // Freezes the rigidbody's rotation and position
                nutRigidbody.constraints = RigidbodyConstraints.FreezeAll;

                // Enables the trophy
                obmTrophy.SetActive(true);

            }

        }

    }

    void OnTriggerExit(Collider wrench)
    {

        // If the wrench exits the trigger area, then the nut will pause
        if (wrench == wrenchObject.GetComponent<Collider>())
        {

            coroutineHasExecuted = false;
            isScrewingIn = false;

        }

    }

    public IEnumerator NutMotion()
    {

        while (isScrewingIn && nutRigidbody.transform.position != targetNutPosition)
        {

            // Rotates the nut around the screw
            nutRigidbody.transform.Rotate(rotationRate * Time.deltaTime * nutRotation);

            // Moves the nut downwards
            nutRigidbody.transform.position = Vector3.MoveTowards(nutRigidbody.transform.position, targetNutPosition, threadingRate);

            yield return new WaitForSecondsRealtime(0.01f);

        }

        yield break;

    }

    public void GameVictory()
    {

        // Sets Boolean values across the game to simulate a victory condition
        toolBReference.executeAnimation = true;
        storyIsPaused = true;
        winConditionHasExecuted = true;

        // Continues dialogue
        dcReference.PrepareSpeechBlock(dcReference.dialogueFifthFloor, dcReference.pausesFifthFloor, 2, 3, true);

        // Re-enables the victory box collider on the first floor, allowing it to be triggered
        ffRefernce.victoryBox.enabled = true;

    }

}
