/* Charlie Dye, PACE Team - 2024.09.20

This is the updated button script for the control panel */

using UnityEngine;

public class ButtonsUpdated : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the level one control panel puzzle.")] public MasterControlPanel mcpReference;

    [Header("Controller GameObjects")]
    [Tooltip("The player's left controller.")] public GameObject leftController;
    [Tooltip("The player's right controller.")] public GameObject rightController;

    [Header("Order Number")]
    [Tooltip("The order, by adding one to this value, that this specific button must be pressed in.")] [Range(0, 7)] public int orderOfOperation;

    [Header("Boolean Value")]
    [Tooltip("Whether or not this button is illuminated.")] public bool thisButtonIsLit = false;

    [Header("Light Source")]
    [Tooltip("The light source attached to this button.")] public Light buttonLight;

    [Header("Audio Source")]
    [Tooltip("The audio source attached to this button.")] public AudioSource buttonSound;

    // When a designated collider enters the vicinity of the button
    void OnTriggerEnter(Collider pressDown)
    {
        // Since blue comes first in the sequence, it can be activated without consequence anytime
        if (pressDown == leftController.GetComponent<Collider>() || pressDown == rightController.GetComponent<Collider>())
        {
            //Call a seperate OnButtonPress method so that the MQTT Button manager can also call the OnButtonPress method when the physical buttons are pressed
            OnButtonPress();
        }
    }
    //called either when the digital buttons are pressed with a controllelr or when a physical button is pressed
    public void OnButtonPress()
    {
        if (!thisButtonIsLit && mcpReference.canInteractWithPuzzle)
        {
            
            // Plays a sound
            buttonSound.Play();

            // Enables the light source
            buttonLight.enabled = true;

            // Modifdies the booleans
            thisButtonIsLit = true;
            mcpReference.colliderArray[orderOfOperation] = true;

            // Executes the button check method in the master script if the win condition has not executed
            if (!mcpReference.winConditionHasExecuted) mcpReference.CheckButtonOrder();

        }
        else buttonSound.Play();

    }

    public void DeactivateButton()
    {

        // Deactivates the light source
        buttonLight.enabled = false;

        // Modifies the booleans
        thisButtonIsLit = false;
        mcpReference.colliderArray[orderOfOperation] = false;

    }

}
