/* Charlie Dye, PACE Team - 2025.04.21

This is the script for Papa's updated silo */

using System.Collections;
using UnityEngine;

public class NewSiloBehavior : MonoBehaviour
{

    [Header("Script References")]
    [Tooltip("The script for the player's timer.")] public TimerBehavior tbReference;
    [Tooltip("The script that recalibrates the player's timer.")] public TimerRecalibration trReference;
    [Tooltip("The script for the flagpole in the background.")] public FlagBehavior[] fbReferences;

    [Header("Silo GameObjects")]
    [Tooltip("The GameObject of the silo lid.")] public GameObject siloCover;
    [Tooltip("The GameObject of the silo lifting arms.")] public GameObject siloLifts;

    [Header("Float Variables")]
    [Tooltip("The speed that the silo lid opens.")] public float doorSpeed;
    [Tooltip("The maximum allowed angle that the silo's lid may open relative to its closed position. Should be less than 90 for ideal operation.")] public float maxOpenAngle;
    private float initialStartingAngle;

    [Header("Boolean Variables")]
    [Tooltip("Denotes whether the silo's lid is opened or closed.")] public bool siloIsOpen = false;
    [Tooltip("Initiates the animation for the silo's lid.")] public bool executeCoverAnimation = false;
    [Tooltip("Denotes whether the silo's lid has opened due to the timer reaching a given time. Kill bool.")] public bool timeHasPassed = false;

    void Start()
    {

        // Writes the starting rotation of the closed silo lid, making it immutable
        initialStartingAngle = siloCover.transform.localRotation.eulerAngles.x;

        // Failsafe corrections for the above variables
        if (doorSpeed <= 0f) doorSpeed = 1f;
        if (maxOpenAngle <= 0f) maxOpenAngle = 90f;
        if (siloIsOpen) siloIsOpen = false;
        if (executeCoverAnimation) executeCoverAnimation = false;
        if (timeHasPassed) timeHasPassed = false;

    }

    void FixedUpdate()
    {

        // If the timer dips below a given time, the silo's lid will open
        if (!timeHasPassed)
        {

            if (tbReference.timeRemaining <= trReference.recalibrateToThisAmount)
            {

                // Opens the lid
                executeCoverAnimation = true;

                // Prevents repeated opening animations via the kill bool
                timeHasPassed = true;

            }

        }

        // If the bool to start the animation becomes true
        if (executeCoverAnimation)
        {

            // If the silo's lid is closed
            if (!siloIsOpen)
            {

                // If the lid is not yet at its target position, then it will keep calling the method to rotate
                if (siloCover.transform.rotation.eulerAngles.x < initialStartingAngle + maxOpenAngle) OperateSilo(true);

                // Otherwise, it will stop
                else
                {

                    siloIsOpen = true;
                    executeCoverAnimation = false;

                }

            }
            // Otherwise, if the silo's lid is opened
            else if (siloIsOpen)
            {

                // If the lid is not yet at its target position, then it will keep calling the method to rotate
                if (siloCover.transform.localRotation.eulerAngles.x > initialStartingAngle) OperateSilo(false);
                // Otherwise, it will stop
                else
                {

                    siloIsOpen = false;
                    executeCoverAnimation = false;

                }

            }

        }

    }

    public void OperateSilo(bool can_open)
    {

        // If the silo's lid must open
        if (can_open)
        {

            // Rotates the cover around the its local transform, which is offset
            siloCover.transform.Rotate(doorSpeed * Time.fixedDeltaTime * Vector3.left, Space.Self);

        }
        // Otherwise, if it must close
        else
        {

            // Rotates the cover around the its local transform, which is offset
            siloCover.transform.Rotate(doorSpeed * Time.fixedDeltaTime * Vector3.right, Space.Self);

        }

    }

}
