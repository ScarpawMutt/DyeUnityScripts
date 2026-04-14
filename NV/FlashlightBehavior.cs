/* Charlie Dye - 2025.04.26

This is the script for summoning the flashlight */

using Oculus.Interaction.Locomotion;
using UnityEngine;

public class FlashlightBehavior : MonoBehaviour
{

    [Header("Flashlight GameObjects")]
    public GameObject flashlightObject;
    public GameObject flashlightProp;

    [Header("Float Variable")]
    public float summonSpeed;

    [Header("Boolean Variable")]
    public bool flashlightIsHeld;
    public bool buttonIsHeldDown;
    public bool buttonIsReleased;

    void Start()
    {

        // Initially disables the held flashlight object
        flashlightObject.SetActive(true);
        flashlightProp.SetActive(false);

        // Initially sets bools appropriately
        if (buttonIsHeldDown) buttonIsHeldDown = false;
        if (!buttonIsReleased) buttonIsReleased = true;

    }

    void FixedUpdate()
    {

        // If the player presses the B button on the joysticks, then the flashlight will be summoned or dropped, depending on current state
        if (OVRInput.Get(OVRInput.Button.Two))
        {

            if (!buttonIsHeldDown)
            {

                if (flashlightIsHeld) ControlFlashlight(false);
                else ControlFlashlight(true);

                buttonIsHeldDown = true;
                buttonIsReleased = false;

            }

        }
        // If the player releases the B button
        else
        {

            if (!buttonIsReleased)
            {

                buttonIsHeldDown = false;
                buttonIsReleased = true;

            }

        }
        
    }

    public void ControlFlashlight(bool summon_flashlight)
    {

        if (summon_flashlight)
        {

            flashlightProp.SetActive(true);
            flashlightObject.SetActive(false);

            flashlightIsHeld = true;

        }
        else
        {

            flashlightObject.transform.position = flashlightProp.transform.position;

            // Adjusts appropriate gameObject statuses
            flashlightObject.SetActive(true);
            flashlightProp.SetActive(false);

            flashlightIsHeld = false;

        }

    }

}
