/* Charlie Dye, PACE Team - 2024.10.04

This is the script for the elevator's buttons */

using UnityEngine;

public class ElevatorButtons : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the elevator.")] public ElevatorBehavior ebReference;

    [Header("Trigger GameObjects")]
    [Tooltip("The trigger component of the left controller.")] public GameObject triggerLeft;
    [Tooltip("The trigger component of the right controller.")] public GameObject triggerRight;

    [Header("Boolean Variable")]
    [Tooltip("Should the button cause the elevator to ascend (true) or descend (false)?")] public bool moveUp = false;

    // Depending on the value of the gameObject, a different method will execute in ElevatorBehavior.cs
    void OnTriggerEnter(Collider trigger)
    {
        // Ascending to the next floor requires the floor it is on to have been "completed"
        if (ebReference.floorHasBeenCompleted)
        {

            // If the button moves the elevator up
            if (moveUp)
            {

                if ((trigger == triggerLeft.GetComponent<Collider>() || trigger == triggerRight.GetComponent<Collider>()) && ebReference.arrayIndexer <= 3)
                {
                    ebReference.AscendOneFloor();
                }

            }
            // Otherwise, if it moves the elevator down and the elevator is on the fifth floor
            else
            {

                if ((trigger == triggerLeft.GetComponent<Collider>() || trigger == triggerRight.GetComponent<Collider>()) && ebReference.arrayIndexer == 5)
                {

                    ebReference.DescendOneFloor();

                }

            }

        }
    }
    public void OnPhysicalButtonPress(bool isUp)
    {
        if (ebReference.floorHasBeenCompleted)
        {
            //if isUp is true you pressed the physical up button, if false you pressed the physical down button
            if (isUp)
            {
                if (ebReference.arrayIndexer <= 3)
                {
                    //check to see if we need to turn on the steam for floor 3
                    if (ebReference.arrayIndexer == 2)
                    {

                    }

                    ebReference.AscendOneFloor();
                }
            }
            else
            {
                if (ebReference.arrayIndexer == 5)
                {
                    ebReference.DescendOneFloor();
                }
            }
        }
    }

}
