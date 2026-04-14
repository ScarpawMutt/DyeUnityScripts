/* Charlie Dye, PACE Team - 2024.09.19

This is the script to display the floor the elevator is at */

using UnityEngine;
using TMPro;

public class FloorDisplay : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the elevator.")]  public ElevatorBehavior ebReference;

    [Header("Text")]
    [Tooltip("The text that displays the elevator's current location on the wall.")] public TextMeshPro displayText;

    void FixedUpdate()
    {

        if (ebReference.arrayIndexer == 0) displayText.text = "G";
        else displayText.text = (ebReference.arrayIndexer).ToString();
        
    }

}
