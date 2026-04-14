/* Charlie Dye, PACE Team - 2024.11.20

This is the script for the release nut on the second floor railing puzzle */

using System.Collections;
using UnityEngine;

public class ReleaseCrank : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script attached to the master dialogue module.")] public DialogueControl dcReference;
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;
    [Tooltip("The script attached to the missile.")] public MissileBehavior mbReference;
    [Tooltip("The script attached to the second floor railing puzzle.")] public SecondFloorRail sfrReference;
    [Tooltip("The script attached to the timer.")] public TimerBehavior tbReference;

    [Header("GameObjects")]
    [Tooltip("The wrench object.")] public GameObject wrenchObject;

    [Header("Target Screw")]
    [Tooltip("The rigid body attached to the release crank.")] public Rigidbody nutRigidbody;

    [Header("Movement Attributes")]
    [Tooltip("How fast the crank rotates.")] public float rotation_rate;
    [Tooltip("How fast the crank moves down on its shaft.")] public float threadingRate;
    [Tooltip("How far the crank moves down.")] public float threadingDistance;

    [Header("Light")]
    [Tooltip("The light attached to the top of the crank.")] public Light tipLight;

    [Header("Audio")]
    [Tooltip("The audio source responsible for the sloshing fuel noise in the missile.")] public AudioSource missileSloshing;

    [Header("Boolean Variables")]
    [Tooltip("Whether or not the crank is actively screwing in.")] public bool isScrewingIn = false;
    [Tooltip("Whether or not the crank has been fully screwed in.")] public bool isScrewedIn = false;
    private bool coroutineHasExecuted = false;
    private bool rewardHasExecuted = false;
    private bool minerIsImpatient = false;

    // Vector3 variable
    private Vector3 targetNutPosition;

    // Start is called before the first frame update
    void Start()
    {

        // Turns the light off
        if (tipLight.enabled) tipLight.enabled = false;

        // Sets parameters for the Vector3 variable
        targetNutPosition = new Vector3(nutRigidbody.position.x, nutRigidbody.position.y - threadingDistance, nutRigidbody.position.z);

    }

    void FixedUpdate()
    {

        // If a button has been pressed from the adjacent button panel
        if (sfrReference.buttonHasBeenPressed)
        {

            // Once the screw reaches its target position, the bool becomes true
            if (nutRigidbody.position == targetNutPosition) isScrewedIn = true;

            // If the screw is "screwing in", it will rotate; otherwise, it will switch the light on if already screwed in
            if (isScrewingIn && !isScrewedIn) RotateCrank();
            else if (isScrewedIn) tipLight.enabled = true;

        }

        // If there is less than 60 seconds on the timer, the nut has not been screwed in, and the player is on the proper floor
        if (tbReference.timeRemaining < 60f && !isScrewedIn && !minerIsImpatient && ebReference.arrayIndexer == 2)
        {

            // Major Miner will reprimand the player
            dcReference.PrepareSpeechBlock(dcReference.dialogueSecondFloor, dcReference.pausesSecondFloor, 4, 4, false);

            // The kill Boolean becomes true
            minerIsImpatient = true;

        }

    }

    void OnTriggerStay(Collider wrench)
    {

        if (sfrReference.buttonHasBeenPressed)
        {

            // Returns true as long as the wrench and nut triggers collide
            if (wrench == wrenchObject.GetComponent<Collider>())
            {

                isScrewingIn = true;

                if (!isScrewedIn && !coroutineHasExecuted)
                {

                    // Starts the coroutine using the kill Boolean
                    StartCoroutine(NutMotion());
                    coroutineHasExecuted = true;

                }

                // If the nut is screwed in fully
                if (isScrewedIn)
                {

                    // Freezes the rigidbody's rotation and position
                    nutRigidbody.constraints = RigidbodyConstraints.FreezeAll;

                    // If the reward has not been given, it will do so once
                    if (!rewardHasExecuted)
                    {

                        CompletePuzzle();
                        rewardHasExecuted = true;

                    }

                }

            }

        }

    }

    void OnTriggerExit(Collider wrench)
    {

        if (sfrReference.buttonHasBeenPressed)
        {

            // If the wrench exits the trigger area, then the nut will pause
            if (wrench == wrenchObject.GetComponent<Collider>())
            {

                coroutineHasExecuted = false;
                isScrewingIn = false;

            }

        }

    }

    private void RotateCrank()
    {

        nutRigidbody.transform.Rotate(rotation_rate * Time.deltaTime * Vector3.forward, Space.Self);

    }

    private void CompletePuzzle()
    {

        // Plays a fuel sloshing sound effect from within the missile
        missileSloshing.Play();

        // Sends messages to the HUD
        tbReference.SendWinMessage();
        tbReference.returnToElevatorText.enabled = true;
        tbReference.takeWrenchText.enabled = true;

        // Allows the player to progress
        ebReference.floorHasBeenCompleted = true;

        // Allows dialogue to continue
        dcReference.PrepareSpeechBlock(dcReference.dialogueSecondFloor, dcReference.pausesSecondFloor, 5, 7, true);

    }

    private IEnumerator NutMotion()
    {

        while (isScrewingIn && nutRigidbody.transform.position != targetNutPosition)
        {

            // Moves the nut downwards
            nutRigidbody.transform.position = Vector3.MoveTowards(nutRigidbody.transform.position, targetNutPosition, threadingRate);

            yield return new WaitForSecondsRealtime(0.03f);

        }

        yield return null;

    }

}
