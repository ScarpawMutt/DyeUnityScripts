/* Charlie Dye, PACE Team - 2024.12.12

This is the script for buttons on the fourth floor railing puzzle */

using UnityEngine;

public class FFRailButtons : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the primary controls of this button's associated puzzle.")] public FourthFloorRail ffrReference;

    [Header("Colliding GameObjects")]
    [Tooltip("The collider attached to the left hand controller.")] public GameObject triggerCubeLeft;
    [Tooltip("The collider attached to the right hand controller.")] public GameObject triggerCubeRight;

    [Header("Order Number")]
    [Tooltip("The order, beginning with zero, that this button should be pressed in the combination.")][Range(0, 3)] public int orderOfOperation;

    [Header("Boolean Value")]
    [Tooltip("The bool that determines whether or not the button has been activated.")] public bool thisButtonIsLit = false;

    [Header("Light")]
    [Tooltip("The light source found as a child of this script's GameObject.")] public Light buttonLight;

    void Start()
    {

        // Switches off the light
        buttonLight.enabled = false;

    }

    // When a designated collider enters the vicinity of the button
    void OnTriggerEnter(Collider press_down)
    {

        // Since blue comes first in the sequence, it can be activated without consequence anytime
        if (press_down == triggerCubeLeft.GetComponent<Collider>() || press_down == triggerCubeRight.GetComponent<Collider>() && ffrReference.ebReference.arrayIndexer == 4)
        {

            if (!thisButtonIsLit)
            {

                // Enables the light source
                buttonLight.enabled = true;

                // Logs the current indexer value into the player-entered integer array
                ffrReference.enteredCombination[ffrReference.orderIndexer] = orderOfOperation;

                // Increases the parent script's indexer by a value of one
                ffrReference.orderIndexer++;

                // Modifdies the boolean
                thisButtonIsLit = true;

            }

        }

    }
    public void OnButtonPress()
    {
        if (!thisButtonIsLit && ffrReference.ebReference.arrayIndexer == 4)
        {

            // Enables the light source
            buttonLight.enabled = true;

            // Logs the current indexer value into the player-entered integer array
            ffrReference.enteredCombination[ffrReference.orderIndexer] = orderOfOperation;

            // Increases the parent script's indexer by a value of one
            ffrReference.orderIndexer++;

            // Modifdies the boolean
            thisButtonIsLit = true;

        }
    }
}
