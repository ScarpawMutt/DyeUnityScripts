/* Charlie Dye, PACE Team - 2024.11.25

This is the script for the combination code for its respective puzzle */

using UnityEngine;

public class TriggerZone : MonoBehaviour
{

    [Header("Parent Script Reference")]
    [Tooltip("The script attached to the combination puzzle.")] public CombinationPuzzle cpReference;

    [Header("Colliding GameObject")]
    [Tooltip("The spinner object with the associated collider.")] public GameObject spinnerCollider;

    [Header("Position in Parent Array")]
    [Tooltip("The order, starting at zero, of the trigger in the correct sequence.")] [Range(0, 6)] public int localValue;

    // Boolean variables
    [HideInInspector] public bool clockwiseNotCounterclockwise;
    [HideInInspector] public bool isTriggered = false;

    void OnTriggerEnter(Collider pointer)
    {
        
        if (pointer == spinnerCollider.GetComponent<Collider>() && !isTriggered)
        {

            /* If the spinner is rotating in the correct direction, then the puzzle will advance;
            otherwise, it will reset */
            if (clockwiseNotCounterclockwise)
            {

                if (cpReference.rotateRight)
                {

                    // Sets the boolean to true
                    isTriggered = true;

                    // Calls the parent method, passing the local script's array element position as an argument
                    cpReference.AdvanceInPuzzle(localValue);

                }
                else cpReference.ResetPuzzle();

            }
            else if (!clockwiseNotCounterclockwise)
            {

                if (cpReference.rotateLeft)
                {

                    // Sets the boolean to true
                    isTriggered = true;

                    // Calls the parent method, passing the local script's array element position as an argument
                    cpReference.AdvanceInPuzzle(localValue);

                }
                else cpReference.ResetPuzzle();

            }


        }

    }

}
