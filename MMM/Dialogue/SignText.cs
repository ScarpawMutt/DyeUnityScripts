/* Charlie Dye - 2024.04.16

This is the script for sign text */

using System.Collections;
using TMPro;
using UnityEngine;

public class SignText : MonoBehaviour
{

    [Header("Script References")]
    public PlayerMovement pm_ref;
    public SpriteBehavior sb_ref;

    [Header("UI Attributes")]
    public TextMeshProUGUI text_box;
    public TextMeshProUGUI message_box;

    [Header("Sign Message")] [TextArea (1, 3)]
    public string sign_message;

    [Header("Interaction Key")]
    public KeyCode interaction_key = KeyCode.E;

    // Boolean values
    public bool can_interact;
    public bool is_reading;

    void Start()
    {

        // Clears the text box of any text
        text_box.text = null;

    }

    void Update ()
    {

        // Determines whether to activate or deactivate text depending on if the player is "reading" or not
        if (can_interact && Input.GetKeyDown(interaction_key) && (!sb_ref.is_moving_up && !sb_ref.is_moving_down && !sb_ref.is_moving_left && !sb_ref.is_moving_right))
        {

            if (!is_reading)
            {

                DisplayText();

            }
            else if (is_reading)
            {

                ExitText();

            }

        }

    }

    // If the player enters the area, this will trigger
    void OnTriggerEnter(Collider reader)
    {

        if (reader.CompareTag("Miriam") || reader.name == "Player")
        {

            // Enables interaction
            can_interact = true;

            // Prints a message to the console for a specified amount of time if the player is not "reading"
            if (!is_reading)
            {

                message_box.text = "Hmmm, there's a sign here. Press " + interaction_key.ToString() + " to read it.";

            }

        }

    }

    // If the player exits the area, this will trigger
    void OnTriggerExit(Collider reader)
    {

        if (reader.CompareTag("Miriam") || reader.name == "Player")
        {

            // Disables interaction
            can_interact = false;

            // Resets the console message
            message_box.text = null;

        }

    }

    // Displays text
    public void DisplayText()
    {

        // Disables movement
        pm_ref.can_move = false;

        // Enables the reading boolean
        is_reading = true;

        // Displays the sign's text in the player's UI
        text_box.text = sign_message;

    }

    // Undoes the above method
    public void ExitText()
    {

        // Reenables movement
        pm_ref.can_move = true;

        // Disables the reading boolean
        is_reading = false;

        // Reverts the UI text to a null value
        text_box.text = null;

    }

}
