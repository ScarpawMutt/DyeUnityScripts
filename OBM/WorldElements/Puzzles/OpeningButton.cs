/* Charlie Dye, PACE Team - 2024.10.11

This is the script for the button in the opening room */

using UnityEngine;

public class OpeningButton : MonoBehaviour
{

    [Header("Script Variable")]
    [Tooltip("The script attached to the master dialogue controls.")] public DialogueControl dcReference;

    [Header("Player GameObjects")]
    [Tooltip("The hitbox attached to the left controller.")] public GameObject triggerCubeLeft;
    [Tooltip("The hitbox attached to the right controller.")] public GameObject triggerCubeRight;

    [Header("Light Source")]
    [Tooltip("The light source attached to the door button.")] public Light buttonLight;

    [Header("Boolean Variable")]
    [Tooltip("Whether or not the button has activated.")] public bool buttonHasActivated;

    void Start()
    {

        // Switches the button's light off
        buttonLight.enabled = false;

    }

    void OnTriggerEnter(Collider hand)
    {
        
        if ((hand == triggerCubeLeft.GetComponent<Collider>() || hand == triggerCubeRight.GetComponent<Collider>()) && !buttonHasActivated)
        {

            // Switches the button's light on
            buttonLight.enabled = true;

            // Sets the bool to true for the other script
            buttonHasActivated = true;

            // Causes further speech to execute in the dialogue script
            dcReference.PrepareSpeechBlock(dcReference.dialogueArrivalFloor, dcReference.pausesArrivalFloor, 4, 5, false);

        }

    }
    public void OnEntryButtonPress()
    {
        // Switches the button's light on
        buttonLight.enabled = true;

        // Sets the bool to true for the other script
        buttonHasActivated = true;

        // Plays the appropriate block of script
        dcReference.PrepareSpeechBlock(dcReference.dialogueArrivalFloor, dcReference.pausesArrivalFloor, 4, 5, false);
    }

}
