/* Charlie Dye, PACE Team - 2024.04.02

This is the script for the elevator mechanic */

using System;
using System.Collections;
using TestMQTT;
using UnityEngine;

public class ElevatorBehavior : MonoBehaviour
{

    [Header("Script References and Arrays")]
    [Tooltip("The script attached to the doomsday clock.")] public ClockBehavior cbReference;
    [Tooltip("The script attached to the master dialogue controls.")] public DialogueControl dcReference;
    [Tooltip("The script attached to the concluding animations of the experience.")] public LossTransition ltReference;
    [Tooltip("The script attached to the missile.")] public MissileBehavior mbReference;
    [Tooltip("The script attached to the new silo.")] public NewSiloBehavior nsbReference;
    [Tooltip("The script attached to the fifth floor nut puzzle.")] public NutBehavior nbReference;
    [Tooltip("The script attached to the player timer.")] public TimerBehavior timerBReference;
    [Tooltip("The script attached to the toolbox object.")] public ToolboxBehavior toolBReference;
    [Tooltip("The array of scripts attached to the pinion gears on the elevator mechanism.")] public GearBehavior[] gbReferences;
    [Tooltip("The script attached to the pointer.")] public PointerBehavior[] pbReferences;
    [Tooltip("The array of scripts attached to the recalibration points for the timer.")] public TimerRecalibration[] trReferences;
    [Tooltip("The script that sends messages to the mqtt server for the buttkickers")] public ButtKickerManager buttKickerManager;

    [Header("Elevator Objects")]
    [Tooltip("The rigid body component attached to the elevator.")] public Rigidbody elevatorRigidbody;
    [Tooltip("The rigid body component attached to the elevator's gates.")] public Rigidbody elevatorDoors;
    [Tooltip("The player object.")] public GameObject controllerEntity;
    [Tooltip("The player's head as it is positioned in VR.")] public GameObject playerHead;
    [Tooltip("The approximate center of the elevator platform.")] public Transform elevatorCenter;

    [Header("Floor Transforms")]
    [Tooltip("The array of transform positions that the elevator must stop at.")] public Transform[] floorStations;

    [Header("Numerical Variables")]
    [Tooltip("The current speed that the elevator is traveling at as controlled by the coroutine.")] public float currentElevatorSpeed;
    [Tooltip("The maximum permitted speed that the elevator ascends and descends.")] public float maximumElevatorSpeed;
    [Tooltip("The rate at which the elevator accelerates or deccelerates.")] public float incrementAmount;
    [Tooltip("The threshold at which the elevator begins to slow down as it reaches its target floor.")] public float distanceToFloor;
    [Tooltip("The speed that the gates raise and lower by.")] public float doorSpeed;
    [Tooltip("The distance that the gates travel up or down.")] public float distanceDoorsTravel;
    [Tooltip("The quotient of the gear's rotation rates divided by the elevator's current speed, used to scale the gear speeds with the elevator's.")] public float gearRotationRatio;
    [Tooltip("The vertical distance between two common points on each floor.")] public float floorHeight;
    [Tooltip("The maximum distance the player's head can stray from the center of the elevator before they \"die\".")] public float marginOfClearance;
    [Tooltip("The rate at which the coroutines refresh.")] public float refreshRate;
    [Tooltip("The value that logs the current location of the elevator and where to travel to.")] public int arrayIndexer = 0;
    private float elevatorHeightMultiplier;
    private float doorHeightMultiplier;

    [Header("Button Lights")]
    [Tooltip("The light for the up button.")] public Light upLight;
    [Tooltip("The light for the down button.")] public Light downLight;

    [Header("Audio Sources and Arrays")]
    [Tooltip("The array of audio sources that play while the elevator is in motion.")] public AudioSource[] elevatorAudioSources;
    [Tooltip("The audio that plays when the gates rise or lower.")] public AudioSource doorAudio;
    [Tooltip("The audio that plays when the gates come to a stop.")] public AudioSource doorSlam;
    [Tooltip("The chime that plays when the hand scanner is activated successfully.")] public AudioSource scannerActivation;
    [Tooltip("The audio that plays if the player attempts to move the elevator to a place it cannot go.")] public AudioSource elevatorStrain;

    [Header("Boolean Variables")]
    [Tooltip("Has the current floor's objective been met?")] public bool floorHasBeenCompleted = false;
    [Tooltip("Is the elevator is in the process of moving to a floor?")] public bool elevatorIsMoving = false;
    [Tooltip("Can the elevator can be moved?")] public bool canOperateElevator = true;
    [Tooltip("Can the buttons can be interacted with?")] public bool canOperateButtons = true;
    [Tooltip("Can the hand scanner be interacted with?")] public bool canOperateScanner = false;
    [Tooltip("Has the timer's glitch begun?")] public bool glitchHasActivated = false;

    //Vector3 variables
    private Vector3 elevatorTargetPosition;
    private Vector3 doorTargetPosition;

    void Start()
    {

        // If the array indexer is set incorrectly, then this will fix it
        if (arrayIndexer != 0) arrayIndexer = 0;

        // Disables all light sources
        ResetLights();

        // Teleports the elevator to its starting position on the first level
        elevatorTargetPosition = floorStations[arrayIndexer].transform.position;
        elevatorRigidbody.transform.position = elevatorTargetPosition;

        // If specific float variables are set to zero or below, this will set them to appropriate values
        if (maximumElevatorSpeed <= 0f) maximumElevatorSpeed = 0.005f;
        if (incrementAmount <= 0f) incrementAmount = 0.01f;
        if (distanceToFloor <= 0f) distanceToFloor = 0.5f;
        if (marginOfClearance == 0f) marginOfClearance = 1f;
        else if (marginOfClearance < 0f) marginOfClearance *= -1f;
        if (refreshRate <= 0f) refreshRate = 0.02f;

    }

    void FixedUpdate()
    {

        // Mathematical logic to prevent an index out-of-bounds error
        if (arrayIndexer > 5) arrayIndexer = 5;
        if (arrayIndexer < 0) arrayIndexer = 0;

        // Logic for button usage
        if (arrayIndexer == 4) canOperateButtons = false;
        else canOperateButtons = true;

        if (arrayIndexer == 4) canOperateScanner = true;
        else canOperateScanner = false;

    }

    public void AscendOneFloor()
    {

        if (elevatorRigidbody.transform.position == elevatorTargetPosition && canOperateElevator)
        {

            if (canOperateButtons)
            {

                try
                {
                    // Logic for where the elevator needs to go
                    if (arrayIndexer < 4) arrayIndexer++;
                    elevatorHeightMultiplier = 1.0f;
                    elevatorTargetPosition = floorStations[arrayIndexer].transform.position;

                    // Illuminates the up button
                    upLight.enabled = true;

                    // If specific timer text is active, then it will deactivate
                    if (timerBReference.returnToElevatorText.enabled) timerBReference.returnToElevatorText.enabled = false;
                    if (timerBReference.takeExtinguisherText.enabled) timerBReference.takeExtinguisherText.enabled = false;
                    if (timerBReference.takeWrenchText.enabled) timerBReference.takeWrenchText.enabled = false;

                    // Depending on which floor the elevator should travel to, different dialogue will play
                    if (arrayIndexer == 1) dcReference.PrepareSpeechBlock(dcReference.dialogueElevator0To1, dcReference.pausesElevator0To1, 1, 1, false);
                    else if (arrayIndexer == 2) dcReference.PrepareSpeechBlock(dcReference.dialogueElevator1To2, dcReference.pausesElevator1To2, 1, 1, false);
                    else if (arrayIndexer == 4) dcReference.PrepareSpeechBlock(dcReference.dialogueElevator3To4, dcReference.pausesElevator3To4, 1, 2, false);

                    // Sets the correct direction for the gears to rotate
                    for (int j = 0; j < gbReferences.Length; j++)
                    {

                        gbReferences[j].reverseRotation = false;

                    }

                    // Begins elevator door animation
                    StartCoroutine(DoorMotion(false, true));

                    //Start the buttkicker Audio
                    buttKickerManager.PlayButtKicker("Elevator", 1, 2, false);

                }
                catch (IndexOutOfRangeException)
                {

                    // Makes the elevator strain
                    elevatorStrain.Play();

                }

            }
            else
            {

                // Makes the elevator strain
                elevatorStrain.Play();

                // Makes the timer glitch before adjusting the time if it has not already done so
                if (!glitchHasActivated)
                {

                    timerBReference.timerIsGlitching = true;
                    timerBReference.timeUntilCountdown = 0.5f;
                    timerBReference.timeRemaining = 60f;
                    glitchHasActivated = true;

                }

            }

        }
        else
        {

            // Makes the elevator strain
            elevatorStrain.Play();

        }

    }

    public void AscendToFifthFloor()
    {

        if (elevatorRigidbody.transform.position == floorStations[4].transform.position &&
            elevatorDoors.transform.position == doorTargetPosition && canOperateScanner)
        {

            try
            {

                // Logic for where the elevator needs to go
                arrayIndexer = 5;
                elevatorHeightMultiplier = 1.0f;
                elevatorTargetPosition = floorStations[arrayIndexer].transform.position;

                // Plays dialogue
                dcReference.PrepareSpeechBlock(dcReference.dialogueElevator4To5, dcReference.pausesElevator4To5, 0, 1, true);

                // If the RTE text is active, then it will deactivate
                if (timerBReference.returnToElevatorText.enabled) timerBReference.returnToElevatorText.enabled = false;

                // Plays audio
                scannerActivation.Play();

                // Sets the correct direction for the gears to rotate
                for (int k = 0; k < gbReferences.Length; k++)
                {

                    gbReferences[k].reverseRotation = false;

                }

                // Begins elevator door animation
                StartCoroutine(DoorMotion(false, true));

                //Start the buttkicker Audio
                buttKickerManager.PlayButtKicker("Elevator", 1, 2, false);

            }
            catch (IndexOutOfRangeException)
            {

                // Makes the elevator strain
                elevatorStrain.Play();

            }

        }

    }

    public void DescendOneFloor()
    {

        if (elevatorRigidbody.transform.position == elevatorTargetPosition && canOperateElevator)
        {

            if (canOperateButtons)
            {

                if (nbReference.winConditionHasExecuted)
                {

                    // Logic for where the elevator needs to go
                    arrayIndexer = 1;
                    elevatorHeightMultiplier = -1.0f;
                    elevatorTargetPosition = floorStations[arrayIndexer].transform.position;
                    currentElevatorSpeed *= 2f;

                    // Illuminates the down button
                    downLight.enabled = true;

                    // If the RTE text is active, then it will deactivate
                    if (timerBReference.returnToElevatorText.enabled) timerBReference.returnToElevatorText.enabled = false;

                    // Readies the missile for launch
                    mbReference.InitiateLaunchSequence();

                    // Sets the correct direction for the gears to rotate
                    for (int l = 0; l < gbReferences.Length; l++)
                    {

                        gbReferences[l].reverseRotation = true;
                        gbReferences[l].gearRotationSpeed *= 1.5f;

                    }

                    // Begins elevator door animation
                    StartCoroutine(DoorMotion(false, true));

                    //Start the buttkicker Audio
                    buttKickerManager.PlayButtKicker("Elevator", 1, 3, false);

                }
                else elevatorStrain.Play();

            }
            else
            {

                // Makes the elevator strain
                elevatorStrain.Play();

                // Makes the timer glitch before adjusting the time if it has not already done so
                if (!glitchHasActivated)
                {

                    timerBReference.timerIsGlitching = true;
                    timerBReference.timeUntilCountdown = 0.5f;
                    timerBReference.timeRemaining = 60f;
                    glitchHasActivated = true;

                }

            }

            if (!nsbReference.siloIsOpen)
            {

                nsbReference.siloIsOpen = true;
                nsbReference.executeCoverAnimation = true;

            }

        }
        else
        {

            // Makes the elevator strain
            elevatorStrain.Play();

        }

    }

    private IEnumerator ElevatorMotion()
    {

        // Enables and disables timer recalibration using the respective scripts and target floor
        if (arrayIndexer >= 2)
        {

            // Makes the timer glitch
            timerBReference.timerIsGlitching = true;

            try
            {

                trReferences[arrayIndexer - 2].RecalibrateTimer();

            }
            catch (IndexOutOfRangeException)
            {

                // Makes the elevator strain
                elevatorStrain.Play();

            }  

        }
        else if (arrayIndexer == 1 && toolBReference.storyHasRestarted)
        {

            // Makes the timer glitch
            timerBReference.timerIsGlitching = true;

        }

        // Plays audio
        for (int m = 0; m < elevatorAudioSources.Length; m++)
        {

            elevatorAudioSources[m].Play();

        }

        // Enables gear animation and sound
        for (int n = 0; n < gbReferences.Length; n++)
        {

            gbReferences[n].executeAnimation = true;
            gbReferences[n].gearNoise.Play();

        }

        // If the game has been won, then the timer will freeze forever
        if (nbReference.winConditionHasExecuted && arrayIndexer == 1)
        {

            timerBReference.timerIsSoftlocked = true;

        }

        // This while-loop plays until the elevator reaches its target position
        while (elevatorRigidbody.transform.position != elevatorTargetPosition)
        {

            /* If the elevator's current speed is not at its maximum when in motion and not in range of the floor, then it will slowly accelerate;
            otherwise, it will slowly deccelerate to a crawl until it reaches the floor height */
            if (!nbReference.winConditionHasExecuted)
            {

                if (currentElevatorSpeed < maximumElevatorSpeed && elevatorRigidbody.transform.position.y < (elevatorTargetPosition.y - distanceToFloor))
                    currentElevatorSpeed += incrementAmount;
                else if (currentElevatorSpeed > (incrementAmount * 10f) && elevatorRigidbody.transform.position.y >= (elevatorTargetPosition.y - distanceToFloor))
                    currentElevatorSpeed -= incrementAmount;

            }
            else
            {

                if (currentElevatorSpeed < maximumElevatorSpeed && elevatorRigidbody.transform.position.y > (elevatorTargetPosition.y + distanceToFloor))
                    currentElevatorSpeed += incrementAmount;
                else if (currentElevatorSpeed > (incrementAmount * 10f) && elevatorRigidbody.transform.position.y <= (elevatorTargetPosition.y + distanceToFloor))
                    currentElevatorSpeed -= incrementAmount;

            }

            // If the player strays too far from the center of the elevator, then they will "die" via LossTransition.cs
            if (playerHead.transform.position.x - elevatorCenter.transform.position.x > marginOfClearance ||
                playerHead.transform.position.x - elevatorCenter.transform.position.x < -marginOfClearance ||
                playerHead.transform.position.z - elevatorCenter.transform.position.z > marginOfClearance ||
                playerHead.transform.position.z - elevatorCenter.transform.position.z < -marginOfClearance)
                ltReference.playerHasDied = true;

            // Scales the speed of the gears to that of the elevator, using the ratio float
            for (int o = 0; o < gbReferences.Length; o++)
            {

                gbReferences[o].gearRotationSpeed = currentElevatorSpeed * gearRotationRatio;

            }

            // Moves the elevator up or down
            elevatorRigidbody.transform.position = Vector3.MoveTowards(elevatorRigidbody.transform.position, elevatorTargetPosition, currentElevatorSpeed);

            // Moves the player up or down accordingly with the elevator
            controllerEntity.transform.position = Vector3.MoveTowards(controllerEntity.transform.position,
                new Vector3(controllerEntity.transform.position.x, controllerEntity.transform.position.y +
                (floorHeight * elevatorHeightMultiplier), controllerEntity.transform.position.z), currentElevatorSpeed);

            yield return new WaitForSecondsRealtime(refreshRate);

        }

        // If the elevator needs to change floors, then the coroutine will transition
        StartCoroutine(DoorMotion(true, false));

        // Resets the elevator's speed to zero
        currentElevatorSpeed = 0f;

        // Stops audio
        for (int p = 0; p < elevatorAudioSources.Length; p++)
        {

            elevatorAudioSources[p].Stop();

        }

        // Disables gear animation and sound
        for (int q = 0; q < gbReferences.Length; q++)
        {

            gbReferences[q].executeAnimation = false;
            gbReferences[q].gearNoise.Stop();

        }

        // Switches the lights off
        ResetLights();

        // Disables further operation of elevator
        floorHasBeenCompleted = false;

    }

    private IEnumerator DoorMotion(bool isGoingDown, bool mustMoveElevator)
    {

        // Disables booleans
        canOperateElevator = false;
        canOperateButtons = false;

        // If the doors are moving up
        if(!isGoingDown)
        {

            // Readjusts the hands on the clock
            cbReference.ReadjustHands();

            // Disables pointers, if enabled
            for (int r = 0; r < pbReferences.Length; r++)
            {

                if (pbReferences[r].renderPointer) pbReferences[r].renderPointer = false;

                if (arrayIndexer == 2) pbReferences[0].hidePointerPermanently = true;
                else if (arrayIndexer == 3) pbReferences[1].hidePointerPermanently = true;

            }

        }

        // Sets this bool value to true, freezing the UI's timer (see TimerBehavior.cs)
        elevatorIsMoving = true;

        /* If the first local bool parameter returns true, then the door will lower;
        otherwise, it will raise up */
        if (isGoingDown) doorHeightMultiplier = -1.0f;
        else doorHeightMultiplier = 1.0f;

        /* Sets the target position of the doors as a new Vector3 position depending on the distance to rise/lower
        and the height multiplier float from earlier */
        doorTargetPosition = new Vector3(elevatorDoors.transform.position.x, elevatorDoors.transform.position.y
           + (distanceDoorsTravel * doorHeightMultiplier), elevatorDoors.transform.position.z);

        // Plays audio
        doorAudio.Play();

        // This while-loop plays until the doors reach their target position
        while (elevatorDoors.transform.position != doorTargetPosition)
        {

            // Moves the doors up or down depending on the speed float and earlier Vector3
            elevatorDoors.transform.position = Vector3.MoveTowards(elevatorDoors.transform.position, doorTargetPosition, doorSpeed);

            yield return new WaitForSecondsRealtime(refreshRate);

        }

        // Ends with the slam sound
        doorAudio.Stop();
        doorSlam.Play();

        // If the second bool parameter returns true
        if (mustMoveElevator)
        {

            // The coroutine transitions, handing it off to the main elevator animation
            StartCoroutine(ElevatorMotion());

        }
        else
        {

            // Sets Boolean values as false
            elevatorIsMoving = false;
            canOperateButtons = true;
            canOperateElevator = true;

        }

    }

    private void ResetLights()
    {

        if (upLight.enabled) upLight.enabled = false;
        if (downLight.enabled) downLight.enabled = false;

    }

}
