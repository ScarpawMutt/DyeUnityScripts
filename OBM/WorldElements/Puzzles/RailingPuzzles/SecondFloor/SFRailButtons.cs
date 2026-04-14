/* Charlie Dye, PACE Team - 2024.11.20

This is the script for buttons on the second floor railing puzzle */

using UnityEngine;

public class SFRailButtons : MonoBehaviour
{

    [Header("Script Reference")]
    [Tooltip("The script attached to the second floor railing puzzle.")] public SecondFloorRail sfrReference;

    [Header("Colliding GameObjects")]
    [Tooltip("The player's left trigger.")] public GameObject triggerCubeLeft;
    [Tooltip("The player's right trigger.")] public GameObject triggerCubeRight;

    [Header("Light")]
    [Tooltip("The light from within the button.")] public Light button_light;

    void Start()
    {

        // Switches off the light
        button_light.enabled = false;

    }

    void OnTriggerEnter(Collider hand)
    {
        
        if (hand == triggerCubeLeft.GetComponent<Collider>() || hand == triggerCubeRight.GetComponent<Collider>() && sfrReference.ebReference.arrayIndexer == 1)
        {

            // Sets the parent script's bool to true
            if (!sfrReference.buttonHasBeenPressed) sfrReference.buttonHasBeenPressed = true;

            // Enables the light source if it has not already been switched on
            if (!button_light.enabled) button_light.enabled = true;

        }

    }
    public void OnButtonPress()
    {
        if (sfrReference.ebReference.arrayIndexer == 2)
        {
            Debug.Log("Button Pressed");
            
            // Sets the parent script's bool to true
            if (!sfrReference.buttonHasBeenPressed) sfrReference.buttonHasBeenPressed = true;

            // Enables the light source if it has not already been switched on
            if (!button_light.enabled) button_light.enabled = true;
        }
        
        
    }

}
